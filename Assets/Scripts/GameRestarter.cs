using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRestarter
{
    public static GameRestarter Instance { get; private set; }

    public GameRestarter()
    {
        if (Instance != null)
            return;

        Instance = this;
    }

    public void RestartGame()
    {
        Debug.Log("Restart game");
        SceneManager.LoadScene(0);
    }
}