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
        Vector3 unitVector;
        if (Boat.player.CheckIfAlive())
        {
            unitVector = Boat.player.transform.position - transform.position;
        }
        else
        {
            unitVector = new Vector3(Random.Range(-5, 5), -10) - transform.position;
        }
       
        //http://answers.unity3d.com/questions/654222/make-sprite-look-at-vector2-in-unity-2d-1.html
        float angle = Mathf.Atan2(unitVector.y, unitVector.x) * Mathf.Rad2Deg - 180;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        while (true)
        {
            transform.position = Vector2.MoveTowards(transform.position, transform.position + unitVector, speed * Time.deltaTime);
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
        explosionCollider.enabled = false;
        effector.enabled = false;
    }

    void Explode()
    {
        dead = true;
        explosionCollider.enabled = true;
        effector.enabled = true;
        AudioController.controller.PlayMissileSound();
        Instantiate(particles, transform.position, Quaternion.identity);
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject, .15f);
    }


    void HitPlayer(GameObject o)
    {
        o.GetComponent<Boat>().TakeMissileDamage();
        AudioController.controller.PlayMissileSound();
        Instantiate(particles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
