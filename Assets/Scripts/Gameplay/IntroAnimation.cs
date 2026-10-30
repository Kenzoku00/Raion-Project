using UnityEngine;
using DG.Tweening;

public class IntroAnimation : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float startSize = 25f;
    [SerializeField] private float targetSize = 5f;
    [SerializeField] private float zoomDuration = 1.2f;
    [SerializeField] private Ease zoomEase = Ease.OutCubic;

    [Header("UI Elements (Canvas Group)")]
    [SerializeField] private CanvasGroup healthBarGroup;
    [SerializeField] private CanvasGroup expBarGroup;
    [SerializeField] private CanvasGroup counterHudGroup;
    [SerializeField] private CanvasGroup scoreGroup;

    [Header("UI Settings")]
    [SerializeField] private float uiFadeDuration = 0.8f;
    [SerializeField] private float uiSlideOffset = 50f;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera != null)
            mainCamera.orthographicSize = startSize;

        SetupUIInitialState(healthBarGroup, new Vector2(0, uiSlideOffset));
        SetupUIInitialState(expBarGroup, new Vector2(0, -uiSlideOffset));
        SetupUIInitialState(counterHudGroup, new Vector2(-uiSlideOffset, 0));
        SetupUIInitialState(scoreGroup, new Vector2(uiSlideOffset, 0));
    }

    public void PlayIntroAnimation()
    {
        if (mainCamera == null) return;

        mainCamera.DOOrthoSize(targetSize, zoomDuration)
            .SetEase(zoomEase)
            .OnComplete(() =>
            {
                // tes
            });

        AnimateUIIn(healthBarGroup, uiFadeDuration);
        AnimateUIIn(expBarGroup, uiFadeDuration);
        AnimateUIIn(counterHudGroup, uiFadeDuration);
        AnimateUIIn(scoreGroup, uiFadeDuration);
    }

    private void SetupUIInitialState(CanvasGroup group, Vector2 offset)
    {
        if (group == null) return;
        group.alpha = 0f;
        RectTransform rect = group.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchoredPosition += offset;
        }
    }

    private void AnimateUIIn(CanvasGroup group, float duration)
    {
        if (group == null) return;

        group.DOFade(1f, duration);

        RectTransform rect = group.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.DOAnchorPos(rect.anchoredPosition - GetOffsetForGroup(group), duration).SetEase(Ease.OutQuad);
        }
    }

    private Vector2 GetOffsetForGroup(CanvasGroup group)
    {
        if (group == healthBarGroup) return new Vector2(0, uiSlideOffset);
        if (group == expBarGroup) return new Vector2(0, -uiSlideOffset);
        if (group == counterHudGroup) return new Vector2(-uiSlideOffset, 0);
        if (group == scoreGroup) return new Vector2(uiSlideOffset, 0);
        return Vector2.zero;
    }
}