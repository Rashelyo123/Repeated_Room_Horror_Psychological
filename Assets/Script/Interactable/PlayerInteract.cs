using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float distance = 3f;
    [SerializeField] private LayerMask interactionBlockMask;

    private Interactable currentTarget; // track target sebelumnya (buat icon)

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
        Debug.DrawRay(ray.origin, ray.direction * distance, Color.red);

        Interactable hitInteractable = null;

        if (Physics.Raycast(ray, out RaycastHit hit, distance, interactionBlockMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.TryGetComponent<Interactable>(out var interactable))
            {
                // Kalau CanInteract false (misal InteractOnce udah kepake), gak usah tampilin prompt
                if (interactable.CanInteract)
                {
                    hitInteractable = interactable;
                    playerUI.UpdateText(interactable.PromptMessage);
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.BaseInteract();

                    if (interactable.HideUIAfterInteract)
                        playerUI.UpdateText(string.Empty);

                    if (interactable is IDialogTrigger dialogTrigger)
                        dialogTrigger.TriggerDialog();
                }
            }
        }

        // Kalau target berubah (beda objek, atau gak ada objek sama sekali di depan)
        if (hitInteractable != currentTarget)
        {
            currentTarget?.SetIconTargeted(false); // matiin icon target LAMA
            hitInteractable?.SetIconTargeted(true); // nyalain icon target BARU
            currentTarget = hitInteractable;
        }
    }
}