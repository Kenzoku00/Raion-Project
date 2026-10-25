using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerStatsData data;

    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }
    public float Attack { get; set; }
    public float Speed { get; set; }

    public bool IsDead => CurrentHealth <= 0f;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDied;

    private void Awake()
    {
        MaxHealth = data.maxHealth;
        Attack = data.attack;
        Speed = data.speed;
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0f);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (IsDead) OnDied?.Invoke();
    }

    public void Heal(float amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }
}