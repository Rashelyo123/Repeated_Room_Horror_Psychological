using UnityEngine;

public class TimeUpJumpscare : MonoBehaviour
{
    [SerializeField] TimeLimitManager timer;
    [SerializeField] DeathCamSwap ghostCam;
    [SerializeField] Transform player;
    [SerializeField] DeathCameraProfile profileOverride;   // opsional

    void OnEnable()  => timer.OnTimeUp += Fire;
    void OnDisable() => timer.OnTimeUp -= Fire;

    void Fire() => ghostCam.Trigger(player, timer.GetIntensity(), profileOverride);
}