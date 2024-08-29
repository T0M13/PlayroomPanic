using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        RespawnObject(other.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        RespawnObject(other);
    }

    private void RespawnObject(Collider other)
    {
        if (other.gameObject.GetComponent<IHoldableObject>() != null)
        {
            other.gameObject.GetComponent<IHoldableObject>().Respawn();
            return;
        }

        if (other.gameObject.GetComponent<PlayerReferences>())
        {
            other.gameObject.GetComponent<PlayerReferences>().RespawnAtSpawnPosition();
        }
    }
}
