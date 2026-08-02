using UnityEngine;

public class AxePickup : Interactable
{
    [SerializeField] private GameObject heldAxePrefab;

    protected override void Interact()
    {
        base.Interact();
        WeaponHoldManager.Instance.EquipAxe(heldAxePrefab);
        gameObject.SetActive(false);
    }
}