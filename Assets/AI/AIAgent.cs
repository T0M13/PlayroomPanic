using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AI_Needs;
using AI_Traits;

public class AIAgent : MonoBehaviour, IHoldableObject
{
    [Header("References")]
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Collider aiCollider;
    [SerializeField] private Rigidbody aiBody;
    [Header("Tasks")]
    public List<AIBrain> availableTasks = new List<AIBrain>();
    [SerializeField][ShowOnly] private AIBrain currentTask;

    [Header("Needs")]
    public Energy energy = new Energy();

    [Header("Personality Traits")]
    [Tooltip("Increases the likelihood of engaging in playful activities, such as running around, playing with toys, or interacting with other children.")]
    public Playfulness playfulness = new Playfulness();

    [Header("Settings")]
    [SerializeField] private Vector3 spawnPoint;
    [SerializeField] private Vector3 spawnPointOffset;
    [SerializeField][ShowOnly] private bool isPickedUp = false;
    [SerializeField][ShowOnly] private bool shouldFixate = true;
    [SerializeField][ShowOnly] private bool isPlaced = false;

    [Header("Gizmos")]
    [SerializeField] private bool showGizmos = true;

    public NavMeshAgent NavMeshAgent { get => navMeshAgent; set => navMeshAgent = value; }
    public AIBrain CurrentTask { get => currentTask; set => currentTask = value; }

    private void OnValidate()
    {
        GetReferences();
    }

    private void Awake()
    {
        GetReferences();
    }

    private void Start()
    {
        SetSpawnPoint(transform.position);
    }

    private void GetReferences()
    {
        if (NavMeshAgent == null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        if (aiCollider == null)
        {
            aiCollider = GetComponent<CapsuleCollider>();
        }
        if (aiBody == null)
        {
            aiBody = GetComponent<Rigidbody>();
        }
    }

    private void Update()
    {
        if (isPickedUp || isPlaced) return;
        if (currentTask == null || currentTask.ShouldSwitch(this))
        {
            currentTask = ChooseBestTask();
        }

        currentTask?.DoAction(this);
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        if (currentTask != null)
        {
            currentTask.DrawGizmos(this);
        }
    }

    private AIBrain ChooseBestTask()
    {
        AIBrain bestTask = null;
        float highestPriority = float.MinValue;

        foreach (var task in availableTasks)
        {
            float taskPriority = task.CalculatePriority(this);
            if (taskPriority > highestPriority)
            {
                highestPriority = taskPriority;
                bestTask = task;
            }
        }

        return bestTask;
    }

    public bool IsBeingHeld()
    {
        return isPickedUp;
    }

    public GameObject ObjectBeingHeld()
    {
        return this.gameObject;
    }

    public void SetIsBeingHeld(bool value)
    {
        isPickedUp = value;
    }

    public NavMeshAgent NavMeshAgentOfObject()
    {
        return navMeshAgent;
    }

    public Rigidbody RigidbodyOfObject()
    {
        return aiBody;
    }

    public Collider ColliderOfObject()
    {
        return aiCollider;
    }
    public void SetSpawnPoint(Vector3 position)
    {
        spawnPoint = position;
    }

    public void Respawn()
    {
        transform.position = spawnPoint + spawnPointOffset;
    }

    public bool ShouldFixate()
    {
        return shouldFixate;
    }

    public bool IsPlaced()
    {
        return isPlaced;
    }

    public void SetIsPlaced(bool value)
    {
       isPlaced = value;
    }
}
