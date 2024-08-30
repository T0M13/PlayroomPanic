using UnityEngine;
using UnityEngine.AI;

public class HoldableObject : MonoBehaviour, IHoldableObject
{
    [Header("References")]
    [SerializeField] private Rigidbody rbody;
    [SerializeField] private Collider objCollider;
    [Header("Settings")]
    [SerializeField] private Vector3 spawnPoint;
    [SerializeField] private Vector3 spawnPointOffset;
    [SerializeField] [ShowOnly] private bool isPickedUp = false;
    [SerializeField] [ShowOnly] private bool shouldFixate = true;
    [SerializeField][ShowOnly] private bool isPlaced = false;


    private void Awake()
    {
        rbody = GetComponent<Rigidbody>();
        objCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        SetSpawnPoint(transform.position);
    }

    public bool IsBeingHeld()
    {
        return isPickedUp;
    }

    public NavMeshAgent NavMeshAgentOfObject()
    {
       return null;
    }

    public GameObject ObjectBeingHeld()
    {
        return this.gameObject;
    }

    public Rigidbody RigidbodyOfObject()
    {
        return rbody;
    }

    public void SetIsBeingHeld(bool value)
    {
        isPickedUp = value;
    }

    public Collider ColliderOfObject()
    {
       return objCollider;
    }

    public void SetSpawnPoint(Vector3 position)
    {
        spawnPoint = position;
    }

    public void Respawn()
    {
        transform.position = spawnPoint + spawnPointOffset;
    }

    public bool ShouldFixate()
    {
        return shouldFixate;
    }

    public bool IsPlaced()
    {
       return isPlaced;
    }
    public void SetIsPlaced(bool value)
    {
        isPlaced = value;
    }
    public IInteractable InteractableOfObject()
    {
        return GetComponent<IInteractable>();
    }
}
