using UnityEngine;

public class SinkKill : MonoBehaviour
{
    public delegate void BoatDeath();
    public event BoatDeath BoatDied;
    public static SinkKill controller;

    void Awake()
    {
        controller = this;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            BoatDied();
        }
    }
}