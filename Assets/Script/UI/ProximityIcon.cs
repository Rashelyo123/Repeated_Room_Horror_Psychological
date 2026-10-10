using UnityEngine;

public class ProximityIcon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup borderInteract;

    [Header("Settings")]
    [SerializeField] private float showDistance = 3f;
    [SerializeField] private float fadeSpeed = 8f;
    [SerializeField] private bool billboardToCamera = true;

    private Camera mainCamera;
    private float currentAlpha = 0f;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (borderInteract == null)
            borderInteract = GetComponent<CanvasGroup>();

        if (borderInteract == null)
            borderInteract = gameObject.AddComponent<CanvasGroup>();

        borderInteract.alpha = 0f;
        currentAlpha = 0f;
    }

    private void OnEnable()
    {
        if (ProximityIconManager.Instance != null)
            ProximityIconManager.Instance.Register(this);
    }

    private void Start()
    {
        if (ProximityIconManager.Instance != null)
            ProximityIconManager.Instance.Register(this);
    }

    private void OnDisable()
    {
        ProximityIconManager.Instance?.Unregister(this);
    }

    // Dipanggil dari ProximityIconManager, BUKAN dari Update() sendiri
    public void UpdateVisibility(Vector3 playerPosition)
    {
        if (borderInteract == null) return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        float distance = Vector3.Distance(transform.position, playerPosition);
        bool shouldShow = distance <= showDistance;

        float target = shouldShow ? 1f : 0f;
        currentAlpha = Mathf.Lerp(currentAlpha, target, Time.deltaTime * fadeSpeed);
        borderInteract.alpha = currentAlpha;

        if (billboardToCamera && mainCamera != null && currentAlpha > 0.01f)
        {
            Vector3 lookDirection = transform.position - mainCamera.transform.position;
            if (lookDirection.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }
}