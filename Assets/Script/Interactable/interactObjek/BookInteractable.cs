using System.Collections;
using UnityEngine;

public class BookInteractable : Interactable
{
    [Header("Target")]
    [SerializeField] private Transform coverTransform; // drag child yang mau dirotasi (misal Cube.001)

    [Header("Book Settings")]
    [SerializeField] private float closedRotationX = -170f;
    [SerializeField] private float openRotationX = -60f;
    [SerializeField] private float openDuration = 0.5f;
    [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("FMOD (opsional)")]
    [SerializeField] private FMODUnity.EventReference openSound;
    [SerializeField] private FMODUnity.EventReference closeSound;

    private bool isOpen = false;
    private bool isAnimating = false;
    private Quaternion closedRotation;

    private void Awake()
    {
        if (coverTransform == null)
            coverTransform = transform; // fallback kalau gak di-assign, pakai diri sendiri

        closedRotation = coverTransform.localRotation;
    }

    protected override void Interact()
    {
        base.Interact();
        if (CanInteract)
        {
            ToggleBook();
        }
    }

    public void ToggleBook()
    {
        if (isAnimating) return;
        StopAllCoroutines();

        bool willOpen = !isOpen;
        float targetX = willOpen ? openRotationX : closedRotationX;

        StartCoroutine(AnimateBook(targetX));
        isOpen = willOpen;

        PlayBookSound(willOpen);
    }

    private void PlayBookSound(bool opening)
    {
        FMODUnity.EventReference soundToPlay = opening ? openSound : closeSound;

        if (!soundToPlay.IsNull)
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(soundToPlay, gameObject);
        }
    }

    private IEnumerator AnimateBook(float targetRotationX)
    {
        isAnimating = true;

        Quaternion startRot = coverTransform.localRotation;
        Quaternion targetRot = Quaternion.Euler(targetRotationX, 0f, 0f);

        float t = 0f;

        while (t < openDuration)
        {
            t += Time.deltaTime;
            float normalized = easeCurve.Evaluate(t / openDuration);
            coverTransform.localRotation = Quaternion.Slerp(startRot, targetRot, normalized);
            yield return null;
        }

        coverTransform.localRotation = targetRot;
        isAnimating = false;
    }
}