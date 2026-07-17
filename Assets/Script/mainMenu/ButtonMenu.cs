using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonMenu : MonoBehaviour
{
    public GameObject transitionPanel; // Panel untuk transisi
    public float transitionTime = 1f; // Waktu transisi
    public GameObject video;
    public GameObject fadeIn;
    public GameObject SettingOpen;

    public AudioSource audioSource;

    void Start()
    {
        fadeIn.SetActive(true);
        StartCoroutine(StartFadeIn());
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    private IEnumerator StartFadeIn()
    {
        yield return new WaitForSeconds(2.2f);
        fadeIn.SetActive(false);
    }

    public void Setting()
    {
        SettingOpen.SetActive(true);
    }
    public void SettingBack()
    {
        SettingOpen.SetActive(false);
    }

    public void StartGame()
    {
        StartCoroutine(LoadSceneWithTransition("SceneAwal"));
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private IEnumerator LoadSceneWithTransition(string sceneName)
    {
        // Aktifkan panel transisi
        transitionPanel.SetActive(true);
        audioSource.Play();

        // Tunggu selama waktu transisi
        yield return new WaitForSeconds(transitionTime);

        video.SetActive(true);
        transitionPanel.SetActive(false);
        yield return new WaitForSeconds(3f);

        // Load scene baru
        SceneManager.LoadScene(sceneName);


    }
}
