using System.Collections;
using UnityEngine;

public class WorldItemInspectable : Interactable
{
    [Header("Inspect Settings")]
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private Vector3 inspectLocalOffset = new Vector3(0, 0, 1.2f); // offset dari kamera
    [SerializeField] private Vector3 inspectRotation = Vector3.zero;
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float rotationSpeed = 200f;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private Collider[] colliders;
    private Rigidbody rb;

    private bool isInspecting = false;
    private Camera playerCamera;

    protected override void Interact()
    {
        base.Interact();
        if (!isInspecting)
            ItemInspectionController.Instance.BeginInspect(this);
    }

    public void Setup(Camera cam)
    {
        playerCamera = cam;
        colliders = GetComponentsInChildren<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    public IEnumerator MoveToInspectPoint()
    {
        isInspecting = true;
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        foreach (var col in colliders) col.enabled = false;
        if (rb != null) rb.isKinematic = true;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        float t = 0f;
        while (t < moveDuration)
        {
            t += Time.unscaledDeltaTime;
            float normalized = moveCurve.Evaluate(t / moveDuration);

            Vector3 targetWorldPos = playerCamera.transform.TransformPoint(inspectLocalOffset);
            Quaternion targetWorldRot = playerCamera.transform.rotation * Quaternion.Euler(inspectRotation);

            transform.position = Vector3.Lerp(startPos, targetWorldPos, normalized);
            transform.rotation = Quaternion.Slerp(startRot, targetWorldRot, normalized);
            yield return null;
        }
    }

    // Dipanggil tiap frame dari ItemInspectionController.LateUpdate() selama mode inspect aktif diam
    public void UpdateInspectPosition()
    {
        transform.position = playerCamera.transform.TransformPoint(inspectLocalOffset);
        // rotasi TIDAK di-update di sini karena udah dikontrol manual lewat drag (RotateByInput)
    }

    public void RotateByInput(float mouseX, float mouseY)
    {
        transform.Rotate(playerCamera.transform.up, -mouseX * rotationSpeed * Time.unscaledDeltaTime, Space.World);
        transform.Rotate(playerCamera.transform.right, mouseY * rotationSpeed * Time.unscaledDeltaTime, Space.World);
    }

    public IEnumerator MoveBackToOriginal()
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        float t = 0f;
        while (t < moveDuration)
        {
            t += Time.unscaledDeltaTime;
            float normalized = moveCurve.Evaluate(t / moveDuration);

            transform.position = Vector3.Lerp(startPos, originalPosition, normalized);
            transform.rotation = Quaternion.Slerp(startRot, originalRotation, normalized);
            yield return null;
        }

        transform.position = originalPosition;
        transform.rotation = originalRotation;

        foreach (var col in colliders) col.enabled = true;
        if (rb != null) rb.isKinematic = false;

        isInspecting = false;
    }
}