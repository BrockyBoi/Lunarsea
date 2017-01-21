using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour {
	public float speed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector2.MoveTowards (transform.position, new Vector2(transform.position.x - speed, transform.position.y + Mathf.Sin (0) * Mathf.Deg2Rad), speed);
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.CompareTag ("Player")) {
			other.gameObject.GetComponent<Boat> ().TakeDamage ();
		}
	}
}
