using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Tasks/Toilet Task")]
public class AIToiletTask : AIBrain
{
    [Header("Task Settings")]
    [SerializeField] private float acceptableDistance = 1.5f;
    [SerializeField] private float waitForToiletSeconds = 3f;

    public override void DoAction(AIAgent ai)
    {
        if (ai.Age >= ai.toilet.AcceptableAgeForToilet)
        {
            if (ai.toilet.CurrentToilet == null || ai.toilet.CurrentToilet.IsOccupied && ai.toilet.CurrentToilet.CurrentAI != ai)
            {
                ai.toilet.CurrentToilet = FindNearestAvailableToilet(ai);

                if (ai.toilet.CurrentToilet == null && !ai.toilet.IsWaitingForToilet)
                {
                    ai.StartCoroutine(WaitAndRetryToilet(ai));
                }

                return;
            }

            MoveToToilet(ai);

            if (Vector3.Distance(ai.transform.position, ai.toilet.CurrentToilet.transform.position) <= acceptableDistance)
            {
                UseToilet(ai);
            }
        }

        if (!ai.toilet.NeedsDiaperChange && ai.Age < ai.toilet.AcceptableAgeForToilet)
        {
            ai.NavMeshAgent.isStopped = true;
            ai.toilet.NeedsDiaperChange = true;
        }

    }

    private void MoveToToilet(AIAgent ai)
    {
        if (!ai.toilet.IsWaitingForToilet)
        {
            ai.NavMeshAgent.SetDestination(ai.toilet.CurrentToilet.transform.position);
        }
    }

    private void UseToilet(AIAgent ai)
    {
        if (!ai.toilet.CurrentToilet.IsOccupied)
        {
            ai.toilet.CurrentToilet.IsOccupied = true;
            ai.toilet.CurrentToilet.CurrentAI = ai;
            ai.toilet.IsUsingToilet = true;
        }

        if (ai.toilet.CurrentToilet.IsOccupied && ai.toilet.CurrentToilet.CurrentAI == ai)
        {
            ai.NavMeshAgent.isStopped = true;
            ai.toilet.ReplenishByTime(1f);

            if (ai.toilet.CurrentValue >= ai.toilet.SufficientThreshold)
            {
                ResetToiletUsage(ai);
            }
        }
    }

    private void ResetToiletUsage(AIAgent ai)
    {
        ai.CurrentTask = null;
        ai.NavMeshAgent.isStopped = false;
        ai.toilet.IsUsingToilet = false;
        ai.toilet.CurrentToilet.IsOccupied = false;
        ai.toilet.CurrentToilet.CurrentAI = null;
    }

    private ToiletZone FindNearestAvailableToilet(AIAgent ai)
    {
        ToiletZone[] toilets = GameObject.FindObjectsOfType<ToiletZone>();
        ToiletZone closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (ToiletZone toilet in toilets)
        {
            if (!toilet.IsOccupied)
            {
                float distance = Vector3.Distance(ai.transform.position, toilet.transform.position);
                if (distance < closestDistance)
                {
                    closest = toilet;
                    closestDistance = distance;
                }
            }
        }

        return closest;
    }

    private IEnumerator WaitAndRetryToilet(AIAgent ai)
    {
        ai.toilet.IsWaitingForToilet = true;
        ai.NavMeshAgent.isStopped = true;
        yield return new WaitForSeconds(waitForToiletSeconds);

        ai.toilet.CurrentToilet = FindNearestAvailableToilet(ai);

        if (ai.toilet.CurrentToilet == null)
        {
            ai.StartCoroutine(WaitAndRetryToilet(ai));
        }
        else
        {
            ai.toilet.IsWaitingForToilet = false;
            ai.NavMeshAgent.isStopped = false;
            MoveToToilet(ai);
        }
    }


    public override void DrawGizmos(AIAgent ai)
    {
        if (ai.Age < ai.toilet.AcceptableAgeForToilet) return;

        if (ai.toilet.CurrentToilet != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(ai.transform.position, ai.toilet.CurrentToilet.transform.position);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(ai.transform.position, acceptableDistance);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(ai.toilet.CurrentToilet.transform.position, ai.toilet.CurrentToilet.ZoneRadius);
        }
        else
        {
            ai.toilet.CurrentToilet = FindNearestAvailableToilet(ai);
        }
    }

    public override float CalculatePriority(AIAgent ai)
    {
        return 100f - ai.toilet.CurrentValue;
    }

    public override bool ShouldSwitch(AIAgent ai)
    {
        return ai.toilet.CurrentValue >= ai.toilet.SufficientThreshold;
    }
}
