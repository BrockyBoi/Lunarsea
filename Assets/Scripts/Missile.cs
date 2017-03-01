using System.Collections;
using System.Collections.Generic;
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
        if (!IgnoreWater)
            Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)
            return;

        Vector3 forward = new Vector3(transform.position.x + -speed, transform.position.y + Mathf.Sin(Time.time / .3f));
        transform.position = Vector2.MoveTowards(transform.position, forward, speed * Time.deltaTime);

        Quaternion rotation = Quaternion.LookRotation(forward - transform.position, transform.up);
        transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HitPlayer(other.gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            Explode();
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!dead && other.gameObject.CompareTag("Player"))
        {
            HitPlayer(other.gameObject);
        }

		if(other.gameObject.CompareTag("Pillar") || other.gameObject.CompareTag("Platform"))
		{
			Destroy(other.gameObject);
			Explode();
		}
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

    void Init()
    {
        explosionCollider.enabled = false;
        effector.enabled = false;
    }
}
