using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    public static float bottomY = -20f;
    void Update()
    {
        if (transform.position.y < bottomY)
        {
            Destroy(this.gameObject); //ignore this, destroy is only used in local multiplayer
        }
    }
}
