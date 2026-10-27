using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffSelectionUI : MonoBehaviour
{
    [Serializable]
    private class CardSlot
    {
        public Button button;
        public TextMeshProUGUI title;
        public TextMeshProUGUI description;
    }

    [SerializeField] private BuffManager buffManager;
    [SerializeField] private GameObject panel;
    [SerializeField] private CardSlot[] cards;

    private void Awake()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            int index = i;
            cards[i].button.onClick.AddListener(() => buffManager.SelectBuff(index));
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
        panel.SetActive(true);

        for (int i = 0; i < cards.Length; i++)
        {
            bool hasChoice = i < choices.Length;
            cards[i].button.gameObject.SetActive(hasChoice);

            if (!hasChoice) continue;

            cards[i].title.text = choices[i].title;
            cards[i].description.text = choices[i].description;
        }
    }

    private void Hide()
    {
        panel.SetActive(false);
    }
}