using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using FMODUnity;

public class BloodCreature : MonoBehaviour
{
    [SerializeField] private GameObject[] bloodLinePrefab;

    [Header("FMOD (opsional)")]
    [SerializeField] private FMODUnity.EventReference bloodSound;
    [SerializeField] private LockedDoorHandle lockedDoorHandle;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayableDirector timeLineBloodReaction;
    [SerializeField] private DialogData dialogData;

    [SerializeField] private EventReference SwitchOn;

    public void ShowBloodLine()
    {
        StartCoroutine(ScenarioCoroutine());
    }
    public void PlaySwitchOnSound()
    {
        RuntimeManager.PlayOneShotAttached(SwitchOn, gameObject);
    }
    private IEnumerator ScenarioCoroutine()
    {
        for (int i = 0; i < bloodLinePrefab.Length; i++)
        {
            bloodLinePrefab[i].SetActive(true);
            FMODUnity.RuntimeManager.PlayOneShotAttached(bloodSound, gameObject);
            yield return new WaitForSeconds(0.5f);
        }

        lockedDoorHandle.Unlock();
    }

    public void StartBloodReactionTimeline()
    {
        StartCoroutine(BloodReactionRoutine());
    }

    private IEnumerator BloodReactionRoutine()
    {
        playerController.enabled = false;

        yield return new WaitForSeconds(0.5f);

        timeLineBloodReaction.stopped += OnTimelineFinished;
        timeLineBloodReaction.Play();

        DialogManager.TriggerDialog(dialogData);
    }

    private void OnTimelineFinished(PlayableDirector director)
    {
        timeLineBloodReaction.stopped -= OnTimelineFinished; // unsubscribe, penting!
        playerController.enabled = true;
    }
}