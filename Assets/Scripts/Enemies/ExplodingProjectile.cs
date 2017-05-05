using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingProjectile : MonoBehaviour
{

	#region Variables

	public GameObject particles;
	[SerializeField]
	CircleCollider2D explosionCollider;
	[SerializeField]
	PointEffector2D effector;

	new 	AudioSource audio;
	public AudioClip tickBoom;

	bool exploded;

	#endregion

	void Start ()
	{
		audio = gameObject.AddComponent<AudioSource> ();
		audio.clip = tickBoom;
		audio.Play ();
		audio.loop = false;

		Invoke ("Explode", 3);
	}

	void Explode ()
	{
		exploded = true;
		explosionCollider.enabled = true;
		effector.enabled = true;
		AudioController.controller.PlayMissileSound ();
		Instantiate (particles, transform.position, Quaternion.identity);
		GetComponent<SpriteRenderer> ().enabled = false;
		Destroy (gameObject, .15f);
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (exploded && other.gameObject.CompareTag ("Player")) {
			Boat.player.TakeDamage ();

			Vector3 vec = Boat.player.transform.position - transform.position;
			other.gameObject.GetComponent<Rigidbody2D> ().AddForce (vec.normalized * 75, ForceMode2D.Impulse);
		}

		if (exploded && other.gameObject.CompareTag ("Enemy Boat")) {
			other.gameObject.GetComponent<EnemyBoat> ().TakeDamage ();
			Vector3 vec = other.transform.position - transform.position;
			other.gameObject.GetComponent<Rigidbody2D> ().AddForce (vec.normalized * 5, ForceMode2D.Impulse);
		}
	}
}
