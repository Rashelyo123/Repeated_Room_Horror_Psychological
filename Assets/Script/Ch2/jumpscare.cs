using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Jumpscare : MonoBehaviour
{
    public GameObject jumpscareObject;
    public AudioSource audioSource;
   
   
   

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
           
            StartCoroutine(BackToMenu());
        }
    }

    private IEnumerator BackToMenu()
    {
        
        audioSource.Play();
      yield return new WaitForSeconds(0.1f); // Tunggu 2 detik
       jumpscareObject.SetActive(true); // Aktifkan jumpscare
        
       
    }
}
