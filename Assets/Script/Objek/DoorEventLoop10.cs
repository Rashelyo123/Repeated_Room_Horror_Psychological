using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorEventLoop10 : MonoBehaviour
{
    [SerializeField] private CameraShake cameraShake;

    [Header("Camera Shake Settings")]
    [SerializeField] private float swingShakeDuration = 0.15f;
    [SerializeField] private float swingShakeMagnitude = 0.03f;
    [SerializeField] private float hitShakeDuration = 0.2f;
    [SerializeField] private float hitShakeMagnitude = 0.08f;
    [Header("FMOD (opsional)")]
    [SerializeField] private FMODUnity.EventReference doorSlamp;



    public void CallCameraShake()
    {
        cameraShake.Shake(swingShakeMagnitude, hitShakeDuration, hitShakeMagnitude);
        FMODUnity.RuntimeManager.PlayOneShot(doorSlamp);
    }
}
