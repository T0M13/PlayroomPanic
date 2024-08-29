using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlacementZone : MonoBehaviour
{
    [SerializeField] private Transform placementPoint;
    [SerializeField] private bool isOccupied = false;

    public void PlaceObject(IHoldableObject holdableObject)
    {
        if (isOccupied) return;

        holdableObject.ObjectBeingHeld().transform.SetParent(null);
        holdableObject.ObjectBeingHeld().transform.position = placementPoint.position;
        holdableObject.ObjectBeingHeld().transform.rotation = placementPoint.rotation;

        // Additional logic, like snapping or animations, can go here

        if (holdableObject.ShouldFixate())
        {
            Rigidbody rb = holdableObject.RigidbodyOfObject();
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            NavMeshAgent navAgent = holdableObject.NavMeshAgentOfObject();
            if (navAgent != null)
            {
                navAgent.enabled = false;
            }

        }

        isOccupied = true;

    }

    public void RemoveObject(IHoldableObject holdableObject)
    {
        if (!isOccupied) return;

        if (holdableObject.IsPlaced())
        {
            Rigidbody holdableObjctRB = holdableObject.RigidbodyOfObject();
            if (holdableObjctRB != null)
            {
                holdableObjctRB.isKinematic = false;
            }

            NavMeshAgent holdableNavAgent = holdableObject.NavMeshAgentOfObject();
            if (holdableNavAgent != null)
            {
                holdableNavAgent.enabled = true;
            }

        }
        isOccupied = false;
    }
}
