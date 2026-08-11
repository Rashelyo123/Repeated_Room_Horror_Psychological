using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class Key : Interactable
{

    [SerializeField] private EventReference _onPickUpSound;
    [SerializeField] private DialogData dialogData;
    [SerializeField] private GameObject Complete;

    protected override void Interact()
    {
        base.Interact();
        if (CanInteract)
        {
            // TriggerDialog();
            Complete.SetActive(true);
            RuntimeManager.PlayOneShotAttached(_onPickUpSound, gameObject);
        }
    }
    public void TriggerDialog()
    {
        if (dialogData != null)
        {
            DialogManager.TriggerDialog(dialogData);
        }
    }

}