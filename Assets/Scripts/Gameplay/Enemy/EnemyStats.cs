using System;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private EnemyStatsData data;

    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }
    public float Attack { get; set; }
    public float Speed { get; set; }

    public bool IsDead => CurrentHealth <= 0f;

    public event Action OnDied;

    private void Awake()
    {
        MaxHealth = data.maxHealth;
        CurrentHealth = MaxHealth;
        Attack = data.attack;
        Speed = data.speed;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0f);

        if (IsDead) Die();
    }

    private void Die()
    {
        OnDied?.Invoke();

        if (LevelManager.instance != null)
            LevelManager.instance.AddKill();

        Destroy(gameObject);
    }
}