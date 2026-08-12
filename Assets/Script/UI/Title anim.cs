using UnityEngine;
using UnityEngine.UI;

public class TitleSway : MonoBehaviour
{
    [Header("Rotation Sway")]
    public float swayAngle = 3f;
    public float swaySpeed = 0.8f;

    [Header("Scale Breathe")]
    public float scaleAmount = 0.02f;
    public float scaleSpeed = 0.6f;

    [Header("Position Float")]
    public float floatAmount = 5f;
    public float floatSpeed = 0.5f;

    private Vector3 startPosition;
    private Vector3 startScale;
    private float randomOffset;

    void Start()
    {
        startPosition = transform.localPosition;
        startScale = transform.localScale;
        randomOffset = Random.Range(0f, 100f); // biar tidak sync antar object
    }

    void Update()
    {
        float time = Time.time + randomOffset;

        // Goyang rotasi perlahan
        float rotZ = Mathf.Sin(time * swaySpeed) * swayAngle;
        transform.localRotation = Quaternion.Euler(0f, 0f, rotZ);

        // Napas scale naik turun
        float scale = 1f + Mathf.Sin(time * scaleSpeed) * scaleAmount;
        transform.localScale = startScale * scale;

        // Float naik turun
        float floatY = Mathf.Sin(time * floatSpeed) * floatAmount;
        transform.localPosition = startPosition + new Vector3(0f, floatY, 0f);
    }
}