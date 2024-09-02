
public interface IInteractable
{
    public void Interact();
    public bool IsInteractable();
    public void SetInteractable(bool value);
    public bool IsOnlyInteractableWhenPlaced();
    public bool IsOnlyInteractableWhenNeeded();
    public float InteractionThreshhold();
    public bool HasInteractionIcon();
    public NeedIcon InteractionIcon();

}
