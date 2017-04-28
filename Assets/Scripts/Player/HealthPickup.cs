using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    static float startingSpeed = 7;
    static float speed;
    void Update()
    {
        Vector3 forward = new Vector3(transform.position.x + -speed, transform.position.y + Mathf.Sin(Time.time / .4f), 10);
        transform.position = Vector3.MoveTowards(transform.position, forward, speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Boat>().AddHealth();
           // Destroy(gameObject);
           gameObject.SetActive(false);
        }
    }

    public static void GiveSpeedMultiplier(float mult)
    {
        speed = startingSpeed + mult;
    }
}
