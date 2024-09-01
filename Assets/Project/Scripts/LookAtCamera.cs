using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [Header("Look At Settings")]
    [SerializeField][ShowOnly] private Transform cameraTransform;
    [SerializeField] private bool lookAtCamera = true;
    [SerializeField] private bool lookAtCameraOnceOnStart = false;

    private void Start()
    {
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning("Main Camera not found..");
        }

        if (lookAtCameraOnceOnStart)
        {
            LookCamera();
        }
    }

    private void Update()
    {
        if (lookAtCameraOnceOnStart) return;

        LookCamera();
    }

    private void LookCamera()
    {
        if (!lookAtCamera) return;

        if (cameraTransform != null)
        {
            transform.LookAt(cameraTransform);
        }
    }
}
