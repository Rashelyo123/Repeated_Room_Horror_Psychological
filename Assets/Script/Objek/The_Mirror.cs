using UnityEngine;
using UnityEngine.Playables;

public class The_Mirror : MonoBehaviour
{
    [Header("Timeline")]
    [SerializeField] private PlayableDirector mirrorTimeline;

    [Header("Dialog")]
    [SerializeField] private DialogData dialogData;

    [Header("Player Lock (opsional)")]
    [SerializeField] private PlayerController playerController;

    public GameObject Completel;

    private void OnEnable()
    {
        if (mirrorTimeline != null)
            mirrorTimeline.stopped += OnTimelineFinished;
    }

    private void OnDisable()
    {
        if (mirrorTimeline != null)
            mirrorTimeline.stopped -= OnTimelineFinished;
    }

    // Dipanggil dari Animation Event atau UnityEvent
    public void StartMirrorTimeline()
    {
        if (mirrorTimeline == null)
        {
            Debug.LogWarning($"[The_Mirror] Timeline belum di-assign di {gameObject.name}");
            return;
        }

        if (playerController != null)
            playerController.enabled = false;

        mirrorTimeline.time = 0;
        mirrorTimeline.Play();
    }

    // Dipanggil dari Animation Event atau UnityEvent
    public void StopMirrorTimeline()
    {
        if (mirrorTimeline == null) return;
        mirrorTimeline.Stop();
    }

    // Dipanggil dari Animation Event atau UnityEvent
    public void TriggerMirrorDialog()
    {
        if (dialogData == null)
        {
            Debug.LogWarning($"[The_Mirror] DialogData belum di-assign di {gameObject.name}");
            return;
        }

        DialogManager.TriggerDialog(dialogData);
    }

    private void OnTimelineFinished(PlayableDirector director)
    {
        if (playerController != null)
            playerController.enabled = true;
        Completel.SetActive(true);
    }
}