using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScroll : MonoBehaviour {

	SpriteRenderer spr;
	Vector3 backPos;
	public float speed = 0.1f;
	Collider2D collider;
	public float initialViewportXPos;
	[SerializeField]
	float zpos;
	void Awake() {
		spr = GetComponent<SpriteRenderer> ();
		collider = GetComponent<EdgeCollider2D> ();
	}

	void Start() {
		transform.position = Camera.main.ViewportToWorldPoint (new Vector3(initialViewportXPos,0.5f));
		transform.position = new Vector3 (transform.position.x,transform.position.y,zpos);
	}

	// Update is called once per frame
	void Update () {
		//Debug.Log (Camera.main.WorldToViewportPoint(transform.position).x < -0.5);
		if (Camera.main.WorldToViewportPoint (transform.position).x < -0.5f) {
			Vector3 newpos= Camera.main.ViewportToWorldPoint (new Vector3(1.5f,0.5f));
			newpos.z = zpos;
			transform.position = newpos;
		}
		transform.Translate (new Vector3(-speed,0));
	}

		
}
