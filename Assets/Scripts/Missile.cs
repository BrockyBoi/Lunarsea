
using UnityEngine;

public class Missile : MonoBehaviour
{
    public float speed;
    float variable;

    public GameObject particles;
    [SerializeField]
    CircleCollider2D explosionCollider;
    [SerializeField]
    PointEffector2D effector;

    public bool IgnoreWater;
    bool dead;

    // Use this for initialization
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)
            return;

        Vector3 forward = new Vector3(transform.position.x + -speed, transform.position.y + Mathf.Sin(Time.time / .3f));
        forward.z = 10;
        transform.position = Vector3.MoveTowards(transform.position, forward, speed * Time.deltaTime);

        Quaternion rotation = Quaternion.LookRotation(forward - transform.position, transform.up);
        transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (dead)
            return;

        if (other.gameObject.CompareTag("Player"))
        {
            HitPlayer(other.gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            Explode();
        }

        if (other.gameObject.CompareTag("Enemy Boat"))
        {
            Explode();
            other.gameObject.GetComponent<EnemyBoat>().DoDamage();
            other.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 10, ForceMode2D.Impulse);
        }

        if (other.gameObject.CompareTag("Missile") || other.gameObject.CompareTag("Cloud Enemy") || other.gameObject.CompareTag("Rock"))
        {
            Explode();
        }

        if (other.gameObject.CompareTag("Boss"))
        {
            Explode();
            other.gameObject.GetComponent<Boss>().TakeDamage();
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!dead && other.gameObject.CompareTag("Player"))
        {
            HitPlayer(other.gameObject);
        }

        if (other.gameObject.CompareTag("Pillar") || other.gameObject.CompareTag("Platform"))
        {
            Destroy(other.transform.parent.gameObject);
            Explode();
        }
    }

    void Explode()
    {
        if (dead)
            return;

        if (Boat.player.CheckIfAlive())
            TempGoalController.controller.MissileDestryoed();
        dead = true;
        explosionCollider.enabled = true;
        effector.enabled = true;
        AudioController.controller.PlayMissileSound();
        Instantiate(particles, transform.position, Quaternion.identity);
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject, .1f);
    }

    void HitPlayer(GameObject o)
    {
        o.GetComponent<Boat>().TakeMissileDamage();
        Explode();
    }

    void Init()
    {
        explosionCollider.enabled = false;
        effector.enabled = false;
    }

    public void GiveSpeedMultiplier(float mult)
    {
        speed += mult;
    }
}
