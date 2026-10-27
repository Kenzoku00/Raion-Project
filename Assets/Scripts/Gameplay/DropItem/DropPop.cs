using DG.Tweening;
using UnityEngine;

public class DropPop : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float duration = 0.4f;
    [SerializeField] private float jumpHeight = 0.6f;
    [SerializeField] private Ease scaleEase = Ease.OutBack;

    public bool IsPopping { get; private set; }

    private Collider2D[] colliders;

    public void Play(Vector3 landingPosition)
    {
        colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
            col.enabled = false;

        IsPopping = true;

        Vector3 originalScale = transform.localScale;
        transform.localScale = Vector3.zero;

        transform.DOScale(originalScale, duration).SetEase(scaleEase);
        transform.DOJump(landingPosition, jumpHeight, 1, duration)
            .OnComplete(OnLanded);
    }

    private void OnLanded()
    {
        foreach (Collider2D col in colliders)
            col.enabled = true;

        IsPopping = false;
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}