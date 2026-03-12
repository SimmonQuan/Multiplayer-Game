using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Basket : NetworkBehaviour
{
    public float speed = 150f;
    public ScoreCounter scoreCounter;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
        GameObject scoreGO = GameObject.Find("ScoreCounter");
        scoreCounter = scoreGO.GetComponent<ScoreCounter>();
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        float move = 0f;             //only player 2 can control the basket
        if (Input.GetKey(KeyCode.A)) //A = left, D = right movement
        {
            move = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            move = 1f;
        }
        transform.position += new Vector3(move * speed * Time.deltaTime, 0, 0);
    }

    void OnCollisionEnter(Collision coll)
    {
        if (!IsOwner) 
        {
            return;
        }

        GameObject collidedWith = coll.gameObject;

        if (collidedWith.tag == "Apple")
        {
            CollectAppleServerRpc(coll.gameObject.GetComponent<NetworkObject>().NetworkObjectId); //if basket touches apple, tell server to collect it
        }

        if (collidedWith.tag == "Bomb")
        {
            BombSuccessServerRpc(coll.gameObject.GetComponent<NetworkObject>().NetworkObjectId); //if basket touches bomb, tell server player loses a life
        }
    }

    [Rpc(SendTo.Server)] //only occurs on the server
    void CollectAppleServerRpc(ulong appleNetworkId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(appleNetworkId, out NetworkObject apple))
        {
            apple.Despawn(); //despawn that specific apple on the server, this will be reflected across all devices
            GameManager.Instance.AddScore(100); //add 100 to score which is shared across all devices
        }
    }

    [Rpc(SendTo.Server)]
    void BombSuccessServerRpc(ulong bombNetworkId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(bombNetworkId, out NetworkObject bomb))
        {
            bomb.Despawn(); //despawn that specific bomb on the server, this will be reflected across all devices
            GetComponent<ApplePicker>().lives.Value--; //decrease life counter across all devices
        }
    }
}