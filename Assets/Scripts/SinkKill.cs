using UnityEngine;

public class SinkKill : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Boat.player.Die();
        }
    }
}