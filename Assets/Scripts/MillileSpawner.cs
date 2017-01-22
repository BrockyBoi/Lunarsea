using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MillileSpawner : MonoBehaviour {
	public GameObject missilePrefab;
	public GameObject[] rockPrefabs;
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

	void SpawnRock()
	{
		int num = Random.Range (0, 2);
		float offScreen = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0)).x;
		Instantiate (rockPrefabs[num], new Vector3 (offScreen, -3), Quaternion.identity);
	}

	void SpawnHealth()
	{
		float minHeight = Camera.main.ViewportToWorldPoint (new Vector3 (0, .5f)).y;
		float maxHeight = Camera.main.ViewportToWorldPoint (new Vector3 (0, .75f)).y;
		Vector2 offScreen = new Vector2(Camera.main.ViewportToWorldPoint(new Vector3(1,.5f)).x + 5, Random.Range(minHeight,maxHeight));

		Instantiate (healthPrefab, offScreen, Quaternion.identity);
	}
}
