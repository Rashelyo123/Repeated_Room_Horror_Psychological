using UnityEngine;

public class PintuAwal : Interactable
{
    public Animator doorAnimator;
    public AudioSource audioSource;
    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;
    private bool isOpen = false;
    private bool isPlayerInTrigger = false;
    private bool hasOpenedOnce = false;



    protected override void Interact()
    {
        base.Interact();
        if (!hasOpenedOnce)
        {

            if (!isOpen)
            {
                OpenDoor();
                hasOpenedOnce = true;
            }
        }
    }
    void Start()
    {
        isOpen = false;

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            if (isOpen)
            {
                CloseDoor();
            }
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        doorAnimator.SetTrigger("Open");
        PlaySound(doorOpenSound);

    }

    private void CloseDoor()
    {
        isOpen = false;
        doorAnimator.SetTrigger("Close");
        PlaySound(doorCloseSound);
        Debug.Log("Door closed");
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {

            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("AudioSource or AudioClip is missing!");
        }
    }
}
