using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triggerGhost : MonoBehaviour
{
    [SerializeField] private AudioClip ghostSound;
    bool isTrigger = false;
    [SerializeField] Animator DoorAnimator;
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && !isTrigger)
        {
            StartCoroutine(TriggerGhostCoroutine());
            isTrigger = true;
            if (DoorAnimator != null)
            {
                DoorAnimator.SetTrigger("CloseDoor");
            }

        }
    }

    private IEnumerator TriggerGhostCoroutine()
    {
        yield return new WaitForSeconds(0.3f);
        AudioSource.PlayClipAtPoint(ghostSound, transform.position);
        Destroy(gameObject);
    }
}
