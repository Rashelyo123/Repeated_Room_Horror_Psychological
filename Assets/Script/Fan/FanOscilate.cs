using UnityEngine;

public class FanOscillate : MonoBehaviour
{
    [Header("Gerak Menengok")]
    public Vector3 axis = Vector3.up;   // sumbu putar kanan-kiri
    public float angle = 45f;           // sudut maksimal ke tiap sisi
    public float turnSpeed = 20f;       // derajat per detik
    public bool isOn = true;

    [Header("Delay")]
    public float pauseAtEnds = 1f;      // detik berhenti di tiap ujung
    public float startDelay = 0f;       // delay sebelum mulai menengok pertama kali

    Quaternion startRotation;
    float currentAngle;                 // posisi sekarang (-angle sampai +angle)
    int direction = 1;                  // 1 = ke kanan, -1 = ke kiri
    float pauseTimer;

    void Start()
    {
        startRotation = transform.localRotation;
        currentAngle = 0f;
        pauseTimer = startDelay;
    }

    void Update()
    {
        if (!isOn) return;

        // Sedang menunggu (delay awal atau di ujung)
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
            return;
        }

        currentAngle += direction * turnSpeed * Time.deltaTime;

        // Sampai ujung: tahan posisi, balik arah, mulai hitung delay
        if (Mathf.Abs(currentAngle) >= angle)
        {
            currentAngle = Mathf.Clamp(currentAngle, -angle, angle);
            direction *= -1;
            pauseTimer = pauseAtEnds;
        }

        transform.localRotation = startRotation * Quaternion.AngleAxis(currentAngle, axis);
    }

    public void SetAngle(float newAngle) { angle = newAngle; }
    public void SetTurnSpeed(float newSpeed) { turnSpeed = newSpeed; }
    public void SetPause(float seconds) { pauseAtEnds = seconds; }
}