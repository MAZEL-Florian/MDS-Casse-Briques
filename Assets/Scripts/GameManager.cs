using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int level = 1;
    public int score = 0;
    public int lives = 3;

    public float levelTimer = 0f;
    public float totalTimer = 0f;
    public float[] levelTimes;

    public TMP_Text livesText;
    public TMP_Text timerText;
    public TMP_Text levelCompleteText;
    public TMP_Text winText;

    public GameObject levelCompletePanel;
    public GameObject gameOverPanel;
    public GameObject winPanel;

    private AudioSource audioSource;
    private bool isLevelCompleted = false;
    public AudioClip levelCompleteSound;
    public AudioClip gameOverSound;
    public Ball ball { get; private set; }
    public Paddle paddle { get; private set; }
    public PowerUp powerUp { get; private set; }
    public Brick[] bricks { get; private set; }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnLevelLoaded;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
    }

    private void Update()
    {
        if (SceneManager.GetSceneByName("MainMenu").isLoaded)
        {
            return;
        }

        if (!isLevelCompleted)
        {
            levelTimer += Time.deltaTime;
            totalTimer += Time.deltaTime;

            if (timerText != null)
            {
                timerText.text = $"Temps : {levelTimer:F1}s";
            }
        }
    }

    public void StartNewGame()
    {
        Debug.Log("Resetting GameManager state for new game.");

        level = 1;
        score = 0;
        lives = 3;
        isLevelCompleted = false;

        totalTimer = 0f;
        levelTimer = 0f;
        levelTimes = new float[1];

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        UpdateLivesText();

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var loadedScene = SceneManager.GetSceneAt(i);
            if (loadedScene.name != "GlobalScene")
            {
                SceneManager.UnloadSceneAsync(loadedScene);
            }
        }

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

        if (level > 1)
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

        levelTimer = 0f;

        if (paddle != null) paddle.enabled = true;
        if (ball != null && ball.GetComponent<AudioSource>() == null)
        {
            ball.gameObject.AddComponent<AudioSource>();
        }
        if (paddle != null && paddle.GetComponent<AudioSource>() == null)
        {
            paddle.gameObject.AddComponent<AudioSource>();
        }

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

        SceneManager.sceneLoaded -= OnLevelLoaded;

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var loadedScene = SceneManager.GetSceneAt(i);
            SceneManager.UnloadSceneAsync(loadedScene);
        }

        SceneManager.LoadScene("GlobalScene", LoadSceneMode.Single);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
    }




    private void GameOver()
    {
        Debug.Log("GameOver triggered!");
        if (ball != null) ball.StopBall();
        if (paddle != null) paddle.enabled = false;
        if (audioSource != null && gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }

        if (gameOverPanel != null)
        {
            Debug.Log("Activating GameOverPanel");
            gameOverPanel.SetActive(true);
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

        // ✅ Check pour éviter dépassement de tableau
        if (level - 1 < levelTimes.Length)
        {
            levelTimes[level - 1] = levelTimer; // store time for this level
        }

        if (ball != null) ball.StopBall();
        if (paddle != null) paddle.enabled = false;

        if (audioSource != null && levelCompleteSound != null)
        {
            audioSource.PlayOneShot(levelCompleteSound);
        }

        if (level >= 1)
        {
            ShowWinScreen();
        }
        else
        {
            if (levelCompletePanel != null && levelCompleteText != null)
            {
                levelCompletePanel.SetActive(true);
                levelCompleteText.text = $"Niveau {level} terminé !\nTemps : {levelTimer:F1}s";
            }
        }
    }


    private void ShowWinScreen()
    {
        if (winPanel != null && winText != null)
        {
            winPanel.SetActive(true);

            string summary = $"Bravo ! Tu as terminé le jeu.\n\n";
            summary += $"Temps total : {totalTimer:F1}s\n\n";

            for (int i = 0; i < levelTimes.Length; i++)
            {
                summary += $"Niveau {i + 1} : {levelTimes[i]:F1}s\n";
            }

            winText.text = summary;
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
