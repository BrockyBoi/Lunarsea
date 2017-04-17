using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{

    #region Variables
    public GameObject waveMaker;
    [SerializeField]
    int health;
    bool dead;
    public GameObject endArmLeft;
    public GameObject endArmRight;

    #endregion

    void Start()
    {
        //ArtificialWave();
       // ShootArmUp(endArmRight);
	   StartCoroutine(MoveArmAround(endArmRight));
    }


    void Update()
    {

    }

    IEnumerator MoveArmAround(GameObject arm)
    {
        Vector3 fwrd = Vector3.right;
        do
        {
			fwrd *= -1;
            float time = 0;
            float maxTime = Random.Range(3, 6f);
            while (time < maxTime)
            {
                arm.transform.position += fwrd * 3 * Time.deltaTime;
				time += Time.deltaTime;
                yield return null;
            }
        } while (!dead);
    }

    void ShootArmUp(GameObject arm)
    {
        arm.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 750);
    }

    void ArtificialWave()
    {
        Destroy(Instantiate(waveMaker, Camera.main.ViewportToWorldPoint(new Vector2(1, 0.5f)), Quaternion.identity), 10f);
    }

    public void TakeDamage()
    {
        health--;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        dead = true;
        GetComponent<Rigidbody2D>().gravityScale = 1;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            TakeDamage();
        }
    }
}
