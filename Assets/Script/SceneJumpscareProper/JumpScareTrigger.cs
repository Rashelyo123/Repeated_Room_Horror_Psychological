using UnityEngine;

/// Syarat OnTriggerEnter: player punya Rigidbody atau CharacterController,
/// dan tag "Player" ada di object yang punya collider.
[RequireComponent(typeof(Collider))]
public class JumpscareTrigger : MonoBehaviour
{
    [SerializeField] DeathCamSwap ghostCam;
    [SerializeField] TimeLimitManager timer;                // kosong = pakai fallbackIntensity
    [SerializeField] DeathCameraProfile profileOverride;    // opsional
    [SerializeField, Range(0.1f, 3f)] float intensityScale = 1f;
    [SerializeField] float fallbackIntensity = 1f;
    [SerializeField] string playerTag = "Player";

    void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        float baseIntensity = timer != null ? timer.GetIntensity() : fallbackIntensity;
        ghostCam.Trigger(other.transform, baseIntensity * intensityScale, profileOverride);
    }
}