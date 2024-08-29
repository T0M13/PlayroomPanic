using UnityEngine;
using UnityEngine.AI;

public interface IHoldableObject
{
    public bool IsBeingHeld();
    public void SetIsBeingHeld(bool value);
    public GameObject ObjectBeingHeld();
    public void SetSpawnPoint(Vector3 position);
    public void Respawn();
    public Rigidbody RigidbodyOfObject();
    public Collider ColliderOfObject();
    public NavMeshAgent NavMeshAgentOfObject();
    public bool ShouldFixate();
    public bool IsPlaced();
    public void SetIsPlaced(bool value);

}

