using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;

public class TimelinePlayer : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;

    [Header("Optional Player Lock")]
    [SerializeField] private MonoBehaviour playerController;
    [SerializeField] private bool lockPlayerDuringTimeline = false;

    [Header("Events")]
    public UnityEvent onTimelineStart;
    public UnityEvent onTimelineFinished;

    private void OnEnable()
    {
        if (director != null)
            director.stopped += HandleTimelineStopped;
    }

    private void OnDisable()
    {
        if (director != null)
            director.stopped -= HandleTimelineStopped;
    }

    // Ini yang dipanggil dari UnityEvent
    public void Play()
    {
        if (director == null)
        {
            Debug.LogWarning($"[TimelinePlayer] Director belum di-assign di {gameObject.name}");
            return;
        }

        if (lockPlayerDuringTimeline && playerController != null)
            playerController.enabled = false;

        director.time = 0;
        director.Play();

        onTimelineStart?.Invoke();
    }

    public void Stop()
    {
        if (director != null)
            director.Stop();
    }

    private void HandleTimelineStopped(PlayableDirector d)
    {
        if (lockPlayerDuringTimeline && playerController != null)
            playerController.enabled = true;
        Debug.Log("Timeline selesai, player dikembalikan kontrolnya.");

        onTimelineFinished?.Invoke();
    }
}