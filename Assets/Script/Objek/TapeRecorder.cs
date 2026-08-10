using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class TapeRecorder : Interactable
{
    [SerializeField] private GameObject tapeRecorderObjek;
    [SerializeField] private bool isRingGrabbed = false;
    [SerializeField] private GameObject Complete;
    [SerializeField] private EventReference tapeRecorderSound;
    [SerializeField] private EventReference put;
    [SerializeField] private DialogData dialogData;
    [SerializeField] private DialogData dialogData2;
    [SerializeField] private GameObject tapeRecorderObjek2;
    [SerializeField] private EventReference tapeRecorderSoundPickup;



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
        if (isRingGrabbed)
        {
            tapeRecorderObjek.SetActive(true);
            StartCoroutine(PlayTapeRecorderSound());

        }
    }
    public void GrabRing()
    {
        isRingGrabbed = true;
        RuntimeManager.PlayOneShotAttached(tapeRecorderSoundPickup, gameObject);
    }

    private IEnumerator PlayTapeRecorderSound()
    {
        RuntimeManager.PlayOneShotAttached(put, gameObject);
        yield return new WaitForSeconds(1f);
        EventInstance tapeRecorderInstance = RuntimeManager.CreateInstance(tapeRecorderSound);
        RuntimeManager.AttachInstanceToGameObject(tapeRecorderInstance, transform);
        tapeRecorderInstance.start();

        // Tunggu sampai suara selesai diputar
        yield return new WaitForSeconds(5f); // Ganti 5f dengan durasi suara yang sesuai

        tapeRecorderInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        tapeRecorderInstance.release();
        yield return new WaitForSeconds(1f);
        DialogManager.TriggerDialog(dialogData);
        Complete.SetActive(true);
    }
}
