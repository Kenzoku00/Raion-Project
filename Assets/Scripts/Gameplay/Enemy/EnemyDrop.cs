using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
public class EnemyDrop : MonoBehaviour
{
    [Header("EXP")]
    [SerializeField] private ExpOrb expOrbPrefab;

    [Header("Coin")]
    [SerializeField] private Coin coinPrefab;
    [SerializeField, Range(0f, 100f)] private float coinDropChance = 30f;
    [SerializeField, Min(0)] private int minCoin = 1;
    [SerializeField, Min(0)] private int maxCoin = 3;
    [SerializeField] private float scatterRadius = 0.5f;

    private EnemyStats stats;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        stats.OnDied += HandleDeath;
    }

    private void OnDestroy()
    {
        if (stats != null)
            stats.OnDied -= HandleDeath;
    }

    private void OnValidate()
    {
        maxCoin = Mathf.Max(minCoin, maxCoin);
    }

    private void HandleDeath()
    {
        DropExp();
        TryDropCoins();
    }

    private void DropExp()
    {
        if (expOrbPrefab == null) return;

        ExpOrb orb = SpawnDrop(expOrbPrefab);
        orb.SetValue(stats.ExpReward);
    }

    private void TryDropCoins()
    {
        if (coinPrefab == null) return;
        if (Random.Range(0f, 100f) >= coinDropChance) return;

        int amount = Random.Range(minCoin, maxCoin + 1);

        for (int i = 0; i < amount; i++)
            SpawnDrop(coinPrefab);
    }

    private T SpawnDrop<T>(T prefab) where T : Component
    {
        T item = Instantiate(prefab, transform.position, Quaternion.identity);
        Vector3 landing = transform.position + (Vector3)(Random.insideUnitCircle * scatterRadius);

        if (item.TryGetComponent(out DropPop pop))
            pop.Play(landing);
        else
            item.transform.position = landing;

        return item;
    }
}