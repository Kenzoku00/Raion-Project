using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsData", menuName = "Game/Player Stats Data")]
public class PlayerStatsData : ScriptableObject
{
    public float maxHealth = 100f;
    public float attack = 10f;
    public float speed = 5f;
}