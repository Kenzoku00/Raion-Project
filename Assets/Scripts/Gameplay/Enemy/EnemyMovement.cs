using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(EnemyStats))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float stoppingDistance = 0f;

    private Rigidbody2D rb;
    private EnemyStats stats;
    private Transform player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<EnemyStats>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        if (stats.IsDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 toPlayer = player.position - transform.position;

        if (toPlayer.magnitude <= stoppingDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = toPlayer.normalized * stats.Speed;
    }
}