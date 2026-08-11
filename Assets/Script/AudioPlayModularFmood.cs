using UnityEngine;
using System.Collections;
using FMOD.Studio;
using FMODUnity;

public class AudioPlayModularFmod : MonoBehaviour
{
    [System.Serializable]
    public class PlaylistItem
    {
        public string label; // opsional, cuma buat gampang baca di Inspector (misal "Static Radio")
        public EventReference audioEvent;

        [Tooltip("0 = main sekali lalu lanjut. -1 = loop selamanya di item ini (playlist berhenti di sini). >0 = jumlah pengulangan sebelum lanjut ke item berikutnya")]
        public int repeatCount = 0;
    }

    [Header("Auto Play")]
    [SerializeField] private bool playOnStart = false;
    [SerializeField] private bool playPlaylistOnStart = false;

    private void Start()
    {
        if (playOnStart)
            Play();

        if (playPlaylistOnStart)
            PlayPlaylist();
    }

    [Header("FMOD Event (single, opsional)")]
    [SerializeField] private EventReference fmodEvent;

    [Header("FMOD Playlist (urutan custom per-item)")]
    [SerializeField] private PlaylistItem[] playlist;
    [SerializeField] private bool loopEntirePlaylist = false; // setelah semua item selesai, ulang dari awal?

    [Header("Settings")]
    [SerializeField] private bool attachToGameObject = true;

    private EventInstance eventInstance;
    private Coroutine playlistRoutine;

    // ============ SINGLE EVENT ============

    public void PlayOneShot()
    {
        if (fmodEvent.IsNull) return;
        if (attachToGameObject)
            RuntimeManager.PlayOneShotAttached(fmodEvent, gameObject);
        else
            RuntimeManager.PlayOneShot(fmodEvent, transform.position);
    }

    public void Play()
    {
        if (fmodEvent.IsNull) return;
        PlayEventInstance(fmodEvent);
    }

    public void Stop()
    {
        StopInternal(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        StopPlaylist();
    }

    public void StopImmediate()
    {
        StopInternal(FMOD.Studio.STOP_MODE.IMMEDIATE);
        StopPlaylist();
    }

    // ============ PLAYLIST MODE (custom per-item) ============

    public void PlayPlaylist()
    {
        if (playlist == null || playlist.Length == 0)
        {
            Debug.LogWarning($"[AudioPlayModularFmod] Playlist kosong di {gameObject.name}");
            return;
        }

        StopPlaylist();
        playlistRoutine = StartCoroutine(PlaylistRoutine());
    }

    public void StopPlaylist()
    {
        if (playlistRoutine != null)
        {
            StopCoroutine(playlistRoutine);
            playlistRoutine = null;
        }
        StopInternal(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private IEnumerator PlaylistRoutine()
    {
        int index = 0;

        while (index < playlist.Length)
        {
            PlaylistItem item = playlist[index];

            if (item.audioEvent.IsNull)
            {
                index++;
                continue;
            }

            int playsRemaining = item.repeatCount == 0 ? 1 : item.repeatCount; // 0 = main sekali
            bool infiniteLoop = item.repeatCount < 0; // -1 (atau kurang) = infinite

            if (infiniteLoop)
            {
                // Main terus item ini selamanya, playlist berhenti di sini
                while (true)
                {
                    PlayEventInstance(item.audioEvent);
                    yield return new WaitUntil(() => IsInstanceFinished());
                }
            }
            else
            {
                for (int i = 0; i < playsRemaining; i++)
                {
                    PlayEventInstance(item.audioEvent);
                    yield return new WaitUntil(() => IsInstanceFinished());
                }
            }

            index++;

            if (index >= playlist.Length && loopEntirePlaylist)
            {
                index = 0;
            }
        }
    }

    private bool IsInstanceFinished()
    {
        if (!eventInstance.isValid()) return true;
        eventInstance.getPlaybackState(out PLAYBACK_STATE state);
        return state == PLAYBACK_STATE.STOPPED;
    }

    // ============ SHARED HELPERS ============

    private void PlayEventInstance(EventReference evt)
    {
        StopInternal();
        eventInstance = RuntimeManager.CreateInstance(evt);
        if (attachToGameObject)
            RuntimeManager.AttachInstanceToGameObject(eventInstance, transform);
        eventInstance.start();
    }

    private void StopInternal(FMOD.Studio.STOP_MODE mode = FMOD.Studio.STOP_MODE.ALLOWFADEOUT)
    {
        if (eventInstance.isValid())
        {
            eventInstance.stop(mode);
            eventInstance.release();
        }
    }

    public void SetParameter(string parameterName, float value)
    {
        if (eventInstance.isValid())
            eventInstance.setParameterByName(parameterName, value);
    }

    private void OnDestroy()
    {
        StopPlaylist();
    }
}