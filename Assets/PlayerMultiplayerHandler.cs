using UnityEngine;
using Unity.Netcode;

public class PlayerMultiplayerHandler : NetworkBehaviour
{
    [Header("References")]
    private PlayerReferences playerRefs;

    private void Start()
    {
        playerRefs = GetComponent<PlayerReferences>();

        SpawnPointManager spawnManager = FindObjectOfType<SpawnPointManager>();

        if (spawnManager != null && IsOwner)  // Only the owner sets the spawn position
        {
            Transform spawnPoint = spawnManager.GetRandomSpawnPoint();

            if (spawnPoint != null)
            {
                transform.position = spawnPoint.position;
                transform.rotation = spawnPoint.rotation;
            }
        }

        if (playerRefs.PlayerRigidBody != null)
        {
            playerRefs.PlayerRigidBody.isKinematic = false;
        }
    }
}
