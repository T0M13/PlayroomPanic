using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerReferences playerReferences;

    [Header("References Plus")]
    private Rigidbody playerRigidBody;
    private CapsuleCollider playerCollider;

    [Header("Movement")]
    [SerializeField] private MoveComponent moveComponent;
    [SerializeField] private Vector2 movement;
    [SerializeField] private bool canMove = true;
    [SerializeField] private bool isSprinting;
    [SerializeField][ShowOnly] private bool isMoving;
    [SerializeField][ShowOnly] private Vector3 lastMoveDirection;

    [Header("Gizmo Settings")]
    [SerializeField] private Color gizmoColor = Color.red;
    [SerializeField] private float gizmoLineLength = 2f;
    [SerializeField] private float gizmoSphereRadius = 0.1f;
    [SerializeField] private bool showGizmos = true;

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
        if (playerReferences == null)
        {
            playerReferences = GetComponent<PlayerReferences>();
            if (playerReferences == null)
            {
                Debug.LogWarning("PlayerReferences component is missing!");
            }
        }

        if (playerRigidBody == null && playerReferences != null)
        {
            playerRigidBody = playerReferences.PlayerRigidBody;
            if (playerRigidBody == null)
            {
                Debug.LogWarning("PlayerRigidBody is missing in PlayerReferences!");
            }
        }

        if (playerCollider == null && playerReferences != null)
        {
            playerCollider = playerReferences.PlayerCollider;
            if (playerCollider == null)
            {
                Debug.LogWarning("PlayerCollider is missing in PlayerReferences!");
            }
        }
    }

    private void FixedUpdate()
    {
        //if (!IsOwner) { return; }
            Move();
    }

    private void Move()
    {
        if (!canMove) return;

        moveComponent.Move(playerRigidBody, movement, isSprinting);

        if (movement != Vector2.zero)
        {
            isMoving = true;
            lastMoveDirection = new Vector3(movement.x, 0f, movement.y).normalized;
        }
        else
        {
            isMoving = false;
        }
    }



    private void OnDrawGizmos()
    {
        if (showGizmos && moveComponent != null)
        {
            Gizmos.color = gizmoColor;
            if (Application.isPlaying)
            {
                Gizmos.DrawLine(transform.position, transform.position + lastMoveDirection * gizmoLineLength);
                Gizmos.DrawSphere(transform.position + lastMoveDirection * gizmoLineLength, gizmoSphereRadius);
            }
            else
            {
                Gizmos.DrawLine(transform.position, transform.position + Vector3.forward * gizmoLineLength);
                Gizmos.DrawSphere(transform.position + Vector3.forward * gizmoLineLength, gizmoSphereRadius);
            }
        }
    }

    #region Inputs

    public void OnMove(InputAction.CallbackContext value)
    {
        //if (!IsOwner) return;
        movement = value.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext value)
    {
        //if (!IsOwner) return;
        isSprinting = value.ReadValue<float>() >= 1f;
    }

    #endregion
}
