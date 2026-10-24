using UnityEngine;
using UnityEngine.Playables;

/// Pasang di object hantu. Menukar kamera player ke kamera death,
/// mematikan kontrol pemain, lalu memutar Timeline jumpscare.
public class DeathCamSwap : MonoBehaviour
{
    [Header("Kamera")]
    [Tooltip("Kosongkan untuk memakai Camera.main")]
    [SerializeField] Camera playerCam;
    [SerializeField] Camera deathCam;
    [SerializeField] DeathCamDynamicLayer dynamicLayer;

    [Header("Timeline")]
    [Tooltip("Matikan 'Play On Awake' di PlayableDirector ini.")]
    [SerializeField] PlayableDirector director;

    [Header("Pemain")]
    [Tooltip("Movement, mouse look, dll.")]
    [SerializeField] Behaviour[] playerControlsToDisable;

    [Header("Opsional: snap hantu ke depan pemain")]
    [Tooltip("Biarkan mati kalau hantu diam di tempat dan posisinya dianimasi Timeline.")]
    [SerializeField] bool snapGhostToPlayer = false;
    [SerializeField] float snapDistance = 1.2f;

    bool triggered;

    void Awake()
    {
        SetCamera(deathCam, false);   // kamera death mati di awal
    }

    public void Trigger(Transform player, float intensity, DeathCameraProfile profileOverride = null)
    {
        if (triggered) return;        // cegah terpanggil dua kali (trigger + waktu habis)
        triggered = true;

        if (playerCam == null) playerCam = Camera.main;

        foreach (var b in playerControlsToDisable) if (b) b.enabled = false;

        if (snapGhostToPlayer && player != null) SnapGhostInFrontOf(player);

        SetCamera(playerCam, false);  // matikan listener player dulu
        SetCamera(deathCam, true);

        if (director) director.Play();
        if (dynamicLayer) dynamicLayer.Play(intensity, profileOverride);
    }

    void SnapGhostInFrontOf(Transform player)
    {
        Vector3 forward = player.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.001f) forward = Vector3.forward;
        forward.Normalize();

        Vector3 pos = player.position + forward * snapDistance;
        pos.y = transform.position.y;
        transform.position = pos;

        Vector3 look = player.position - pos;
        look.y = 0f;
        if (look.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(look);
    }

    static void SetCamera(Camera c, bool on)
    {
        if (!c) return;
        c.enabled = on;
        var listener = c.GetComponent<AudioListener>();
        if (listener) listener.enabled = on;   // hanya satu AudioListener aktif
    }
}