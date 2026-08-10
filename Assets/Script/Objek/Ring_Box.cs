using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring_Box : Interactable
{
    [SerializeField] private GameObject ringObjek;
    [SerializeField] private bool isRingGrabbed = false;
    [SerializeField] private GameObject Complete;
    [Header("FMOD (opsional)")]
    [SerializeField] private FMODUnity.EventReference PickupSound;
    [SerializeField] private FMODUnity.EventReference PutRingSound;
    [SerializeField] private DialogData dialogData;


    protected override void Interact()
    {
        base.Interact();
        if (CanInteract && isRingGrabbed)
        {
            ActiveRing();
            FMODUnity.RuntimeManager.PlayOneShotAttached(PutRingSound, gameObject);
        }
        else
        {
            CanInteract = false;
            DialogManager.TriggerDialog(dialogData);
        }

    }

    public void ActiveRing()
    {
        if (isRingGrabbed)
        {
            ringObjek.SetActive(true);
            Complete.SetActive(true);
            CanInteract = false;
        }
    }
    public void GrabRing()
    {
        isRingGrabbed = true;
        CanInteract = true;
        FMODUnity.RuntimeManager.PlayOneShotAttached(PickupSound, gameObject);
    }
}
