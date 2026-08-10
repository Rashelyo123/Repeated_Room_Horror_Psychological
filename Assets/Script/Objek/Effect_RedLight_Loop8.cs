using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Effect_RedLight_Loop8 : MonoBehaviour
{
    [SerializeField] private Volume globalVolume;
    [SerializeField] private Color targetFilterColor = new Color(1f, 0.4f, 0f); // warna oranye di gambar

    private ColorAdjustments colorAdjustments;

    void Awake()
    {
        if (globalVolume.profile.TryGet(out colorAdjustments))
        {
            // pastikan awalnya off dulu kalau perlu
            // colorAdjustments.colorFilter.overrideState = false;
        }
        else
        {
            Debug.LogWarning("ColorAdjustments tidak ditemukan di profile!");
        }
    }

    // Function ini yang dipanggil dari Animation Event
    public void ActivateColorFilter()
    {
        if (colorAdjustments == null) return;

        colorAdjustments.colorFilter.overrideState = true;
        colorAdjustments.colorFilter.value = targetFilterColor;
    }

    public void DeactivateColorFilter()
    {
        if (colorAdjustments == null) return;
        colorAdjustments.colorFilter.overrideState = false;
    }
}