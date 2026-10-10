using UnityEngine;

public class WorldInteractIcon : MonoBehaviour
{
    [SerializeField] private CanvasGroup interactUI;
    [SerializeField] private float fadeSpeed = 8f;
    [SerializeField] private bool billboardToCamera = true;

    private Camera mainCamera;
    private bool isTargeted = false;

    private void Awake()
    {
        mainCamera = Camera.main;
        if (interactUI != null)
            interactUI.alpha = 0f;
    }

    public void SetTargeted(bool targeted)
    {
        isTargeted = targeted;
    }

    private void Update()
    {
        if (interactUI == null) return;

        float target = isTargeted ? 1f : 0f;
        interactUI.alpha = Mathf.Lerp(interactUI.alpha, target, Time.deltaTime * fadeSpeed);

        if (billboardToCamera && mainCamera != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
        }
    }
}