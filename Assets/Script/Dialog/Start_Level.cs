using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Start_Level : MonoBehaviour
{
    [SerializeField] private DialogData dialogData;

    private void Start()
    {
        if (dialogData != null)
        {
            DialogManager.TriggerDialog(dialogData);
        }
    }

}
