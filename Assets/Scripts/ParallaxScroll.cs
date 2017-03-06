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
		ResizeToScreen ();
	}

	void ResizeToScreen() {
		float height = Camera.main.orthographicSize * 2;
		float width = height * Screen.width / Screen.height;

		Sprite s = spr.sprite;
		float unitWidth = s.textureRect.width / s.pixelsPerUnit;
		float unitHeight = s.textureRect.height / s.pixelsPerUnit;

		spr.transform.localScale = new Vector3(width / unitWidth, height / unitHeight);
	}

	// Update is called once per frame
	void FixedUpdate () {
		//Debug.Log (Camera.main.WorldToViewportPoint(transform.position).x < -0.5);
		UpdateScroll ();
	}

	void UpdateScroll() {
		if (Camera.main.WorldToViewportPoint (transform.position).x < -0.5f) {
			Vector3 newpos= Camera.main.ViewportToWorldPoint (new Vector3(1.5f,0.5f));
			newpos.z = zpos;
			transform.position = newpos;
		}
		transform.position = transform.position + new Vector3 (-speed * Time.deltaTime,0);
		//transform.position = Vector3.MoveTowards(transform.position,Camera.main.ViewportToWorldPoint(new Vector3(0,0.5f)),Time.fixedDeltaTime);
		//transform.Translate (new Vector3(-speed,0));
	}

	public void GiveSpeedMultiplier(float mult)
    {
        speed += mult;
    }

		
}
