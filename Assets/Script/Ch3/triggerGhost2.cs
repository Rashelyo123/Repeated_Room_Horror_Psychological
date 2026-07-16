using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class triggerGhost2 : MonoBehaviour
{
    public GameObject ghost1;
    public GameObject ghost2;

    bool isGhost2Active = false;

    public Light[] lightarray;
    public AudioClip lightBroken;
    public AudioClip ghostSound;
    public GameObject Completed;
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && !isGhost2Active)
        {
            StartCoroutine(CinemacineGhost());
            isGhost2Active = true;
        }
    }
    private IEnumerator CinemacineGhost()
    {
        yield return new WaitForSeconds(1f);
        lightarray[0].enabled = false;
        AudioSource.PlayClipAtPoint(lightBroken, transform.position);
        ghost1.SetActive(false);
        yield return new WaitForSeconds(0.4f);
        lightarray[1].enabled = false;
        AudioSource.PlayClipAtPoint(lightBroken, transform.position);
        yield return new WaitForSeconds(0.4f);
        ghost2.SetActive(true);
        AudioSource.PlayClipAtPoint(ghostSound, transform.position);

    }

    public void Destroy()
    {
        Completed.SetActive(true);
        Destroy(gameObject);
    }

}
