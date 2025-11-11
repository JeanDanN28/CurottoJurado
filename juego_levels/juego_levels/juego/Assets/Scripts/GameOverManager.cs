using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameOverManager : MonoBehaviour
{
    [SerializeField] GameObject gameOverUI; 

    public static GameOverManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
        
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }

    void Update()
    {

        if (gameOverUI != null && gameOverUI.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                RestartGame();
            }
        }
    }

    public void ShowGameOver()
    {
        if (gameOverUI != null)
        {

            gameOverUI.SetActive(true);
            
            Time.timeScale = 0f; 
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}