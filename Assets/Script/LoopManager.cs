using UnityEngine;
using FMODUnity;

public class LoopManager : MonoBehaviour
{
    public static LoopManager Instance;

    [Header("Loop State")]
    public int currentLoopIndex = 1;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetLoopCycle(currentLoopIndex);

        if (AudioManager.Instance != null)
            AudioManager.Instance.StartRoomTone();
    }

    public void SetLoopCycle(int loopNumber)
    {
        currentLoopIndex = loopNumber;
        RuntimeManager.StudioSystem.setParameterByName("Loop Cycle Count", (float)currentLoopIndex);
        Debug.Log($"[FMOD] Loop Cycle Count updated to: {currentLoopIndex}");
    }
}