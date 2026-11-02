using UnityEngine;
using FMODUnity;

public class LampSpeedManager : MonoBehaviour
{
    [Header("Lampu yang diatur")]
    public Animator[] lamps;

    [Header("Controller")]
    public RuntimeAnimatorController normalController; // Lantern_Swing_Animator
    public RuntimeAnimatorController chaseController;  // Lantern_Swing_Loop9_Animator 1

    [Header("FMOD (global parameter)")]
    public string fmodParam = "LampSpeed";
    public float normalValue = 1f;
    public float chasedValue = 2f;

    void Start() => SetNormal();

    public void SetNormal() => Apply(normalController, normalValue);
    public void SetChased() => Apply(chaseController, chasedValue);

    void Apply(RuntimeAnimatorController ctrl, float fmodValue)
    {
        foreach (var a in lamps)
            if (a != null) a.runtimeAnimatorController = ctrl;

        RuntimeManager.StudioSystem.setParameterByName(fmodParam, fmodValue);
    }
}