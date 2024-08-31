using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlacementZone : MonoBehaviour
{
    [Header("Placement Settings")]
    [SerializeField] private Transform placementPoint;
    [SerializeField] private IHoldableObject objOnPlacementZone = null;
    [SerializeField][ShowOnly] private GameObject gobjOnPlacementZone = null;
    [SerializeField] private bool isOccupied = false;

    [Header("Layer Settings")]
    [SerializeField] private LayerMask placementLayer;
    [SerializeField][ShowOnly] private int lastOriginalLayer;

    public bool IsOccupied { get => isOccupied; set => isOccupied = value; }
    public IHoldableObject ObjOnPlacementZone { get => objOnPlacementZone; set => objOnPlacementZone = value; }

    public virtual void PlaceObject(IHoldableObject holdableObject)
    {
        if (IsOccupied) return;
        if (!holdableObject.ShouldFixate() && objOnPlacementZone != null) return;

        holdableObject.ObjectBeingHeld().transform.SetParent(null);
        holdableObject.ObjectBeingHeld().transform.position = placementPoint.position;
        holdableObject.ObjectBeingHeld().transform.rotation = placementPoint.rotation;

        lastOriginalLayer = holdableObject.ObjectBeingHeld().layer;
        holdableObject.ObjectBeingHeld().layer = LayerMask.NameToLayer(LayerMask.LayerToName(placementLayer.value));

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

        IInteractable interactable = holdableObject.InteractableOfObject();
        if (interactable != null)
        {
            if (interactable.IsOnlyInteractableWhenPlaced())
                interactable.SetInteractable(true);
        }

        holdableObject.SetIsPlaced(true);
        ObjOnPlacementZone = holdableObject;
        gobjOnPlacementZone = objOnPlacementZone.ObjectBeingHeld();
        IsOccupied = true;

    }

    public virtual void RemoveObject(IHoldableObject holdableObject)
    {
        if (!IsOccupied) return;
        if (!holdableObject.IsPlaced() && holdableObject != ObjOnPlacementZone) return;

        holdableObject.ObjectBeingHeld().layer = lastOriginalLayer;

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

        IInteractable interactable = holdableObject.InteractableOfObject();
        if (interactable != null)
        {
            if (interactable.IsOnlyInteractableWhenPlaced())
                interactable.SetInteractable(false);
        }

        holdableObject.SetIsPlaced(false);
        ObjOnPlacementZone = null;
        gobjOnPlacementZone = null;
        IsOccupied = false;

    }

}
