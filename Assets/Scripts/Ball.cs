using UnityEngine;

public class Ball : MonoBehaviour
{
    public new Rigidbody2D rigidbody { get; private set; }
    public bool isLaunched { get; private set; } = false;
    private Paddle paddle;
    public float speed = 400f;
    public float constantSpeed = 10f;

    private void Awake()
    {
        this.rigidbody = GetComponent<Rigidbody2D>();
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
}