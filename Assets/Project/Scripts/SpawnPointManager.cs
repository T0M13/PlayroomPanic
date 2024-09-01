using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] spawnPoints;  

    public Transform GetRandomSpawnPoint()
    {
        if (spawnPoints.Length > 0)
        {
            return spawnPoints[Random.Range(0, spawnPoints.Length)];
        }
        else
        {
            Debug.LogError("No spawn points assigned in the SpawnPointManager!");
            return null;
        }
    }
}
