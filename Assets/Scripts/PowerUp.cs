using UnityEngine;
using System.Collections;
public enum PowerUpType
{
    // Bonus
    ExpandPaddle,
    ExpandBall,
    BallPassThrough,
    SlowBall,
    ExtraLife,
    // Malus
    ShrinkPaddle,
    ShrinkBall,
    SpeedUpBall
}
public class PowerUp : MonoBehaviour
{
    public PowerUpType type;
    public float speed = 5f;
    public float duration = 10f;
    public Sprite[] powerUpSprites; // Assigne les sprites dans l'�diteur Unity

    // Ajouter cette variable pour le son
    public AudioClip powerupSound;

    private SpriteRenderer spriteRenderer;
    private GameManager gameManager;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = FindObjectOfType<GameManager>();
    }
    void Start()
    {
        // D�finir le sprite en fonction du type de powerup
        if (powerUpSprites.Length > (int)type)
        {
            spriteRenderer.sprite = powerUpSprites[(int)type];
        }
    }
    void Update()
    {
        // D�placement vers le bas
        Vector3 newPosition = transform.position + Vector3.down * speed * Time.deltaTime;
        // V�rification optionnelle si on va toucher le paddle
        RaycastHit2D hit = Physics2D.Linecast(transform.position, newPosition);
        if (hit.collider != null)
        {
            Debug.Log("LineCheck d�tecte : " + hit.collider.gameObject.name);
            if (hit.collider.CompareTag("Paddle"))
            {
                Debug.Log("LineCheck a trouv� le paddle!");
                ApplyPowerUp();
                PlayPowerUpSound(hit.collider.gameObject);
                Destroy(gameObject);
                return;
            }
        }
        transform.position = newPosition;
        // Destruction si sort de l'�cran
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger d�tect� avec : " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Paddle"))
        {
            Debug.Log("Collision avec le paddle d�tect�e!");
            // Activer l'effet du powerup
            ApplyPowerUp();
            PlayPowerUpSound(collision.gameObject);
            Destroy(gameObject);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision d�tect�e avec : " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Paddle"))
        {
            Debug.Log("Collision avec le paddle d�tect�e!");
            // Activer l'effet du powerup
            ApplyPowerUp();
            PlayPowerUpSound(collision.gameObject);
            Destroy(gameObject);
        }
    }

    void PlayPowerUpSound(GameObject paddle)
    {
        if (powerupSound != null)
        {
            // V�rifier si le paddle a un AudioSource
            AudioSource audioSource = paddle.GetComponent<AudioSource>();

            // Si le paddle n'a pas d'AudioSource, on joue le son via AudioSource.PlayClipAtPoint
            if (audioSource == null)
            {
                AudioSource.PlayClipAtPoint(powerupSound, paddle.transform.position);
            }
            else
            {
                // Si le paddle a d�j� un AudioSource, on l'utilise
                audioSource.PlayOneShot(powerupSound);
            }
        }
    }

    void ApplyPowerUp()
    {
        switch (type)
        {
            case PowerUpType.ExpandPaddle:
                StartCoroutine(gameManager.paddle.ResizePaddle(1.5f, duration));
                break;
            case PowerUpType.ExpandBall:
                StartCoroutine(gameManager.ball.ResizeBall(1.5f, duration));
                break;
            case PowerUpType.BallPassThrough:
                StartCoroutine(gameManager.ball.SetPassThrough(true, duration));
                break;
            case PowerUpType.SlowBall:
                StartCoroutine(gameManager.ball.ModifySpeed(0.7f, duration));
                break;
            case PowerUpType.ExtraLife:
                gameManager.AddLife();
                break;
            case PowerUpType.ShrinkPaddle:
                StartCoroutine(gameManager.paddle.ResizePaddle(0.5f, duration));
                break;
            case PowerUpType.ShrinkBall:
                StartCoroutine(gameManager.ball.ResizeBall(0.5f, duration));
                break;
            case PowerUpType.SpeedUpBall:
                StartCoroutine(gameManager.ball.ModifySpeed(1.5f, duration));
                break;
        }
    }
}