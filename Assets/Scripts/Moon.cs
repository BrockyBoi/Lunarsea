using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : MonoBehaviour {
	bool running;
	Rigidbody2D rb2d;

	void Awake()
	{
		rb2d = GetComponent<Rigidbody2D> ();
	}
	// Use this for initialization
	void Start () {
		StartCoroutine (Return(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
	}
	
	// Update is called once per frame
	void Update () {		
		if (!running && Input.GetAxisRaw ("Jump") == 1) {
			StartCoroutine (Return(Boat.player.transform.position));
		}
	}
		
	IEnumerator Return(Vector3 pos)
	{
		running = true;
		bool player = false;
		if (pos == Boat.player.transform.position)
			player = true;
		pos.z = 0;
		float t = 0;
		while (Vector3.Distance(transform.position, pos) > .5f) {
			transform.position = Vector3.Lerp (transform.position, pos, t);
			t += Time.deltaTime;

			yield return null;
		}
		running = false;
		if (player) {
			Boat.player.MoonReturned ();
			Destroy (gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer ("Water")) {
			StartCoroutine (Return(Boat.player.transform.position));
		}
	}
}
