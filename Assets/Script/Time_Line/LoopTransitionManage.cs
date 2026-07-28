using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class LoopTransitionManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayableDirector timelineTransition;
    [SerializeField] private DialogData dialogData;

    private void OnEnable()
    {
        timelineTransition.stopped += OnFinishTimeline;
    }

    private void OnDisable()
    {
        timelineTransition.stopped -= OnFinishTimeline;
    }

    private void Start()
    {
        playerController.enabled = false;
        timelineTransition.Play();
    }

    private void OnFinishTimeline(PlayableDirector director)
    {
        playerController.enabled = true;
        DialogTrigger();
    }
    private void DialogTrigger()
    {
        if (dialogData != null && (SceneManager.GetActiveScene().name == "Loop2" || SceneManager.GetActiveScene().name == "Loop3"))
        {
            DialogManager.TriggerDialog(dialogData);
        }

    }
}