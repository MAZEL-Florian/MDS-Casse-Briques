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
    public Sprite[] powerUpSprites; // Assigne les sprites dans l'éditeur Unity

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
        // Définir le sprite en fonction du type de powerup
        if (powerUpSprites.Length > (int)type)
        {
            spriteRenderer.sprite = powerUpSprites[(int)type];
        }
    }
    void Update()
    {
        // Déplacement vers le bas
        Vector3 newPosition = transform.position + Vector3.down * speed * Time.deltaTime;
        // Vérification optionnelle si on va toucher le paddle
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
        // Destruction si sort de l'écran
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
            // Activer l'effet du powerup
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
            // Vérifier si le paddle a un AudioSource
            AudioSource audioSource = paddle.GetComponent<AudioSource>();

            // Si le paddle n'a pas d'AudioSource, on joue le son via AudioSource.PlayClipAtPoint
            if (audioSource == null)
            {
                AudioSource.PlayClipAtPoint(powerupSound, paddle.transform.position);
            }
            else
            {
                // Si le paddle a déjà un AudioSource, on l'utilise
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