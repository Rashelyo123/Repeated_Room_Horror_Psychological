using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{

    [SerializeField] private GameObject loadingScreen;
    public void QuitGame()
    {
        Application.Quit();
    }
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadScenee(sceneName));
    }
    private IEnumerator LoadScenee(string sceneName)
    {
        loadingScreen.SetActive(true);
        yield return new WaitForSeconds(2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);

    }
}
