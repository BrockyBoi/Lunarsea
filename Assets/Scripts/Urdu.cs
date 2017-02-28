using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Urdu : MonoBehaviour {
public bool side;
	void Start()
	{
		if(side)
		transform.position = Camera.main.ViewportToWorldPoint (new Vector3(-.1f, .5f));
		else 	transform.position = Camera.main.ViewportToWorldPoint (new Vector3(0, -.5f));
	}
	void OnTriggerEnter2D(Collider2D INFIDEL)
	{
		if(INFIDEL.gameObject.layer != LayerMask.NameToLayer("Water") && INFIDEL.gameObject.CompareTag("ArtificialWave"))
			Destroy (INFIDEL.gameObject);
	}
}
