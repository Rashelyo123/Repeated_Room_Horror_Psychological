using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SojaExiles
{
    public class opencloseDoor1 : MonoBehaviour
    {
        public Animator openandclose1; // Animator untuk pintu
        public bool open; // Status apakah pintu terbuka atau tertutup
        public Transform Player; // Transform pemain untuk menghitung jarak
        public float closeDistance = 1f; // Jarak maksimum agar pintu tetap terbuka

        public AudioClip doorOpenSound; // Drag your door open sound here in the inspector
        public AudioClip doorCloseSound; // Drag your door close sound here in the inspector
        private AudioSource audioSource;
        private bool hasBeenOpenedOnce = false; // Status apakah pintu sudah pernah dibuka

        void Start()
        {
            open = false;
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        void Update()
        {
            // Periksa jarak pemain dari pintu secara terus-menerus
            if (Player)
            {
                float dist = Vector3.Distance(Player.position, transform.position);
                if (open && dist >= closeDistance)
                {
                    StartCoroutine(closing());
                }

                // Periksa apakah pemain menekan tombol E dan berada dalam jarak dekat dengan pintu
                if (!hasBeenOpenedOnce && dist < closeDistance && Input.GetKeyDown(KeyCode.E))
                {
                    if (!open)
                    {
                        StartCoroutine(opening());
                    }
                    else
                    {
                        StartCoroutine(closing());
                    }
                }
            }
        }

        IEnumerator opening()
        {
            print("You are opening the door");
            openandclose1.Play("Opening 1");
            audioSource.PlayOneShot(doorOpenSound);
            open = true;
            hasBeenOpenedOnce = true; // Menandai bahwa pintu sudah pernah dibuka
            yield return new WaitForSeconds(0.5f);
        }

        IEnumerator closing()
        {
            print("You are closing the door");
            openandclose1.Play("Closing 1");
            audioSource.PlayOneShot(doorCloseSound);
            open = false;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
