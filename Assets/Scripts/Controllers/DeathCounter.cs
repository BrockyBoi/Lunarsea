using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathCounter : MonoBehaviour
{

	public static DeathCounter controller;

	[SerializeField]
	int deathCount;

	public int timesBeforeAd;

	void OnEnable ()
	{
		Debug.Log (deathCount);
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable ()
	{
		Boat.player.onBoatDeath -= PlayerDeath;
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	void Awake ()
	{
		if (controller == null)
			controller = this;
		else if (controller != this)
			Destroy (gameObject);

		DontDestroyOnLoad (this);
	}

	void OnLevelFinishedLoading (Scene scene, LoadSceneMode mode)
	{
		if (Boat.player != null) {
			Boat.player.onBoatDeath += PlayerDeath;
		}
	}

	void PlayerDeath ()
	{
		#if UNITY_ANDROID || UNITY_IOS
		deathCount++;

		if (!MonetizationController.controller.CheckIfAdsTurnedOff () && deathCount % timesBeforeAd == 0) {
			MonetizationController.controller.ShowNormalAd ();
		}
		#endif
	}

	public int GetDeathCount ()
	{
		return deathCount;
	}

}
