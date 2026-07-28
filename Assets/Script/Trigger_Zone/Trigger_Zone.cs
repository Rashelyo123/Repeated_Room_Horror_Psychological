using UnityEngine;
using UnityEngine.Events;

public class Trigger_Zone : MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private bool useTagCheck = true;
    [SerializeField] private bool disableAfterTrigger = true;

    [Header("Events")]
    [SerializeField] private UnityEvent onTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (useTagCheck && !other.CompareTag(targetTag))
        {
            return;
        }

        onTriggered?.Invoke();

        if (disableAfterTrigger)
        {
            gameObject.SetActive(false);
        }
    }
}
