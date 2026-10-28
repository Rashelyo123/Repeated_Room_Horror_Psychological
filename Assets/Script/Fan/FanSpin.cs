using UnityEngine;

public class FanSpin : MonoBehaviour
{
    [Header("Putaran Baling-baling")]
    public Vector3 axis = Vector3.back;   // sumbu poros (ganti kalau arah salah)
    public float speed = 720f;               // derajat per detik
    public bool isOn = true;

    [Header("Transisi Nyala/Mati")]
    public float accelTime = 1.5f;           // detik untuk naik/turun kecepatan

    float currentSpeed;
    float speedVelocity;

    void Update()
    {
        float target = isOn ? speed : 0f;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, target, ref speedVelocity, accelTime);
        transform.Rotate(axis, currentSpeed * Time.deltaTime, Space.Self);
    }

    // Bisa dipanggil dari script lain / trigger
    public void SetSpeed(float newSpeed) { speed = newSpeed; }
    public void TurnOn()  { isOn = true; }
    public void TurnOff() { isOn = false; }
}