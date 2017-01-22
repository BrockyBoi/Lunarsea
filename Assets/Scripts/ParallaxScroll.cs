using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScroll : MonoBehaviour {

	SpriteRenderer spr;
	Vector3 backPos;
	public float speed = 5;
	void Awake() {
		spr = GetComponent<SpriteRenderer> ();
	}

	void Start() {
		Vector3 backgroundBounds = spr.sprite.bounds.extents;
		Vector2 bottomRight = new Vector3 (Camera.main.orthographicSize*2,Camera.main.orthographicSize*2/Screen.width*Screen.height);
		float xScale = bottomRight.x/backgroundBounds.x;
		float yScale = bottomRight.y/backgroundBounds.y;
		Vector3 scaleVector = new Vector3 (xScale, yScale);
		transform.localScale = scaleVector;
	}

	// Update is called once per frame
	void Update () {
		Vector3 end = Camera.main.ViewportToWorldPoint (new Vector3(0,0.5f));
		end.z = transform.position.z;
		transform.position = Vector3.MoveTowards (transform.position, end, Time.deltaTime * speed);
		Vector3 spritebounds = Camera.main.ScreenToWorldPoint (spr.sprite.bounds.extents);
		if (transform.position.x + (spritebounds.x*2) > end.x) {
			Vector3 back = Camera.main.ViewportToWorldPoint (new Vector3(2.5f,0.5f));
			back.z = transform.position.z;
			transform.position = back;
		}
	}

		
}
