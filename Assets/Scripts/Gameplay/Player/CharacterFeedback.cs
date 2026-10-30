using DG.Tweening;
using UnityEngine;

public class CharacterFeedback : MonoBehaviour
{
    [Header("Hit")]
    [SerializeField] private float flashDuration = 0.08f;
    [SerializeField] private float punchAmount = 0.25f;
    [SerializeField] private float punchDuration = 0.2f;

    [Header("Death")]
    [SerializeField] private float deathDuration = 0.35f;

    private static Material flashMaterial;

    private IDamageable damageable;
    private SpriteRenderer[] renderers;
    private Material[] originalMaterials;
    private Vector3 baseScale;
    private Tween flashTween;
    private bool isDead;

    private void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
        originalMaterials = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
            originalMaterials[i] = renderers[i].sharedMaterial;

        baseScale = transform.localScale;
        damageable = GetComponentInParent<IDamageable>();

        if (flashMaterial == null)
            flashMaterial = Resources.Load<Material>("SpriteFlash");

        if (flashMaterial == null)
            Debug.LogWarning("Material 'SpriteFlash' tidak ditemukan.");
    }

    private void OnEnable()
    {
        if (damageable == null) return;

        damageable.OnDamaged += HandleDamaged;
        damageable.OnDied += HandleDied;
    }

    private void OnDisable()
    {
        if (damageable == null) return;

        damageable.OnDamaged -= HandleDamaged;
        damageable.OnDied -= HandleDied;
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
        flashTween?.Kill();
    }

    private void HandleDamaged(float amount)
    {
        if (isDead) return;

        Flash();
        Punch();
    }

    private void HandleDied()
    {
        isDead = true;

        DOTween.Kill(this);
        transform.localScale = baseScale;

        const float popTime = 0.08f;
        float shrinkTime = Mathf.Max(0.05f, deathDuration - popTime);

        Sequence seq = DOTween.Sequence().SetId(this).SetUpdate(true);
        seq.Append(transform.DOScale(baseScale * 1.2f, popTime).SetEase(Ease.OutQuad));
        seq.Append(transform.DOScale(Vector3.zero, shrinkTime).SetEase(Ease.InBack));

        foreach (SpriteRenderer r in renderers)
            seq.Join(r.DOFade(0f, shrinkTime));
    }

    private void Flash()
    {
        if (flashMaterial == null) return;

        flashTween?.Kill();

        foreach (SpriteRenderer r in renderers)
            r.sharedMaterial = flashMaterial;

        flashTween = DOVirtual.DelayedCall(flashDuration, RestoreMaterials);
    }

    private void RestoreMaterials()
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].sharedMaterial = originalMaterials[i];
    }

    private void Punch()
    {
        DOTween.Kill(this);
        transform.localScale = baseScale;

        Vector3 punch = new Vector3(baseScale.x * punchAmount, -baseScale.y * punchAmount, 0f);
        transform.DOPunchScale(punch, punchDuration, 8, 0.8f)
            .SetId(this)
            .SetUpdate(true);
    }
}