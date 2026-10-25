using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    GameManager gameManager;
    private bool isLoading = false;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isLoading = false;
    }

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        if (isLoading) yield break;
        isLoading = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("Scene " + sceneName + " has been loaded!");
    }

    public void LoadScene(string sceneName) => StartCoroutine(LoadSceneAsync(sceneName));

    public void LoadMainMenu()
    {
        gameManager.gameState = GameState.MainMenu;
        LoadScene("MainMenu");
    }

    public void RestartScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        LoadScene(currentSceneName);
    }
}