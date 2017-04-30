﻿
using UnityEngine;

public class Coin : MonoBehaviour
{
    public GameObject particleEffect;
    bool followPlayer;

    [SerializeField]
    static float startingSpeed = 4;
    static float speed;

	AudioSource audio;
	public AudioClip coinPickUp1;
	public AudioClip coinPickUp2;

    void Start()
    {
		audio = gameObject.AddComponent<AudioSource> ();
		speed = startingSpeed;
    }
    void Update()
    {
		Move(speed * 1.25f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ChooseFX();
            CoinController.controller.addCoin();
            Destroy(Instantiate(particleEffect, transform.position, Quaternion.identity), 1);
            gameObject.SetActive(false);
        }

        if (other.gameObject.CompareTag("Magnet"))
        {
            FollowPlayer();
        }
    }

    void ChooseFX()
    {
        int randomNum = Random.Range(0, 2);
        if (randomNum == 0)
			audio.PlayOneShot(coinPickUp1);
		else audio.PlayOneShot(coinPickUp2);
    }

    void OnDisable()
    {
        followPlayer = false;
		CoinController.controller.EnqueueCoin (gameObject);
    }
    void Move(float speed)
    {
        if (!followPlayer)
        {
            if (speed == 0)
            {
                return;
            }
            else if (speed > 0)
            {
                Vector3 vec = Vector3.MoveTowards(transform.position, transform.position - Vector3.right * speed * Time.deltaTime, speed);
                vec.z = 10;
                transform.position = vec;
            }
            else
            {
                //negative case must be handled slightly differently so that the direction is simply reversed
                Vector3 vec = Vector3.MoveTowards(transform.position, transform.position - Vector3.right * speed * Time.deltaTime, 0 - speed);
                vec.z = 10;

                transform.position = vec;
            }
        }
        else if (followPlayer)
        {
            transform.position = Vector3.MoveTowards(transform.position, Boat.player.transform.position, 20 * Time.deltaTime);
        }
    }

    void FollowPlayer()
    {
        followPlayer = true;
    }

    public static void GiveSpeedMultiplier(float speedMult)
    {
        speed = startingSpeed + speedMult;
    }
}