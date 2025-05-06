using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{
    public new Rigidbody2D rigidbody { get; private set; }
    public bool isLaunched { get; private set; } = false;
    public AudioClip bounceSound;
    private AudioSource audioSource;

    private Paddle paddle;
    public float speed = 400f;
    public float constantSpeed = 10f;

    private Vector3 originalScale;
    private bool passThrough = false;
    private float originalConstantSpeed;
    private CircleCollider2D ballCollider;

    private Color originalColor;
    public Color passThroughColor = Color.red;
    private SpriteRenderer spriteRenderer;

    private Coroutine passThroughCoroutine;

    private void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.originalColor = spriteRenderer.color;
        this.audioSource = GetComponent<AudioSource>();
        this.rigidbody = GetComponent<Rigidbody2D>();
        this.ballCollider = GetComponent<CircleCollider2D>();
        this.originalScale = transform.localScale;
        this.originalConstantSpeed = constantSpeed;
    }

    private void Start()
    {
        ResetBall();
    }

    private void SetRandomTrajectory()
    {
        Vector2 force = new Vector2(Random.Range(-1f, 1f), -1f);
        this.rigidbody.AddForce(force.normalized * this.speed);
    }

    public void ResetBall()
    {
        this.transform.position = Vector2.zero;
        this.rigidbody.linearVelocity = Vector2.zero;
        this.isLaunched = false;
        this.paddle = FindObjectOfType<Paddle>();

        transform.localScale = originalScale;
        constantSpeed = originalConstantSpeed;

        if (passThroughCoroutine != null)
        {
            StopCoroutine(passThroughCoroutine);
            passThroughCoroutine = null;
        }
        passThrough = false;
        spriteRenderer.color = originalColor;
    }

    private void Update()
    {
        if (!isLaunched && paddle != null)
        {
            Vector3 paddlePosition = paddle.transform.position;
            this.transform.position = new Vector3(paddlePosition.x, paddlePosition.y + 0.5f, 0f);
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                LaunchBall();
            }
        }
        else if (isLaunched)
        {
            if (this.rigidbody.linearVelocity.magnitude != 0)
            {
                Vector2 velocity = this.rigidbody.linearVelocity.normalized;

                if (Mathf.Abs(velocity.x) < 0.1f)
                {
                    velocity.x = 0.1f * Mathf.Sign(velocity.x == 0 ? Random.Range(-1f, 1f) : velocity.x);
                    velocity = velocity.normalized;
                }
                if (Mathf.Abs(velocity.y) < 0.1f)
                {
                    velocity.y = 0.1f * Mathf.Sign(velocity.y == 0 ? Random.Range(-1f, 1f) : velocity.y);
                    velocity = velocity.normalized;
                }

                this.rigidbody.linearVelocity = velocity * constantSpeed;
            }
        }
    }

    private void LaunchBall()
    {
        isLaunched = true;
        Vector2 force = new Vector2(Random.Range(-1f, 1f), 1f);
        this.rigidbody.linearVelocity = force.normalized * constantSpeed;
    }

    public IEnumerator ResizeBall(float scaleFactor, float duration)
    {
        Vector3 newScale = originalScale;
        newScale.x *= scaleFactor;
        newScale.y *= scaleFactor;
        transform.localScale = newScale;

        yield return new WaitForSeconds(duration);

        transform.localScale = originalScale;
    }

    public IEnumerator SetPassThrough(bool enabled, float duration)
    {
        if (enabled)
        {
            if (passThroughCoroutine != null)
            {
                StopCoroutine(passThroughCoroutine);
            }
            passThroughCoroutine = StartCoroutine(PassThroughRoutine(duration));
        }
        else
        {
            if (passThroughCoroutine != null)
            {
                StopCoroutine(passThroughCoroutine);
                passThroughCoroutine = null;
            }
            passThrough = false;
            spriteRenderer.color = originalColor;
        }

        yield break;
    }

    private IEnumerator PassThroughRoutine(float duration)
    {
        passThrough = true;
        spriteRenderer.color = passThroughColor;

        yield return new WaitForSeconds(duration);

        passThrough = false;
        spriteRenderer.color = originalColor;
        passThroughCoroutine = null;
    }

    public IEnumerator ModifySpeed(float speedFactor, float duration)
    {
        constantSpeed = originalConstantSpeed * speedFactor;

        yield return new WaitForSeconds(duration);

        constantSpeed = originalConstantSpeed;
    }

    public void PlayHitSound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void StopBall()
    {
        this.rigidbody.linearVelocity = Vector2.zero;
        isLaunched = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Brick"))
        {
            Brick brick = collision.gameObject.GetComponent<Brick>();
            if (brick != null)
            {
                if (passThrough)
                {
                    while (brick.health > 0)
                    {
                        brick.SendMessage("Hit", this, SendMessageOptions.DontRequireReceiver);
                    }

                    rigidbody.linearVelocity = rigidbody.linearVelocity.normalized * constantSpeed;
                }
                else
                {
                    brick.SendMessage("Hit", this, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
        else if (collision.gameObject.CompareTag("Paddle") && bounceSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(bounceSound);
        }
    }
}
