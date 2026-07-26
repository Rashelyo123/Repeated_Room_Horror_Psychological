
using UnityEngine;

public class Key : Interactable
{

    [SerializeField] private AudioClip _onPickUpSound;
    [SerializeField] private DialogData dialogData;
    [SerializeField] private GameObject Complete;

    protected override void Interact()
    {
        base.Interact();
        if (CanInteract)
        {
            // TriggerDialog();
            Complete.SetActive(true);

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