using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance { get; private set; }

    [Header("References")]
    [SerializeField] private PlayerStats player;

    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI killText;
    [SerializeField] private TextMeshProUGUI coinText;

    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;

    public int KillCount { get; private set; }
    public int CoinCount { get; private set; }

    private bool isPaused;
    private bool isGameOver;
    private bool isSelectingBuff;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1f;
        SetPanel(pausePanel, false);
        SetPanel(gameOverPanel, false);

        if (player == null)
            player = FindFirstObjectByType<PlayerStats>();

        if (player != null)
            player.OnDied += HandlePlayerDied;

        if (GameManager.instance != null)
            GameManager.instance.SetGameState(GameState.Gameplay);

        UpdateKillText();
        UpdateCoinText();
    }

    private void OnDestroy()
    {
        if (player != null)
            player.OnDied -= HandlePlayerDied;

        if (instance == this)
        {
            instance = null;
            Time.timeScale = 1f;
        }
    }

    private void Update()
    {
        if (isGameOver || isSelectingBuff) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        if (isGameOver || isPaused || isSelectingBuff) return;

        isPaused = true;
        GameManager.instance.SetGameState(GameState.Pause);
        Time.timeScale = 0f;
        SetPanel(pausePanel, true);
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        GameManager.instance.SetGameState(GameState.Gameplay);
        Time.timeScale = 1f;
        SetPanel(pausePanel, false);
    }

    public void BackToMenu()
    {
        isPaused = false;
        GameManager.instance.SetGameState(GameState.MainMenu);
        Time.timeScale = 1f;
        SetPanel(pausePanel, false);
        GameManager.instance.SceneLoader.LoadMainMenu();
    }

    public void BeginBuffSelection()
    {
        isSelectingBuff = true;
        Time.timeScale = 0f;
    }

    public void EndBuffSelection()
    {
        isSelectingBuff = false;
        Time.timeScale = 1f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        GameManager.instance.SceneLoader.RestartScene();
    }

    private void HandlePlayerDied()
    {
        isGameOver = true;
        GameManager.instance.SetGameState(GameState.GameOver);
        Time.timeScale = 0f;
        SetPanel(gameOverPanel, true);
    }

    public void AddKill()
    {
        KillCount++;
        UpdateKillText();
    }

    public void AddCoin(int amount)
    {
        CoinCount += amount;
        UpdateCoinText();
    }

    private void UpdateKillText()
    {
        if (killText != null) killText.text = "Kills: " + KillCount;
    }

    private void UpdateCoinText()
    {
        if (coinText != null) coinText.text = "Coins: " + CoinCount;
    }

    private void SetPanel(GameObject panel, bool active)
    {
        if (panel != null) panel.SetActive(active);
    }
}