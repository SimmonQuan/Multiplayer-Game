using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AppleTree : NetworkBehaviour
{
    public GameObject applePrefab;
    public GameObject bombPrefab;
    public float speed = 17f;
    public float spawnCooldown = 1.2f;
    public float spawnRangeX = 28f;
    public float spawnHeight = 14f;
    private float spawnTime;
    public float appleDropDelay = 0.5f;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) //only owner has permission to control the tree
        {
            enabled = false;
            return;
        }
        spawnTime = 0f;
    }

    void Update()
    {
        if (!IsOwner) //only player who owns the tree can move it using input from keyboard
        {
            return;
        }

        float move = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) //left arrow key moves the tree to the left
        {
            move = -1f;
        }
        if (Input.GetKey(KeyCode.RightArrow)) //right arrow key moves the tree to the right
        {
            move = 1f;
        }

        transform.position += new Vector3(move * speed * Time.deltaTime, 0, 0);

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            DropBombServerRpc(transform.position); //down arrow tells server to drop a bomb at current location
        }

        spawnTime -= Time.deltaTime;

        if (spawnTime <= 0) //once apple cooldown is done, spawn an apple at a random position in screen boundaries
        {
            float randomXposition = Random.Range(-spawnRangeX, spawnRangeX);
            Vector3 spawnPosition = new Vector3(randomXposition, spawnHeight, 0f);
            SpawnAppleServerRpc(spawnPosition);
            spawnTime = spawnCooldown;
        }
    }

    [Rpc(SendTo.Server)] //tells server to spawn the bomb which will be reflected across all devices
    void DropBombServerRpc(Vector3 spawnPosition)
    {
        GameObject bomb = Instantiate(bombPrefab, spawnPosition, Quaternion.identity);
        bomb.GetComponent<NetworkObject>().Spawn();
    }

    [Rpc(SendTo.Server)] //tells server to spawn apple which will be reflected across all devices
    void SpawnAppleServerRpc(Vector3 spawnPosition)
    {
        GameObject apple = Instantiate(applePrefab, spawnPosition, Quaternion.identity);
        apple.GetComponent<NetworkObject>().Spawn();
    }

}