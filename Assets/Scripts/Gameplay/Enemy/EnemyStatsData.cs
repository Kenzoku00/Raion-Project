using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatsData", menuName = "Game/Enemy Stats Data")]
public class EnemyStatsData : ScriptableObject
{
    public float maxHealth = 30f;
    public float attack = 5f;
    public float speed = 3f;
}