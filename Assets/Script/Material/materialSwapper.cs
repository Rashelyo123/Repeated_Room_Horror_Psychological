using UnityEngine;

public class MaterialSwapper : MonoBehaviour
{
    [Header("Target")]
    public Renderer targetRenderer;
    public int materialIndex = 0;

    [Header("Material")]
    public Material materialOff;   // M_Lampu (tanpa emission)
    public Material materialOn;    // M_Lamp_On (dengan emission)

    public void SetOn()  { Swap(materialOn); }
    public void SetOff() { Swap(materialOff); }

    public void Toggle(bool on) { Swap(on ? materialOn : materialOff); }

    void Swap(Material m)
    {
        if (targetRenderer == null || m == null) return;

        var mats = targetRenderer.sharedMaterials;
        if (materialIndex < 0 || materialIndex >= mats.Length) return;

        mats[materialIndex] = m;
        targetRenderer.sharedMaterials = mats;
    }
}