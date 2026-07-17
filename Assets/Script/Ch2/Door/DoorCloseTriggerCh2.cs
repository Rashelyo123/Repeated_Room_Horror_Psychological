using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCloseTriggerCh2 : MonoBehaviour
{
    public DoorOpenCH2 doorOpenCH2; // Referensi ke script DoorOpenCH2


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorOpenCH2.CloseTheDoor(); // Panggil fungsi CloseDoor di script DoorOpenCH2
            Debug.Log("Close the door");
        }
    }

    
}
