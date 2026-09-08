using UnityEngine;

public class PlayerZoom : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;

    [Header("Zoom Settings")]
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float zoomFOV = 30f;
    [SerializeField] private float zoomSpeed = 8f;
    [SerializeField] private KeyCode zoomKey = KeyCode.Mouse1;

    private bool isZooming = false;
    private float targetFOV;
    private float fovVelocity;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = GetComponent<PlayerController>().playerCamera;

        normalFOV = playerCamera.fieldOfView;
        targetFOV = normalFOV;
    }

    private void Update()
    {
        if (Input.GetKeyDown(zoomKey))
        {
            isZooming = true;
            targetFOV = zoomFOV;

        }

        if (Input.GetKeyUp(zoomKey))
        {
            isZooming = false;
            targetFOV = normalFOV;

        }

        playerCamera.fieldOfView = Mathf.SmoothDamp(
            playerCamera.fieldOfView,
            targetFOV,
            ref fovVelocity,
            1f / Mathf.Max(zoomSpeed, 0.01f));
    }

    public bool IsZooming() => isZooming;
}