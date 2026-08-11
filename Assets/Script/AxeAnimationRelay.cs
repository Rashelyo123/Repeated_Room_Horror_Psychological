using UnityEngine;

public class AxeAnimationRelay : MonoBehaviour
{
    // Dipanggil dari Animation Event di clip AxeAnim
    public void OnSwingHitFrame()
    {
        WeaponHoldManager.Instance.OnSwingHitFrame();
    }

    public void OnSwingAnimationEnd()
    {
        WeaponHoldManager.Instance.OnSwingAnimationEnd();
    }
}