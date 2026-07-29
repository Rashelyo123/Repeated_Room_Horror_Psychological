using System.Collections;
using UnityEngine;
using FMODUnity;

public class LockedDoorHandle : Interactable
{
    [Header("Lock Settings")]
    [SerializeField] private bool isLocked = true;

    [Header("Shake Animation Settings")]
    [SerializeField] private float shakeDuration = 0.4f;
    [SerializeField] private float shakeAngle = 12f;      // seberapa jauh gagang goyang (derajat)
    [SerializeField] private int shakeCount = 4;           // berapa kali goyang bolak-balik
    [SerializeField] private AnimationCurve shakeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0); // intensitas makin lama makin kecil

    [Header("Door Animation Settings")]
    [SerializeField] private Transform doorPivot;           // objek pintu yang beneran berputar (bukan gagang)
    [SerializeField] private float doorOpenAngle = 90f;     // sudut buka pintu (derajat)
    [SerializeField] private float doorOpenDuration = 1f;   // lama animasi buka
    [SerializeField] private AnimationCurve doorOpenCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private bool openTowardsPlayer = false; // opsional: buka kearah player atau selalu 1 arah

    [Header("FMOD Sound")]
    [SerializeField] private EventReference lockedRattleSound;  // suara gagang digoyang tapi terkunci
    [SerializeField] private EventReference unlockedOpenSound;  // suara kalau berhasil kebuka (opsional)

    [Header("Events")]
    public UnityEngine.Events.UnityEvent onLockedInteract;   // dipanggil kalau dicoba buka tapi masih terkunci
    public UnityEngine.Events.UnityEvent onUnlockedInteract; // dipanggil kalau pintu udah nggak terkunci & berhasil diinteraksi

    private Quaternion closedRotation;
    private Quaternion doorClosedRotation;
    private bool isAnimating = false;
    private bool isDoorOpen = false;
    private bool isDoorAnimating = false;

    protected override void Interact()
    {
        base.Interact();
        if (CanInteract)
        {
            TryOpen();
        }
    }

    private void Awake()
    {
        closedRotation = transform.localRotation;

        if (doorPivot != null)
        {
            doorClosedRotation = doorPivot.localRotation;
        }
    }

    public void TryOpen()
    {
        if (isAnimating) return;

        if (isLocked)
        {
            StartCoroutine(ShakeHandle());

            if (!lockedRattleSound.IsNull)
            {
                RuntimeManager.PlayOneShotAttached(lockedRattleSound, gameObject);
            }

            onLockedInteract?.Invoke();
        }
        else
        {
            if (!unlockedOpenSound.IsNull)
            {
                RuntimeManager.PlayOneShotAttached(unlockedOpenSound, gameObject);
            }

            if (doorPivot != null && !isDoorAnimating)
            {
                StartCoroutine(AnimateDoor(!isDoorOpen)); // toggle: buka kalau tertutup, tutup kalau terbuka
            }

            onUnlockedInteract?.Invoke();
        }
    }

    public void Unlock()
    {
        isLocked = false;
    }

    public void Lock()
    {
        isLocked = true;
    }

    public bool IsLocked()
    {
        return isLocked;
    }

    private IEnumerator ShakeHandle()
    {
        isAnimating = true;
        float t = 0f;

        while (t < shakeDuration)
        {
            t += Time.deltaTime;
            float normalized = t / shakeDuration;
            float intensity = shakeCurve.Evaluate(normalized);

            // gagang goyang naik-turun pakai sin wave, makin lama makin kecil (dikali intensity)
            float angle = Mathf.Sin(normalized * shakeCount * Mathf.PI * 2f) * shakeAngle * intensity;
            transform.localRotation = closedRotation * Quaternion.Euler(angle, 0f, 0f);

            yield return null;
        }

        transform.localRotation = closedRotation;
        isAnimating = false;
    }

    private IEnumerator AnimateDoor(bool open)
    {
        isDoorAnimating = true;

        Quaternion startRot = doorPivot.localRotation;
        float targetAngle = open ? doorOpenAngle : 0f;

        // opsional: kalau openTowardsPlayer true, bisa dibalik arahnya berdasarkan posisi player
        Quaternion targetRot = doorClosedRotation * Quaternion.Euler(0f, targetAngle, 0f);

        float t = 0f;
        while (t < doorOpenDuration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / doorOpenDuration);
            float eased = doorOpenCurve.Evaluate(normalized);

            doorPivot.localRotation = Quaternion.Slerp(startRot, targetRot, eased);
            yield return null;
        }

        doorPivot.localRotation = targetRot;
        isDoorOpen = open;
        isDoorAnimating = false;
    }
}