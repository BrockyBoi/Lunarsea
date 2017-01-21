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
		running = false;
	}
	
	// Update is called once per frame
	void Update () {		
		if (!running && Input.GetAxisRaw ("Jump") == 1) {
			StartCoroutine (Return());
		}
	}
		
	IEnumerator Return()
	{
		running = true;
		float t = 0;
		while (Vector3.Distance (transform.position, Boat.player.transform.position) > 1) {
			transform.position = Vector3.Lerp (transform.position, Boat.player.transform.position, t);
			t += Time.deltaTime;

			yield return null;
		}
		Boat.player.MoonReturned ();
		Destroy (gameObject);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer ("Water")) {
			StartCoroutine (Return ());
		}
	}
}
