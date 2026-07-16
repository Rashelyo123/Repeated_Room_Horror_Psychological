using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DoorCh3 : MonoBehaviour
{
   
   public Animator DoorAnim; // Animator untuk pintu
   public AudioSource audioSource; // Sumber audio untuk suara pintu

   public GameObject E;
   public bool isOpen = false;


void Update(){
   if(Input.GetKeyDown(KeyCode.E ) && E.activeSelf && !isOpen){
       OpenDoor();
         isOpen = true;

   }

}

private void OnTriggerEnter(Collider other){
    if (other.gameObject.tag == "Player")
    {
        E.SetActive(true);
    }

  
}

private void OnTriggerExit(Collider other){
    if (other.gameObject.tag == "Player")
    {
        E.SetActive(false);
    }
}

   public void OpenDoor()
   {
      DoorAnim.SetTrigger("OpenDoor");
        audioSource.Play();
   }
}
