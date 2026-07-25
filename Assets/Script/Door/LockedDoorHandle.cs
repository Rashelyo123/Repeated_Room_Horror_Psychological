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

    [Header("FMOD Sound")]
    [SerializeField] private EventReference lockedRattleSound;  // suara gagang digoyang tapi terkunci
    [SerializeField] private EventReference unlockedOpenSound;  // suara kalau berhasil kebuka (opsional)

    [Header("Events")]
    public UnityEngine.Events.UnityEvent onLockedInteract;   // dipanggil kalau dicoba buka tapi masih terkunci
    public UnityEngine.Events.UnityEvent onUnlockedInteract; // dipanggil kalau pintu udah nggak terkunci & berhasil diinteraksi

    private Quaternion closedRotation;
    private bool isAnimating = false;

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
}