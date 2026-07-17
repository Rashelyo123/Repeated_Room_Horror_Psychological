using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class PlayerInteract : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private float Distance = 3f;
    [SerializeField] private LayerMask Mask;
    private PlayerUI playerUI;

    void Start()
    {
        cam = GetComponent<PlayerController>().playerCamera;
        playerUI = GetComponent<PlayerUI>();

    }

    private void Update()
    {
        playerUI?.UpdateText(string.Empty);

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * Distance, Color.red);

        RaycastHit[] hits = Physics.RaycastAll(ray, Distance, Mask, QueryTriggerInteraction.Collide);
        foreach (var hitInfo in hits)
        {
            // Hanya proses collider yang bukan trigger
            if (!hitInfo.collider.isTrigger && hitInfo.collider.TryGetComponent<Interactable>(out var interactable))
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
                break; // Hanya tangani collider non-trigger pertama
            }
        }
    }

}
