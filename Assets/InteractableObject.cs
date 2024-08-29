using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private bool canInteract = true;

    public void Interact()
    {
        Debug.Log("Interacting with " + gameObject.name);
    }

    public bool IsInteractable()
    {
       return canInteract;
    }

    public void SetInteractable(bool value)
    {
        canInteract = value;
    }
}
