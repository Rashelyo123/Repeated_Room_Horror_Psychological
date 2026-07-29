using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start_Level : MonoBehaviour
{
    [SerializeField] private DialogData dialogData;

    private void Start()
    {
        if (dialogData != null && SceneManager.GetActiveScene().name == "Loop1")
        {
            DialogManager.TriggerDialog(dialogData);
        }
    }

}
