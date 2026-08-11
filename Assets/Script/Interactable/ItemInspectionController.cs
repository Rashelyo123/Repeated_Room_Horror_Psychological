using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ItemInspectionController : MonoBehaviour
{
    public static ItemInspectionController Instance;

    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject inspectUI;

    [Header("Blur Effect")]
    [SerializeField] private Volume inspectionBlurVolume;
    [SerializeField] private float blurFadeDuration = 0.3f;
    [SerializeField] private float targetBlurWeight = 1f;
    private DepthOfField dofOverride;

    [Header("Player Lock")]
    [SerializeField] private MonoBehaviour playerController;
    [SerializeField] private MonoBehaviour playerInteract;

    private WorldItemInspectable currentItem;
    private bool isInspecting = false;
    private Coroutine blurRoutine;

    private void Awake()
    {
        Instance = this;
        if (inspectUI != null) inspectUI.SetActive(false);

        if (inspectionBlurVolume != null)
        {
            inspectionBlurVolume.weight = 0f;
            inspectionBlurVolume.profile.TryGet(out dofOverride); // <-- ambil reference DoF dari Profile
        }
    }

    public void BeginInspect(WorldItemInspectable item)
    {
        currentItem = item;
        currentItem.Setup(playerCamera);
        isInspecting = true;

        // Set Focus Distance otomatis sesuai jarak object ke kamera
        // if (dofOverride != null)
        // {
        //     dofOverride.focusDistance.value = item.GetInspectDistance(); // <-- baris baru
        // }

        if (playerController != null) playerController.enabled = false;
        if (playerInteract != null) playerInteract.enabled = false;
        if (inspectUI != null) inspectUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        FadeBlur(targetBlurWeight);
        StartCoroutine(currentItem.MoveToInspectPoint());
    }

    public void EndInspect()
    {
        if (currentItem == null) return;
        StartCoroutine(EndInspectRoutine());
    }

    private System.Collections.IEnumerator EndInspectRoutine()
    {
        FadeBlur(0f);
        yield return StartCoroutine(currentItem.MoveBackToOriginal());

        if (playerController != null) playerController.enabled = true;
        if (playerInteract != null) playerInteract.enabled = true;
        if (inspectUI != null) inspectUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isInspecting = false;
        currentItem = null;
    }

    private void FadeBlur(float target)
    {
        if (inspectionBlurVolume == null) return;
        if (blurRoutine != null) StopCoroutine(blurRoutine);
        blurRoutine = StartCoroutine(FadeBlurRoutine(target));
    }

    private System.Collections.IEnumerator FadeBlurRoutine(float target)
    {
        float start = inspectionBlurVolume.weight;
        float t = 0f;

        while (t < blurFadeDuration)
        {
            t += Time.unscaledDeltaTime;
            inspectionBlurVolume.weight = Mathf.Lerp(start, target, t / blurFadeDuration);
            yield return null;
        }
        inspectionBlurVolume.weight = target;
    }

    private void LateUpdate()
    {
        if (isInspecting && currentItem != null)
        {
            if (Input.GetKeyDown(KeyCode.Q))
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
            else
            {
                currentItem.UpdateInspectPosition();
            }
        }
    }
}