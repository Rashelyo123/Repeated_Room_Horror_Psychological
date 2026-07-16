using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openOtomatis : MonoBehaviour
{
    public Animator doorAnimator;

    void Start(){
        doorAnimator.SetTrigger("OpenOtomatis");
    }
}
