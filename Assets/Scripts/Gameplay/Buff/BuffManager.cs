using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BuffManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats player;
    [SerializeField] private Gun gun;

    [Header("Settings")]
    [SerializeField] private int choiceCount = 3;
    [SerializeField] private int pityThreshold = 3;

    [Header("Loaded Buffs")]
    [SerializeField] private BuffData[] buffs;

    [Header("Debug")]
    [SerializeField] private int pityCount;

    public event Action<BuffData[]> OnChoicesReady;
    public event Action OnSelectionFinished;

    private const string BuffResourcePath = "Buff";
    private BuffData[] currentChoices;
    private int pendingLevelUps;
    private bool isSelecting;

    private void Awake()
    {
        buffs = Resources.LoadAll<BuffData>(BuffResourcePath);

        if (buffs.Length == 0)
            Debug.LogError($"BuffData tidak ada di Resources/{BuffResourcePath}!");
    }

    private void Start()
    {
        if (player == null) player = FindFirstObjectByType<PlayerStats>();
        if (gun == null) gun = FindFirstObjectByType<Gun>();

        if (player != null)
            player.OnLevelUp += HandleLevelUp;
    }

    private void OnDestroy()
    {
        if (player != null)
            player.OnLevelUp -= HandleLevelUp;
    }

    private void HandleLevelUp(int newLevel)
    {
        pendingLevelUps++;

        if (!isSelecting)
            ShowNextChoices();
    }

    private void ShowNextChoices()
    {
        pendingLevelUps--;
        isSelecting = true;
        currentChoices = RollChoices();

        if (LevelManager.instance != null)
            LevelManager.instance.BeginBuffSelection();

        OnChoicesReady?.Invoke(currentChoices);
    }

    public void SelectBuff(int index)
    {
        if (!isSelecting || index < 0 || index >= currentChoices.Length) return;

        BuffData chosen = currentChoices[index];
        ApplyBuff(chosen);

        pityCount = chosen.type == BuffType.WeaponLevel ? 0 : pityCount + 1;

        isSelecting = false;

        if (pendingLevelUps > 0)
        {
            ShowNextChoices();
        }
        else
        {
            OnSelectionFinished?.Invoke();

            if (LevelManager.instance != null)
                LevelManager.instance.EndBuffSelection();
        }
    }

    private void ApplyBuff(BuffData buff)
    {
        switch (buff.type)
        {
            case BuffType.MaxHealth:
                player.AddMaxHealth(buff.value);
                break;
            case BuffType.Attack:
                player.AddAttack(buff.value);
                break;
            case BuffType.Speed:
                player.AddSpeed(buff.value);
                break;
            case BuffType.WeaponLevel:
                gun.LevelUp();
                break;
        }
    }

    private BuffData[] RollChoices()
    {
        List<BuffData> pool = new List<BuffData>(buffs);

        if (gun != null && gun.IsMaxLevel)
            pool.RemoveAll(b => b.type == BuffType.WeaponLevel);

        BuffData guaranteedWeapon = null;

        if (pityCount >= pityThreshold)
        {
            guaranteedWeapon = pool.Find(b => b.type == BuffType.WeaponLevel);
            if (guaranteedWeapon != null)
                pool.Remove(guaranteedWeapon);
        }

        int slots = guaranteedWeapon != null ? choiceCount - 1 : choiceCount;
        List<BuffData> result = new List<BuffData>();

        while (result.Count < slots && pool.Count > 0)
        {
            BuffData picked = PickWeighted(pool);
            result.Add(picked);
            pool.Remove(picked);
        }

        if (guaranteedWeapon != null)
            result.Insert(Random.Range(0, result.Count + 1), guaranteedWeapon);

        return result.ToArray();
    }

    private BuffData PickWeighted(List<BuffData> pool)
    {
        float total = 0f;
        foreach (BuffData b in pool) total += b.weight;

        float roll = Random.value * total;

        foreach (BuffData b in pool)
        {
            roll -= b.weight;
            if (roll <= 0f) return b;
        }

        return pool[pool.Count - 1];
    }

    [ContextMenu("Debug Trigger Choice")]
    private void DebugTrigger() => HandleLevelUp(0);
}