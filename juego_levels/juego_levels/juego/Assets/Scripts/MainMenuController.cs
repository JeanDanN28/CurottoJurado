using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string introSceneName = "IntroScene";

    public void PlayGame()
    {
        SceneManager.LoadScene(introSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Juego cerrado");
    }
}
