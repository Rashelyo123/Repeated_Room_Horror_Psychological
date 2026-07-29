using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoFragmentPickup : Interactable
{
    [SerializeField] private bool destroyAfterPickup = true;

    protected override void Interact()
    {
        base.Interact();
        PhotoFragmentManager.Instance.CollectFragment();

        if (destroyAfterPickup)
            gameObject.SetActive(false);
    }
}
