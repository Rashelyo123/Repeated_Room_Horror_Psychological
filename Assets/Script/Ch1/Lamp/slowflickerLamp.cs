using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowBflickerLamp : MonoBehaviour
{
    public Light blinkingLight; // Referensi ke lampu yang ingin dikedipkan
    public float blinkInterval = 1.0f; // Interval waktu antara kedipan (dalam detik)
    public float blinkDuration = 0.5f; // Durasi lampu menyala (dalam detik)

    private bool isBlinking = false;

    void Start()
    {
        if (blinkingLight == null)
        {
            blinkingLight = GetComponent<Light>(); // Coba ambil referensi lampu dari game object
        }

        if (blinkingLight != null)
        {
            StartCoroutine(BlinkLight());
        }
    }

    IEnumerator BlinkLight()
    {
        while (true)
        {
            if (!isBlinking)
            {
                isBlinking = true;

                // Nyalakan lampu
                blinkingLight.enabled = true;
                yield return new WaitForSeconds(blinkDuration);

                // Matikan lampu
                blinkingLight.enabled = false;
                yield return new WaitForSeconds(blinkInterval - blinkDuration);

                isBlinking = false;
            }
        }
    }
}
