using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    public bool CanInteract = true;
    public string PromptMessage;
    public Sprite PromptIcon;

    [Header("Behavior")]
    [Tooltip("Kalau true, UI prompt hilang setelah interact (misal buat objek sekali pakai)")]
    public bool HideUIAfterInteract = false;

    [Tooltip("Kalau true, objek ini cuma bisa di-interact 1x (misal matikan radio). Kalau false, bisa berkali-kali (misal buka-tutup laci)")]
    public bool InteractOnce = false;

    [Header("World Icon (opsional)")]
    [SerializeField] private WorldInteractIcon worldIcon;

    public UnityEvent OnInteract;

    private bool hasInteracted = false;

    public virtual void BaseInteract()
    {
        if (!CanInteract) return;
        if (InteractOnce && hasInteracted) return;

        Interact();
        hasInteracted = true;

        if (InteractOnce)
            CanInteract = false; // kunci permanen setelah interact pertama
    }
    public void ResetInteraction()
    {
        hasInteracted = false;
        CanInteract = true;
    }
    protected virtual void Interact()
    {
        OnInteract?.Invoke();
    }
    // Dipanggil dari PlayerInteract buat nyalain/matiin icon dunia object ini
    public void SetIconTargeted(bool targeted)
    {
        if (worldIcon != null)
            worldIcon.SetTargeted(targeted);
    }
}