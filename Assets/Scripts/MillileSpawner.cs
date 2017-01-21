using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MillileSpawner : MonoBehaviour {
	public GameObject missilePrefab;
	float timer;
	float nextTime;
	// Use this for initialization
	void Start () {
		nextTime = 3;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer >= nextTime) {
			nextTime += 3 * .999f;
			Spawn ();
		}
	}

	void Spawn()
	{
		float minHeight = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0)).y;
		float maxHeight = Camera.main.ViewportToWorldPoint (new Vector3 (0, 1)).y;
		Vector2 offScreen = new Vector2(Camera.main.ViewportToWorldPoint(new Vector3(1,.5f)).x + 10, Random.Range(minHeight,maxHeight));

		Instantiate (missilePrefab, offScreen, Quaternion.identity);
	}
}
