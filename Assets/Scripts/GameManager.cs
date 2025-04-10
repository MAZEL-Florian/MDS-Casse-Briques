using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public int level = 1;
    public int score = 0;
    public int lives = 3;

    public Ball ball {  get; private set; }
    public Paddle paddle { get; private set; }

    public Brick[] bricks { get; private set; }

   private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        SceneManager.sceneLoaded += OnLevelLoaded;
    }
    private void Start()
    {
        NewGame();
    }
    private void NewGame()
    {
        this.score = 0;
        this.lives = 3;

        LoadLevel(1);

    }
    
    private void LoadLevel(int level)
    {
        this.level = level;

        if(level > 10)
        {
            SceneManager.LoadScene("WinScreen");
        }
        else
        {
            SceneManager.LoadScene("Level" + level);

        }
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        this.ball = FindObjectOfType<Ball>();
        this.paddle = FindObjectOfType<Paddle>();
        this.bricks = FindObjectsOfType<Brick>();

    }

    private void ResetLevel()
    {
        this.ball.ResetBall();
        this.paddle.ResetPaddle();

        //for (int i = 0; i < this.bricks.Length; i++)
        //{
        //    this.bricks[i].ResetBrick();
        //}
    }

    private void GameOver()
    {
        //SceneManager.loadScene("GameOver");
        NewGame();
    }



    public void Miss()
    {
        this.lives--;

        if(this.lives < 0 )
        {
            ResetLevel();
        }
        else
        {
            GameOver();
        }
    }

    public void Hit(Brick brick)
    {
        this.score += brick.points;

        if (Cleared())
        {
            LoadLevel(this.level + 1);
        }
    }

    private bool Cleared()
    {
        for (int i = 0; i < this.bricks.Length; i++)
        {
            if (this.bricks[i].gameObject.activeInHierarchy && !this.bricks[i].unbreakable)
            {
                return false; 
            }
        }
        return true;
    }
    public void AddLife()
    {
        lives++;
        // Tu pourrais ajouter ici un effet visuel ou sonore
    }
}
