using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("FMOD Event Reference")]
    [SerializeField] private EventReference roomToneEvent;
    private EventInstance roomToneInstance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartRoomTone();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartRoomTone()
    {
        if (roomToneInstance.isValid()) return; // sudah jalan, jangan dobel

        roomToneInstance = RuntimeManager.CreateInstance(roomToneEvent);
        roomToneInstance.start();
    }

    public void StopRoomTone(bool immediate = true)
    {
        if (!roomToneInstance.isValid()) return;

        roomToneInstance.stop(immediate
            ? FMOD.Studio.STOP_MODE.IMMEDIATE
            : FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        roomToneInstance.release();
        roomToneInstance.clearHandle();
    }

    void OnDestroy()
    {
        if (Instance != this) return;
        StopRoomTone(false);
    }
}