using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    [SerializeField] private int expValue = 1;
    AudioManager audioManager;

    private bool isCollected;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    public void SetValue(int value)
    {
        expValue = value;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        audioManager.PlaySFX("XP");

        if (isCollected || !collision.CompareTag("Player")) return;

        PlayerStats player = collision.GetComponentInParent<PlayerStats>();
        if (player == null) return;

        isCollected = true;
        player.AddExp(expValue);
        Destroy(gameObject);
    }
}