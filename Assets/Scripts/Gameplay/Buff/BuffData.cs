using UnityEngine;

[CreateAssetMenu(fileName = "BuffData", menuName = "Game/Buff Data")]
public class BuffData : ScriptableObject
{
    public BuffType type;
    public string title;
    [TextArea] public string description;
    public Sprite icon;

    public float value;
    public float weight = 1f;
}