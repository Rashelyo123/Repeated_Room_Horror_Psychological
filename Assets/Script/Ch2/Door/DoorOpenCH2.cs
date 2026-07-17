using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenCH2 : MonoBehaviour
{
    public Animator DoorAnim; // Animator untuk pintu
    public bool Key = false; // Menentukan apakah pemain memiliki kunci atau tidak
    private bool isPlayerInRange = false; // Menentukan apakah pemain berada dalam jangkauan pintu
    public AudioSource audioSource; // Sumber audio untuk suara pintu
    public AudioClip doorIsLocked; // Suara ketika pintu terkunci
    public AudioClip doorIsOpen; // Suara ketika pintu terbuka
    public AudioClip CloseDoor; // Suara ketika pintu ditutup
    public Transform playerTransform; // Transform pemain untuk menghitung jarak

    private bool isDoorOpen = false; // Status pintu apakah terbuka atau tidak
    private bool isDoorClosed = true; // Status pintu apakah tertutup atau tidak
    private bool isEKeyDisabled = false; // Flag untuk menonaktifkan tombol E

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
    }

    private void Update()
    {
        if (isEKeyDisabled)
        {
            // Nonaktifkan semua interaksi jika E telah dinonaktifkan
            return;
        }

        if (isDoorOpen)
        {
            // Jika pintu sudah terbuka, nonaktifkan E secara permanen
            isEKeyDisabled = true;
            return;
        }

        if (Key && !isDoorOpen)
        {
            OpenDoor();
        }
        else
        {
            if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(DoorLock());
            }
        }
    }

    public void OpenDoor()
    {
        DoorAnim.SetTrigger("OpenDoor");
        audioSource.clip = doorIsOpen;
        audioSource.Play();
        isDoorOpen = true;
        isDoorClosed = false;
    }

    private IEnumerator DoorLock()
    {
        DoorAnim.SetTrigger("LockDoor");

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        audioSource.volume = 1.0f / (distance + 1.0f);

        audioSource.clip = doorIsLocked;
        audioSource.Play();

        yield return new WaitForSeconds(0.5f);
        DoorAnim.SetTrigger("StartDoor");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    // Fungsi untuk mendapatkan kunci
    public void ObtainKey()
    {
        Key = true;
    }

    public void CloseTheDoor()
    {
        if (!isDoorClosed)
        {
            DoorAnim.SetTrigger("CloseDoor");
            StartCoroutine(PlayCloseDoorSoundWithDelay(0.1f)); // Atur delay sesuai kebutuhan
            Key = false;
            isDoorOpen = false;
            isDoorClosed = true;
        }
    }

    private IEnumerator PlayCloseDoorSoundWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.clip = CloseDoor;
        audioSource.Play();
    }
}
