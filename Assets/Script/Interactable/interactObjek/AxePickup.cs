using UnityEngine;

public class AxePickup : Interactable
{
    [Header("FMOD (opsional)")]
    [SerializeField] private FMODUnity.EventReference PickupSound;
    protected override void Interact()
    {
        base.Interact();
        FMODUnity.RuntimeManager.PlayOneShotAttached(PickupSound, gameObject);
        WeaponHoldManager.Instance.EquipAxe(); // gak perlu kirim prefab lagi
        gameObject.SetActive(false); // kapak yang ada di dunia hilang
    }
}