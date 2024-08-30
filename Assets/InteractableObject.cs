using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool canOnlyInteractWhenPlaced = false;

    public void Interact()
    {
        Debug.Log("Interacting with " + gameObject.name);
    }

    public bool IsInteractable()
    {
       return canInteract;
    }

    public bool IsOnlyInteractableWhenPlaced()
    {
        return canOnlyInteractWhenPlaced;
    }

    public void SetInteractable(bool value)
    {
        canInteract = value;
    }

}
