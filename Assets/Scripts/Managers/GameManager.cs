using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [Header("Sub-Managers")]
    [field: SerializeField] public AudioManager audioManager { get; private set; }
    [field: SerializeField] public UIManager uiManager { get; private set; }

    [SerializeField] private SceneLoader sceneLoader;
    public SceneLoader SceneLoader => sceneLoader;
    public System.Action<GameState> OnGameStateChanged;

    [Header("Game Enumerators")]
    public GameState gameState;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;

        audioManager?.Initialize();
        uiManager?.HideAllPopups();

        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;

        uiManager?.HideAllPopups();
    }

    public void SetGameState(GameState newGameState)
    {
        if (gameState == newGameState) return;

        gameState = newGameState;
        OnGameStateChanged?.Invoke(gameState);
    }

    private void OnDestroy()
    {
        if (instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}