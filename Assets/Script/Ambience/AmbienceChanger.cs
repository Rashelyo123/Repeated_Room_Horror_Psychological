using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class AmbientChanger : MonoBehaviour
{
    [Header("Warna Ambient")]
    public Color ambientOn = new Color(0.25f, 0.05f, 0.05f);   // saat ON
    public Color ambientOff = new Color(0.04f, 0.06f, 0.12f);  // saat OFF (kondisi awal)

    [Header("Transisi")]
    public float fadeDuration = 1f;   // 0 = langsung berubah

    Coroutine routine;

    void Start()
    {
        // pastikan mode Flat (Color) supaya ambientLight dipakai
        RenderSettings.ambientMode = AmbientMode.Flat;
    }

    public void SetOn()  { ChangeTo(ambientOn); }
    public void SetOff() { ChangeTo(ambientOff); }

    void ChangeTo(Color target)
    {
        if (routine != null) StopCoroutine(routine);
        if (fadeDuration <= 0f) { RenderSettings.ambientLight = target; return; }
        routine = StartCoroutine(Fade(target));
    }

    IEnumerator Fade(Color target)
    {
        Color start = RenderSettings.ambientLight;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            RenderSettings.ambientLight = Color.Lerp(start, target, t / fadeDuration);
            yield return null;
        }
        RenderSettings.ambientLight = target;
    }
}