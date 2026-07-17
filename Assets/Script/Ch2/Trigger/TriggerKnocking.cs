using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerKnocking : MonoBehaviour
{
    public GameObject triggerKnocking;
    public AudioSource audioSource;
    public DoorOpenCH2 doorScript; // Referensi ke skrip DoorOpenCH2

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.Play(); // Mainkan suara
            StartCoroutine(PlaySoundAndObtainKey()); // Mulai coroutine untuk memainkan suara dan memanggil ObtainKey
        }
    }

    private IEnumerator PlaySoundAndObtainKey()
    {
        yield return new WaitForSeconds(audioSource.clip.length); // Tunggu sampai suara selesai dimainkan
        doorScript.ObtainKey(); // Panggil fungsi ObtainKey di skrip DoorOpenCH2
        Destroy(triggerKnocking); // Hancurkan objek trigger
    }
}
