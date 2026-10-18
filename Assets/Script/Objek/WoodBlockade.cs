using UnityEngine;

public class WoodBlockade : MonoBehaviour, IAxeHittable
{
    [Header("Hit Settings")]
    [SerializeField] private int hitsRequired = 3;

    [Header("Effects")]
    [SerializeField] private GameObject destroyedVFX;
    [SerializeField] private FMODUnity.EventReference DestroyedSound;

    [Header("Blockade")]
    public Collider Blockade;

    [Header("Kayu yang Jatuh")]
    public Rigidbody[] woodPieces;
    [SerializeField] private string debrisLayerName = "Debris";

    [Header("Events")]
    public UnityEngine.Events.UnityEvent OnBlockadeDestroyed;

    private int currentHits = 0;
    private bool isDestroyed = false;

    public void OnAxeHit()
    {
        if (isDestroyed) return;

        currentHits++;
        Debug.Log($"Blokade kena hit: {currentHits}/{hitsRequired}");

        if (currentHits >= hitsRequired)
        {
            isDestroyed = true;

            if (destroyedVFX != null)
                Instantiate(destroyedVFX, transform.position, transform.rotation);

            if (Blockade != null)
                Blockade.enabled = false;

            DestroyWoodDrop();

            OnBlockadeDestroyed?.Invoke();
            FMODUnity.RuntimeManager.PlayOneShotAttached(DestroyedSound, gameObject);
        }
    }

    private void DestroyWoodDrop()
    {
        int debrisLayer = LayerMask.NameToLayer(debrisLayerName);
        if (debrisLayer == -1)
            Debug.LogError($"Layer '{debrisLayerName}' belum dibuat.", this);

        foreach (var rb in woodPieces)
        {
            if (rb == null) continue;

            // Ganti layer dulu supaya player tidak ketahan saat kayu jatuh
            if (debrisLayer != -1)
                SetLayerRecursive(rb.gameObject, debrisLayer);

            rb.isKinematic = false;
            rb.useGravity = true;
            rb.WakeUp();
        }
    }

    private void SetLayerRecursive(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
            SetLayerRecursive(child.gameObject, layer);
    }
}