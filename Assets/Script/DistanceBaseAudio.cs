using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFX_CREPPY : MonoBehaviour
{
    [SerializeField] private GameObject triggerObject;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerObject.SetActive(true);
            Debug.Log("Player has entered the trigger area.");
        }
    }

}