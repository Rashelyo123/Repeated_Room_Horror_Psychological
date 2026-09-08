using UnityEngine;

public class WorldInteractIcon : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeSpeed = 8f;
    [SerializeField] private bool billboardToCamera = true;

    private Camera mainCamera;
    private bool isTargeted = false;

    private void Awake()
    {
        mainCamera = Camera.main;
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    public void SetTargeted(bool targeted)
    {
        isTargeted = targeted;
    }

    private void Update()
    {
        if (canvasGroup == null) return;

        float target = isTargeted ? 1f : 0f;
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, target, Time.deltaTime * fadeSpeed);

        if (billboardToCamera && mainCamera != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
        }
    }
}