using DG.Tweening;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TMP_Text label;

    [Header("Animation")]
    [SerializeField] private float lifetime = 0.8f;
    [SerializeField] private float popTime = 0.2f;
    [SerializeField] private float riseHeight = 0.6f;
    [SerializeField] private float driftX = 0.25f;
    [SerializeField] private float fadeTime = 0.3f;

    private static FloatingText prefab;

    public static void Spawn(Vector3 position, string text, Color color)
    {
        if (prefab == null)
            prefab = Resources.Load<FloatingText>("FloatingText");

        if (prefab == null)
        {
            Debug.LogWarning("Prefab 'FloatingText' tidak ditemukan.");
            return;
        }

        FloatingText instance = Instantiate(prefab, position, Quaternion.identity);
        instance.Play(text, color);
    }

    private void Play(string text, Color color)
    {
        label.text = text;
        label.color = color;

        Vector3 baseScale = transform.localScale;
        Vector3 start = transform.position;
        float targetX = start.x + Random.Range(-driftX, driftX);
        float targetY = start.y + riseHeight;

        transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence().SetId(this).SetUpdate(true);

        seq.Insert(0f, transform.DOScale(baseScale, popTime).SetEase(Ease.OutBack));

        seq.Insert(0f, transform.DOMoveY(targetY, lifetime).SetEase(Ease.OutCubic));
        seq.Insert(0f, transform.DOMoveX(targetX, lifetime).SetEase(Ease.Linear));

        seq.Insert(lifetime - fadeTime,
            DOTween.ToAlpha(() => label.color, c => label.color = c, 0f, fadeTime));

        seq.OnComplete(() => Destroy(gameObject));
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
    }
}