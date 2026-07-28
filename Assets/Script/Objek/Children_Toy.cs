using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Children_Toy : MonoBehaviour
{
    [SerializeField] private DialogData dialogData;


    public void StartDialogChildrenToy()
    {
        if (dialogData != null)
        {
            DialogManager.TriggerDialog(dialogData);
        }
    }
}
