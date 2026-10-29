using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private AttackType attackType = AttackType.Melee;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Ranged Only")]
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private Transform firePoint;

    private EnemyStats stats;
    private PlayerStats player;
    private float nextAttackTime;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.GetComponentInParent<PlayerStats>();
    }

    private void Update()
    {
        if (stats.IsDead || player == null || player.IsDead) return;
        if (Time.time < nextAttackTime) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance > attackRange) return;

        Attack();
        nextAttackTime = Time.time + attackCooldown;
    }

    private void Attack()
    {
        switch (attackType)
        {
            case AttackType.Melee:
                MeleeAttack();
                break;
            case AttackType.Ranged:
                RangedAttack();
                break;
        }
    }

    private void MeleeAttack()
    {
        player.TakeDamage(stats.Attack);
    }

    private void RangedAttack()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning($"{name}: Projectile Prefab belum diisi untuk tipe Ranged!");
            return;
        }

        Vector2 origin = firePoint != null ? firePoint.position : transform.position;
        Vector2 direction = (Vector2)player.transform.position - origin;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        EnemyProjectile bullet = Instantiate(projectilePrefab, origin, Quaternion.Euler(0f, 0f, angle - 90f));
        bullet.SetDamage(stats.Attack);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}