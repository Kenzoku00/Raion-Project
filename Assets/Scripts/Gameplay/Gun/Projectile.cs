using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;

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
        if (!collision.CompareTag("Enemy")) return;

        if (collision.TryGetComponent(out EnemyStats enemy))
            enemy.TakeDamage(damage);

        Destroy(gameObject);
    }
}