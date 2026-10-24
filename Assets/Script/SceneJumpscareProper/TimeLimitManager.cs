using System;
using UnityEngine;

public class TimeLimitManager : MonoBehaviour
{
    [Header("Waktu")]
    [SerializeField] float totalTime = 60f;
    [SerializeField] float criticalWindow = 10f;
    [SerializeField] bool autoStart = true;

    [Header("Intensitas")]
    [SerializeField] float minIntensity = 0.6f;
    [SerializeField] float maxIntensity = 1.8f;
    [Tooltip("Sumbu X = tekanan waktu (0 = di luar window, 1 = waktu habis).")]
    [SerializeField] AnimationCurve intensityByPressure = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    public float Remaining { get; private set; }
    public bool Running { get; private set; }
    public event Action OnTimeUp;

    void Start()
    {
        Remaining = totalTime;
        if (autoStart) StartTimer();
    }

    public void StartTimer(float time = -1f)
    {
        Remaining = time > 0f ? time : totalTime;
        Running = true;
    }

    public void StopTimer() => Running = false;

    void Update()
    {
        if (!Running) return;

        Remaining -= Time.deltaTime;
        if (Remaining <= 0f)
        {
            Remaining = 0f;
            Running = false;
            OnTimeUp?.Invoke();
        }
    }

    /// 0 = di luar window, 1 = waktu habis
    public float Pressure => criticalWindow <= 0f ? 0f : 1f - Mathf.Clamp01(Remaining / criticalWindow);

    public float GetIntensity()
    {
        float k = intensityByPressure.Evaluate(Pressure);
        return Mathf.Lerp(minIntensity, maxIntensity, k);
    }
}