using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
public class PhotoFragmentPickup : Interactable
{
    [SerializeField] private bool destroyAfterPickup = true;
    [SerializeField] private EventReference pickupSound;

    protected override void Interact()
    {
        base.Interact();
        PhotoFragmentManager.Instance.CollectFragment();
        RuntimeManager.PlayOneShotAttached(pickupSound, gameObject);

        if (destroyAfterPickup)
            gameObject.SetActive(false);
    }
}
