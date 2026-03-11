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
    //Network variable so everybody has access to the countdown timer for UI purposes
    public NetworkVariable<bool> gameOver = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    //Network boolean, tracks if gameover has occured or not
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
            tree.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId); //ensures tree is spawned across all devices and only owner has access
        }
    }

    void OnClientConnected(ulong clientId)
    {
        var connectedCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
        if (connectedCount == 2)
        {
            StartCoroutine(SpawnBasketDelayed(clientId));
        }
    }

    IEnumerator SpawnBasketDelayed(ulong clientId)
    {
        yield return new WaitForSeconds(0.5f);
        var basket = Instantiate(basketPrefab, new Vector3(0, -14f, 0), Quaternion.identity);
        basket.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        gameStarted = true;
    }

    void Update()
    {

        if (!IsServer)
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

        timeRemaining.Value -= Time.deltaTime;

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
            ShowPlayer2WinsClientRpc();
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
        gameOver.Value = true;
        ShowPlayer1WinsClientRpc(); //game over, player 1 won
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ShowPlayer1WinsClientRpc() //show all devices play 1 won
    {
        WinScreen.Instance.ShowPlayer1Wins();
    }

    [Rpc(SendTo.ClientsAndHost)] //show all devicesplayer 2 won
    public void ShowPlayer2WinsClientRpc()
    {
        WinScreen.Instance.ShowPlayer2Wins();
    }
}