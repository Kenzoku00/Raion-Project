using UnityEngine;

public class Heal : MonoBehaviour
{
    [Header("Heal Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float healPercentage = 0.15f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager.instance.audioManager.PlaySFX("HEALING");

        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent<PlayerStats>(out PlayerStats playerStats))
            {
                float healAmount = playerStats.MaxHealth * healPercentage;
                playerStats.Heal(healAmount);

                Destroy(gameObject);
            }
        }
    }
}