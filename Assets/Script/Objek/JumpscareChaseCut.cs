using UnityEngine;
using UnityEngine.Playables;

public class JumpscareChaseCut : MonoBehaviour
{
    [SerializeField] private TapeRecorder tapeRecorder;
    private PlayableDirector director;

    private void Awake() { director = GetComponent<PlayableDirector>(); }
    private void OnEnable() { director.played += OnPlayed; }
    private void OnDisable() { director.played -= OnPlayed; }

    private void OnPlayed(PlayableDirector d)
    {
        if (tapeRecorder != null)
            tapeRecorder.StopChaseImmediate();

        if (AudioManager.Instance != null)
            AudioManager.Instance.StopRoomTone(true);   // true = cut, false = fade out
    }
}