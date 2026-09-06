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
            InitAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitAudio()
    {
        roomToneInstance = RuntimeManager.CreateInstance(roomToneEvent);
        roomToneInstance.start();
    }

    void OnDestroy()
    {
        roomToneInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        roomToneInstance.release();
    }
}