﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {
	public float speed;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 forward = new Vector3 (transform.position.x + 1, transform.position.y + Mathf.Sin (Time.time / .2f) );
		transform.position = Vector2.MoveTowards(transform.position, forward, speed * Time.deltaTime);
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.CompareTag ("Player")) {
			other.gameObject.GetComponent<Boat> ().TakeDamage ();
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer ("Water")) {
			Destroy (gameObject);
		}
	}
}
