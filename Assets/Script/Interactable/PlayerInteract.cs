using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float Distance = 3f;
    [SerializeField] private LayerMask interactionBlockMask; // isi SEMUA layer solid: Interactable + Wall/Environment/Furniture, dll

    private Camera cam;
    private PlayerUI playerUI;

    private void Start()
    {
        cam = GetComponent<PlayerController>().playerCamera;
        playerUI = GetComponent<PlayerUI>();
    }

    private void Update()
    {
        playerUI?.UpdateText(string.Empty);

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * Distance, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, Distance, interactionBlockMask, QueryTriggerInteraction.Ignore))
        {
            // objek pertama yang kena secara fisik, apapun itu (bisa laci, tembok, atau si kunci)
            if (hit.collider.TryGetComponent<Interactable>(out var interactable))
            {
                playerUI.UpdateText(interactable.PromptMessage);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.BaseInteract();

                    if (interactable is IDialogTrigger dialogTrigger)
                    {
                        dialogTrigger.TriggerDialog();
                    }
                }
            }
            // kalau yang kena BUKAN Interactable (misal laci tertutup / tembok), otomatis nggak ada prompt
        }
    }
}