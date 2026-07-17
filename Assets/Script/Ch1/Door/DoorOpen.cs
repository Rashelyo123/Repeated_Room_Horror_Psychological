using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : Interactable
{
    public Animator DoorAnim;
    public bool Key = false;
    public AudioSource audioSource;
    public AudioClip doorIsLocked;
    public AudioClip doorOpenSound;


    protected override void Interact()
    {
        base.Interact();
        if (CanInteract)
        {
            if (Key)
            {
                DoorAnim.SetTrigger("OpenDoor");
                audioSource.PlayOneShot(doorOpenSound);
                CanInteract = false;
            }
            else
            {
                audioSource.Play();
                StartCoroutine(DoorTimer());
            }
        }
    }
    void Start()
    {
        // Periksa apakah AudioSource sudah ada, jika tidak tambahkan
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        audioSource.clip = doorIsLocked;
    }


    private IEnumerator DoorTimer()
    {
        DoorAnim.SetTrigger("LockDoor");


        yield return new WaitForSeconds(0.5f);
        DoorAnim.SetTrigger("StartDoor");
    }



}
