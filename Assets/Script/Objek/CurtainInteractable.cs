using System.Collections;
using UnityEngine;

public class CurtainInteractable : Interactable
{
    [Header("Curtain Targets")]
    [SerializeField] private Transform leftCurtain;   // misal Cube.002
    [SerializeField] private Transform rightCurtain;  // misal Cube.003

    [Header("Open Settings")]
    [Tooltip("Jarak geser relatif dari posisi awal (tertutup), dalam local space")]
    [SerializeField] private Vector3 leftOpenOffset = new Vector3(-0.5f, 0f, 0f);
    [SerializeField] private Vector3 rightOpenOffset = new Vector3(0.5f, 0f, 0f);
    [SerializeField] private float duration = 0.6f;
    [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("FMOD (opsional)")]
    [SerializeField] private FMODUnity.EventReference openSound;
    [SerializeField] private FMODUnity.EventReference closeSound;

    private Vector3 leftClosedPos;
    private Vector3 rightClosedPos;
    private bool isOpen = false;
    private bool isAnimating = false;

    private void Awake()
    {
        // posisi awal (tertutup) diambil otomatis dari posisi di scene
        if (leftCurtain != null) leftClosedPos = leftCurtain.localPosition;
        if (rightCurtain != null) rightClosedPos = rightCurtain.localPosition;
    }

    protected override void Interact()
    {
        base.Interact();
        ToggleCurtain();
    }

    public void ToggleCurtain()
    {
        if (isAnimating) return;

        bool willOpen = !isOpen;
        isOpen = willOpen;

        Vector3 leftTarget = willOpen ? leftClosedPos + leftOpenOffset : leftClosedPos;
        Vector3 rightTarget = willOpen ? rightClosedPos + rightOpenOffset : rightClosedPos;

        StartCoroutine(AnimateCurtains(leftTarget, rightTarget));
        PlayCurtainSound(willOpen);
    }

    private void PlayCurtainSound(bool opening)
    {
        FMODUnity.EventReference sound = opening ? openSound : closeSound;
        if (!sound.IsNull)
            FMODUnity.RuntimeManager.PlayOneShotAttached(sound, gameObject);
    }

    private IEnumerator AnimateCurtains(Vector3 leftTarget, Vector3 rightTarget)
    {
        isAnimating = true;

        Vector3 leftStart = leftCurtain.localPosition;
        Vector3 rightStart = rightCurtain.localPosition;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float n = easeCurve.Evaluate(t / duration);

            leftCurtain.localPosition = Vector3.Lerp(leftStart, leftTarget, n);
            rightCurtain.localPosition = Vector3.Lerp(rightStart, rightTarget, n);
            yield return null;
        }

        leftCurtain.localPosition = leftTarget;
        rightCurtain.localPosition = rightTarget;
        isAnimating = false;
    }
}