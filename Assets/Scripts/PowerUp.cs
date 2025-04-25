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
    public Sprite[] powerUpSprites;

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
        if (powerUpSprites.Length > (int)type)
        {
            spriteRenderer.sprite = powerUpSprites[(int)type];
        }
    }
    void Update()
    {
        Vector3 newPosition = transform.position + Vector3.down * speed * Time.deltaTime;
        RaycastHit2D hit = Physics2D.Linecast(transform.position, newPosition);
        if (hit.collider != null)
        {
            Debug.Log("LineCheck détecte : " + hit.collider.gameObject.name);
            if (hit.collider.CompareTag("Paddle"))
            {
                Debug.Log("LineCheck a trouvé le paddle!");
                ApplyPowerUp();
                PlayPowerUpSound(hit.collider.gameObject);
                Destroy(gameObject);
                return;
            }
        }
        transform.position = newPosition;
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger détecté avec : " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Paddle"))
        {
            Debug.Log("Collision avec le paddle détectée!");
            ApplyPowerUp();
            PlayPowerUpSound(collision.gameObject);
            Destroy(gameObject);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision détectée avec : " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Paddle"))
        {
            Debug.Log("Collision avec le paddle détectée!");
            ApplyPowerUp();
            PlayPowerUpSound(collision.gameObject);
            Destroy(gameObject);
        }
    }

    void PlayPowerUpSound(GameObject paddle)
    {
        if (powerupSound != null)
        {
            AudioSource audioSource = paddle.GetComponent<AudioSource>();

            if (audioSource == null)
            {
                AudioSource.PlayClipAtPoint(powerupSound, paddle.transform.position);
            }
            else
            {
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