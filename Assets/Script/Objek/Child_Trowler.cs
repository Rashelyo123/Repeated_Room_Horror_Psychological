using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class Child_Trowler : MonoBehaviour
{
    [Header("FMOD (opsional)")]
    [SerializeField] private EventReference ChildCry;

    private EventInstance childCryInstance;

    public void PlayChildCry()
    {
        childCryInstance = RuntimeManager.CreateInstance(ChildCry);
        RuntimeManager.AttachInstanceToGameObject(childCryInstance, transform);
        childCryInstance.start();
    }

    public void StopChildCry()
    {
        if (childCryInstance.isValid())
        {
            childCryInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); // atau .IMMEDIATE kalau mau langsung putus
            childCryInstance.release(); // penting, biar gak memory leak
        }
    }

    private void OnDestroy()
    {
        // Jaga-jaga kalau object di-destroy sementara suara masih main
        StopChildCry();
    }
}