using System.Collections;
using UnityEngine;

public class LightFlickering : MonoBehaviour
{
    [System.Serializable]
    public class EmissionTarget
    {
        public Renderer renderer;
        public int materialIndex = 0;   // slot material emission
    }

    [Header("Target (boleh lebih dari satu)")]
    public Light[] lights;
    public EmissionTarget[] emissionTargets;
    [Tooltip("Nama property emission. URP/Lit = _EmissionColor")]
    public string emissionProperty = "_EmissionColor";

    [Header("Intensitas")]
    public float onMultiplier = 1f;

    [Header("Redup (Dim)")]
    public Vector2 dimIntensity = new Vector2(0.05f, 0.4f);
    public bool dimRandomPerBlink = true;
    [Range(0f, 0.1f)] public float dimSmoothing = 0.02f;

    [Header("Siklus Flicker (detik)")]
    public Vector2 stableOnTime = new Vector2(3f, 8f);
    public Vector2Int blinkCount = new Vector2Int(2, 5);
    public Vector2 blinkOffTime = new Vector2(0.03f, 0.12f);
    public Vector2 blinkOnTime = new Vector2(0.03f, 0.2f);

    [Header("Mati Lama")]
    [Range(0f, 1f)] public float longOffChance = 0.15f;
    public Vector2 longOffTime = new Vector2(0.4f, 1.0f);
    public bool longOffFullDark = true;
    [Range(0f, 2f)] public float recoverTime = 0.4f;

    [Header("Kontrol")]
    public bool flickerEnabled = true;
    public bool randomStartOffset = true;

    float[] originalIntensity;
    Color[][] originalEmission;     // [target][...] warna emission asli
    MaterialPropertyBlock block;
    int emissionId;
    Coroutine routine, fadeRoutine;
    float currentLevel = 1f;

    void Awake()
    {
        if (lights == null || lights.Length == 0)
            lights = GetComponentsInChildren<Light>();

        originalIntensity = new float[lights.Length];
        for (int i = 0; i < lights.Length; i++)
            if (lights[i] != null) originalIntensity[i] = lights[i].intensity;

        block = new MaterialPropertyBlock();
        emissionId = Shader.PropertyToID(emissionProperty);

        // simpan warna emission asli dari material
        originalEmission = new Color[emissionTargets != null ? emissionTargets.Length : 0][];
        for (int i = 0; i < originalEmission.Length; i++)
        {
            var t = emissionTargets[i];
            originalEmission[i] = new Color[1];
            if (t.renderer == null) continue;
            var mats = t.renderer.sharedMaterials;
            if (t.materialIndex >= 0 && t.materialIndex < mats.Length && mats[t.materialIndex] != null
                && mats[t.materialIndex].HasProperty(emissionId))
                originalEmission[i][0] = mats[t.materialIndex].GetColor(emissionId);
        }
    }

    void OnEnable()
    {
        ApplyLevel(1f);
        routine = StartCoroutine(FlickerLoop());
    }

    void OnDisable()
    {
        if (routine != null) StopCoroutine(routine);
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        ApplyLevel(1f);
    }

    IEnumerator FlickerLoop()
    {
        if (randomStartOffset)
            yield return new WaitForSeconds(Random.Range(0f, 2f));

        while (true)
        {
            if (!flickerEnabled)
            {
                ApplyLevel(1f);
                yield return null;
                continue;
            }

            SetLevel(1f);
            yield return new WaitForSeconds(Random.Range(stableOnTime.x, stableOnTime.y));

            int count = Random.Range(blinkCount.x, blinkCount.y + 1);
            float fixedDim = RandomDim();
            for (int i = 0; i < count; i++)
            {
                SetLevel(dimRandomPerBlink ? RandomDim() : fixedDim);
                yield return new WaitForSeconds(Random.Range(blinkOffTime.x, blinkOffTime.y));
                SetLevel(1f);
                yield return new WaitForSeconds(Random.Range(blinkOnTime.x, blinkOnTime.y));
            }

            if (Random.value < longOffChance)
            {
                SetLevel(longOffFullDark ? 0f : RandomDim());
                yield return new WaitForSeconds(Random.Range(longOffTime.x, longOffTime.y));
                SetLevel(1f, recoverTime);
                yield return new WaitForSeconds(recoverTime);
            }
        }
    }

    float RandomDim()
    {
        return Random.Range(Mathf.Min(dimIntensity.x, dimIntensity.y),
                            Mathf.Max(dimIntensity.x, dimIntensity.y));
    }

    void SetLevel(float target, float duration = -1f)
    {
        if (duration < 0f) duration = dimSmoothing;
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);

        if (duration <= 0f) { ApplyLevel(target); return; }
        fadeRoutine = StartCoroutine(FadeTo(target, duration));
    }

    IEnumerator FadeTo(float target, float duration)
    {
        float start = currentLevel;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            ApplyLevel(Mathf.Lerp(start, target, t / duration));
            yield return null;
        }
        ApplyLevel(target);
    }

    // level 0..1
    void ApplyLevel(float level)
    {
        currentLevel = level;

        for (int i = 0; i < lights.Length; i++)
        {
            if (lights[i] == null) continue;
            lights[i].intensity = originalIntensity[i] * onMultiplier * level;
        }

        if (emissionTargets == null) return;
        for (int i = 0; i < emissionTargets.Length; i++)
        {
            var t = emissionTargets[i];
            if (t.renderer == null) continue;

            t.renderer.GetPropertyBlock(block, t.materialIndex);
            block.SetColor(emissionId, originalEmission[i][0] * level);
            t.renderer.SetPropertyBlock(block, t.materialIndex);
        }
    }

    public void SetFlicker(bool value) { flickerEnabled = value; }

    public void ForceOff()
    {
        if (routine != null) StopCoroutine(routine);
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        flickerEnabled = false;
        ApplyLevel(0f);
    }

    public void ForceOn()
    {
        if (routine != null) StopCoroutine(routine);
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        flickerEnabled = true;
        ApplyLevel(1f);
        routine = StartCoroutine(FlickerLoop());
    }

    [ContextMenu("Preset: Subtle")]
    void PresetSubtle()
    {
        stableOnTime = new Vector2(6f, 14f);
        blinkCount = new Vector2Int(1, 3);
        blinkOffTime = new Vector2(0.04f, 0.1f);
        blinkOnTime = new Vector2(0.1f, 0.3f);
        dimIntensity = new Vector2(0.4f, 0.7f);
        longOffChance = 0.05f;
        longOffFullDark = false;
    }

    [ContextMenu("Preset: Normal")]
    void PresetNormal()
    {
        stableOnTime = new Vector2(3f, 8f);
        blinkCount = new Vector2Int(2, 5);
        blinkOffTime = new Vector2(0.03f, 0.12f);
        blinkOnTime = new Vector2(0.03f, 0.2f);
        dimIntensity = new Vector2(0.05f, 0.4f);
        longOffChance = 0.15f;
        longOffFullDark = true;
    }

    [ContextMenu("Preset: Intense")]
    void PresetIntense()
    {
        stableOnTime = new Vector2(1f, 4f);
        blinkCount = new Vector2Int(3, 7);
        blinkOffTime = new Vector2(0.05f, 0.2f);
        blinkOnTime = new Vector2(0.03f, 0.15f);
        dimIntensity = new Vector2(0f, 0.3f);
        longOffChance = 0.3f;
        longOffTime = new Vector2(0.6f, 1.5f);
        longOffFullDark = true;
    }
}