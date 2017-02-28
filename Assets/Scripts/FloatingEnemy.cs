using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingEnemy : MonoBehaviour
{

    #region Variables

    public bool throwsProjectile;

    public float speed;
    bool hitPlayer;

    public GameObject projectile;

    public float force;
    public float spawnRate;
    #endregion

    void Start()
    {
        if (throwsProjectile)
            SpawnProjectiles();
    }


    void Update()
    {
        MoveParentObject();
    }

    void SpawnProjectiles()
    {
        GameObject proj = Instantiate(projectile, transform.position, Quaternion.identity) as GameObject;
        Vector2 dir = new Vector2(-.25f, Mathf.Sin(Random.Range(0.05f, 0.35f)));
        proj.GetComponent<Rigidbody2D>().AddForce(dir * force);

        Invoke("SpawnProjectiles", spawnRate);

    }

    void MoveParentObject()
    {
        Vector3 forward = new Vector3(transform.parent.position.x + -speed, transform.position.y);
        transform.parent.position = Vector2.MoveTowards(transform.parent.position, forward, speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
