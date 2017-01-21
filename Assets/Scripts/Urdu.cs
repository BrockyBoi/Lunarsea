using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Urdu : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D INFIDEL)
	{
		if(INFIDEL.gameObject.layer != LayerMask.NameToLayer("Water"))
			Destroy (INFIDEL.gameObject);
	}
}
