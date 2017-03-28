using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Urdu : MonoBehaviour
{
    public bool side;
    void Start()
    {
        if (side)
            transform.position = Camera.main.ViewportToWorldPoint(new Vector3(-.1f, .5f));
        else transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0, -.5f));
    }
    void OnTriggerEnter2D(Collider2D INFIDEL)
    {
        switch (INFIDEL.tag)
        {
            case "Missile":
                INFIDEL.gameObject.SetActive(false);
                break;
            case "Rock":
                INFIDEL.gameObject.SetActive(false);
                break;
            case "Boat":
                if (!side)
                    Boat.player.Die();
                break;
            case "Coin":
                INFIDEL.gameObject.SetActive(false);
                break;
            default:
                break;

        }
    }
}
