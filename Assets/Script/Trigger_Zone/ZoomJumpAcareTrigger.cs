using UnityEngine;
using UnityEngine.Playables;

public class ZoomJumpscareTrigger : MonoBehaviour
{
    [Header("Timeline")]
    [SerializeField] private PlayableDirector jumpscareTimeline;

    [Header("Input Settings")]
    [SerializeField] private bool triggerOnce = true;
    [SerializeField] private float requiredZoomDuration = 5f; // <-- durasi yg dibutuhkan

    [Header("Look Check")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform doorTarget;
    [SerializeField] private float lookDotThreshold = 0.8f;

    [Header("Zoom Reference")]
    [SerializeField] private PlayerZoom playerZoom;

    [Header("Player Lock (opsional)")]
    [SerializeField] private MonoBehaviour playerController;

    [Header("Complete")]
    [SerializeField] private GameObject completeObject;

    private bool playerInZone = false;
    private bool hasTriggered = false;
    private float holdTimer = 0f; // <-- akumulasi waktu

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInZone = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            holdTimer = 0f; // reset kalau keluar zone
        }
    }

    private void Update()
    {
        if (!playerInZone) return;
        if (triggerOnce && hasTriggered) return;

        bool conditionMet = playerZoom.IsZooming() && IsLookingAtDoor();

        if (conditionMet)
        {
            holdTimer += Time.deltaTime;


            if (holdTimer >= requiredZoomDuration)
            {
                TriggerJumpscare();
            }
        }
        else
        {
            holdTimer = 0f; // reset kalau syarat putus (lepas zoom / noleh arah lain)
        }
    }

    private bool IsLookingAtDoor()
    {
        if (playerCamera == null || doorTarget == null) return true;

        Vector3 dirToDoor = (doorTarget.position - playerCamera.transform.position).normalized;
        float dot = Vector3.Dot(playerCamera.transform.forward, dirToDoor);
        return dot >= lookDotThreshold;
    }

    private void TriggerJumpscare()
    {
        hasTriggered = true;

        if (playerController != null)
            playerController.enabled = false;

        jumpscareTimeline.stopped += OnJumpscareFinished;
        jumpscareTimeline.Play();
        completeObject.SetActive(true);
    }

    private void OnJumpscareFinished(PlayableDirector director)
    {
        jumpscareTimeline.stopped -= OnJumpscareFinished;

        if (playerController != null)
            playerController.enabled = true;
    }
}