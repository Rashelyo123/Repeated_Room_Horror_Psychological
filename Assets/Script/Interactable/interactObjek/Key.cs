
using UnityEngine;

public class Key : Interactable
{

    [SerializeField] private AudioClip _onPickUpSound;
    [SerializeField] private DialogData dialogData;
    protected override void Interact()
    {
        base.Interact();
        if (CanInteract)
        {
            TriggerDialog();
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