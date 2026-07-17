using UnityEngine;

[CreateAssetMenu(fileName = "New Dialog", menuName = "Dialog/New Dialog Data", order = 1)]
public class DialogData : ScriptableObject
{
    [TextArea(3, 10)]
    public string[] dialogLines;
    public string characterName = "Unknown";
    public float[] delays;
}