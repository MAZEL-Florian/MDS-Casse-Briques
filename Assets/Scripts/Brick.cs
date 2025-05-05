using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Brick : MonoBehaviour
{
    public SpriteRenderer spriteRenderer { get; private set; }

    public Sprite[] states;

    public int health { get; private set; }

    public int points = 100;

    public bool unbreakable;

    public GameObject powerUpPrefab;
    [Range(0f, 1f)]
    public float powerUpChance = 0.05f;

    [Header("Sound")]
    public AudioClip hitSound;  // assigné dans l’inspector

    private void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        ResetBrick();
    }

    public void ResetBrick()
    {
        this.gameObject.SetActive(true);
        if (!this.unbreakable)
        {
            this.health = this.states.Length;
            this.spriteRenderer.sprite = this.states[this.health - 1];
        }
    }
    private void Hit(Ball ball)
    {
        if (hitSound != null && ball != null)
        {
            ball.PlayHitSound(hitSound);
        }

        if (this.unbreakable)
        {
            return;
        }

        this.health--;

        if (this.health <= 0)
        {
            this.gameObject.SetActive(false);
            SpawnPowerUp();
        }
        else
        {
            this.spriteRenderer.sprite = this.states[this.health - 1];
        }

        FindFirstObjectByType<GameManager>().Hit(this);
    }


    private void SpawnPowerUp()
    {
        if (powerUpPrefab != null && Random.value <= powerUpChance)
        {
            GameObject powerUpObj = Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
            PowerUp powerUp = powerUpObj.GetComponent<PowerUp>();

            if (powerUp != null)
            {
                PowerUpType[] allTypes = (PowerUpType[])System.Enum.GetValues(typeof(PowerUpType));
                powerUp.type = allTypes[Random.Range(0, allTypes.Length)];
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Ball ball = collision.gameObject.GetComponent<Ball>();
        if (ball != null)
        {
            Hit(ball);
        }
    }

}
