using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Backet")
        {
            Destroy(GetComponent<Road>());
            EventCenter.Broadcast(EventDefine.SpawnZombies);
        }
    }
}
