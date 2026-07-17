using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionCh2 : MonoBehaviour
{
    public Laci laci;
    public Animator DoorLast;
    public AudioSource audioSource;
    public GameObject E;
    public GameObject fadeOut;

    
    

    private bool isPlayerInTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && laci.missionComplete)
        {
            E.SetActive(true);
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            E.SetActive(false);
            isPlayerInTrigger = false;
        }
    }

    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(MissionComplete());
        }
    }

    private IEnumerator MissionComplete()
    {
        DoorLast.SetTrigger("open");
        fadeOut.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Ch3");
    }
}
