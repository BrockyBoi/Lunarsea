﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MillileSpawner : MonoBehaviour {
	public GameObject missilePrefab;
	public GameObject rockPrefab;

	float mTimer;
	float mNextTime;

	float rTimer;
	float rNextTime;
	// Use this for initialization
	void Start () {
		mNextTime = 3;
	}
	
	// Update is called once per frame
	void Update () {
		mTimer += Time.deltaTime;
		if (mTimer >= mNextTime) {
			mNextTime += 3 * .999f;
			SpawnMissile ();
		}

		rTimer += Time.deltaTime;
		if (rTimer >= rNextTime) {
			rNextTime += 5 * .999f;
			SpawnRock ();
		}
	}

	void SpawnMissile()
	{
		float minHeight = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0)).y;
		float maxHeight = Camera.main.ViewportToWorldPoint (new Vector3 (0, 1)).y;
		Vector2 offScreen = new Vector2(Camera.main.ViewportToWorldPoint(new Vector3(1,.5f)).x + 5, Random.Range(minHeight,maxHeight));

		Instantiate (missilePrefab, offScreen, Quaternion.identity);
	}

	void SpawnRock()
	{
		float offScreen = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0)).x;
		Instantiate (rockPrefab, new Vector3 (offScreen, -3), Quaternion.identity);
	}
}
