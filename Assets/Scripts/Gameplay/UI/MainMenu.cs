using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.instance.SetGameState(GameState.Gameplay);
        GameManager.instance.SceneLoader.LoadScene("Gameplay");
    }

}
