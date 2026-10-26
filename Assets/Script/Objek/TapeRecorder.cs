using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using FMOD.Studio;
using FMODUnity;

public class TapeRecorder : Interactable
{
    [Header("Tape Recorder")]
    [SerializeField] private GameObject tapeRecorderObjek;
    [SerializeField] private bool isRingGrabbed = false;
    [SerializeField] private GameObject Complete;
    [SerializeField] private EventReference tapeRecorderSound;
    [SerializeField] private EventReference put;
    [SerializeField] private DialogData dialogData;
    [SerializeField] private DialogData dialogData2;
    [SerializeField] private GameObject tapeRecorderObjek2;
    [SerializeField] private EventReference tapeRecorderSoundPickup;

    [Header("Tape Playback")]
    [Tooltip("Batas waktu maksimal nunggu suara tape selesai (detik). Jaga-jaga kalau event-nya loop.")]
    [SerializeField] private float tapeMaxWait = 15f;

    [Header("Chase Ambience")]
    [SerializeField] private EventReference chaseAmbience;   // event:/System_Adaptive/Loop9_Ambience
    [Tooltip("Dipanggil saat sesi dikejar mulai (spawn musuh, lampu mati, dll).")]
    [SerializeField] private UnityEvent onChaseStart;
    [Tooltip("Dipanggil saat sesi dikejar berhenti.")]
    [SerializeField] private UnityEvent onChaseStop;

    private EventInstance chaseInstance;
    private bool isSequencePlaying = false;

    protected override void Interact()
    {
        base.Interact();
        if (CanInteract && isRingGrabbed)
        {
            ActiveRing();
        }
        else
        {
            DialogManager.TriggerDialog(dialogData2);
            tapeRecorderObjek2.SetActive(true);
        }
    }

    public void ActiveRing()
    {
        if (!isRingGrabbed || isSequencePlaying) return;

        tapeRecorderObjek.SetActive(true);
        StartCoroutine(PlayTapeRecorderSound());
    }

    public void GrabRing()
    {
        isRingGrabbed = true;
        RuntimeManager.PlayOneShotAttached(tapeRecorderSoundPickup, gameObject);
    }

    private IEnumerator PlayTapeRecorderSound()
    {
        isSequencePlaying = true;

        RuntimeManager.PlayOneShotAttached(put, gameObject);
        yield return new WaitForSeconds(1f);

        EventInstance tapeRecorderInstance = RuntimeManager.CreateInstance(tapeRecorderSound);
        RuntimeManager.AttachInstanceToGameObject(tapeRecorderInstance, transform);
        tapeRecorderInstance.start();

        // Tunggu sampai suara tape benar-benar selesai (bukan hardcode durasi)
        float timer = 0f;
        PLAYBACK_STATE state;
        do
        {
            yield return null;
            timer += Time.deltaTime;
            tapeRecorderInstance.getPlaybackState(out state);
        }
        while (state != PLAYBACK_STATE.STOPPED && timer < tapeMaxWait);

        tapeRecorderInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        tapeRecorderInstance.release();

        yield return new WaitForSeconds(1f);

        DialogManager.TriggerDialog(dialogData);
        Complete.SetActive(true);

        StartChase();   // masuk ke ambience dikejar-kejar
        isSequencePlaying = false;
    }

    // ===== Chase =====

    public void StartChase()
    {
        if (chaseInstance.isValid()) return; // cegah double start

        // Ambience 2D, jadi tidak perlu AttachInstanceToGameObject
        chaseInstance = RuntimeManager.CreateInstance(chaseAmbience);
        chaseInstance.start();
        onChaseStart?.Invoke();
    }

    public void StopChase()
    {
        if (!chaseInstance.isValid()) return;

        chaseInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        chaseInstance.release();
        chaseInstance.clearHandle();
        onChaseStop?.Invoke();
    }

    public void StopChaseImmediate()
    {
        if (!chaseInstance.isValid()) return;

        chaseInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        chaseInstance.release();
        chaseInstance.clearHandle();
        onChaseStop?.Invoke();
    }

    // Opsional: kontrol intensitas kalau di FMOD ada parameter "ChaseIntensity"
    public void SetChaseIntensity(float value)
    {
        if (chaseInstance.isValid())
            chaseInstance.setParameterByName("ChaseIntensity", value);
    }

    private void OnDestroy()
    {
        // Biar ambience tidak nyangkut saat pindah scene
        if (chaseInstance.isValid())
        {
            chaseInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            chaseInstance.release();
            chaseInstance.clearHandle();
        }
    }
}