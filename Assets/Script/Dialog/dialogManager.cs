using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogPanel; // Panel UI untuk dialog
    [SerializeField] private TextMeshProUGUI dialogText; // Teks dialog
    [SerializeField] private TextMeshProUGUI characterNameText; // Nama karakter (opsional)
    [SerializeField] private float fadeDuration = 0.5f; // Durasi fade-in (detik)

    private CanvasGroup canvasGroup; // Untuk mengontrol opacity
    private string[] currentDialog;
    private float[] currentDelays;
    private int currentLine = 0;
    private string currentCharacterName;
    private bool isDialogActive;

    private void Awake()
    {
        canvasGroup = dialogPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = dialogPanel.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0; // Mulai dengan opacity 0 (tak terlihat)
        dialogPanel.SetActive(false); // Sembunyikan panel saat mulai
        dialogText.text = "";
        characterNameText.text = "";
    }

    public void StartDialog(DialogData dialogData)
    {
        if (dialogData == null || dialogData.dialogLines.Length == 0) return;

        currentDialog = dialogData.dialogLines;
        currentDelays = dialogData.delays;
        currentCharacterName = dialogData.characterName;
        currentLine = 0;
        dialogPanel.SetActive(true);
        isDialogActive = true;
        StartCoroutine(FadeInAndDisplay());
    }

    private System.Collections.IEnumerator FadeInAndDisplay()
    {
        // Fade-in panel
        float elapsedTime = 0f;
        canvasGroup.alpha = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        // Tampilkan dialog dengan jeda
        while (currentLine < currentDialog.Length && isDialogActive)
        {
            dialogText.text = currentDialog[currentLine];
            characterNameText.text = currentCharacterName;
            float delay = (currentLine < currentDelays.Length) ? currentDelays[currentLine] : 2.0f;
            yield return new WaitForSeconds(delay);
            currentLine++;
        }

        // Fade-out setelah selesai
        yield return StartCoroutine(FadeOut());
    }

    private System.Collections.IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = 1 - Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
        EndDialog();
    }

    public void NextDialog()
    {
        if (isDialogActive && currentLine < currentDialog.Length)
        {
            StopAllCoroutines();
            StartCoroutine(FadeInAndDisplay());
        }
    }

    private void EndDialog()
    {
        dialogPanel.SetActive(false);
        dialogText.text = "";
        characterNameText.text = "";
        currentDialog = null;
        currentDelays = null;
        currentLine = 0;
        currentCharacterName = null;
        isDialogActive = false;
    }

    // Fungsi statis untuk dipanggil dari script lain
    public static void TriggerDialog(DialogData dialogData)
    {
        DialogManager manager = FindObjectOfType<DialogManager>();
        if (manager != null)
        {
            manager.StartDialog(dialogData);
        }
        else
        {
            Debug.LogError("DialogManager not found in scene!");
        }
    }
}