using FMODUnity;
using UnityEngine;

[System.Serializable]
public class DialogLine
{
    [TextArea(2, 5)] public string text;
    public EventReference voiceEvent;
    public float delay = 0.5f;
}

[CreateAssetMenu(fileName = "New Dialog", menuName = "Dialog/New Dialog Data", order = 1)]
public class DialogData : ScriptableObject
{
    public string characterName = "Unknown";
    public DialogLine[] lines;
}