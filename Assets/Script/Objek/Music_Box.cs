using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music_Box : Interactable
{
    [Header("FMOD (opsional)")]
    [SerializeField] private FMODUnity.EventReference MusicBoxSound;
    [SerializeField] private FMODUnity.EventReference PickupSound;
    [SerializeField] private bool isBroken = true;
    [SerializeField] private bool isInspect = false;
    [SerializeField] private GameObject Engkol;

    [SerializeField] private DialogData dialogData_isBroken;
    [SerializeField] private DialogData dialogData_isFixed;
    [SerializeField] private GameObject Complete;


    protected override void Interact()
    {
        if (CanInteract && !isInspect)
        {
            DialogManager.TriggerDialog(dialogData_isBroken);
            Engkol.SetActive(true);
            isInspect = true;
            CanInteract = false;
        }
        else if (CanInteract && isInspect)
        {
            StartCoroutine(PlayMusicBoxSoundCoroutine());

        }


    }

    public void GrabMusicBox()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(PickupSound, gameObject);
    }
    private void PlayMusicBoxSound()
    {
        if (!isBroken)
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(MusicBoxSound, gameObject);
        }
    }

    public void FixMusicBox()
    {
        isBroken = false;
    }
    private IEnumerator PlayMusicBoxSoundCoroutine()
    {
        PlayMusicBoxSound();
        isInspect = false;
        CanInteract = false;
        yield return new WaitForSeconds(2f);
        DialogManager.TriggerDialog(dialogData_isFixed);
        Complete.SetActive(true);


    }
}
