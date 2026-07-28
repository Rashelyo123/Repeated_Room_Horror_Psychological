using UnityEngine;
using FMODUnity;

public class RadioInteractable : Interactable
{
    [Header("FMOD Audio References")]
    [Tooltip("Event Stinger berisi siaran & noise loop")]
    [SerializeField] private EventReference radioBroadcastEvent;

    [Tooltip("SFX Tek Sakelar Radio (InteractRadio)")]
    [SerializeField] private EventReference radioClickSound;

    [Header("FMOD Parameters")]
    [SerializeField] private float loopCycleValue = 2.0f;

    private FMOD.Studio.EventInstance radioInstance;
    private bool isRadioPlaying = false;
    private bool hasBeenTriggered = false;
    [SerializeField] private GameObject Complete;

    // Dipanggil otomatis saat Player melintasi Trigger Zone di lorong
    public void StartRadioBroadcast()
    {
        if (hasBeenTriggered || isRadioPlaying) return;

        // 1. Play SFX "Tek" ON saat radio pertama kali terpicu di lorong
        if (!radioClickSound.IsNull)
        {
            RuntimeManager.PlayOneShotAttached(radioClickSound, gameObject);
        }

        // 2. Buat Instance FMOD & Atur Parameter Loop
        radioInstance = RuntimeManager.CreateInstance(radioBroadcastEvent);
        radioInstance.setParameterByName("Loop Cycle Count", loopCycleValue);

        // 3. Tempelkan posisi 3D ke GameObject Radio ini & Mainkan
        RuntimeManager.AttachInstanceToGameObject(radioInstance, transform);
        radioInstance.start();

        isRadioPlaying = true;
        hasBeenTriggered = true;
    }

    // Overriding fungsi Interact bawaan sistem utama
    protected override void Interact()
    {
        base.Interact();

        if (CanInteract && isRadioPlaying)
        {
            TurnOffRadio();
        }
    }

    private void TurnOffRadio()
    {
        // 1. Play SFX "Tek" OFF saat pemain menekan tombol 'E'
        if (!radioClickSound.IsNull)
        {
            RuntimeManager.PlayOneShotAttached(radioClickSound, gameObject);
        }

        // 2. Hentikan siaran radio FMOD
        radioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        radioInstance.release();

        isRadioPlaying = false;
        Complete.SetActive(true);
    }

    // Pengaman: Memastikan memori FMOD dibersihkan jika scene berganti/object dihancurkan
    private void OnDestroy()
    {
        if (isRadioPlaying)
        {
            radioInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            radioInstance.release();
        }
    }
}