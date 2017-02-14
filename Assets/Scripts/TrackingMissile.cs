using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingMissile : MonoBehaviour {
	public float speed;
	public GameObject particles;

	// Use this for initialization
	void Start () {
		StartCoroutine (TakeShot ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator TakeShot()
	{
		Vector3 unitVector = (Boat.player.transform.position - transform.position).normalized;

		while (true) {
			transform.position = Vector3.MoveTowards (transform.position, transform.position + unitVector, speed * Time.deltaTime);
			yield return null;
		}
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.CompareTag ("Player")) {
			HitPlayer (other.gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag ("Player")) {
			HitPlayer (other.gameObject);
		}
	}

	void HitPlayer(GameObject o)
	{
		o.GetComponent<Boat> ().TakeMissileDamage ();
		AudioController.controller.PlayMissileSound ();
		Instantiate (particles,transform.position, Quaternion.identity);
		Destroy (gameObject);
	}
}
