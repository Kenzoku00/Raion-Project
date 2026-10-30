using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    private const string HighScoreKey = "HighScore";

    public static LevelManager instance { get; private set; }

    [Header("References")]
    [SerializeField] private PlayerStats player;
    [SerializeField] private IntroAnimation introAnimation;

    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI killText;
    [SerializeField] private TextMeshProUGUI coinText;

    [Header("HUD Labels")]
    [SerializeField] private string scoreLabel = "Score : ";
    [SerializeField] private string killLabel = "Kill : ";
    [SerializeField] private string coinLabel = "Coin : ";

    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Game Over UI (angka saja)")]
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private TextMeshProUGUI gameOverHighScoreText;
    [SerializeField] private TextMeshProUGUI gameOverKillText;
    [SerializeField] private TextMeshProUGUI gameOverCoinText;
    [SerializeField] private GameObject newBestObject;

    public int KillCount { get; private set; }
    public int CoinCount { get; private set; }
    public int ScoreCount { get; private set; }
    public int HighScore { get; private set; }

    private bool isPaused;
    private bool isGameOver;
    private bool isSelectingBuff;

    private void Awake()
    {
        instance = this;
        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);

        GameManager.instance.audioManager.PlayBGM("GAMEPLAY_BGM");
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

        UpdateScoreText();
        UpdateKillText();
        UpdateCoinText();

        if (introAnimation != null)
        {
            introAnimation.PlayIntroAnimation();
        }
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

        bool isNewBest = SaveHighScore();

        SetText(gameOverScoreText, ScoreCount.ToString());
        SetText(gameOverHighScoreText, HighScore.ToString());
        SetText(gameOverKillText, KillCount.ToString());
        SetText(gameOverCoinText, CoinCount.ToString());
        SetPanel(newBestObject, isNewBest);

        SetPanel(gameOverPanel, true);
    }

    private bool SaveHighScore()
    {
        if (ScoreCount <= HighScore) return false;

        HighScore = ScoreCount;
        PlayerPrefs.SetInt(HighScoreKey, HighScore);
        PlayerPrefs.Save();
        return true;
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

    public void AddScore(int amount)
    {
        if (isGameOver || amount <= 0) return;

        ScoreCount += amount;
        UpdateScoreText();
    }

    private void UpdateScoreText() => SetText(scoreText, scoreLabel + ScoreCount);
    private void UpdateKillText() => SetText(killText, killLabel + KillCount);
    private void UpdateCoinText() => SetText(coinText, coinLabel + CoinCount);

    private void SetText(TextMeshProUGUI text, string value)
    {
        if (text != null) text.text = value;
    }

    private void SetPanel(GameObject panel, bool active)
    {
        if (panel != null) panel.SetActive(active);
    }

    [ContextMenu("Reset High Score")]
    private void DebugResetHighScore()
    {
        PlayerPrefs.DeleteKey(HighScoreKey);
        HighScore = 0;
    }
}