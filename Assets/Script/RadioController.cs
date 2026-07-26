using UnityEngine;

public class RadioController : Interactable
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource radioAudioSource;
    [SerializeField] private float maxVolume = 1.0f;
    [SerializeField] private float minDistance = 5.0f;
    [SerializeField] private float maxDistance = 20.0f;
    [SerializeField] private GameObject Complete;
    [SerializeField] private DialogData dialogData;

    [Header("Player Reference")]
    [SerializeField] private Transform player;

    private bool isPlayerInRange;
    private bool isRadioOn;
    private bool isPermanentlyDisabled;

    private void Awake()
    {
        InitializeAudioSource();
    }

    private void InitializeAudioSource()
    {
        if (radioAudioSource == null)
        {
            Debug.LogError("AudioSource is not assigned on RadioController!", this);
            return;
        }

        radioAudioSource.playOnAwake = false;
        radioAudioSource.loop = true;
        PlayRadio();
    }

    protected override void Interact()
    {
        base.Interact();

        if (isPermanentlyDisabled || radioAudioSource == null) return;

        // Matikan radio secara permanen saat interaksi
        TurnOffRadioPermanently();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPermanentlyDisabled || !other.CompareTag("Player")) return;

        isPlayerInRange = true;
        if (!isRadioOn)
        {
            PlayRadio();
        }
    }


    private void Update()
    {
        if (isPlayerInRange && isRadioOn && !isPermanentlyDisabled)
        {
            AdjustVolumeBasedOnDistance();
        }
    }

    private void PlayRadio()
    {
        if (radioAudioSource == null) return;

        radioAudioSource.Play();
        isRadioOn = true;
        Debug.Log("Radio started playing.");
    }

    private void TurnOffRadioPermanently()
    {
        if (radioAudioSource == null) return;

        radioAudioSource.Stop();
        isRadioOn = false;
        isPermanentlyDisabled = true;
        Debug.Log("Radio permanently turned off.");
        DialogManager.TriggerDialog(dialogData);
        Complete.SetActive(true);
    }

    private void AdjustVolumeBasedOnDistance()
    {
        if (player == null || radioAudioSource == null) return;

        float distance = Vector3.Distance(player.position, transform.position);
        float volume = Mathf.Clamp01(1.0f - ((distance - minDistance) / (maxDistance - minDistance)));
        radioAudioSource.volume = volume * maxVolume;
    }
}