using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class ApplePicker : NetworkBehaviour
{
    [Header("Inscribed")]
    public int numLives = 3;

    public NetworkVariable<int> lives = new NetworkVariable<int>(3, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    //lives count is viewable by all but only server can update the life count
    public override void OnNetworkSpawn()
    {
        lives.OnValueChanged += OnLivesChanged;
    }

    public override void OnNetworkDespawn()
    {
        lives.OnValueChanged -= OnLivesChanged;
    }

    void OnLivesChanged(int previous, int current)
    {
        if (current <= 0 && IsServer)
        {
            GameManager.Instance.OnBasketOutOfLives(); //tell game manager basket life count has reached 0 therefore gameover
        }
    }
}