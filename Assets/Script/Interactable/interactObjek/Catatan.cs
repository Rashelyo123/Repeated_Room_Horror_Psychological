using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Catatan : Interactable
{
    [SerializeField] private GameObject catatan;
    [SerializeField] private AudioClip onPickUpSound;
    [SerializeField] private AudioClip brokenLampu;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject[] Light;

    private bool lampuSudahPadam = false;
    private bool isInteracting = false;

    void Start()
    {
        catatan.SetActive(false);
    }

    void Update()
    {
        if (catatan.activeSelf && Input.GetMouseButton(0))
        {
            TutupCatatan();
        }
    }

    protected override void Interact()
    {
        if (isInteracting || !CanInteract) return;

        isInteracting = true;
        base.Interact();

        // Toggle status catatan
        catatan.SetActive(!catatan.activeSelf);

        if (catatan.activeSelf)
        {
            // Membuka catatan
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (onPickUpSound != null)
            {
                AudioSource.PlayClipAtPoint(onPickUpSound, transform.position);
            }

            PromptMessage = "";
            Time.timeScale = 0f;
            Debug.Log("Catatan dibuka");
        }
        else
        {
            // Menutup catatan
            TutupCatatan();
        }

        StartCoroutine(ResetInteract());
    }

    private void TutupCatatan()
    {
        catatan.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PromptMessage = "E";
        Time.timeScale = 1f;
        Debug.Log("Catatan ditutup");

        if (!lampuSudahPadam)
        {
            lampuSudahPadam = true;
            StartCoroutine(LampuPadam());
        }
    }

    private IEnumerator LampuPadam()
    {
        yield return new WaitForSeconds(3f);

        if (brokenLampu != null)
        {
            AudioSource.PlayClipAtPoint(brokenLampu, transform.position);
        }

        if (Light != null && Light.Length >= 2)
        {
            Light[0].SetActive(false);
            yield return new WaitForSeconds(0.5f);
            Light[1].SetActive(true);
            yield return new WaitForSeconds(0.5f);
            Light[2].SetActive(true);
        }
        else
        {
            Debug.LogWarning("Array Light tidak valid atau tidak memiliki cukup elemen.");
        }
    }

    private IEnumerator ResetInteract()
    {
        yield return new WaitForSeconds(0.1f);
        isInteracting = false;
    }
}
