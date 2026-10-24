using UnityEngine;

/// Jembatan antara UnityEvent (misal interaksi pintu) dan DeathCamSwap.
/// Tidak butuh collider. Pasang di object mana pun yang selalu aktif.
public class JumpscareEventRelay : MonoBehaviour
{
    [SerializeField] DeathCamSwap ghostCam;
    [SerializeField] TimeLimitManager timer;                // kosong = pakai fallbackIntensity
    [SerializeField] Transform player;                      // kosong = dicari lewat tag
    [SerializeField] DeathCameraProfile profileOverride;    // opsional
    [SerializeField, Range(0.1f, 3f)] float intensityScale = 1f;
    [SerializeField] float fallbackIntensity = 1f;
    [SerializeField] string playerTag = "Player";
    [SerializeField] bool stopTimerOnFire = true;

    // Dipanggil dari UnityEvent
    public void Fire() => FireInternal(intensityScale);

    // Versi dengan skala yang diisi langsung di UnityEvent (satu parameter float)
    public void FireWithScale(float scale) => FireInternal(scale);

    void FireInternal(float scale)
    {
        if (ghostCam == null)
        {
            Debug.LogWarning("JumpscareEventRelay: ghostCam belum diisi.", this);
            return;
        }

        if (player == null)
        {
            GameObject p = GameObject.FindWithTag(playerTag);
            if (p != null) player = p.transform;
        }
        if (player == null)
        {
            Debug.LogWarning($"JumpscareEventRelay: pemain dengan tag '{playerTag}' tidak ditemukan.", this);
            return;
        }

        float baseIntensity = timer != null ? timer.GetIntensity() : fallbackIntensity;
        if (timer != null && stopTimerOnFire) timer.StopTimer();   // cegah waktu habis memicu di tengah jumpscare

        ghostCam.Trigger(player, baseIntensity * scale, profileOverride);
    }
}