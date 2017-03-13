using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    GameObject coinController;
    GameObject backgroundController;
    float speed;

    // Use this for initialization
    void Start()
    {
        backgroundController = GameObject.Find("Background Controller");
        coinController = GameObject.Find("CoinController");
    }

    // Update is called once per frame
    void Update()
    {
        Move(backgroundController.GetComponent<BackgroundConroller>().getSpeed());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            coinController.GetComponent<CoinController>().addCoin();
            Destroy(gameObject);
        }
    }
    void Move(float speed)
    {
        if (speed== 0)
        {
            return;
        }
        else if (speed > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, transform.position - Vector3.right * speed * Time.deltaTime, speed);
        }
        else {
            //negative case must be handled slightly differently so that the direction is simply reversed
            transform.position = Vector2.MoveTowards(transform.position, transform.position - Vector3.right * speed * Time.deltaTime, 0 - speed);
        }
    }
}