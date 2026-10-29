using System;
using UnityEngine;

public class EnemyStats : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyStatsData data;
    [SerializeField] private float destroyDelay = 0.35f;

    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }
    public float Attack { get; set; }
    public float Speed { get; set; }

    public bool IsDead => CurrentHealth <= 0f;

    public int ExpReward => data.expReward;

    public event Action<float> OnDamaged;
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
        OnDamaged?.Invoke(amount);

        if (IsDead) Die();
    }

    private void Die()
    {
        OnDied?.Invoke();

        if (LevelManager.instance != null)
            LevelManager.instance.AddKill();

        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        Destroy(gameObject, destroyDelay);
    }
}