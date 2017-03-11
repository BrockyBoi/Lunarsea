using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoat : MonoBehaviour
{

    #region Variables
    int health;
    bool dead;
    public bool throwsProjectile;
    public GameObject projectile;
    [SerializeField]
    List<Collider2D> colliders;

    public float force;
    public float spawnRate;
    #endregion

    void Start()
    {
        health = 1;
        if (throwsProjectile)
            SpawnProjectiles();
    }

    void SpawnProjectiles()
    {
        GameObject proj = Instantiate(projectile, transform.position, Quaternion.identity) as GameObject;
        Vector2 dir = new Vector2(-.25f, Mathf.Sin(Random.Range(0.05f, 0.35f)));
        proj.GetComponent<Rigidbody2D>().AddForce(dir.normalized * force);

        if (!dead)
            Invoke("SpawnProjectiles", spawnRate);
    }


    public void DoDamage()
    {
        health--;
        if (!dead && health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        dead = true;
        AudioController.controller.BoatDeath();
        gameObject.layer = LayerMask.NameToLayer("IgnoreWater");
    }
}
