using UnityEngine;

public class WeaponHoldManager : MonoBehaviour
{
    public static WeaponHoldManager Instance;

    [Header("References")]
    [SerializeField] private Transform holdPoint;
    [SerializeField] private Camera playerCamera;

    [Header("Weapons (sudah ada di scene, tinggal di-enable)")]
    [SerializeField] private GameObject axeObject; // kapak yang statis nempel di HoldPoint, default nonaktif
    private Animator axeAnimator;

    [Header("Sway Settings")]
    [SerializeField] private bool enableSway = true;
    [SerializeField] private float swayAmount = 0.02f;
    [SerializeField] private float swaySpeed = 8f;
    [SerializeField] private float maxSwayOffset = 0.03f;
    [SerializeField] private float bobAmount = 0.015f;
    [SerializeField] private float bobSpeed = 6f;

    [Header("Swing Settings")]
    [SerializeField] private float hitCheckDistance = 1.5f;
    [SerializeField] private LayerMask hitLayerMask;

    [Header("FMOD (opsional)")]
    [SerializeField] private FMODUnity.EventReference swingSound;
    [SerializeField] private FMODUnity.EventReference hitSound;

    [Header("Camera Shake Settings")]
    [SerializeField] private float swingShakeDuration = 0.15f;
    [SerializeField] private float swingShakeMagnitude = 0.03f;
    [SerializeField] private float hitShakeDuration = 0.2f;
    [SerializeField] private float hitShakeMagnitude = 0.08f;

    private bool hasAxeEquipped = false;
    private Vector3 holdPointOriginalLocalPos;
    private bool isSwinging = false;

    private void Awake()
    {
        Instance = this;
        if (holdPoint != null)
            holdPointOriginalLocalPos = holdPoint.localPosition;

        if (axeObject != null)
        {
            axeAnimator = axeObject.GetComponent<Animator>();
            axeObject.SetActive(false); // pastikan nonaktif dari awal
        }
    }

    public void EquipAxe()
    {
        if (axeObject == null) return;

        axeObject.SetActive(true);
        hasAxeEquipped = true;
    }

    public void Unequip()
    {
        if (axeObject != null)
            axeObject.SetActive(false);

        hasAxeEquipped = false;
    }

    public bool HasAxe() => hasAxeEquipped;

    public void TrySwing()
    {
        Debug.Log("TrySwing() called");
        if (!hasAxeEquipped || isSwinging || axeAnimator == null) return;

        isSwinging = true;
        axeAnimator.SetTrigger("Swing");

        CameraShake.Instance.Shake(swingShakeDuration, swingShakeMagnitude); // <-- shake pas mulai ayun

        if (!swingSound.IsNull)
            FMODUnity.RuntimeManager.PlayOneShotAttached(swingSound, axeObject);
    }
    public void OnSwingHitFrame()
    {
        CheckHit();
    }

    public void OnSwingAnimationEnd()
    {
        isSwinging = false;
    }

    private void CheckHit()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, hitCheckDistance, hitLayerMask))
        {
            CameraShake.Instance.Shake(hitShakeDuration, hitShakeMagnitude); // <-- shake lebih kuat pas KENA sesuatu

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
        if (Input.GetMouseButtonDown(0) && hasAxeEquipped)
        {
            TrySwing();
        }

        if (!enableSway || holdPoint == null || !hasAxeEquipped || isSwinging) return;

        float mouseX = Input.GetAxis("Mouse X") * swayAmount;
        Vector3 swayPos = new Vector3(-mouseX, 0, 0);
        swayPos = Vector3.ClampMagnitude(swayPos, maxSwayOffset);

        float bobY = Mathf.Sin(Time.time * bobSpeed) * bobAmount *
            ((Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f) ? 1 : 0);

        Vector3 targetLocalPos = holdPointOriginalLocalPos + swayPos + new Vector3(0, bobY, 0);
        holdPoint.localPosition = Vector3.Lerp(holdPoint.localPosition, targetLocalPos, Time.deltaTime * swaySpeed);
    }
}