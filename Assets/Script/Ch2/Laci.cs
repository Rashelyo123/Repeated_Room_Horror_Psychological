using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Laci : MonoBehaviour
{
    public GameObject E;
    public AudioSource audioSource;
    public AudioSource writeBoardAudioSource; // AudioSource untuk sound "write board"
    public GameObject TriggerText;
    public GameObject jumpscare;

     public AudioSource doorOpen;

    private bool IsLaciOpen = false;
    public Animator LaciAnim;
    

    public bool missionComplete = false;

    public Animator door;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            E.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            E.SetActive(false);
        }
    }

    private void Update()
    {
        if (E.activeSelf && Input.GetKeyDown(KeyCode.E) && !IsLaciOpen)
        {
            OpenLaci();
            StartCoroutine(TriggerTextCoroutine());
            jumpscare.SetActive(true);
        }
    }

    public void OpenLaci()
    {
        IsLaciOpen = true;
        audioSource.Play();
        LaciAnim.SetTrigger("open");
    }

    private IEnumerator TriggerTextCoroutine()
    {
        writeBoardAudioSource.Play(); // Play sound "write board"
        yield return new WaitForSeconds(1.5f); // Tunggu 1.5 detik sebelum teks muncul
        TriggerText.SetActive(true); // Aktifkan teks
        yield return new WaitForSeconds(5f); // Tunggu 5 detik setelah teks aktif
      door.SetTrigger("OpenDoor"); // Buka pintu
        doorOpen.Play(); // Play sound "door open"
      
        yield return new WaitForSeconds(1.5f); // Tunggu 1.5 detik setelah pintu terbuka
      Debug.Log("Door Opened");
        missionComplete = true; // Set missionComplete menjadi true
       
    }
}
