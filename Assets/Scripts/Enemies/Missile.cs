
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField]
    protected static float startingSpeed = 8;
    protected static float speed;

    public GameObject particles;
    [SerializeField]
    protected CircleCollider2D explosionCollider;
    [SerializeField]
    protected PointEffector2D effector;

    protected bool dead;

    protected void Start()
    {
        speed = startingSpeed;
    }

    protected virtual void Update()
    {
        if (dead)
            return;

        Vector3 forward = new Vector3(transform.position.x + -speed, transform.position.y + Mathf.Sin(Time.time / .3f));
        forward.z = 10;
        transform.position = Vector3.MoveTowards(transform.position, forward, speed * Time.deltaTime);

        Quaternion rotation = Quaternion.LookRotation(forward - transform.position, transform.up);
        transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);

    }

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (dead)
            return;

		switch (other.gameObject.tag) {
		case "Player":
			HitPlayer (other.gameObject);
			break;
		case "Enemy Boat":
			Explode ();
			other.gameObject.GetComponent<EnemyBoat> ().TakeDamage ();
			other.gameObject.GetComponent<Rigidbody2D> ().AddForce (Vector2.left * 10, ForceMode2D.Impulse);
			break;
		case "Boss":
			Explode ();
			other.gameObject.GetComponent<Boss> ().TakeDamage ();
			break;
		case "Rock":
			Explode ();
			break;
		default:
                //Explode();
			break;
		}

        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            Explode();
        }

    }

    protected void OnTriggerEnter2D(Collider2D other)
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

    protected virtual void OnEnable()
    {
        Init();
        GiveSpeedMultiplier(MillileSpawner.controller.GetSpeedMultiplier());
        GetComponent<SpriteRenderer>().enabled = true;
    }

    protected virtual void OnDisable()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		if (gameObject.layer == LayerMask.NameToLayer ("Default"))
			MillileSpawner.controller.EnqueueMissile (gameObject);
		else
			MillileSpawner.controller.EnqueueTorpedo (gameObject);
    }


	public virtual void Explode()
    {
        if (dead)
            return;

        if (Boat.player.CheckIfAlive())
            TempGoalController.controller.MissileDestryoed();

        dead = true;
        explosionCollider.enabled = true;
        effector.enabled = true;
        AudioController.controller.PlayMissileSound();
        GameObject part = Instantiate(particles, transform.position, Quaternion.identity) as GameObject;
        part.transform.SetParent(MainCanvas.controller.particles);
        GetComponent<SpriteRenderer>().enabled = false;
        Invoke("TurnOffObject", .15f);
    }

    protected void TurnOffObject()
    {
        gameObject.SetActive(false);
    }

    protected void HitPlayer(GameObject o)
    {
        o.GetComponent<Boat>().TakeMissileDamage();
        Explode();
    }

    protected void Init()
    {
        dead = false;
        explosionCollider.enabled = false;
        effector.enabled = false;
    }

    public static void GiveSpeedMultiplier(float mult)
    {
        speed = startingSpeed + mult;
    }
}
