using UnityEngine;

public class Rock : MonoBehaviour
{
    public float speed;
    bool hitPlayer;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x - speed * Time.deltaTime, transform.position.y + Mathf.Sin(0) * Mathf.Deg2Rad), speed);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && !hitPlayer)
        {
            other.gameObject.GetComponent<Boat>().TakeDamage();
            AudioController.controller.BoatHitsRock();
            hitPlayer = true;
            Debug.Log("Rock hit boat");
        }

        if(other.gameObject.CompareTag("Enemy Boat"))
        {
            other.gameObject.GetComponent<EnemyBoat>().DoDamage();
            AudioController.controller.BoatHitsRock();
        }
    }

    public void GiveSpeedMultiplier(float mult)
    {
        speed += mult;
    }
}