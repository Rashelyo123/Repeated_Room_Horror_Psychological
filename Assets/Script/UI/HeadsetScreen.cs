using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeadsetScreen : MonoBehaviour
{
    [SerializeField] CanvasGroup blackScreen;  // CanvasGroup di panel HeadsetScreen
    [SerializeField] CanvasGroup headsetText;  // CanvasGroup di teks
    [SerializeField] string nextScene = "Loop1";

    [SerializeField] float fadeToBlack = 1f;
    [SerializeField] float textFadeIn = 1.5f;
    [SerializeField] float textHold = 3f;
    [SerializeField] float textFadeOut = 1.5f;
    [SerializeField] float blackPause = 0.5f; // jeda hitam sebelum pindah scene

    bool started;

    public void StartSequence()
    {
        if (started) return; // cegah dobel klik
        started = true;
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        blackScreen.blocksRaycasts = true;
        headsetText.alpha = 0;

        yield return Fade(blackScreen, 0, 1, fadeToBlack);
        yield return Fade(headsetText, 0, 1, textFadeIn);
        yield return new WaitForSeconds(textHold);
        yield return Fade(headsetText, 1, 0, textFadeOut);
        yield return new WaitForSeconds(blackPause);

        SceneManager.LoadScene(nextScene);
    }

    IEnumerator Fade(CanvasGroup cg, float from, float to, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        cg.alpha = to;
    }
}