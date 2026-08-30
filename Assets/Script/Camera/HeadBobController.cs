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
    [Header("Bob Profiles")]
    [SerializeField] private HeadBobProfile idleProfile = new HeadBobProfile { amplitude = 0.01f, frequency = 2f, fov = 60f };
    [SerializeField] private HeadBobProfile walkProfile = new HeadBobProfile { amplitude = 0.1f, frequency = 12f, fov = 60f };
    [SerializeField] private HeadBobProfile sprintProfile = new HeadBobProfile { amplitude = 0.2f, frequency = 18f, fov = 68f };

    [SerializeField] private float walkSpeedThreshold = 0.1f;
    [SerializeField] private float transitionSpeed = 6f;
    [SerializeField] private float horizontalRatio = 0.8f;
    [SerializeField, Range(0f, 1f)] private float weight = 1f;

    private float currentAmplitude, currentFrequency, timer;
    private Vector3 startLocalPos;
    private float externalSpeed;
    private bool externalIsRunning;

    private void Awake()
    {
        startLocalPos = transform.localPosition;
    }

    public void UpdateMovementState(float horizontalSpeed, bool isRunning)
    {
        externalSpeed = horizontalSpeed;
        externalIsRunning = isRunning;
    }

    private void LateUpdate()
    {
        HeadBobProfile target = GetTargetProfile();

        currentAmplitude = Mathf.Lerp(currentAmplitude, target.amplitude, Time.deltaTime * transitionSpeed);
        currentFrequency = Mathf.Lerp(currentFrequency, target.frequency, Time.deltaTime * transitionSpeed);

        ApplyBob();
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
        transform.localPosition = startLocalPos + new Vector3(xOffset, yOffset, 0f) * weight;
    }

    public void SetWeight(float newWeight) => weight = Mathf.Clamp01(newWeight);
}