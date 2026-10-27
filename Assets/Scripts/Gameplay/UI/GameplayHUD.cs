using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayHUD : MonoBehaviour
{
    [SerializeField] private PlayerStats player;

    [Header("HP Bar")]
    [SerializeField] private Image hpFill;

    [Header("EXP Bar")]
    [SerializeField] private Image expFill;
    [SerializeField] private TextMeshProUGUI levelText;

    private void Start()
    {
        if (player == null)
            player = FindFirstObjectByType<PlayerStats>();

        if (player == null) return;

        player.OnHealthChanged += UpdateHpBar;
        player.OnExpChanged += UpdateExpBar;

        UpdateHpBar(player.CurrentHealth, player.MaxHealth);
        UpdateExpBar(player.CurrentExp, player.ExpToNextLevel);
    }

    private void OnDestroy()
    {
        if (player == null) return;

        player.OnHealthChanged -= UpdateHpBar;
        player.OnExpChanged -= UpdateExpBar;
    }

    private void UpdateHpBar(float current, float max)
    {
        if (hpFill == null || max <= 0f) return;

        hpFill.fillAmount = current / max;
    }

    private void UpdateExpBar(int current, int required)
    {
        if (expFill != null && required > 0)
            expFill.fillAmount = (float)current / required;

        if (levelText != null)
            levelText.text = "Level " + player.Level;
    }

    private void UpdateLevelText(int level)
    {
        if (levelText != null)
            levelText.text = "Level " + level;
    }
}