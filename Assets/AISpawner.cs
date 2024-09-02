using Mono.CSharp.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AISpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject aiPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("AI Management")]
    [SerializeField] private int aiAmount = 5;
    [SerializeField] private List<AIAgent> spawnedAis = new List<AIAgent>();

    private void OnEnable()
    {
        GetReferences();
        if (gameManager != null)
        {
            gameManager.onHourIncrement += CheckAndSpawnAIs;
        }
    }

    private void OnDisable()
    {
        GetReferences();
        if (gameManager != null)
        {
            gameManager.onHourIncrement -= CheckAndSpawnAIs;
        }
    }

    private void OnValidate()
    {
        GetReferences();
    }

    private void Awake()
    {
        GetReferences();
    }

    private void GetReferences()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }
    }

    private void CheckAndSpawnAIs()
    {
        if (gameManager.CurrentTimeOfDay == TimeOfDay.Opening)
        {
            SpawnAIsForArrival();
        }
    }

    private void SpawnAIsForArrival()
    {
        for (int i = 0; i < aiAmount; i++)
        {
            GameObject aiObject = Instantiate(aiPrefab, spawnPoint.position, Quaternion.identity);
            AIAgent aiAgent = aiObject.GetComponent<AIAgent>();
            spawnedAis.Add(aiAgent);

            AssignInitialTask(aiAgent);
        }
    }

    private void AssignInitialTask(AIAgent ai)
    {
        //if (ai.Age >= 3)
        //{
        //    //ai.CurrentTask = AIDressingRoomTask;
        //}
        //else
        //{
        //    ai.CurrentTask = ai.availableTasks.Find(task => task.GetType() == typeof(AIIdleTask));
        //    Debug.Log($"{ai.gameObject.name} needs player assistance.");
        //}
    }
}
