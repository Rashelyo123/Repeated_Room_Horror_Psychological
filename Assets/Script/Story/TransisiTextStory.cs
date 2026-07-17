using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransisiTextStory : MonoBehaviour
{
    public GameObject jam;
    public GameObject tanggal;
    public float delay = 1.5f; // Waktu jeda antara setiap transisi

    // Referensi ke skrip TypewriterEffectTMP
    

    void Start()
    {
        StartCoroutine(Awal());
    }

    private IEnumerator Awal()
    {
        yield return new WaitForSeconds(delay);
        jam.SetActive(true);
        tanggal.SetActive(false);

        yield return new WaitForSeconds(delay);
        jam.SetActive(false);

        // Jalankan skrip TypewriterEffectTMP setelah TransisiTextStory selesai
       
    }
}
