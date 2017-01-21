using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour {
	public float speed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 forward = new Vector3 (transform.position.x + -speed, transform.position.y + Mathf.Sin (Time.time / .4f) );
		transform.position = Vector2.MoveTowards(transform.position, forward, speed * Time.deltaTime);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag ("Player")) {
			other.gameObject.GetComponent<Boat> ().AddHealth ();
			Destroy (gameObject);
		}
	}
}
