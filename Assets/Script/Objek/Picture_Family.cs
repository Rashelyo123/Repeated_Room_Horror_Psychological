using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Picture_Family : MonoBehaviour
{
    [SerializeField] private GameObject pictureFamily;
    [SerializeField] private GameObject pictureFamilyBroken;
    [Header("FMOD (opsional)")]
    [SerializeField] private FMODUnity.EventReference BrokenPictureSound;



    public void HidePictureFamily()
    {
        StartCoroutine(ScenarioPictureFamily());
    }

    private IEnumerator ScenarioPictureFamily()
    {
        yield return new WaitForSeconds(2f);
        FMODUnity.RuntimeManager.PlayOneShot(BrokenPictureSound, transform.position);
        if (pictureFamily != null && pictureFamilyBroken != null)
        {
            pictureFamily.SetActive(false);
            pictureFamilyBroken.SetActive(true);
        }

    }
}
