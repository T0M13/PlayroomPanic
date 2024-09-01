
public interface IInteractable
{
    public void Interact();
    public bool IsInteractable();
    public void SetInteractable(bool value);
    public bool IsOnlyInteractableWhenPlaced();
    public bool IsOnlyInteractableWhenNeeded();
    public bool HasInteracted();
    public bool OneTimeInteraction();
    public float InteractionThreshhold();

}
