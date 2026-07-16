using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Pastikan untuk menambahkan ini

public class TextOne : MonoBehaviour
{
    // Referensi untuk transisi objek
    public GameObject jam;
    public GameObject tanggal;
    public float delay = 1.5f; // Waktu jeda antara setiap transisi

    // Referensi untuk efek pengetikan
    public TextMeshProUGUI tmpText; // Referensi ke TextMeshProUGUI component
    public AudioSource typingSound; // AudioSource untuk efek suara ketik
    public float typingSpeed = 0.05f; // Kecepatan teks muncul (0.05 detik per karakter)
    public float paragraphDelay = 1.5f; // Jeda antar paragraf
    public GameObject transition; // GameObject transisi
    public float transitionWaitTime = 1f; // Waktu tunggu setelah transisi aktif

    string[] storyText = new string[]
{
    "Nightmares haunt me every night—shadows lurk in my sleep, vague but menacing. What are they, and why can't I remember?",
    "I wake at the same hour, drenched in cold sweat, piecing together dream fragments that feel too real.",
    "The house feels wrong—heavy air, suffocating silence, broken only by my pounding heartbeat.",
    "Is this a dream or madness? Everything feels real yet twisted, like a warped mirror.",
    "The hallways stretch endlessly into darkness, each step echoing as if the house is alive, mocking me.",
    "The walls are unnaturally cold, the warmth of home replaced by something sinister.",
    "Every door leads back to the same hallway, the same shadows, trapping me in an endless loop.",
    "Why am I here? A deep fear whispers this is more than a nightmare.",
    "Am I dead?"
};
    private string currentText = "";


    void Start()
    {
        StartCoroutine(PlayStory());
    }

    private IEnumerator PlayStory()
    {
        // Transisi awal
        yield return new WaitForSeconds(delay);
        jam.SetActive(true);
        tanggal.SetActive(false);

        yield return new WaitForSeconds(delay);
        jam.SetActive(false);

        // Mulai efek pengetikan setelah transisi selesai
        yield return StartCoroutine(ShowStory());

        // Setelah semua teks selesai, muat scene baru
        SceneManager.LoadScene("CH1"); // Ganti dengan nama scene yang ingin kamu muat
    }

    private IEnumerator ShowStory()
    {
        // Aktifkan transisi
        transition.SetActive(true);

        // Tunggu waktu transisi selesai
        yield return new WaitForSeconds(transitionWaitTime);

        foreach (string paragraph in storyText)
        {
            yield return StartCoroutine(ShowText(paragraph));
            yield return new WaitForSeconds(paragraphDelay); // Jeda antar paragraf
        }
    }

    private IEnumerator ShowText(string fullText)
    {
        typingSound.Play(); // Mulai memutar suara ketikan

        for (int i = 0; i <= fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);
            tmpText.text = currentText;
            yield return new WaitForSeconds(typingSpeed);
        }

        typingSound.Stop(); // Hentikan suara ketikan
    }
}
