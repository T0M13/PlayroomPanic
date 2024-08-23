using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AI_Needs;
using AI_Traits;

public class AIAgent : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent navMeshAgent;
    [Header("Tasks")]
    public List<AIBrain> availableTasks = new List<AIBrain>();
    [SerializeField][ShowOnly] private AIBrain currentTask;

    [Header("Needs")]
    public Energy energy = new Energy();

    [Header("Personality Traits")]
    [Tooltip("Increases the likelihood of engaging in playful activities, such as running around, playing with toys, or interacting with other children.")]
    public Playfulness playfulness = new Playfulness();

    [Header("Settings")]
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

    private void GetReferences()
    {
        if (NavMeshAgent == null)
        {
            GetComponent<NavMeshAgent>();
        }
    }

    private void Update()
    {
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

}
