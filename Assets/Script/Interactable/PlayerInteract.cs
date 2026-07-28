using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float distance = 3f;
    [SerializeField] private LayerMask interactionBlockMask;

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

        if (Physics.Raycast(ray, out RaycastHit hit, distance, interactionBlockMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.TryGetComponent<Interactable>(out var interactable))
            {
                // Kalau CanInteract false (misal InteractOnce udah kepake), gak usah tampilin prompt
                if (interactable.CanInteract)
                    playerUI.UpdateText(interactable.PromptMessage);

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
    }
}