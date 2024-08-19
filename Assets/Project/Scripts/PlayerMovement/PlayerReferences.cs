using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevel;

[RequireComponent(typeof(Rigidbody))]
public class PlayerReferences : MonoBehaviour
{

    [Header("References")]
    [ShowOnly][SerializeField] private PlayerMovement playerMovement;
    [ShowOnly][SerializeField] private PlayerInput playerInput;
    [Header("References Plus")]
    [SerializeField] private Rigidbody playerRigidBody;
    [SerializeField] private CapsuleCollider playerCollider;

    public PlayerMovement PlayerMovement { get => playerMovement; set => playerMovement = value; }
    public Rigidbody PlayerRigidBody { get => playerRigidBody; set => playerRigidBody = value; }
    public CapsuleCollider PlayerCollider { get => playerCollider; set => playerCollider = value; }
    public PlayerInput PlayerInput { get => playerInput; set => playerInput = value; }

    private void Awake()
    {
        GetReferences();
    }

    private void OnValidate()
    {
        GetReferences();
    }

    private void GetReferences()
    {
        if (PlayerInput == null)
        {
            try
            {
                PlayerInput = GetComponent<PlayerInput>();
            }
            catch
            {
                Debug.LogWarning("PlayerInput Missing!");
            }
        }

        if (PlayerMovement == null)
        {
            try
            {
                PlayerMovement = GetComponent<PlayerMovement>();
            }
            catch
            {
                Debug.LogWarning("PlayerMovement Missing!");
            }
        }

        if (PlayerRigidBody == null)
        {
            try
            {
                PlayerRigidBody = GetComponent<Rigidbody>();
            }
            catch
            {
                Debug.LogWarning("Rigidbody Missing!");
            }
        }

        if (PlayerCollider == null)
        {
            try
            {
                PlayerCollider = GetComponent<CapsuleCollider>();
            }
            catch
            {
                Debug.LogWarning("CapsuleCollider Missing!");
            }
        }


    }

}

