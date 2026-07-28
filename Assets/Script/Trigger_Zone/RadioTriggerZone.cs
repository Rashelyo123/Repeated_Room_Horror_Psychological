using UnityEngine;

public class RadioTriggerZone : MonoBehaviour
{
    [SerializeField] private RadioInteractable radioController;

    private void OnTriggerEnter(Collider other)
    {
        // Ketika Player melintasi area lorong
        if (other.CompareTag("Player"))
        {
            // Jalankan radio di ruang utama
            radioController.StartRadioBroadcast();

            // Matikan trigger ini agar tidak mentrigger ulang
            gameObject.SetActive(false);
        }
    }
}
