using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private GunData gunData;
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform firePoint;

    public int Level { get; private set; } = 1;

    private GunLevel CurrentLevel => gunData.levels[Level - 1];

    private Camera cam;
    private PlayerStats stats;
    private float nextFireTime;

    private void Awake()
    {
        cam = Camera.main;
        stats = GetComponentInParent<PlayerStats>();
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        RotateToMouse();

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
            Shoot();
    }

    public bool LevelUp()
    {
        if (Level >= gunData.MaxLevel) return false;

        Level++;
        return true;
    }

    private void RotateToMouse()
    {
        Vector2 direction = GetMouseWorldPosition() - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Vector3 scale = transform.localScale;
        scale.y = (angle > 90f || angle < -90f) ? -Mathf.Abs(scale.y) : Mathf.Abs(scale.y);
        transform.localScale = scale;
    }

    private void Shoot()
    {
        GunLevel level = CurrentLevel;

        Vector2 direction = GetMouseWorldPosition() - firePoint.position;
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        int count = level.bulletsPerShot;
        float step = count > 1 ? level.spreadAngle / (count - 1) : 0f;
        float startAngle = baseAngle - (count > 1 ? level.spreadAngle / 2f : 0f);

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + step * i;
            Projectile bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.Euler(0f, 0f, angle - 90f));
            bullet.SetDamage(Mathf.RoundToInt(stats.Attack));
        }

        nextFireTime = Time.time + level.fireRate;
    }

    private Vector3 GetMouseWorldPosition()
    {
        return cam.ScreenToWorldPoint(Input.mousePosition);
    }

    [ContextMenu("Level Up")]
    private void DebugLevelUp() => LevelUp();
}