using System.Collections;
using UnityEngine;

public class DrawerInteractable : Interactable
{
    [Header("Drawer Settings")]
    [SerializeField] private Vector3 openLocalPosition = new Vector3(-0.79f, 0.9038773f, 0.0005483627f);
    [SerializeField] private float openDuration = 0.5f;
    [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("FMOD (opsional)")]
    [SerializeField] private FMODUnity.EventReference drawerSound;

    private Vector3 closedLocalPos;
    private bool isOpen = false;
    private bool isAnimating = false;

    protected override void Interact()
    {
        base.Interact();
        if (CanInteract)
        {
            // TriggerDialog();
            ToggleDrawer();

        }
    }

    private void Awake()
    {
        // posisi awal (tertutup) diambil otomatis dari posisi laci di scene saat game mulai
        closedLocalPos = transform.localPosition;
    }

    public void ToggleDrawer()
    {
        if (isAnimating) return;
        StopAllCoroutines();
        Vector3 target = isOpen ? closedLocalPos : openLocalPosition;
        StartCoroutine(AnimateDrawer(target));
        isOpen = !isOpen;

        if (!drawerSound.IsNull)
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(drawerSound, gameObject);
        }
    }

    private IEnumerator AnimateDrawer(Vector3 targetLocalPos)
    {
        isAnimating = true;
        Vector3 startPos = transform.localPosition;
        float t = 0f;

        while (t < openDuration)
        {
            t += Time.deltaTime;
            float normalized = easeCurve.Evaluate(t / openDuration);
            transform.localPosition = Vector3.Lerp(startPos, targetLocalPos, normalized);
            yield return null;
        }

        transform.localPosition = targetLocalPos;
        isAnimating = false;
    }
}