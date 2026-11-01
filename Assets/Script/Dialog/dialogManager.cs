using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private TextMeshProUGUI characterNameText; // opsional
    [SerializeField] private float fadeDuration = 0.5f;

    public event Action OnDialogEnded;
    public bool IsDialogActive { get; private set; }

    private CanvasGroup canvasGroup;
    private Coroutine dialogRoutine;
    private EventInstance voiceInstance;
    private bool skipRequested;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        canvasGroup = dialogPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = dialogPanel.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0;
        dialogPanel.SetActive(false);
        dialogText.text = "";
        if (characterNameText != null) characterNameText.text = "";
    }

    private void OnDestroy()
    {
        StopVoice();
    }

    public void StartDialog(DialogData data)
    {
        if (data == null || data.lines == null || data.lines.Length == 0) return;

        if (dialogRoutine != null) StopCoroutine(dialogRoutine);
        StopVoice();

        dialogRoutine = StartCoroutine(PlayDialog(data));
    }

    public void NextDialog()
    {
        if (IsDialogActive) skipRequested = true;
    }

    private IEnumerator PlayDialog(DialogData data)
    {
        IsDialogActive = true;
        dialogPanel.SetActive(true);

        if (characterNameText != null) characterNameText.text = data.characterName;
        yield return Fade(0f, 1f);

        foreach (DialogLine line in data.lines)
        {
            dialogText.text = line.text;
            skipRequested = false;

           bool hasVoice = !line.voiceEvent.IsNull;
            if (hasVoice) PlayVoice(line.voiceEvent);

            // 1) Tunggu VO selesai (atau di-skip)
            while (hasVoice && IsVoicePlaying() && !skipRequested)
                yield return null;

            // 2) Jeda setelah VO / durasi baris kalau tanpa VO
            float timer = 0f;
            while (timer < line.delay && !skipRequested)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            StopVoice();
        }

        yield return Fade(1f, 0f);
        EndDialog();
    }

    // ---------- FMOD ----------
    private void PlayVoice(EventReference evt)
    {
        StopVoice();
        voiceInstance = RuntimeManager.CreateInstance(evt);
        voiceInstance.start();
    }

    private bool IsVoicePlaying()
    {
        if (!voiceInstance.isValid()) return false;
        voiceInstance.getPlaybackState(out PLAYBACK_STATE state);
        return state != PLAYBACK_STATE.STOPPED;
    }

    private void StopVoice()
{
    if (!voiceInstance.isValid()) return;
    voiceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    voiceInstance.release();
    voiceInstance.clearHandle();
}

    // ---------- UI ----------
    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }

    private void EndDialog()
    {
        dialogPanel.SetActive(false);
        dialogText.text = "";
        if (characterNameText != null) characterNameText.text = "";
        IsDialogActive = false;
        dialogRoutine = null;
        OnDialogEnded?.Invoke();
    }

    public static void TriggerDialog(DialogData data)
    {
        if (Instance != null) Instance.StartDialog(data);
        else Debug.LogError("DialogManager not found in scene!");
    }
}