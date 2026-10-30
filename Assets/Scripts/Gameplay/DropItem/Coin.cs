using UnityEngine;

public class Coin : MonoBehaviour
{
    AudioManager audioManager;
    public int coinValue = 1;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (LevelManager.instance != null)
            {
                audioManager.PlaySFX("COIN");
                LevelManager.instance.AddCoin(coinValue);
                Destroy(gameObject);
            }
        }
    }
}
