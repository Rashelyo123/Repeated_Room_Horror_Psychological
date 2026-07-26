using System.Collections;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light[] lightSources; // Array untuk menampung beberapa lampu
    public float minIntensity = 0.5f;
    public float maxIntensity = 2.0f;
    public float flickerSpeed = 0.1f;
    public Renderer[] lightRenderers; // Array untuk menampung renderer material lampu
    public Color emissionColor = Color.white; // Warna emission
    public AudioSource audioSource; // AudioSource untuk suara flicker
    public AudioClip flickerSound; // Suara flicker
    public Transform playerTransform; // Transform pemain untuk menghitung jarak

    private Color blackColor = Color.black; // Warna hitam untuk emission

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>(); // Mengambil AudioSource dari objek ini jika belum diatur
        }

        audioSource.clip = flickerSound;
        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        while (true)
        {
            float intensity = Random.Range(minIntensity, maxIntensity);
            foreach (Light lightSource in lightSources)
            {
                lightSource.intensity = intensity;
            }
            foreach (Renderer renderer in lightRenderers)
            {
                // Set the emission color based on the intensity
                if (intensity <= minIntensity)
                {
                    renderer.material.SetColor("_EmissionColor", blackColor);
                }
                else
                {
                    renderer.material.SetColor("_EmissionColor", emissionColor * intensity);
                }
                // Ensure emission is enabled
                renderer.material.EnableKeyword("_EMISSION");
            }

            // Adjust audio volume based on the distance to the player
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            audioSource.volume = 1.0f / (distance + 1.0f); // Simple distance attenuation

            // Play the flicker sound
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            yield return new WaitForSeconds(flickerSpeed);
        }
    }
}
