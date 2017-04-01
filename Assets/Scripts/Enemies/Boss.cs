using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour {

	#region Variables
	public GameObject waveMaker;
	[SerializeField]
	int health;

	#endregion

	void Start () {
		//ArtificialWave();
	}
	

	void Update () {
		
	}

	void ArtificialWave()
	{
		Destroy(Instantiate(waveMaker, Camera.main.ViewportToWorldPoint(new Vector2(1, 0.5f)), Quaternion.identity), 10f);
	}

	public void TakeDamage()
	{
		health--;
		if(health <= 0)
		{
			Die();
		}
	}

	void Die()
	{
		GetComponent<Rigidbody2D>().gravityScale = 1;
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			TakeDamage();
		}
	}
}
