using System.Collections;
using UnityEngine;

public class TrackingMissile : MonoBehaviour
{
    public float speed;
    public GameObject particles;

    bool dead;

    [SerializeField]
    PointEffector2D effector;

    [SerializeField]
    CircleCollider2D explosionCollider;
    Vector3 dirVector;

    LineRenderer lineRend;

    // Use this for initialization
    void Start()
    {
        Init();
        StartCoroutine(TakeShot());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator TakeShot()
    {
        Vector3 endSpot = Vector3.zero;
        if (Boat.player.CheckIfAlive())
        {
            dirVector = Boat.player.transform.position - transform.position;
            endSpot = Boat.player.transform.position;
        }
        else
        {
            dirVector = new Vector3(Random.Range(-5, 5), -10, 10) - transform.position;
        }

        //http://answers.unity3d.com/questions/654222/make-sprite-look-at-vector2-in-unity-2d-1.html
        float angle = Mathf.Atan2(dirVector.y, dirVector.x) * Mathf.Rad2Deg - 180;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        while (true)
        {
            Vector3 vec = Vector3.MoveTowards(transform.position, transform.position + dirVector, speed * Time.deltaTime);
            vec.z = -10;
            transform.position = vec;
            lineRend.SetPosition(0, transform.position);
            lineRend.SetPosition(1, endSpot);
            yield return null;
        }
    }

    #region Collisions
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HitPlayer(other.gameObject);
            Debug.Log("Tracking missile hit player");
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            Explode();
        }

        if (other.gameObject.CompareTag("Enemy Boat"))
        {
            Explode();
            other.gameObject.GetComponent<EnemyBoat>().DoDamage();
            other.gameObject.GetComponent<Rigidbody2D>().AddForce(dirVector * 10, ForceMode2D.Impulse);
        }

        if (other.gameObject.CompareTag("Missile") || other.gameObject.CompareTag("Cloud Enemy"))
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
        if (other.gameObject.CompareTag("Player"))
        {
            HitPlayer(other.gameObject);
            Debug.Log("Tracking missile hit player");
        }

        if (other.gameObject.CompareTag("Pillar") || other.gameObject.CompareTag("Platform"))
        {
            Destroy(other.transform.parent.gameObject);
            Explode();
        }
    }

    #endregion

    void Init()
    {
        lineRend = GetComponent<LineRenderer>();
        explosionCollider.enabled = false;
        effector.enabled = false;
    }

    void Explode()
    {
        if(dead)
            return;
            
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
        AudioController.controller.PlayMissileSound();
        Instantiate(particles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void GiveSpeedMultiplier(float mult)
    {
        speed += mult;
    }
}
