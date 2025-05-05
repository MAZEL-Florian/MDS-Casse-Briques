using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int level = 1;
    public int score = 0;
    public int lives = 3;

    public TMP_Text livesText;
    public GameObject levelCompletePanel;
    public TMP_Text levelCompleteText;
    public AudioClip levelCompleteSound;
    private AudioSource audioSource;
    public GameObject gameOverPanel;
    private bool isLevelCompleted = false;

    public Ball ball { get; private set; }
    public Paddle paddle { get; private set; }
    public PowerUp powerUp { get; private set; }
    public Brick[] bricks { get; private set; }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnLevelLoaded;

        // Désactive les panels dès le départ
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Forcing GameOverPanel ON");
            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Forcing LevelCompletePanel ON");
            if (levelCompletePanel != null)
                levelCompletePanel.SetActive(true);
        }
    }

    public void StartNewGame()
    {
        Debug.Log("Resetting GameManager state for new game.");

        level = 1;
        score = 0;
        lives = 3;
        isLevelCompleted = false;

        // Réinitialise les UI
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        UpdateLivesText();

        // IMPORTANT : décharge explicitement toutes les scènes sauf GlobalScene
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var loadedScene = SceneManager.GetSceneAt(i);
            if (loadedScene.name != "GlobalScene")
            {
                SceneManager.UnloadSceneAsync(loadedScene);
            }
        }

        // Recharge le premier niveau proprement
        SceneManager.LoadScene("Level1", LoadSceneMode.Additive);
    }



    private void LoadLevel(int level)
    {
        this.level = level;

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var loadedScene = SceneManager.GetSceneAt(i);
            if (loadedScene.name != "GlobalScene")
            {
                SceneManager.UnloadSceneAsync(loadedScene);
            }
        }

        if (level > 10)
        {
            SceneManager.LoadScene("WinScreen", LoadSceneMode.Additive);
        }
        else
        {
            SceneManager.LoadScene("Level" + level, LoadSceneMode.Additive);
        }
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        ball = FindObjectOfType<Ball>();
        paddle = FindObjectOfType<Paddle>();
        powerUp = FindObjectOfType<PowerUp>();
        bricks = FindObjectsOfType<Brick>();
        isLevelCompleted = false;

        if (paddle != null) paddle.enabled = true;
        if (ball != null && ball.GetComponent<AudioSource>() == null)
        {
            ball.gameObject.AddComponent<AudioSource>();
        }
        if (paddle != null && paddle.GetComponent<AudioSource>() == null)
        {
            paddle.gameObject.AddComponent<AudioSource>();
        }

        UpdateUIReferences();

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);

        UpdateLivesText();
    }

    private void UpdateUIReferences()
    {
        // Toutes les références sont déjà assignées dans l’inspecteur
    }

    private void ResetLevel()
    {
        if (ball != null) ball.ResetBall();
        if (paddle != null) paddle.ResetPaddle();
    }

    public void ReturnToMenu()
    {
        Debug.Log("Returning to menu, resetting full game state.");

        // Désinscrit les callbacks
        SceneManager.sceneLoaded -= OnLevelLoaded;

        // Décharge toutes les scènes (y compris GlobalScene)
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var loadedScene = SceneManager.GetSceneAt(i);
            SceneManager.UnloadSceneAsync(loadedScene);
        }

        // Recharge GlobalScene en Single → il recrée un GameManager neuf
        SceneManager.LoadScene("GlobalScene", LoadSceneMode.Single);

        // Ajoute MainMenu par-dessus
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
    }




    private void GameOver()
    {
        Debug.Log("GameOver triggered!");
        if (ball != null) ball.StopBall();
        if (paddle != null) paddle.enabled = false;

        if (gameOverPanel != null)
        {
            Debug.Log("Activating GameOverPanel");
            gameOverPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("GameOverPanel is NULL!");
        }
    }

    public void Miss()
    {
        lives--;

        if (lives <= 0)
        {
            GameOver();
        }
        else
        {
            ResetLevel();
        }

        UpdateLivesText();
    }

    public void Hit(Brick brick)
    {
        score += brick.points;

        if (Cleared())
        {
            ShowLevelComplete();
        }
    }

    private bool Cleared()
    {
        foreach (Brick brick in bricks)
        {
            if (brick.gameObject.activeInHierarchy && !brick.unbreakable)
            {
                return false;
            }
        }
        return true;
    }

    private void ShowLevelComplete()
    {
        Debug.Log("Level complete triggered!");
        if (isLevelCompleted) return;

        isLevelCompleted = true;

        if (ball != null) ball.StopBall();
        if (paddle != null) paddle.enabled = false;

        if (audioSource != null && levelCompleteSound != null)
        {
            audioSource.PlayOneShot(levelCompleteSound);
        }

        if (levelCompletePanel != null && levelCompleteText != null)
        {
            Debug.Log("Activating LevelCompletePanel");
            levelCompletePanel.SetActive(true);
            levelCompleteText.text = $"Niveau {level} terminé !";
        }
        else
        {
            Debug.LogWarning("LevelCompletePanel or LevelCompleteText is NULL!");
        }
    }

    public void LoadNextLevel()
    {
        level++;
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        LoadLevel(level);
    }

    public void AddLife()
    {
        lives++;
        UpdateLivesText();
    }

    private void UpdateLivesText()
    {
        if (livesText != null)
        {
            livesText.text = $"Vies : {lives}";
        }
    }
}
