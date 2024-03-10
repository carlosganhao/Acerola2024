using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public event Action<Transform> PlayerDetected;

    void OnTriggerEnter(Collider other)
    {
        if(PlayerDetected != null)
        {
            PlayerDetected.Invoke(other.transform);
        }
    }
}
