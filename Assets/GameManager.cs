using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    public GameObject treePrefab;
    public GameObject basketPrefab;
    public ScoreCounter scoreCounter;

    public NetworkVariable<float> timeRemaining = new NetworkVariable<float>(60f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    //Network variable: syncs across all players so everybody can read the current timer value for UI purposes. Only server can change timeRemaining to prevent cheating
    public NetworkVariable<bool> gameOver = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    //Network boolean, syncs across all players, tracks if gameover has occured or not. Everyone can read but only server can write to maintain game integrity/fairness
    private bool gameStarted = false;

    void Awake() => Instance = this;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return; 
        }   

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        if (IsHost)
        {
            var tree = Instantiate(treePrefab, new Vector3(0, 12f, 0), Quaternion.identity); //spawn tree prefab
            tree.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId); //ensures tree is spawned across all devices and only owner (player 1) has access
        }
    }

    void OnClientConnected(ulong clientId)
    {
        var connectedCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
        if (connectedCount == 2) //wait for two players to be connected before spawning basket
        {
            StartCoroutine(SpawnBasketDelayed(clientId));
        }
    }

    IEnumerator SpawnBasketDelayed(ulong clientId)
    {
        yield return new WaitForSeconds(0.5f);
        var basket = Instantiate(basketPrefab, new Vector3(0, -14f, 0), Quaternion.identity); //delay basket spawning by 0.5f to ensure client connection has occured before attempting to spawn basket (client connection can be slow)
        basket.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        gameStarted = true;
    }

    void Update()
    {

        if (!IsServer) //ensures only server has authority here
        {
            return;
        }
        if (!gameStarted)
        {
            return;
        }
        if (gameOver.Value)
        {
            return;
        }

        timeRemaining.Value -= Time.deltaTime; //timer

        if (timeRemaining.Value <= 0) //if timer runs out, player 1 wins
        {
            timeRemaining.Value = 0;
            gameOver.Value = true;
            ShowPlayer1WinsClientRpc(); //all devices show player 1 won
            return;
        }

        if (scoreCounter.score.Value >= 1000) //if score is 1000 or above, player 2 won
        {
            gameOver.Value = true;
            ShowPlayer2WinsClientRpc(); //show player 2 won
        }
    }

    public void AddScore(int amount)
    {
        if (!IsServer)
        {
            return;
        }
        scoreCounter.score.Value += amount; //server increments networked score 
    }

    public void OnBasketOutOfLives()
    {
        if (!IsServer)
        {
            return;
        }
        if (gameOver.Value)
        {
            return;
        }
        gameOver.Value = true; //only server has ability to declare gameover due to lack of lives
        ShowPlayer1WinsClientRpc(); //game over, player 1 won
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ShowPlayer1WinsClientRpc() //show all devices player 1 won
    {
        WinScreen.Instance.ShowPlayer1Wins();
    }

    [Rpc(SendTo.ClientsAndHost)] //show all devices player 2 won
    public void ShowPlayer2WinsClientRpc()
    {
        WinScreen.Instance.ShowPlayer2Wins();
    }
}