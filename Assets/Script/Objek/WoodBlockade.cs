using UnityEngine;

public class WoodBlockade : MonoBehaviour, IAxeHittable
{
    [SerializeField] private int hitsRequired = 3;
    [SerializeField] private GameObject destroyedVFX;
    [SerializeField] private FMODUnity.EventReference DestroyedSound;
    [SerializeField] private GameObject Complete;
    private int currentHits = 0;

    public void OnAxeHit()
    {
        currentHits++;
        Debug.Log($"Blokade kena hit: {currentHits}/{hitsRequired}");

        if (currentHits >= hitsRequired)
        {
            if (destroyedVFX != null)
                Instantiate(destroyedVFX, transform.position, transform.rotation);

            gameObject.SetActive(false);
            Complete.SetActive(true);
            FMODUnity.RuntimeManager.PlayOneShotAttached(DestroyedSound, gameObject);
        }
    }
}