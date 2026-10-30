using UnityEngine;

public class HealthPopupSpawner : MonoBehaviour
{
    [Header("Posisi")]
    [SerializeField] private Vector2 offset = new Vector2(0f, 0.4f);
    [SerializeField] private float randomX = 0.15f;

    [Header("Damage")]
    [SerializeField] private Color damageColor = Color.white;

    [Header("Heal (hanya dipakai player)")]
    [SerializeField] private Color healColor = new Color(0.3f, 1f, 0.4f);

    private IDamageable damageable;
    private PlayerStats player;

    private void Awake()
    {
        damageable = GetComponent<IDamageable>();
        player = GetComponent<PlayerStats>();
    }

    private void OnEnable()
    {
        if (damageable != null) damageable.OnDamaged += HandleDamaged;
        if (player != null) player.OnHealed += HandleHealed;
    }

    private void OnDisable()
    {
        if (damageable != null) damageable.OnDamaged -= HandleDamaged;
        if (player != null) player.OnHealed -= HandleHealed;
    }

    private void HandleDamaged(float amount)
    {
        Show(Mathf.RoundToInt(amount).ToString(), damageColor);
    }

    private void HandleHealed(float amount)
    {
        Show("+" + Mathf.RoundToInt(amount), healColor);
    }

    private void Show(string text, Color color)
    {
        Vector3 position = transform.position
            + (Vector3)offset
            + new Vector3(Random.Range(-randomX, randomX), 0f, 0f);

        FloatingText.Spawn(position, text, color);
    }
}