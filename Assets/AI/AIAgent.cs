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
    [SerializeField] private ChildUI childUI;
    [SerializeField] private AIInteractor aiinteractor;
    [Header("Tasks")]
    public List<AIBrain> availableTasks = new List<AIBrain>();
    [SerializeField] private AIBrain currentTask;
    [Header("Status")]
    [SerializeField][Range(1, 6)] private int age = 3;
    [Header("Needs")]
    [SerializeField][ShowOnly] private bool hasUnmetNeed = false;
    public Energy energy = new Energy();
    public ToiletNeed toilet = new ToiletNeed();

    //[Header("Personality Traits")]
    //[Tooltip("Increases the likelihood of engaging in playful activities, such as running around, playing with toys, or interacting with other children.")]
    //public Playfulness playfulness = new Playfulness();

    [Header("Settings")]
    [SerializeField] private PlacementCategory placementCategory;
    [SerializeField] private Vector3 spawnPoint;
    [SerializeField] private Vector3 spawnPointOffset;
    [SerializeField][ShowOnly] private bool isPickedUp = false;
    [SerializeField][ShowOnly] private bool shouldFixate = true;
    [SerializeField][ShowOnly] private bool isPlaced = false;

    [Header("Gizmos")]
    [SerializeField] private bool showGizmos = true;

    public NavMeshAgent NavMeshAgent { get => navMeshAgent; set => navMeshAgent = value; }
    public AIBrain CurrentTask { get => currentTask; set => currentTask = value; }
    public int Age { get => age; set => age = value; }
    public ChildUI ChildUI { get => childUI; set => childUI = value; }
    public AIInteractor Aiinteractor { get => aiinteractor; set => aiinteractor = value; }

    private void OnEnable()
    {
        GameManager gameManager = null;
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }

        if (gameManager != null)
        {
            gameManager.onTimeOfDayChange += UpdateNeedsOnTimeOfDayChange;
            gameManager.onHourIncrement += UpdateNeedsOnTheHour;
        }

    }

    private void OnDisable()
    {
        GameManager gameManager = null;
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }

        if (gameManager != null)
        {
            gameManager.onTimeOfDayChange -= UpdateNeedsOnTimeOfDayChange;
            gameManager.onHourIncrement -= UpdateNeedsOnTheHour;
        }

    }

    private void UpdateNeedsOnTimeOfDayChange()
    {

    }

    private void UpdateNeedsOnTheHour()
    {
        toilet.TryDecrease(this);
    }

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
        if (childUI == null)
        {
            childUI = GetComponentInChildren<ChildUI>();
        }
        if (Aiinteractor == null)
        {
            Aiinteractor = GetComponentInChildren<AIInteractor>();
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

    public void SetUrgentTask(AIBrain task)
    {
        currentTask = task;
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

    public void CheckNeeds()
    {
        if (toilet.NeedsDiaperChange)
        {
            EnterNeedState("Needs diaper change.");
        }

    }

    public void EnterNeedState(string message)
    {
        navMeshAgent.isStopped = true;
        hasUnmetNeed = true;
        //currentTask = waitingTask; // Switch to the waiting task
        Debug.Log($"{gameObject.name} is waiting: {message}");
    }

    public void ResolveNeed()
    {
        hasUnmetNeed = false;
        navMeshAgent.isStopped = false;
        //currentTask = ChooseBestTask();
        Debug.Log($"{gameObject.name} need resolved.");
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

    public IInteractable InteractableOfObject()
    {
        return GetComponent<IInteractable>();
    }

    public PlacementCategory GetPlacementCategory()
    {
        return placementCategory;
    }
}
