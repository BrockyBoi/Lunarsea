using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour {
	public float speed;
	bool onTimeDown = false;
	float mTimer = 0;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		transform.position = Vector2.MoveTowards (transform.position, new Vector2(transform.position.x - speed, transform.position.y + Mathf.Sin (0) * Mathf.Deg2Rad), speed);

		if (onTimeDown)
		{
			mTimer += Time.deltaTime;
			if (mTimer >= 1)
			{
				onTimeDown = false;
			}
		}
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.CompareTag ("Player") && !onTimeDown) {
			onTimeDown = true;
			other.gameObject.GetComponent<Boat> ().TakeDamage ();
			AudioController.controller.BoatHitsRock ();
		}
	}
}