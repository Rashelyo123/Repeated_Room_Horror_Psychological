using UnityEngine;
using UnityEngine.Events;

/// Pasang di DeathCamShaker (parent dari kamera, child dari hantu).
/// Hanya menyentuh transform object ini, jadi tidak bentrok dengan Timeline
/// yang menganimasi kamera di bawahnya.
public class DeathCamDynamicLayer : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] DeathCameraProfile defaultProfile;
    public UnityEvent onFinished;   // sambungkan ke fade to black / game over

    DeathCameraProfile profile;
    float intensity = 1f;
    float elapsed;
    float seed;
    bool playing;

    public void Play(float intensity, DeathCameraProfile profileOverride = null)
    {
        profile = profileOverride != null ? profileOverride : defaultProfile;
        if (profile == null || cam == null)
        {
            Debug.LogWarning("DeathCamDynamicLayer: profile atau camera belum diisi.", this);
            return;
        }

        this.intensity = intensity;
        elapsed = 0f;
        seed = Random.value * 100f;
        playing = true;
        ApplyNeutral();
    }

    // LateUpdate: jalan setelah animasi, jadi offset menumpuk di atas hasil Timeline
    void LateUpdate()
    {
        if (!playing) return;

        elapsed += Time.deltaTime;
        float duration = Mathf.Max(0.01f, profile.totalDuration);
        float p = Mathf.Clamp01(elapsed / duration);

        if (!profile.isDynamic)
        {
            ApplyNeutral();   // mode kontrol: tidak menambah apa pun
        }
        else
        {
            float k = profile.intensityOverTime.Evaluate(p) * intensity;
            float n = elapsed * profile.shakeSpeed;

            Vector3 posOffset = new Vector3(Noise(0f, n), Noise(10f, n), 0f) * profile.shakeAmount * k;
            float roll = Noise(20f, n * 0.5f) * 2f * profile.maxRoll * k;

            transform.localPosition = posOffset;
            transform.localRotation = Quaternion.Euler(0f, 0f, roll);

            float fovDelta = (profile.startFov - profile.endFov) * p * intensity;
            cam.fieldOfView = Mathf.Max(20f, profile.startFov - fovDelta);
        }

        if (elapsed >= duration)
        {
            playing = false;
            onFinished?.Invoke();
        }
    }

    float Noise(float offset, float t) => Mathf.PerlinNoise(seed + offset, t) - 0.5f;

    void ApplyNeutral()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        cam.fieldOfView = profile.startFov;
    }
}