using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour {
	[SerializeField]
	float lifetime;
	// Use this for initialization
	void Awake () {
		Destroy (this, lifetime);
	}
}
