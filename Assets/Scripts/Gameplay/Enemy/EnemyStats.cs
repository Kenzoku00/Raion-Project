using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

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

    [Header("Damage Feedback")]
    public Color damageColor = new Color(1, 0, 0, 1);
    public float damageFlashDuration = 0.2f;
    public float deathFadeTime = 0.6f;

    SpriteRenderer[] renderers;
    Color[] originalColors;
    Coroutine flashRoutine;
    EnemyMovement movement;
    Rigidbody2D rb;

    public float despawnDistance = 20f;
    Transform player;

    private void Awake()
    {
        MaxHealth = data.maxHealth;
        CurrentHealth = MaxHealth;
        Attack = data.attack;
        Speed = data.speed;

        renderers = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            originalColors[i] = renderers[i].color;

        movement = GetComponent<EnemyMovement>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        movement = GetComponent<EnemyMovement>();
        player = FindFirstObjectByType<PlayerStats>().transform;
    }

    void Update()
    {
        if(Vector2.Distance(transform.position, player.position) >= despawnDistance)
        {
            ReturnEnemy();
        }
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0f);
        OnDamaged?.Invoke(amount);

        if (IsDead)
        {
            Die();
            return;
        }

        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(DamageFlash());
    }

    private void Die()
    {
        OnDied?.Invoke();

        if (LevelManager.instance != null)
            LevelManager.instance.AddKill();

        if (flashRoutine != null) StopCoroutine(flashRoutine);

        if (movement != null) movement.enabled = false;
        if (rb != null) rb.simulated = false;
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        float elapsed = 0f;

        while (elapsed < deathFadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - (elapsed / deathFadeTime);

            Color c = damageColor;
            c.a = alpha;

            for (int i = 0; i < renderers.Length; i++)
                if (renderers[i] != null) renderers[i].color = c;

            yield return null;
        }

        Destroy(gameObject);
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        Destroy(gameObject, destroyDelay);
    }

    public IEnumerator DamageFlash()
    {
        for (int i = 0; i < renderers.Length; i++)
            if (renderers[i] != null) renderers[i].color = damageColor;

        yield return new WaitForSeconds(damageFlashDuration);

        for (int i = 0; i < renderers.Length; i++)
            if (renderers[i] != null) renderers[i].color = originalColors[i];

        flashRoutine = null;
    }

    void ReturnEnemy()
    {
        EnemySpawner es = FindFirstObjectByType<EnemySpawner>();
        transform.position = player.position + es.relativeSpawnPoints[UnityEngine.Random.Range(0, es.relativeSpawnPoints.Count)].position;
    }

    void OnDestroy()
    {
        EnemySpawner es = FindFirstObjectByType<EnemySpawner>();
        if (es != null) es.OnEnemyKilled();
    }
}