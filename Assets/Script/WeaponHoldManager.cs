using UnityEngine;
using System.Collections;

public class WeaponHoldManager : MonoBehaviour
{
    public static WeaponHoldManager Instance;

    [Header("References")]
    [SerializeField] private Transform holdPoint;
    [SerializeField] private Camera playerCamera;

    [Header("Sway Settings")]
    [SerializeField] private bool enableSway = true;
    [SerializeField] private float swayAmount = 0.02f;
    [SerializeField] private float swaySpeed = 4f;
    [SerializeField] private float bobAmount = 0.015f;
    [SerializeField] private float bobSpeed = 6f;

    [Header("Swing Settings")]
    [SerializeField] private float swingDuration = 0.35f;
    [SerializeField] private Vector3 swingRotation = new Vector3(-60f, 0f, 20f); // sudut ayunan kapak
    [SerializeField] private Vector3 swingPositionOffset = new Vector3(0f, -0.05f, 0.1f); // dorong dikit ke depan
    [SerializeField] private AnimationCurve swingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float hitCheckDistance = 1.5f;
    [SerializeField] private LayerMask hitLayerMask;

    [Header("FMOD (opsional)")]
    [SerializeField] private FMODUnity.EventReference swingSound;
    [SerializeField] private FMODUnity.EventReference hitSound;

    private GameObject currentHeldItem;
    private Vector3 holdPointOriginalLocalPos;
    private bool isSwinging = false;

    private void Awake()
    {
        Instance = this;
        if (holdPoint != null)
            holdPointOriginalLocalPos = holdPoint.localPosition;
    }

    public void EquipAxe(GameObject axePrefab)
    {
        if (currentHeldItem != null) Destroy(currentHeldItem);

        currentHeldItem = Instantiate(axePrefab, holdPoint);
        currentHeldItem.transform.localPosition = Vector3.zero;
        currentHeldItem.transform.localRotation = Quaternion.identity;
        currentHeldItem.transform.localScale = Vector3.one;
    }

    public void Unequip()
    {
        if (currentHeldItem != null)
        {
            Destroy(currentHeldItem);
            currentHeldItem = null;
        }
    }

    public bool HasAxe() => currentHeldItem != null;

    public void TrySwing()
    {
        if (currentHeldItem == null || isSwinging) return;
        StartCoroutine(SwingRoutine());
    }

    private IEnumerator SwingRoutine()
    {
        isSwinging = true;

        if (!swingSound.IsNull)
            FMODUnity.RuntimeManager.PlayOneShotAttached(swingSound, currentHeldItem);

        Vector3 startLocalPos = Vector3.zero;
        Quaternion startLocalRot = Quaternion.identity;

        Vector3 targetLocalPos = swingPositionOffset;
        Quaternion targetLocalRot = Quaternion.Euler(swingRotation);

        bool hitChecked = false;
        float halfDuration = swingDuration * 0.5f;
        float t = 0f;

        // Fase 1: ayun ke depan (siap -> puncak swing)
        while (t < halfDuration)
        {
            t += Time.deltaTime;
            float normalized = swingCurve.Evaluate(t / halfDuration);
            currentHeldItem.transform.localPosition = Vector3.Lerp(startLocalPos, targetLocalPos, normalized);
            currentHeldItem.transform.localRotation = Quaternion.Slerp(startLocalRot, targetLocalRot, normalized);

            // Cek hit tepat di titik tengah ayunan (paling natural buat "kena")
            if (!hitChecked && normalized >= 0.7f)
            {
                hitChecked = true;
                CheckHit();
            }

            yield return null;
        }

        // Fase 2: balik ke pose siap
        t = 0f;
        while (t < halfDuration)
        {
            t += Time.deltaTime;
            float normalized = swingCurve.Evaluate(t / halfDuration);
            currentHeldItem.transform.localPosition = Vector3.Lerp(targetLocalPos, startLocalPos, normalized);
            currentHeldItem.transform.localRotation = Quaternion.Slerp(targetLocalRot, startLocalRot, normalized);
            yield return null;
        }

        currentHeldItem.transform.localPosition = startLocalPos;
        currentHeldItem.transform.localRotation = startLocalRot;

        isSwinging = false;
    }

    private void CheckHit()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, hitCheckDistance, hitLayerMask))
        {
            if (!hitSound.IsNull)
                FMODUnity.RuntimeManager.PlayOneShotAttached(hitSound, hit.collider.gameObject);

            if (hit.collider.TryGetComponent<IAxeHittable>(out var hittable))
            {
                hittable.OnAxeHit();
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && HasAxe())
        {
            TrySwing();
        }

        if (!enableSway || holdPoint == null || currentHeldItem == null || isSwinging) return;

        float mouseX = Input.GetAxis("Mouse X") * swayAmount;
        float mouseY = Input.GetAxis("Mouse Y") * swayAmount;
        Vector3 swayPos = new Vector3(-mouseX, -mouseY, 0);

        float bobY = Mathf.Sin(Time.time * bobSpeed) * bobAmount *
            ((Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f) ? 1 : 0);

        Vector3 targetLocalPos = holdPointOriginalLocalPos + swayPos + new Vector3(0, bobY, 0);
        holdPoint.localPosition = Vector3.Lerp(holdPoint.localPosition, targetLocalPos, Time.deltaTime * swaySpeed);
    }
}