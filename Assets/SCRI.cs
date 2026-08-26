using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    public Camera mainCamera;

    void Start()
    {
        // Menyimpan referensi kamera utama saat script mulai dijalankan
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;

        // Mengarahkan rotasi UI agar sama dengan arah rotasi kamera
        transform.forward = mainCamera.transform.forward;
    }
}