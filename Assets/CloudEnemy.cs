using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudEnemy : MonoBehaviour
{

    #region Variables
    int health;
    bool dead;
    public GameObject bombPrefab;
    public bool FollowPlayer;
    public float speed;
    float[] endPoints = new float[2];
    int currentPoint = 0;
    public float smoothTime = 0.3f;
    Vector2 velocity = Vector3.zero;

    public float bombFrequency;
    #endregion

    void Start()
    {
        Invoke("BombThrow", bombFrequency);
        speed = 5;
        health = 1;
        endPoints[0] = Camera.main.ViewportToWorldPoint(new Vector2(.05f, 1)).x;
        endPoints[1] = Camera.main.ViewportToWorldPoint(new Vector2(.95f, 1)).x;
    }


    void Update()
    {
        Movement();
    }

    void Movement()
    {
        if (dead)
            return;

        if (!FollowPlayer)
        {
            if (Mathf.Abs(transform.position.x - endPoints[currentPoint % 2]) > .05f)
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(endPoints[currentPoint % 2], transform.position.y), speed * Time.deltaTime);
            else currentPoint++;
        }
        else
        {
            transform.position = Vector2.SmoothDamp(transform.position, new Vector2(Boat.player.transform.position.x, transform.position.y), ref velocity, smoothTime, speed, Time.deltaTime);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Missile"))
        {
            TakeDamage();
        }
    }

    void TakeDamage()
    {
        health--;
        if (health <= 0)
            Die();
    }

    void Die()
    {
		gameObject.layer = LayerMask.NameToLayer("IgnoreWater");
        GetComponent<Rigidbody2D>().gravityScale = 1;
		GetComponent<CircleCollider2D>().isTrigger = true;
        dead = true;
    }

    void BombThrow()
    {
		if(dead)
			return;
        Instantiate(bombPrefab, transform.position - Vector3.up, Quaternion.identity);

        Invoke("BombThrow", bombFrequency);
    }
}
