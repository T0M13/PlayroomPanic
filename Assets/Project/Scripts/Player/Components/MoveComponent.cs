using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveBehaviour", menuName = "Behaviours/MoveBehaviour")]
public class MoveComponent : ScriptableObject
{
    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float rotationSpeed = 10f; 

    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public float RunSpeed { get => runSpeed; set => runSpeed = value; }
    public float RotationSpeed { get => rotationSpeed; set => rotationSpeed = value; }

    public void Move(Rigidbody rb, Vector2 movement, bool isSprinting)
    {
        float speed = isSprinting ? RunSpeed : MoveSpeed;
        Vector3 moveDirection = new Vector3(movement.x, 0f, movement.y).normalized;

        Vector3 velocity = moveDirection * speed;
        rb.velocity = velocity;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
