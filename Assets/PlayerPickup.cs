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
    [Header("Gizmos")]
    [SerializeField] private Color gizmoColor = Color.red;
    [SerializeField] private bool showGizmos = true;

    [SerializeField][ShowOnly] IHoldableObject currentHoldableObject;
    [SerializeField][ShowOnly] private bool isHolding = false;


    private void CheckForHoldable()
    {
        Vector3 capsuleStart = pickedUpParent.position + pickupOffset - Vector3.up * (pickupHeight / 2);
        Vector3 capsuleEnd = pickedUpParent.position + pickupOffset + Vector3.up * (pickupHeight / 2);

        RaycastHit[] hits = Physics.CapsuleCastAll(capsuleStart, capsuleEnd, pickupRadius, Vector3.forward, 0f);
        float closestDistance = float.MaxValue;
        currentHoldableObject = null;

        foreach (var hit in hits)
        {
            IHoldableObject holdableObject = hit.collider.GetComponent<IHoldableObject>();
            if (holdableObject != null && !holdableObject.IsBeingHeld())
            {
                Vector3 closestPointOnCapsule = ClosestPointOnLineSegment(capsuleStart, capsuleEnd, holdableObject.ObjectBeingHeld().transform.position);
                float distance = Vector3.Distance(closestPointOnCapsule, holdableObject.ObjectBeingHeld().transform.position);

                if (distance <= pickupRadius && distance < closestDistance)
                {
                    closestDistance = distance;
                    currentHoldableObject = holdableObject;
                }

            }
        }
    }

    private Vector3 ClosestPointOnLineSegment(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        Vector3 lineDirection = lineEnd - lineStart;
        float lineLength = lineDirection.magnitude;
        lineDirection.Normalize();

        float projectLength = Vector3.Dot(point - lineStart, lineDirection);
        projectLength = Mathf.Clamp(projectLength, 0f, lineLength);

        return lineStart + lineDirection * projectLength;
    }


    private void PickUp()
    {
        if (currentHoldableObject != null && CanPickUp())
        {
            UnhighlightObject(currentHoldableObject);

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

    private bool CanPickUp()
    {


        Vector3 capsuleStart = pickedUpParent.position + pickupOffset - Vector3.up * (pickupHeight / 2);
        Vector3 capsuleEnd = pickedUpParent.position + pickupOffset + Vector3.up * (pickupHeight / 2);

        Collider[] colliders = Physics.OverlapCapsule(capsuleStart, capsuleEnd, pickupRadius);

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

            CheckDropPlacementZone(currentHoldableObject);

            currentHoldableObject = null;
            isHolding = false;
        }
    }


    private bool CanDrop()
    {
        if (currentHoldableObject == null)
        {
            return false;
        }

        Vector3 capsuleStart = pickedUpParent.position + dropOffset - Vector3.up * (dropHeight / 2);
        Vector3 capsuleEnd = pickedUpParent.position + dropOffset + Vector3.up * (dropHeight / 2);

        Collider[] colliders = Physics.OverlapCapsule(capsuleStart, capsuleEnd, dropRadius);

        foreach (Collider collider in colliders)
        {

            if (collider != currentHoldableObject.ColliderOfObject())
            {
                if (((1 << collider.gameObject.layer) & dropLayerMask) == 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void CheckDropPlacementZone(IHoldableObject holdableObject)
    {
        Vector3 capsuleStart = pickedUpParent.position + pickupOffset - Vector3.up * (pickupHeight / 2);
        Vector3 capsuleEnd = pickedUpParent.position + pickupOffset + Vector3.up * (pickupHeight / 2);

        Collider[] colliders = Physics.OverlapCapsule(capsuleStart, capsuleEnd, dropRadius);

        foreach (Collider collider in colliders)
        {
            PlacementZone placementZone = collider.GetComponent<PlacementZone>();
            if (placementZone != null)
            {
                placementZone.PlaceObject(holdableObject);
                holdableObject.SetIsPlaced(true);
                break;
            }
        }
    }

    private void CheckPickUpPlacementZone(IHoldableObject holdableObject)
    {
        Vector3 capsuleStart = pickedUpParent.position + pickupOffset - Vector3.up * (pickupHeight / 2);
        Vector3 capsuleEnd = pickedUpParent.position + pickupOffset + Vector3.up * (pickupHeight / 2);

        Collider[] colliders = Physics.OverlapCapsule(capsuleStart, capsuleEnd, dropRadius);

        foreach (Collider collider in colliders)
        {
            PlacementZone placementZone = collider.GetComponent<PlacementZone>();
            if (placementZone != null)
            {
                placementZone.RemoveObject(holdableObject);
                holdableObject.SetIsPlaced(false);
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
                CheckForHoldable();
                if (currentHoldableObject != null)
                {
                    PickUp();
                }
            }
        }
    }

    private void HighlightObject(IHoldableObject holdableObject)
    {
        var highlightScript = holdableObject.ObjectBeingHeld().GetComponent<HighlightObject>();
        if (highlightScript != null)
        {
            highlightScript.Highlight();
        }
    }

    private void UnhighlightObject(IHoldableObject holdableObject)
    {
        var highlightScript = holdableObject.ObjectBeingHeld().GetComponent<HighlightObject>();
        if (highlightScript != null)
        {
            highlightScript.Unhighlight();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmos)
        {
            if (currentHoldableObject == null)
            {
                Gizmos.color = Color.green;

                Vector3 capsuleStart = pickedUpParent.position + pickupOffset - Vector3.up * (pickupHeight / 2);
                Vector3 capsuleEnd = pickedUpParent.position + pickupOffset + Vector3.up * (pickupHeight / 2);

                Gizmos.DrawWireSphere(capsuleStart, pickupRadius);
                Gizmos.DrawWireSphere(capsuleEnd, pickupRadius);
                Gizmos.DrawLine(capsuleStart + Vector3.forward * pickupRadius, capsuleEnd + Vector3.forward * pickupRadius);
                Gizmos.DrawLine(capsuleStart - Vector3.forward * pickupRadius, capsuleEnd - Vector3.forward * pickupRadius);
                Gizmos.DrawLine(capsuleStart + Vector3.right * pickupRadius, capsuleEnd + Vector3.right * pickupRadius);
                Gizmos.DrawLine(capsuleStart - Vector3.right * pickupRadius, capsuleEnd - Vector3.right * pickupRadius);
            }
            else
            {
                Gizmos.color = CanDrop() ? Color.green : Color.red;

                Vector3 capsuleStart = pickedUpParent.position + dropOffset - Vector3.up * (dropHeight / 2);
                Vector3 capsuleEnd = pickedUpParent.position + dropOffset + Vector3.up * (dropHeight / 2);

                Gizmos.DrawWireSphere(capsuleStart, dropRadius);
                Gizmos.DrawWireSphere(capsuleEnd, dropRadius);
                Gizmos.DrawLine(capsuleStart + Vector3.forward * dropRadius, capsuleEnd + Vector3.forward * dropRadius);
                Gizmos.DrawLine(capsuleStart - Vector3.forward * dropRadius, capsuleEnd - Vector3.forward * dropRadius);
                Gizmos.DrawLine(capsuleStart + Vector3.right * dropRadius, capsuleEnd + Vector3.right * dropRadius);
                Gizmos.DrawLine(capsuleStart - Vector3.right * dropRadius, capsuleEnd - Vector3.right * dropRadius);
            }
        }
    }






}
