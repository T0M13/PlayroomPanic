using UnityEngine;

[CreateAssetMenu(menuName = "AI/Tasks/RestTask")]
public class AIRestTask : AIBrain
{
    [SerializeField] private float energyReplenishMutliplier = 1f;

    public override void DoAction(AIAgent ai)
    {
        ai.NavMeshAgent.isStopped = true;

        if (ai.energy != null)
        {
            ai.energy.Replenish(energyReplenishMutliplier);
            if (ai.energy.CurrentValue >= ai.energy.ActiveThreshold)
            {
                ai.NavMeshAgent.isStopped = false;
            }
        }
    }

    public override float CalculatePriority(AIAgent ai)
    {
        return 100f - ai.energy.CurrentValue;
    }

    public override bool ShouldSwitch(AIAgent ai)
    {
        return ai.energy.CurrentValue >= ai.energy.ActiveThreshold;
    }


    public override void DrawGizmos(AIAgent ai)
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(ai.transform.position, 0.5f);
    }
}



