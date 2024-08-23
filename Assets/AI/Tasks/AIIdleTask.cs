using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AI/Tasks/IdleTask")]
public class AIIdleTask : AIBrain
{
    [SerializeField] private float idleRadius = 5f;
    [SerializeField] private float arrivalThreshold = 0.5f;
    [SerializeField] private float energyDecreaseMutliplier = 1f;
    [SerializeField] private Vector3 currentRandomTargetPoint = Vector3.zero;

    private void OnEnable()
    {
        currentRandomTargetPoint = Vector3.zero;
    }

    private void OnDisable()
    {
        currentRandomTargetPoint = Vector3.zero;
    }

    public override void DoAction(AIAgent ai)
    {
        if (currentRandomTargetPoint == Vector3.zero || IsCloseToDestination(ai))
        {
            currentRandomTargetPoint = GetRandomPointWithinRadius(ai.transform.position, idleRadius);
            MoveToRandomPoint(ai, currentRandomTargetPoint);
        }

        if (ai.energy != null)
        {
            ai.energy.Decrease(energyDecreaseMutliplier);  

            if (ai.energy.CurrentValue <= ai.energy.TiredThreshold)
            {
                ai.NavMeshAgent.isStopped = true;
            }
        }
    }

    private bool IsCloseToDestination(AIAgent ai)
    {
        return !ai.NavMeshAgent.pathPending &&
               ai.NavMeshAgent.remainingDistance <= arrivalThreshold;
    }

    private Vector3 GetRandomPointWithinRadius(Vector3 origin, float radius)
    {
        Vector2 randomPoint = Random.insideUnitCircle * radius;
        Vector3 targetPoint = new Vector3(randomPoint.x, origin.y, randomPoint.y);
        return origin + targetPoint;
    }

    private void MoveToRandomPoint(AIAgent ai, Vector3 randomPoint)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, idleRadius, NavMesh.AllAreas))
        {
            ai.NavMeshAgent.SetDestination(hit.position);
        }
        else
        {
            Debug.LogWarning("Failed to find a valid point on the NavMesh");
        }
    }

    public override float CalculatePriority(AIAgent ai)
    {
        return ai.energy.CurrentValue;
    }

    public override bool ShouldSwitch(AIAgent ai)
    {
        return ai.energy.CurrentValue <= ai.energy.TiredThreshold;
    }

    public override void DrawGizmos(AIAgent ai)
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(ai.transform.position, idleRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(ai.transform.position, arrivalThreshold);

        if (ai.NavMeshAgent != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(ai.transform.position, currentRandomTargetPoint);
            Gizmos.DrawSphere(currentRandomTargetPoint, 0.2f);
        }
    }
}
