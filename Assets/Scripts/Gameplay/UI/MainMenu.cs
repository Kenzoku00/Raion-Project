using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float targetOrthographicSize = 25f;
    [SerializeField] private float cameraZoomDuration = 1.5f;

    [Header("Camera Floating Settings")]
    [SerializeField] private float floatDistance = 0.3f;
    [SerializeField] private float floatDuration = 2f;

    [Header("UI Elements")]
    [SerializeField] private RectTransform titleTransform;
    [SerializeField] private CanvasGroup titleCanvasGroup;
    [SerializeField] private RectTransform playButtonTransform;
    [SerializeField] private CanvasGroup playButtonCanvasGroup;

    [Header("UI Transition Settings")]
    [SerializeField] private float uiSlideDistance = 100f;
    [SerializeField] private float uiTransitionDuration = 0.8f;

    private Tween cameraFloatingTween;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        StartCameraFloating();
    }

    private void StartCameraFloating()
    {
        cameraFloatingTween = mainCamera.transform.DOMoveY(mainCamera.transform.position.y + floatDistance, floatDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void OnPlayButtonPressed()
    {
        if (cameraFloatingTween != null)
            cameraFloatingTween.Kill();

        if (titleCanvasGroup != null && titleTransform != null)
        {
            titleCanvasGroup.DOFade(0f, uiTransitionDuration);
            titleTransform.DOAnchorPosY(titleTransform.anchoredPosition.y + uiSlideDistance, uiTransitionDuration).SetEase(Ease.InBack);
        }

        if (playButtonCanvasGroup != null && playButtonTransform != null)
        {
            playButtonCanvasGroup.DOFade(0f, uiTransitionDuration);
            playButtonTransform.DOAnchorPosY(playButtonTransform.anchoredPosition.y - uiSlideDistance, uiTransitionDuration).SetEase(Ease.InBack);
        }

        mainCamera.DOOrthoSize(targetOrthographicSize, cameraZoomDuration)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() =>
            {
                StartGame();
            });
    }

    public void StartGame()
    {
        GameManager.instance.SetGameState(GameState.Gameplay);
        GameManager.instance.SceneLoader.LoadScene("Gameplay");
    }
}