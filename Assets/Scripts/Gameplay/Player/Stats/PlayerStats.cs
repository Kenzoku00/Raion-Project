using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour , IDamageable
{
    [SerializeField] private PlayerStatsData data;

    [field: Header("Player Stats")]
    [field: SerializeField] public float MaxHealth { get; private set; }
    [field: SerializeField] public float CurrentHealth { get; private set; }
    [field: SerializeField] public float Attack { get; private set; }
    [field: SerializeField] public float Speed { get; private set; }

    [field: Header("Exp Stats")]
    [field: SerializeField] public int Level { get; private set; }
    [field: SerializeField] public int CurrentExp { get; private set; }
    [field: SerializeField] public int ExpToNextLevel { get; private set; }

    public bool IsDead => CurrentHealth <= 0f;

    public event Action<float, float> OnHealthChanged;
    public event Action OnStatsChanged;
    public event Action<float> OnDamaged;
    public event Action OnDied;
    public event Action<int, int> OnExpChanged;
    public event Action<int> OnLevelUp;

    private bool isInitialized;

    private void Awake()
    {
        InitializeStats();
        isInitialized = true;
    }

    private void InitializeStats()
    {
        MaxHealth = data.maxHealth;
        Attack = data.attack;
        Speed = data.speed;
        CurrentHealth = MaxHealth;

        Level = 1;
        CurrentExp = 0;
        ExpToNextLevel = CalculateExpRequired(Level);
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0f);
        OnDamaged?.Invoke(amount);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (IsDead) OnDied?.Invoke();
    }

    public void Heal(float amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public void AddMaxHealth(float amount)
    {
        MaxHealth += amount;
        CurrentHealth += amount;
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public void AddAttack(float amount)
    {
        Attack += amount;
        OnStatsChanged?.Invoke();
    }

    public void AddSpeed(float amount)
    {
        Speed += amount;
        OnStatsChanged?.Invoke();
    }

    public void AddExp(int amount)
    {
        if (IsDead || amount <= 0) return;

        CurrentExp += amount;

        while (CurrentExp >= ExpToNextLevel)
        {
            CurrentExp -= ExpToNextLevel;
            Level++;
            ExpToNextLevel = CalculateExpRequired(Level);
            OnLevelUp?.Invoke(Level);
        }

        OnExpChanged?.Invoke(CurrentExp, ExpToNextLevel);
    }

    private int CalculateExpRequired(int level)
    {
        float required = data.baseExpRequired * Mathf.Pow(data.expGrowth, level - 1);
        return Mathf.Max(1, Mathf.RoundToInt(required));
    }

    private void NotifyAll()
    {
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        OnExpChanged?.Invoke(CurrentExp, ExpToNextLevel);
        OnStatsChanged?.Invoke();
    }

    private void OnValidate()
    {
        if (!Application.isPlaying || !isInitialized) return;

        MaxHealth = Mathf.Max(1f, MaxHealth);
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);

        Level = Mathf.Max(1, Level);
        ExpToNextLevel = CalculateExpRequired(Level);
        CurrentExp = Mathf.Clamp(CurrentExp, 0, ExpToNextLevel - 1);

        NotifyAll();
    }

    [ContextMenu("Reload Stats From Data")]
    private void DebugReloadFromData()
    {
        InitializeStats();
        NotifyAll();
    }

    [ContextMenu("Add 10 EXP")]
    private void DebugAddExp() => AddExp(10);

    [ContextMenu("Take 10 Damage")]
    private void DebugTakeDamage() => TakeDamage(10f);
}