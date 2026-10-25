using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 6f;
    [SerializeField] private float lifeTime = 4f;

    private float damage;

    public void SetDamage(float value)
    {
        damage = value;
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerStats target = collision.GetComponentInParent<PlayerStats>();
        if (target != null)
            target.TakeDamage(damage);

        Destroy(gameObject);
    }
}