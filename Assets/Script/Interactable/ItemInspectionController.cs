using UnityEngine;

public class ItemInspectionController : MonoBehaviour
{
    public static ItemInspectionController Instance;

    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject inspectUI; // UI kecil, misal teks "Klik & drag utk putar, ESC utk keluar"

    [Header("Player Lock")]
    [SerializeField] private MonoBehaviour playerController;
    [SerializeField] private MonoBehaviour playerInteract;

    private WorldItemInspectable currentItem;
    private bool isInspecting = false;

    private void Awake()
    {
        Instance = this;
        if (inspectUI != null) inspectUI.SetActive(false);
    }

    public void BeginInspect(WorldItemInspectable item)
    {
        currentItem = item;
        currentItem.Setup(playerCamera);
        isInspecting = true;

        if (playerController != null) playerController.enabled = false;
        if (playerInteract != null) playerInteract.enabled = false;
        if (inspectUI != null) inspectUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(currentItem.MoveToInspectPoint());
    }

    public void EndInspect()
    {
        if (currentItem == null) return;

        StartCoroutine(EndInspectRoutine());
    }

    private System.Collections.IEnumerator EndInspectRoutine()
    {
        yield return StartCoroutine(currentItem.MoveBackToOriginal());

        if (playerController != null) playerController.enabled = true;
        if (playerInteract != null) playerInteract.enabled = true;
        if (inspectUI != null) inspectUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isInspecting = false;
        currentItem = null;
    }

    private void Update()
    {
        if (!isInspecting || currentItem == null) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndInspect();
            return;
        }

        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            currentItem.RotateByInput(mouseX, mouseY);
        }
    }
}