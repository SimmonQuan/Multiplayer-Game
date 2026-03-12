using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class ApplePicker : NetworkBehaviour
{
    public int numLives = 3; //default life count, can be edited to extend game/add difficulty

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
            GameManager.Instance.OnBasketOutOfLives(); //tells game manager basket life count has reached 0 therefore gameover
        }
    }
}