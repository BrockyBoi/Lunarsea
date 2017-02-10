﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MillileSpawner : MonoBehaviour {
	public GameObject missilePrefab;
	public GameObject rockPrefab;
	public GameObject healthPrefab;

	float mTimer;
	float mNextTime;

	float rTimer;
	float rNextTime;

	float hTimer;
	float hNextTime;
	// Use this for initialization
	void Start () {
		mNextTime = 3;
		rNextTime = 3;
		hNextTime = 5;

		MissileVolley ();
	}
	
	// Update is called once per frame
	void Update () {
		if (SpeechController.controller.CheckTextTime ())
			return;
		
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

		hTimer += Time.deltaTime;
		if (hTimer >= hNextTime) {
			hNextTime += 8;
			SpawnHealth ();
		}
	}

	void SpawnMissile()
	{
		float minHeight = Camera.main.ViewportToWorldPoint (new Vector3 (0, .5f)).y;
		float maxHeight = Camera.main.ViewportToWorldPoint (new Vector3 (0, .75f)).y;
		Vector2 offScreen = new Vector2(Camera.main.ViewportToWorldPoint(new Vector3(1,.5f)).x + 5, Random.Range(minHeight,maxHeight));

		Instantiate (missilePrefab, offScreen, Quaternion.identity);
	}

	void MissileVolley()
	{
		float minHeight = Camera.main.ViewportToWorldPoint (new Vector3 (0, .35f)).y;
		float maxHeight = Camera.main.ViewportToWorldPoint (new Vector3 (0, .9f)).y;
		//Vector2 offScreen = new Vector2(Camera.main.ViewportToWorldPoint(new Vector3(1,.5f)).x + 5, Random.Range(minHeight,maxHeight));
		float offScreenX = Camera.main.ViewportToWorldPoint(new Vector3(1,0)).x;

		for (int i = 0; i < 4; i++) {
			Instantiate (missilePrefab, new Vector3 (offScreenX + 5, minHeight + (i * ((maxHeight - minHeight) / 5.0f))), Quaternion.identity);
		}
	}

	void SpawnRock()
	{
		float minRockHeight = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0)).y;
		float maxRockHeight = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0.2f)).y;
		float offScreen = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0)).x;
		Instantiate (rockPrefab, new Vector3 (offScreen+20, Random.Range(minRockHeight,maxRockHeight)), Quaternion.identity);
	}

	void SpawnHealth()
	{
		float minHeight = Camera.main.ViewportToWorldPoint (new Vector3 (0, .5f)).y;
		float maxHeight = Camera.main.ViewportToWorldPoint (new Vector3 (0, .75f)).y;
		Vector2 offScreen = new Vector2(Camera.main.ViewportToWorldPoint(new Vector3(1,.5f)).x + 5, Random.Range(minHeight,maxHeight));

		Instantiate (healthPrefab, offScreen, Quaternion.identity);
	}
}
