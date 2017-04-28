using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Urdu : MonoBehaviour
{
    public bool side;
    public bool back;

    public static Urdu sideUrdu;
    public delegate void BoatDeath();
    public event BoatDeath BoatDied;

    void Awake()
    {
        if (side)
            sideUrdu = this;
    }
    void Start()
    {

        if (side)
            transform.position = Camera.main.ViewportToWorldPoint(new Vector3(-.2f, .5f));
        else if (back)
        {
            //nothin
        }
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
                    BoatDied();
                break;
            case "Coin":
                INFIDEL.gameObject.SetActive(false);
                break;
            default:
                break;

        }
    }
}