using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool canOnlyInteractWhenPlaced = false;
    [SerializeField] private bool canOnlyInteractWhenNeeded = false;
    [SerializeField] private bool hasInteracted = false;
    [SerializeField] private bool oneTimeInteraction = false;
    [SerializeField] private float interactionHoldTime = 3f;

    public bool HasInteracted()
    {
        return hasInteracted;
    }

    public virtual void Interact()
    {
        if (hasInteracted) return;

        if (oneTimeInteraction)
            hasInteracted = true;

        Debug.Log("Interacted with: " + gameObject.name);
    }

    public float InteractionThreshhold()
    {
        return interactionHoldTime;
    }

    public bool IsInteractable()
    {
        return canInteract;
    }

    public virtual bool IsOnlyInteractableWhenNeeded()
    {
        return canOnlyInteractWhenNeeded;
    }

    public bool IsOnlyInteractableWhenPlaced()
    {
        return canOnlyInteractWhenPlaced;
    }

    public bool OneTimeInteraction()
    {
        return oneTimeInteraction;
    }

    public void SetInteractable(bool value)
    {
        canInteract = value;
    }

}
