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

        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
    }

    public bool IsZooming() => isZooming;
}