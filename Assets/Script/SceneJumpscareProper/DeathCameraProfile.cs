using UnityEngine;

/// Buat 2 asset: Profile_Static (isDynamic = false) dan Profile_Dynamic (isDynamic = true).
/// Create > Camera > Death Camera Profile
[CreateAssetMenu(fileName = "DeathCameraProfile", menuName = "Camera/Death Camera Profile")]
public class DeathCameraProfile : ScriptableObject
{
    [Header("Mode")]
    [Tooltip("true = kamera dinamis (shake, roll, FOV). false = kontrol: Timeline murni.")]
    public bool isDynamic = true;

    [Header("Durasi")]
    [Tooltip("Samakan dengan panjang Timeline.")]
    public float totalDuration = 2.5f;

    [Header("Shake & Roll")]
    public float shakeAmount = 0.03f;   // meter
    public float shakeSpeed = 25f;
    public float maxRoll = 8f;          // derajat

    [Header("FOV")]
    public float startFov = 70f;
    public float endFov = 50f;

    [Header("Kurva Intensitas terhadap Waktu Cekik (0..1)")]
    public AnimationCurve intensityOverTime = AnimationCurve.Linear(0f, 0.3f, 1f, 1f);
}