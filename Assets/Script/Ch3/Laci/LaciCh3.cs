using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LaciCh3 : MonoBehaviour
{
  public AudioSource audioSource;
  public Animator laci;

  public GameObject E;

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.E) && E.activeSelf)
    {
      OpenLaci();
    }
  }

   public void OpenLaci()
    {
       
        audioSource.Play();
        laci.SetTrigger("open");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            E.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            E.SetActive(false);
        }
    }
}
