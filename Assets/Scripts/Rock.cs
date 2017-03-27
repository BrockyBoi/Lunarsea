﻿using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField]
    float startingSpeed;
    float speed;
    bool hitPlayer;

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x - speed * Time.deltaTime, transform.position.y + Mathf.Sin(0) * Mathf.Deg2Rad), speed);
    }

    void OnEnable()
    {
        GiveSpeedMultiplier(MillileSpawner.controller.GetSpeedMultiplier());
    }


    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && !hitPlayer)
        {
            other.gameObject.GetComponent<Boat>().TakeDamage();
            AudioController.controller.BoatHitsRock();
            hitPlayer = true;
        }

        if (other.gameObject.CompareTag("Enemy Boat"))
        {
            other.gameObject.GetComponent<EnemyBoat>().DoDamage();
            AudioController.controller.BoatHitsRock();
        }
    }

    public void StopObject()
    {
        speed = 0;
    }

    public void GiveSpeedMultiplier(float mult)
    {
        speed = startingSpeed + mult;
    }
}