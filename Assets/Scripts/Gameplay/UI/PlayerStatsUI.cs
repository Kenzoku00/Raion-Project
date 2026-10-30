using UnityEngine;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats player;
    [SerializeField] private Gun gun;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI weaponText;

    private void Start()
    {
        if (player == null) player = FindFirstObjectByType<PlayerStats>();
        if (gun == null) gun = FindFirstObjectByType<Gun>();

        if (player != null)
        {
            player.OnHealthChanged += HandleHealthChanged;
            player.OnStatsChanged += Refresh;
        }

        if (gun != null)
            gun.OnLevelChanged += HandleWeaponLevelChanged;

        Refresh();
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnHealthChanged -= HandleHealthChanged;
            player.OnStatsChanged -= Refresh;
        }

        if (gun != null)
            gun.OnLevelChanged -= HandleWeaponLevelChanged;
    }

    private void HandleHealthChanged(float current, float max) => Refresh();
    private void HandleWeaponLevelChanged(int level) => Refresh();

    private void Refresh()
    {
        if (player != null)
        {
            SetText(hpText, $"{Mathf.CeilToInt(player.CurrentHealth)} / {Mathf.CeilToInt(player.MaxHealth)}");
            SetText(attackText, $"{player.Attack:0.##}");
            SetText(speedText, $"{player.Speed:0.##}");
        }

        if (gun != null)
        {
            SetText(weaponText, gun.IsMaxLevel
                ? $"Lv. {gun.Level} (MAX)"
                : $"Lv. {gun.Level}");
        }
    }

    private static void SetText(TextMeshProUGUI text, string value)
    {
        if (text != null) text.text = value;
    }
}