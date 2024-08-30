using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlacementZone : MonoBehaviour
{
    [SerializeField] private Transform placementPoint;
    [SerializeField] private IHoldableObject objOnPlacementZone = null;
    [SerializeField][ShowOnly] private GameObject gobjOnPlacementZone = null;
    [SerializeField] private bool isOccupied = false;

    public bool IsOccupied { get => isOccupied; set => isOccupied = value; }
    public IHoldableObject ObjOnPlacementZone { get => objOnPlacementZone; set => objOnPlacementZone = value; }

    public void PlaceObject(IHoldableObject holdableObject)
    {
        if (IsOccupied) return;
        else
        {
            if (holdableObject.ShouldFixate() && objOnPlacementZone == null)
            {
                holdableObject.ObjectBeingHeld().transform.SetParent(null);
                holdableObject.ObjectBeingHeld().transform.position = placementPoint.position;
                holdableObject.ObjectBeingHeld().transform.rotation = placementPoint.rotation;

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

                holdableObject.SetIsPlaced(true);
                ObjOnPlacementZone = holdableObject;
                gobjOnPlacementZone = objOnPlacementZone.ObjectBeingHeld();
                IsOccupied = true;
            }
        }

    }

    public void RemoveObject(IHoldableObject holdableObject)
    {
        if (!IsOccupied) return;
        else
        {
            if (holdableObject.IsPlaced() && holdableObject == ObjOnPlacementZone)
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

                holdableObject.SetIsPlaced(false);
                ObjOnPlacementZone = null;
                gobjOnPlacementZone = null;
                IsOccupied = false;

            }
        }

    }
}
