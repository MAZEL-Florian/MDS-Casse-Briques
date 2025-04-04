using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Paddle : MonoBehaviour
{
    public new Rigidbody2D rigidbody { get; private set; }

    public Vector2 direction { get; private set; }

    public float speed = 30f;

    public float maxBounceAngle = 75f;
    private Animator animator;
    private Vector3 originalScale;
    private void Awake()
    {
        this.rigidbody = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
        this.originalScale = transform.localScale;

    }

    public void ResetPaddle()
    {
        this.transform.position = new Vector2(0f, this.transform.position.y);
        this.rigidbody.linearVelocity = Vector2.zero;
        transform.localScale = originalScale;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            this.direction = Vector2.left;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            this.direction = Vector2.right;
        }
        else
        {
            this.direction = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        if (this.direction != Vector2.zero)
        {
            this.rigidbody.AddForce(this.direction * this.speed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Ball ball = collision.gameObject.GetComponent<Ball>();

        if (ball != null)
        {
            Vector3 paddlePosition = this.transform.position;
            Vector2 contactPoint = collision.GetContact(0).point;

            float offset = paddlePosition.x - contactPoint.x;
            float width = collision.otherCollider.bounds.size.x / 2;

            float currentAngle = Vector2.SignedAngle(Vector2.up, ball.rigidbody.linearVelocity);
            float bounceAngle = (offset / width) * this.maxBounceAngle;
            float newAngle = Mathf.Clamp(currentAngle + bounceAngle, -this.maxBounceAngle, this.maxBounceAngle);

            Quaternion rotation = Quaternion.AngleAxis(newAngle, Vector3.forward);
            ball.rigidbody.linearVelocity = rotation * Vector2.up * ball.constantSpeed;

            if (this.animator != null)
            {
                this.animator.SetTrigger("Hit");
            }
        }
    }
    public IEnumerator ResizePaddle(float scaleFactor, float duration)
    {
        // Appliquer le changement de taille
        Vector3 newScale = originalScale;
        newScale.x *= scaleFactor;
        transform.localScale = newScale;

        // Attendre la durée du power-up
        yield return new WaitForSeconds(duration);

        // Revenir à la taille normale
        transform.localScale = originalScale;
    }
}