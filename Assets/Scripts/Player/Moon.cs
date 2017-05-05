using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : MonoBehaviour
{
	public Vector3 destination;
	public float distance = .02f;
	public float magnification = 6;
	Vector3 size;
	bool returning;

	void OnEnable ()
	{
		size = transform.localScale * magnification;

		returning = false;
	}

	void Update ()
	{
		#if UNITY_STANDALONE || UNITY_WEBPLAYER
		if (!Boat.player.CheckIfAlive () || (Input.GetMouseButtonDown (0) && !returning)) {
			ReturnMoon ();
		}
		#elif UNITY_IOS || UNITY_ANDROID
		if (!Boat.player.CheckIfAlive ())
			ReturnMoon ();
		
		for (int i = 0; i < Input.touchCount; i++) {
			Touch touch = Input.GetTouch (i);
			if (touch.phase == TouchPhase.Began && Camera.main.ScreenToWorldPoint (touch.position).y > Boat.player.transform.position.y) {
				ReturnMoon ();
			}
		}
		#endif
	}

	void ReturnMoon ()
	{
		StopAllCoroutines ();
		returning = true;
		StartCoroutine (MoveMoon (Boat.player.transform.position));
		if (TutorialController.controller.CheckIfTutorialMode ())
			TutorialController.controller.SetStage (TutorialController.TutorialStage.DONE);
	}

	public void GiveVector (Vector3 pos)
	{
		StartCoroutine (MoveMoon (pos));
	}

	IEnumerator MoveMoon (Vector3 pos)
	{
		if (pos == Boat.player.transform.position) {
			returning = true;
		}

		float t = 0;
		pos.z = 0;

		while (t < 1) {
			if (!returning) {
				transform.localScale = Vector3.Lerp (transform.localScale, size, t);
				transform.position = Vector3.Lerp (transform.position, pos, t);
			} else {
				transform.position = Vector3.Lerp (transform.position, Boat.player.transform.position, t);
				size /= 1.2f;
				transform.localScale = size;
			}

			t += Time.deltaTime * 2f;

			yield return null;
		}

		if (returning) {
			moonDone ();
		}

	}

	void moonDone ()
	{
		AudioController.controller.WaterFall ();
		Boat.player.moonOut = false;

		transform.localScale = new Vector3 (5, 5, 1);
		gameObject.SetActive (false);
		// Destroy(gameObject);
	}

}
