using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    [SerializeField] private Transform cameraTransform; // biasanya child kamera, atau kamera itu sendiri
    private Vector3 originalLocalPos;
    private Coroutine shakeRoutine;

    private void Awake()
    {
        Instance = this;
        if (cameraTransform != null)
            originalLocalPos = cameraTransform.localPosition;
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

            float damper = 1f - Mathf.Clamp01(elapsed / duration); // makin lama makin kecil (decay)
            float offsetX = (Mathf.PerlinNoise(Time.time * frequency, 0f) - 0.5f) * 2f * magnitude * damper;
            float offsetY = (Mathf.PerlinNoise(0f, Time.time * frequency) - 0.5f) * 2f * magnitude * damper;

            cameraTransform.localPosition = originalLocalPos + new Vector3(offsetX, offsetY, 0f);
            yield return null;
        }

        cameraTransform.localPosition = originalLocalPos;
    }
}