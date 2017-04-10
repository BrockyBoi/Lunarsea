using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public GameObject particleEffect;
    bool followPlayer;

    void Update()
    {
        Move(BackgroundConroller.controller.getSpeed() * 4.5f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CoinController.controller.addCoin();
            Destroy(Instantiate(particleEffect, transform.position, Quaternion.identity), 1);
            gameObject.SetActive(false);
        }
        
        if(other.gameObject.CompareTag("Magnet"))
        {
            FollowPlayer();
        }
    }

    void OnDisable()
    {
        followPlayer = false;
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
        else if(followPlayer)
        {
            transform.position = Vector3.MoveTowards(transform.position, Boat.player.transform.position, 20 * Time.deltaTime);
        }
    }

    void FollowPlayer()
    {
        followPlayer = true;
    }
}