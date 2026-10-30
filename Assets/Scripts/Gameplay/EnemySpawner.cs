using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups;
        public int waveQuota;
        public float spawnInterval;
        public int spawnCount;
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public int enemyCount;
        public int spawnCount;
        public GameObject enemyPrefabs;
    }

    public List<Wave> waves;
    public int currentWaveCount;

    [Header("Spawner Attributes")]
    float spawnTimer;
    public float waveInterval;
    public int enemiesAlive;
    public int maxEnemiesAllowed;
    public bool maxEnemiesReached;
    bool waitingForNextWave;

    [Header("Endless Scaling")]
    public float enemyCountMultiplier = 1.2f;     
    public float spawnIntervalMultiplier = 0.97f; 
    public float minSpawnInterval = 0.2f;

    [Header("Spawn Positions")]
    public List<Transform> relativeSpawnPoints;
    Transform player;

    void Start()
    {
        player = FindFirstObjectByType<PlayerStats>().transform;
        CalculateWaveQuota();
    }

    void Update()
    {
        Wave wave = waves[currentWaveCount];

        if (!waitingForNextWave && wave.spawnCount >= wave.waveQuota)
        {
            StartCoroutine(BeginNextWave());
        }

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= wave.spawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemies();
        }
    }

    IEnumerator BeginNextWave()
    {
        waitingForNextWave = true;
        yield return new WaitForSeconds(waveInterval);

        if (currentWaveCount >= waves.Count - 1)
        {
            waves.Add(GenerateNextWave(waves[waves.Count - 1]));
        }

        currentWaveCount++;
        CalculateWaveQuota();
        waitingForNextWave = false;
    }

    Wave GenerateNextWave(Wave previous)
    {
        Wave next = new Wave();
        next.waveName = "Wave " + (waves.Count + 1);
        next.spawnInterval = Mathf.Max(minSpawnInterval, previous.spawnInterval * spawnIntervalMultiplier);
        next.enemyGroups = new List<EnemyGroup>();

        foreach (var group in previous.enemyGroups)
        {
            next.enemyGroups.Add(new EnemyGroup
            {
                enemyName = group.enemyName,
                enemyPrefabs = group.enemyPrefabs,
                enemyCount = Mathf.CeilToInt(group.enemyCount * enemyCountMultiplier),
                spawnCount = 0
            });
        }
        return next;
    }

    void CalculateWaveQuota()
    {
        int currentWaveQuota = 0;
        foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuota += enemyGroup.enemyCount;
        }
        waves[currentWaveCount].waveQuota = currentWaveQuota;
    }

    void SpawnEnemies()
    {
        if (waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota && !maxEnemiesReached)
        {
            foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
            {
                if (enemyGroup.spawnCount < enemyGroup.enemyCount)
                {
                    if (enemiesAlive >= maxEnemiesAllowed)
                    {
                        maxEnemiesReached = true;
                        return;
                    }

                    Instantiate(enemyGroup.enemyPrefabs,
                        player.position + relativeSpawnPoints[Random.Range(0, relativeSpawnPoints.Count)].position,
                        Quaternion.identity);

                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    enemiesAlive++;
                }
            }
        }

        if (enemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
        }
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;
    }
}