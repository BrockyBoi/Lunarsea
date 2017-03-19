﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public GameObject particleEffect;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Move(BackgroundConroller.controller.getSpeed() * 2);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CoinController.controller.addCoin();
            Destroy(Instantiate(particleEffect, transform.position, Quaternion.identity), 1);
            Destroy(gameObject);
        }
    }
    void Move(float speed)
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
}