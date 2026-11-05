
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private CanvasGroup uiGroup;
    [SerializeField] private float fadeDuration = 1.5f;

    private bool isLoading = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;
        // Kondisi awal Main Menu
        loadingScreen.SetActive(true);

        uiGroup.alpha = 1f;
        uiGroup.interactable = true;
        uiGroup.blocksRaycasts = true;

        fadeGroup.alpha = 0f;
        fadeGroup.interactable = false;
        fadeGroup.blocksRaycasts = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadScene(string sceneName)
    {
        if (isLoading) return;

        StartCoroutine(LoadScenee(sceneName));
    }

    private IEnumerator LoadScenee(string sceneName)
    {
        isLoading = true;

        // 1. Nonaktifkan interaksi UI
        uiGroup.interactable = false;
        uiGroup.blocksRaycasts = false;

        // 2. Pastikan overlay aktif
        loadingScreen.SetActive(true);
        fadeGroup.alpha = 0f;
        fadeGroup.blocksRaycasts = true;

        // 3. Mulai loading Loop1 DI AWAL
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        if (operation == null)
        {
            Debug.LogError("Gagal memuat scene: " + sceneName);
            yield break;
        }

        // Tahan aktivasi scene
        operation.allowSceneActivation = false;

        // 4. Fade UI dan background secara bersamaan
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(timer / fadeDuration);

            uiGroup.alpha = Mathf.Lerp(1f, 0f, progress);
            fadeGroup.alpha = Mathf.Lerp(0f, 1f, progress);

            yield return null;
        }

        // Pastikan fade selesai
        uiGroup.alpha = 0f;
        fadeGroup.alpha = 1f;

        // 5. Tunggu loading Loop1 selesai
        while (operation.progress < 0.9f)
        {
            yield return null;
        }

        // 6. Tampilkan layar hitam sebelum berpindah
        yield return new WaitForEndOfFrame();

        // 7. Aktifkan Loop1
        operation.allowSceneActivation = true;

        while (!operation.isDone)
        {
            yield return null;
        }
    }
}
