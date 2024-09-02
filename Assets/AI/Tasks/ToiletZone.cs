using System.Collections.Generic;
using UnityEngine;

public class ToiletZone : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float zoneRadius = 2f;
    [SerializeField] private bool isOccupied = false;
    [SerializeField][ShowOnly] private AIAgent currentAI;

    public float ZoneRadius { get => zoneRadius; set => zoneRadius = value; }
    public bool IsOccupied { get => isOccupied; set => isOccupied = value; }
    public AIAgent CurrentAI { get => currentAI; set => currentAI = value; }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsOccupied ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, zoneRadius);
    }
}
