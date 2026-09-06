using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;

public class TimelinePlayer : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;

    [Header("Optional Player Lock")]
    [SerializeField] private MonoBehaviour playerController;
    [SerializeField] private bool lockPlayerDuringTimeline = false;

    [Header("Head Bob")]
    [SerializeField] private HeadBobController headBobController;

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

    public void Play()
    {
        if (director == null)
        {
            Debug.LogWarning($"[TimelinePlayer] Director belum di-assign di {gameObject.name}");
            return;
        }
        if (headBobController == null)
        {
            headBobController = FindObjectOfType<HeadBobController>();
            if (headBobController == null)
            {
                Debug.LogWarning($"[TimelinePlayer] HeadBobController belum di-assign di {gameObject.name}");
                // headBobController masih null di titik ini!
            }
        }


        if (lockPlayerDuringTimeline && playerController != null)
            playerController.enabled = false;

        headBobController.enabled = false; // matiin head bob pas timeline jalan

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

        headBobController.enabled = true; // hidupin lagi pas timeline selesai

        Debug.Log("Timeline selesai, player dikembalikan kontrolnya.");

        onTimelineFinished?.Invoke();
    }
}