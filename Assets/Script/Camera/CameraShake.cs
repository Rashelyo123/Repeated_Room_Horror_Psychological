using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Coroutine shakeRoutine;
    public Vector3 CurrentShakeOffset { get; private set; } = Vector3.zero;

    private void Awake()
    {
        Instance = this;
    }

    public void Shake(float duration, float magnitude, float frequency = 25f)
    {
        if (shakeRoutine != null) StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(ShakeRoutine(duration, magnitude, frequency));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude, float frequency)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float damper = 1f - Mathf.Clamp01(elapsed / duration);
            float offsetX = (Mathf.PerlinNoise(Time.time * frequency, 0f) - 0.5f) * 2f * magnitude * damper;
            float offsetY = (Mathf.PerlinNoise(0f, Time.time * frequency) - 0.5f) * 2f * magnitude * damper;

            CurrentShakeOffset = new Vector3(offsetX, offsetY, 0f);
            yield return null;
        }

        CurrentShakeOffset = Vector3.zero;
    }
}