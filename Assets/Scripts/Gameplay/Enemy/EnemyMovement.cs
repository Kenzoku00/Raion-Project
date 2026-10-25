using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(EnemyStats))]
public class EnemyMovement : MonoBehaviour
{
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

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * stats.Speed;
    }
}