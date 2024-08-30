using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    [Header("Pickup Capsule")]
    [SerializeField] private float pickupHeight = .7f;
    [SerializeField] private float pickupRadius = 1.5f;
    [SerializeField] private Vector3 pickupOffset = Vector3.zero;

    [Header("Drop Capsule")]
    [SerializeField] private float dropHeight = 1f;
    [SerializeField] private float dropRadius = 1f;
    [SerializeField] private Vector3 dropOffset = Vector3.zero;

    [Header("Settings")]
    [SerializeField] private LayerMask dropLayerMask;
    [SerializeField] private Transform pickedUpParent = null;
    [SerializeField][ShowOnly] IHoldableObject currentHoldableObject;
    [SerializeField][ShowOnly] GameObject currentHoldableGameObject;
    [SerializeField][ShowOnly] private IHoldableObject lastHighlightedObject;
    [SerializeField][ShowOnly] private GameObject lastHighlightedGameObject;
    [SerializeField][ShowOnly] private bool isHolding = false;


    [SerializeField][ShowOnly] private Vector3 pickupCapsuleStart, pickupCapsuleEnd;
    [SerializeField][ShowOnly] private Vector3 dropCapsuleStart, dropCapsuleEnd;
    [Header("Gizmos")]
    [SerializeField] private Color gizmoColor = Color.red;
    [SerializeField] private bool showGizmos = true;


    private void Update()
    {
        UpdateCapsules();
        CheckForHoldable();
        //CheckDropPlacementZone();
    }

    /// <summary>
    /// Updates Caspules for pick and drop
    /// </summary>
    private void UpdateCapsules()
    {
        if (!isHolding)
        {
            pickupCapsuleStart = pickedUpParent.position + pickupOffset - Vector3.up * (pickupHeight / 2);
            pickupCapsuleEnd = pickedUpParent.position + pickupOffset + Vector3.up * (pickupHeight / 2);
        }
        else
        {
            dropCapsuleStart = pickedUpParent.position + dropOffset - Vector3.up * (dropHeight / 2);
            dropCapsuleEnd = pickedUpParent.position + dropOffset + Vector3.up * (dropHeight / 2);
        }
    }

    /// <summary>
    /// Checks for holdable objects
    /// </summary>
    private void CheckForHoldable()
    {

        if (isHolding) return;

        RaycastHit[] hits = Physics.CapsuleCastAll(pickupCapsuleStart, pickupCapsuleEnd, pickupRadius, Vector3.forward, 0f);
        float closestDistance = float.MaxValue;
        currentHoldableObject = null;
        currentHoldableGameObject = null;

        foreach (var hit in hits)
        {
            IHoldableObject holdableObject = hit.collider.GetComponent<IHoldableObject>();
            if (holdableObject != null && !holdableObject.IsBeingHeld())
            {
                Vector3 closestPointOnCapsule = ClosestPointOnLineSegment(pickupCapsuleStart, pickupCapsuleEnd, holdableObject.ObjectBeingHeld().transform.position);
                float distance = Vector3.Distance(closestPointOnCapsule, holdableObject.ObjectBeingHeld().transform.position);

                if (distance <= pickupRadius && distance < closestDistance)
                {
                    closestDistance = distance;
                    currentHoldableObject = holdableObject;
                    currentHoldableGameObject = currentHoldableObject.ObjectBeingHeld();
                }

            }
        }

        if (currentHoldableObject != null)
        {
            if (lastHighlightedObject != currentHoldableObject)
            {
                UnhighlightLastObject();
                HighlightObject(currentHoldableObject.ObjectBeingHeld());
                lastHighlightedObject = currentHoldableObject;
                lastHighlightedGameObject = currentHoldableGameObject;
            }
            else
            {
                UnhighlightLastObject();
                HighlightObject(currentHoldableObject.ObjectBeingHeld());
                lastHighlightedObject = currentHoldableObject;
                lastHighlightedGameObject = currentHoldableGameObject;
            }
        }
        else
        {
            UnhighlightLastObject();
        }
    }

    /// <summary>
    /// Checks the closes object to the player
    /// </summary>
    /// <param name="lineStart"></param>
    /// <param name="lineEnd"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    private Vector3 ClosestPointOnLineSegment(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        Vector3 lineDirection = lineEnd - lineStart;
        float lineLength = lineDirection.magnitude;
        lineDirection.Normalize();

        float projectLength = Vector3.Dot(point - lineStart, lineDirection);
        projectLength = Mathf.Clamp(projectLength, 0f, lineLength);

        return lineStart + lineDirection * projectLength;
    }

    /// <summary>
    /// Picks up the current holdable object
    /// </summary>
    private void PickUp()
    {
        if (currentHoldableObject != null && CanPickUp())
        {
            UnhighlightObject(currentHoldableObject.ObjectBeingHeld());

            CheckPickUpPlacementZone(currentHoldableObject);

            currentHoldableObject.SetIsBeingHeld(true);

            if (currentHoldableObject.NavMeshAgentOfObject() != null)
                currentHoldableObject.NavMeshAgentOfObject().enabled = false;
            if (currentHoldableObject.RigidbodyOfObject() != null)
                currentHoldableObject.RigidbodyOfObject().isKinematic = true;


            currentHoldableObject.ObjectBeingHeld().transform.SetParent(pickedUpParent);
            currentHoldableObject.ObjectBeingHeld().transform.localPosition = Vector3.zero;
            currentHoldableObject.ObjectBeingHeld().transform.localRotation = Quaternion.identity;

            isHolding = true;
        }
    }

    /// <summary>
    /// Checks if the holdable object can be picked up
    /// </summary>
    /// <returns></returns>
    private bool CanPickUp()
    {
        Collider[] colliders = Physics.OverlapCapsule(pickupCapsuleStart, pickupCapsuleEnd, pickupRadius);

        foreach (Collider collider in colliders)
        {
            IHoldableObject holdableObject = collider.GetComponent<IHoldableObject>();
            if (holdableObject != null && !holdableObject.IsBeingHeld())
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Drops the current object held
    /// </summary>
    private void Drop()
    {
        if (currentHoldableObject != null && CanDrop())
        {
            currentHoldableObject.SetIsBeingHeld(false);
            currentHoldableObject.ObjectBeingHeld().transform.SetParent(null);

            if (currentHoldableObject.NavMeshAgentOfObject() != null)
                currentHoldableObject.NavMeshAgentOfObject().enabled = true;
            if (currentHoldableObject.RigidbodyOfObject() != null)
                currentHoldableObject.RigidbodyOfObject().isKinematic = false;

            //if (CheckDropPlacementZone())
            //{
            DropPlacementZone(currentHoldableObject);
            //}

            currentHoldableObject = null;
            currentHoldableGameObject = null;
            isHolding = false;
        }
    }

    /// <summary>
    /// Returns a bool if the curentobject held can be dropped
    /// </summary>
    /// <returns></returns>
    private bool CanDrop()
    {
        if (currentHoldableObject == null)
            return false;

        Collider[] colliders = Physics.OverlapCapsule(dropCapsuleStart, dropCapsuleEnd, dropRadius);

        foreach (Collider collider in colliders)
        {
            if (collider != currentHoldableObject.ColliderOfObject())
            {
                if (((1 << collider.gameObject.layer) & dropLayerMask) == 0)
                    return false;
            }
        }

        return true;
    }

    //private bool CheckDropPlacementZone()
    //{
    //    if (!isHolding)
    //        return false;

    //    Collider[] colliders = Physics.OverlapCapsule(dropCapsuleStart, dropCapsuleEnd, dropRadius);
    //    PlacementZone placementZone = null;
    //    foreach (Collider collider in colliders)
    //    {
    //        placementZone = collider.GetComponent<PlacementZone>();
    //        if (placementZone != null)
    //        {
    //            if (placementZone.GetComponent<HighlightObject>())
    //            {
    //                HighlightObject(placementZone.gameObject);
    //            }
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    private void DropPlacementZone(IHoldableObject holdableObject)
    {
        Collider[] colliders = Physics.OverlapCapsule(dropCapsuleStart, dropCapsuleEnd, dropRadius);

        foreach (Collider collider in colliders)
        {
            PlacementZone placementZone = collider.GetComponent<PlacementZone>();
            if (placementZone != null && !placementZone.IsOccupied && placementZone.ObjOnPlacementZone == null)
            {
                placementZone.PlaceObject(holdableObject);
                break;
            }
        }
    }

    private void CheckPickUpPlacementZone(IHoldableObject holdableObject)
    {
        Collider[] colliders = Physics.OverlapCapsule(pickupCapsuleStart, pickupCapsuleEnd, pickupRadius);

        foreach (Collider collider in colliders)
        {
            PlacementZone placementZone = collider.GetComponent<PlacementZone>();
            if (placementZone != null && placementZone.IsOccupied && holdableObject == placementZone.ObjOnPlacementZone)
            {
                placementZone.RemoveObject(holdableObject);
                break;
            }
        }
    }


    public void OnInteract(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            if (isHolding)
            {
                Drop();
            }
            else
            {
                PickUp();
            }
        }
    }

    private void HighlightObject(GameObject highlightobject)
    {
        var highlightScript = highlightobject.GetComponent<HighlightObject>();
        if (highlightScript != null)
        {
            highlightScript.Highlight();
        }
    }

    private void UnhighlightObject(GameObject highlightobject)
    {
        var highlightScript = highlightobject.GetComponent<HighlightObject>();
        if (highlightScript != null)
        {
            highlightScript.Unhighlight();
        }
    }

    private void UnhighlightLastObject()
    {
        if (lastHighlightedObject != null)
        {
            UnhighlightObject(lastHighlightedObject.ObjectBeingHeld());
            lastHighlightedObject = null;
            lastHighlightedGameObject = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        if (!isHolding)
        {
            Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(pickupCapsuleStart, pickupRadius);
            Gizmos.DrawWireSphere(pickupCapsuleEnd, pickupRadius);
            Gizmos.DrawLine(pickupCapsuleStart + Vector3.forward * pickupRadius, pickupCapsuleEnd + Vector3.forward * pickupRadius);
            Gizmos.DrawLine(pickupCapsuleStart - Vector3.forward * pickupRadius, pickupCapsuleEnd - Vector3.forward * pickupRadius);
            Gizmos.DrawLine(pickupCapsuleStart + Vector3.right * pickupRadius, pickupCapsuleEnd + Vector3.right * pickupRadius);
            Gizmos.DrawLine(pickupCapsuleStart - Vector3.right * pickupRadius, pickupCapsuleEnd - Vector3.right * pickupRadius);
        }
        else
        {
            Gizmos.color = CanDrop() ? Color.green : Color.red;

            Gizmos.DrawWireSphere(dropCapsuleStart, dropRadius);
            Gizmos.DrawWireSphere(dropCapsuleEnd, dropRadius);
            Gizmos.DrawLine(dropCapsuleStart + Vector3.forward * dropRadius, dropCapsuleEnd + Vector3.forward * dropRadius);
            Gizmos.DrawLine(dropCapsuleStart - Vector3.forward * dropRadius, dropCapsuleEnd - Vector3.forward * dropRadius);
            Gizmos.DrawLine(dropCapsuleStart + Vector3.right * dropRadius, dropCapsuleEnd + Vector3.right * dropRadius);
            Gizmos.DrawLine(dropCapsuleStart - Vector3.right * dropRadius, dropCapsuleEnd - Vector3.right * dropRadius);
        }
    }


}
