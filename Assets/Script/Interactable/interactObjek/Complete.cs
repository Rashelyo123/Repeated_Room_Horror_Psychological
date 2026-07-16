using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Complete : MonoBehaviour
{
    [SerializeField] private PintuAkhir quest;



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            quest.isPlayerMissionComplete = true;
            quest.CanInteract = true;
            gameObject.SetActive(false);
            Debug.Log("Mission Complete!");
        }
    }
}
