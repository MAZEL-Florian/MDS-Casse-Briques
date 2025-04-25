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

    private void Awake()
    {
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
        Vector2 force = Vector2.zero;
        force.x = Random.Range(-1f, 1f);
        force.y = -1f;
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
        SetPassThrough(false, 0);
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
                this.rigidbody.linearVelocity = this.rigidbody.linearVelocity.normalized * constantSpeed;
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
        passThrough = enabled;

        if (enabled)
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Bricks"), true);
        }

        if (duration > 0)
        {
            yield return new WaitForSeconds(duration);

            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Bricks"), false);
            passThrough = false;
        }
    }

    public IEnumerator ModifySpeed(float speedFactor, float duration)
    {
        constantSpeed = originalConstantSpeed * speedFactor;

        yield return new WaitForSeconds(duration);

        constantSpeed = originalConstantSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (passThrough && collision.gameObject.CompareTag("Brick"))
        {
            Brick brick = collision.gameObject.GetComponent<Brick>();
            if (brick != null)
            {
                System.Reflection.MethodInfo hitMethod = brick.GetType().GetMethod("Hit",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (hitMethod != null)
                {
                    hitMethod.Invoke(brick, null);
                }
            }
        }

        if (collision.gameObject.CompareTag("Paddle") && bounceSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(bounceSound);
        }
    }

}