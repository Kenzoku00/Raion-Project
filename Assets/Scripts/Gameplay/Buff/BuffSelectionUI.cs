using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BuffSelectionUI : MonoBehaviour
{
    [Serializable]
    private class CardSlot
    {
        public Button button;
        public Image icon;
        public TextMeshProUGUI title;
        public TextMeshProUGUI description;
        [HideInInspector] public RectTransform rectTransform;
        [HideInInspector] public CanvasGroup canvasGroup;
    }

    [Header("References")]
    [SerializeField] private BuffManager buffManager;
    [SerializeField] private GameObject panel;
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private RectTransform playerStatsPanel;
    [SerializeField] private CardSlot[] cards;

    [Header("Animation Settings")]
    [SerializeField] private float slideDistance = 400f;
    [SerializeField] private float animDuration = 0.4f;
    [SerializeField] private float exitDuration = 0.3f;
    [SerializeField] private float staggerDelay = 0.12f;

    private Vector2[] cardOriginalAnchoredPositions;
    private Vector2 statsOriginalAnchoredPosition;
    private Sequence currentSequence;
    private int selectedIndex = -1;

    private void Awake()
    {
        if (playerStatsPanel != null)
        {
            statsOriginalAnchoredPosition = playerStatsPanel.anchoredPosition;
        }

        cardOriginalAnchoredPositions = new Vector2[cards.Length];
        for (int i = 0; i < cards.Length; i++)
        {
            int index = i;
            cards[i].button.onClick.AddListener(() => OnCardClicked(index));

            cards[i].rectTransform = cards[i].button.GetComponent<RectTransform>();

            cards[i].canvasGroup = cards[i].button.GetComponent<CanvasGroup>();
            if (cards[i].canvasGroup == null)
                cards[i].canvasGroup = cards[i].button.gameObject.AddComponent<CanvasGroup>();

            if (cards[i].button.GetComponent<BuffCardHover>() == null)
                cards[i].button.gameObject.AddComponent<BuffCardHover>();

            cardOriginalAnchoredPositions[i] = cards[i].rectTransform.anchoredPosition;
        }

        if (panelCanvasGroup == null && panel != null)
        {
            panelCanvasGroup = panel.GetComponent<CanvasGroup>();
            if (panelCanvasGroup == null)
                panelCanvasGroup = panel.AddComponent<CanvasGroup>();
        }

        panel.SetActive(false);
    }

    private void Start()
    {
        buffManager.OnChoicesReady += ShowChoices;
        buffManager.OnSelectionFinished += Hide;
    }

    private void OnDestroy()
    {
        if (buffManager == null) return;

        buffManager.OnChoicesReady -= ShowChoices;
        buffManager.OnSelectionFinished -= Hide;
    }

    private void ShowChoices(BuffData[] choices)
    {
        currentSequence?.Kill();
        selectedIndex = -1;
        panel.SetActive(true);

        if (panelCanvasGroup != null)
            panelCanvasGroup.alpha = 1f;

        currentSequence = DOTween.Sequence().SetUpdate(true);

        if (playerStatsPanel != null)
        {
            playerStatsPanel.anchoredPosition = statsOriginalAnchoredPosition - new Vector2(slideDistance, 0);

            CanvasGroup statsCG = playerStatsPanel.GetComponent<CanvasGroup>();
            if (statsCG == null) statsCG = playerStatsPanel.gameObject.AddComponent<CanvasGroup>();
            statsCG.alpha = 0f;

            currentSequence.Join(playerStatsPanel.DOAnchorPos(statsOriginalAnchoredPosition, animDuration).SetEase(Ease.OutBack));
            currentSequence.Join(statsCG.DOFade(1f, animDuration));
        }

        for (int i = 0; i < cards.Length; i++)
        {
            bool hasChoice = i < choices.Length;
            cards[i].button.gameObject.SetActive(hasChoice);

            if (!hasChoice) continue;

            cards[i].title.text = choices[i].title;
            cards[i].description.text = choices[i].description;
            cards[i].icon.sprite = choices[i].icon;
            cards[i].icon.enabled = choices[i].icon != null;

            cards[i].rectTransform.anchoredPosition = cardOriginalAnchoredPositions[i] + new Vector2(slideDistance, 0);
            cards[i].canvasGroup.alpha = 0f;
            cards[i].button.transform.localScale = Vector3.one;

            float delay = i * staggerDelay;

            currentSequence.Insert(delay, cards[i].rectTransform.DOAnchorPos(cardOriginalAnchoredPositions[i], animDuration).SetEase(Ease.OutBack));
            currentSequence.Insert(delay, cards[i].canvasGroup.DOFade(1f, animDuration * 0.8f));
        }
    }

    private void OnCardClicked(int index)
    {
        selectedIndex = index;

        buffManager.SelectBuff(index);
    }

    private void Hide()
    {
        currentSequence?.Kill();
        currentSequence = DOTween.Sequence().SetUpdate(true);

        if (playerStatsPanel != null)
        {
            CanvasGroup statsCG = playerStatsPanel.GetComponent<CanvasGroup>();
            if (statsCG != null)
            {
                currentSequence.Join(statsCG.DOFade(0f, exitDuration));
            }
            currentSequence.Join(playerStatsPanel.DOAnchorPos(statsOriginalAnchoredPosition - new Vector2(slideDistance, 0), exitDuration).SetEase(Ease.InBack));
        }

        for (int i = 0; i < cards.Length; i++)
        {
            if (!cards[i].button.gameObject.activeSelf) continue;

            if (i == selectedIndex)
            {
                currentSequence.Join(cards[i].button.transform.DOScale(1.15f, exitDuration * 0.5f).SetEase(Ease.OutQuad));
                currentSequence.Join(cards[i].canvasGroup.DOFade(0f, exitDuration).SetDelay(exitDuration * 0.2f));
            }
            else
            {
                currentSequence.Join(cards[i].rectTransform.DOAnchorPos(cardOriginalAnchoredPositions[i] + new Vector2(slideDistance, 0), exitDuration).SetEase(Ease.InBack));
                currentSequence.Join(cards[i].canvasGroup.DOFade(0f, exitDuration * 0.8f));
            }
        }

        if (panelCanvasGroup != null)
        {
            currentSequence.Join(panelCanvasGroup.DOFade(0f, exitDuration));
        }

        currentSequence.OnComplete(() =>
        {
            panel.SetActive(false);
        });
    }
}