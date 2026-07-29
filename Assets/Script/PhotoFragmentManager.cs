using UnityEngine;
using UnityEngine.Events;

public class PhotoFragmentManager : MonoBehaviour
{
    public static PhotoFragmentManager Instance;

    [Header("Settings")]
    [SerializeField] private int totalFragmentsNeeded = 4;
    [SerializeField] private GameObject completedPhotoObject; // foto utuh yang muncul setelah semua terkumpul
    [SerializeField] private GameObject brokenPhotoObject;    // foto sobek yang hilang setelah selesai

    [Header("Events")]
    public UnityEvent onFragmentCollected; // tiap kali 1 potongan diambil (buat SFX/UI feedback)
    public UnityEvent onAllFragmentsCollected; // semua potongan udah lengkap

    private int currentFragmentCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void CollectFragment()
    {
        currentFragmentCount++;
        Debug.Log($"Fragment terkumpul: {currentFragmentCount}/{totalFragmentsNeeded}");

        onFragmentCollected?.Invoke();

        if (currentFragmentCount >= totalFragmentsNeeded)
        {
            CompletePhoto();
        }
    }

    private void CompletePhoto()
    {
        Debug.Log("Semua fragment terkumpul! Foto disatukan.");

        if (brokenPhotoObject != null) brokenPhotoObject.SetActive(false);
        if (completedPhotoObject != null) completedPhotoObject.SetActive(true);

        onAllFragmentsCollected?.Invoke();

        // Nyambung ke ObjectiveManager kalau ini bagian dari step objective loop
        // ObjectiveManager.Instance.CompleteStep(x);
    }

    public int GetCurrentCount() => currentFragmentCount;
    public int GetTotalNeeded() => totalFragmentsNeeded;
}