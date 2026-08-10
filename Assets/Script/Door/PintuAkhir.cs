using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMOD.Studio;
using FMODUnity;

public class PintuAkhir : Interactable
{

    // public Animator animator;
    public bool isPlayerMissionComplete = false;
    public string SCENE_NAME;
    public AudioClip doorLocked;

    [SerializeField] private EventReference doorLockedSound;



    // public GameObject FadeOut;

    protected override void Interact()
    {
        base.Interact();
        if (CanInteract)
        {
            if (isPlayerMissionComplete)
            {
                StartCoroutine(SwitchScene());
                Debug.Log("Complete");


            }
            else
            {
                // AudioSource.PlayClipAtPoint(doorLocked, transform.position);
                Debug.Log("Mission belum selesai");
                RuntimeManager.PlayOneShotAttached(doorLockedSound, gameObject);
            }
        }


    }


    // Update is called once per frame


    public void OpenDoor()
    {
        //animator.SetTrigger("open");

    }

    private IEnumerator SwitchScene()
    {
        OpenDoor();
        // FadeOut.SetActive(true);
        yield return new WaitForSeconds(1f);
        Debug.Log("Complete");
        SceneManager.LoadScene(SCENE_NAME);
    }


}
