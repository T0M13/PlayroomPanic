using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerReferences playerReferences;


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
    [SerializeField][ShowOnly] private GameObject lastHighlightedGameObject;
    [SerializeField][ShowOnly] private bool isHolding = false;
    [SerializeField][ShowOnly] private bool isInteracting = false;
    [SerializeField][ShowOnly] private float interactionHoldTime = 0f;

    [SerializeField][ShowOnly] private Vector3 pickupCapsuleStart, pickupCapsuleEnd;
    [SerializeField][ShowOnly] private Vector3 dropCapsuleStart, dropCapsuleEnd;
    [Header("Gizmos")]
    [SerializeField] private bool showGizmos = true;

    private void Awake()
    {
        GetReferences();
    }

    private void GetReferences()
    {
        if (playerReferences == null)
        {
            playerReferences = GetComponent<PlayerReferences>();
        }
    }

    private void Update()
    {
        UpdateCapsules();
        CheckForHoldable();
        CheckInteraction();
        //if (isHolding)
        //{
        //    if (CanDrop(out PlacementZone suitablePlacementzone))
        //    {
        //        if (!suitablePlacementzone) return;

        //        if (lastHighlightedGameObject != suitablePlacementzone.gameObject)
        //        {
        //            UnhighlightObject(lastHighlightedGameObject);
        //            HighlightObject(suitablePlacementzone.gameObject);
        //            lastHighlightedGameObject = suitablePlacementzone.gameObject;
        //        }
        //        else
        //        {
        //            UnhighlightObject(lastHighlightedGameObject);
        //            HighlightObject(suitablePlacementzone.gameObject);
        //            lastHighlightedGameObject = suitablePlacementzone.gameObject;
        //        }
        //    }
        //}
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
            if (lastHighlightedGameObject != currentHoldableGameObject)
            {
                UnhighlightObject(lastHighlightedGameObject);
                HighlightObject(currentHoldableGameObject);
                lastHighlightedGameObject = currentHoldableGameObject;
            }
            else
            {
                UnhighlightObject(lastHighlightedGameObject);
                HighlightObject(currentHoldableGameObject);
                lastHighlightedGameObject = currentHoldableGameObject;
            }
        }
        else
        {
            UnhighlightObject(lastHighlightedGameObject);
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
            UnhighlightObject(lastHighlightedGameObject);

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
        if (currentHoldableObject != null && CanDrop(out PlacementZone placementZone))
        {

            currentHoldableObject.SetIsBeingHeld(false);
            currentHoldableObject.ObjectBeingHeld().transform.SetParent(null);

            if (currentHoldableObject.NavMeshAgentOfObject() != null)
                currentHoldableObject.NavMeshAgentOfObject().enabled = true;
            if (currentHoldableObject.RigidbodyOfObject() != null)
                currentHoldableObject.RigidbodyOfObject().isKinematic = false;

            placementZone?.PlaceObject(currentHoldableObject);

            UnhighlightObject(lastHighlightedGameObject);

            currentHoldableObject = null;
            currentHoldableGameObject = null;
            isHolding = false;
        }
    }

    /// <summary>
    /// Returns a bool if the current object held can be dropped and finds a suitable placement zone.
    /// </summary>
    /// <returns>True if the object can be dropped; otherwise, false.</returns>
    private bool CanDrop(out PlacementZone suitablePlacementZone)
    {
        suitablePlacementZone = null;

        if (currentHoldableObject == null)
            return false;

        Collider[] colliders = Physics.OverlapCapsule(dropCapsuleStart, dropCapsuleEnd, dropRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<PlacementZone>())
            {
                PlacementZone placementZone = collider.GetComponent<PlacementZone>();
                if (placementZone != null && !placementZone.IsOccupied && placementZone.ObjOnPlacementZone == null && placementZone.CanPlaceObject(currentHoldableObject))
                {
                    suitablePlacementZone = placementZone;
                    return true;
                }
            }

            if (collider != currentHoldableObject.ColliderOfObject() && ((1 << collider.gameObject.layer) & dropLayerMask) == 0)
            {
                return false;
            }

        }

        return true;
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


    public void OnPickupDrop(InputAction.CallbackContext value)
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
        if (highlightobject == null) return;

        var highlightScript = highlightobject.GetComponent<HighlightObject>();
        if (highlightScript != null)
        {
            highlightScript.Highlight();
        }
    }

    private void UnhighlightObject(GameObject highlightobject)
    {
        if (highlightobject == null) return;

        var highlightScript = highlightobject.GetComponent<HighlightObject>();
        if (highlightScript != null)
        {
            highlightScript.Unhighlight();
        }
    }


    private bool CanInteractWObject()
    {
        if (!isHolding && currentHoldableObject != null && currentHoldableObject.InteractableOfObject() != null)
        {
            if (currentHoldableObject.InteractableOfObject().IsInteractable())
                return true;
            else
                return false;
        }

        return false;
    }

    private void CheckInteraction()
    {
        if (isInteracting && currentHoldableObject != null)
        {
            playerReferences.PlayerUI.SetNeedIcon(currentHoldableObject.InteractableOfObject().InteractionIcon());
            playerReferences.PlayerUI.SetSliderMaxValue(currentHoldableObject.InteractableOfObject().InteractionThreshhold());
            playerReferences.PlayerUI.UpdateSliderValue(interactionHoldTime);

            interactionHoldTime += Time.deltaTime;

            if (interactionHoldTime >= currentHoldableObject.InteractableOfObject().InteractionThreshhold())
            {
                currentHoldableObject.InteractableOfObject().Interact();
                OnStopInteract();
            }
        }

        if (isInteracting && currentHoldableObject == null)
        {
            OnStopInteract();
        }
    }

    private void StartInteraction()
    {
        if (!isHolding && currentHoldableObject != null)
        {
            isInteracting = true;
            interactionHoldTime = 0f;
        }
    }

    public void OnInteract(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            if (CanInteractWObject())
                StartInteraction();
        }
        else if (value.canceled)
        {
            OnStopInteract();
        }
    }

    private void OnStopInteract()
    {
        isInteracting = false;
        interactionHoldTime = 0f;
        playerReferences.PlayerUI.DeactivateSlider();
        playerReferences.PlayerUI.SetDefaultIconAndOff();

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
            Gizmos.color = CanDrop(out _) ? Color.green : Color.red;

            Gizmos.DrawWireSphere(dropCapsuleStart, dropRadius);
            Gizmos.DrawWireSphere(dropCapsuleEnd, dropRadius);
            Gizmos.DrawLine(dropCapsuleStart + Vector3.forward * dropRadius, dropCapsuleEnd + Vector3.forward * dropRadius);
            Gizmos.DrawLine(dropCapsuleStart - Vector3.forward * dropRadius, dropCapsuleEnd - Vector3.forward * dropRadius);
            Gizmos.DrawLine(dropCapsuleStart + Vector3.right * dropRadius, dropCapsuleEnd + Vector3.right * dropRadius);
            Gizmos.DrawLine(dropCapsuleStart - Vector3.right * dropRadius, dropCapsuleEnd - Vector3.right * dropRadius);
        }
    }


}
