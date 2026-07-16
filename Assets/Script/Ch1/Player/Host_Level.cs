using UnityEngine;
using System.Collections;

public class Host_Level : MonoBehaviour
{
    public Animator animator;
    public string animationName = "StartGameAnim"; // Nama animasi yang relevan
    public AudioClip doorOpenSound;
    public AudioSource audioSource;

    public MonoBehaviour playerController; // Tambahkan referensi ke player controller
    public Transform playerTransform; // Tambahkan referensi ke transform player

    private bool isAnimationPlaying = false;
    private Vector3 initialPlayerPosition;
    private Quaternion initialPlayerRotation;

    void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Animator is not assigned!");
            return;
        }

        if (playerTransform != null)
        {
            // Simpan posisi dan rotasi awal player
            initialPlayerPosition = playerTransform.position;
            initialPlayerRotation = playerTransform.rotation;
        }

        // Nonaktifkan player controller saat animasi dimulai
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Memulai animasi
        animator.SetTrigger("Start"); // Ganti dengan nama trigger jika diperlukan
        isAnimationPlaying = true;
        Debug.Log("Animation started");

        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(doorOpenSound);

        // Mulai coroutine untuk menonaktifkan animator setelah 3 detik
        StartCoroutine(DisableAnimatorAfterDelay(3f));
    }

    private IEnumerator DisableAnimatorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (isAnimationPlaying)
        {
            // Simpan posisi dan rotasi setelah animasi
            Vector3 finalPlayerPosition = playerTransform.position;
            Quaternion finalPlayerRotation = playerTransform.rotation;

            animator.runtimeAnimatorController = null;
            animator.enabled = false;
            isAnimationPlaying = false;
            Debug.Log("Animator has been disabled after delay");

            // Kembalikan posisi dan rotasi player setelah animasi selesai
            playerTransform.position = finalPlayerPosition;
            playerTransform.rotation = finalPlayerRotation;

            // Aktifkan kembali player controller setelah animasi selesai
            if (playerController != null)
            {
                playerController.enabled = true;
            }
        }
    }
}
