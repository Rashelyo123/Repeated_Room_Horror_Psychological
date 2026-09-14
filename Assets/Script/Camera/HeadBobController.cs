using UnityEngine;

[System.Serializable]
public class HeadBobProfile
{
    [Tooltip("Vertical/horizontal sway strength")]
    public float amplitude = 0.05f;

    [Tooltip("How fast the bob cycles")]
    public float frequency = 8f;

    [Tooltip("Target FOV when this profile is active")]
    public float fov = 60f;
}

public class HeadBobController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private PlayerZoom playerZoom;
    [SerializeField] private CameraShake cameraShake;

    [Header("Bob Profiles")]
    [SerializeField] private HeadBobProfile idleProfile = new HeadBobProfile { amplitude = 0.01f, frequency = 2f, fov = 60f };
    [SerializeField] private HeadBobProfile walkProfile = new HeadBobProfile { amplitude = 0.1f, frequency = 12f, fov = 60f };
    [SerializeField] private HeadBobProfile sprintProfile = new HeadBobProfile { amplitude = 0.2f, frequency = 18f, fov = 68f };

    [SerializeField] private float walkSpeedThreshold = 0.1f;
    [SerializeField] private float transitionSpeed = 6f;
    [SerializeField] private float horizontalRatio = 0.8f;
    [SerializeField, Range(0f, 1f)] private float weight = 1f;

    private float currentAmplitude, currentFrequency, currentFov, timer;
    private Vector3 startLocalPos;
    private float externalSpeed;
    private bool externalIsRunning;
    private float amplitudeVelocity, frequencyVelocity, fovVelocity;


    private void Awake()
    {
        startLocalPos = transform.localPosition;
        if (cam == null) cam = GetComponent<Camera>();
        if (playerZoom == null) playerZoom = GetComponentInParent<PlayerZoom>();
        currentFov = cam != null ? cam.fieldOfView : idleProfile.fov;

        //  Debug.Log($"[HeadBob] Cam assigned: {(cam != null ? cam.name : "NULL")}");
    }

    public void UpdateMovementState(float horizontalSpeed, bool isRunning)
    {
        externalSpeed = horizontalSpeed;
        externalIsRunning = isRunning;
    }

    private void LateUpdate()
    {
        HeadBobProfile target = GetTargetProfile();

        currentAmplitude = Mathf.SmoothDamp(currentAmplitude, target.amplitude, ref amplitudeVelocity, 0.25f);
        currentFrequency = Mathf.SmoothDamp(currentFrequency, target.frequency, ref frequencyVelocity, 0.25f);
        currentFov = Mathf.SmoothDamp(currentFov, target.fov, ref fovVelocity, 0.25f);

        ApplyBob();
        ApplyFov();
    }

    private HeadBobProfile GetTargetProfile()
    {
        bool isMoving = externalSpeed > walkSpeedThreshold;
        if (!isMoving) return idleProfile;
        return externalIsRunning ? sprintProfile : walkProfile;
    }

    private void ApplyBob()
    {
        timer += Time.deltaTime * currentFrequency;
        float yOffset = Mathf.Sin(timer) * currentAmplitude;
        float xOffset = Mathf.Cos(timer * 0.5f) * currentAmplitude * horizontalRatio;

        Vector3 bobOffset = new Vector3(xOffset, yOffset, 0f) * weight;
        Vector3 shakeOffset = CameraShake.Instance != null ? CameraShake.Instance.CurrentShakeOffset : Vector3.zero;

        transform.localPosition = startLocalPos + bobOffset + shakeOffset;
    }

    private void ApplyFov()
    {
        if (cam == null) return;

        if (playerZoom != null && playerZoom.IsZooming())
            return;

        cam.fieldOfView = Mathf.Lerp(
            cam.fieldOfView,
            currentFov,
            Time.deltaTime * transitionSpeed);
    }

    public void SetWeight(float newWeight) => weight = Mathf.Clamp01(newWeight);
}