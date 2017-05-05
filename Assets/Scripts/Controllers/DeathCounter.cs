using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCounter : MonoBehaviour
{

	public static DeathCounter controller;

	int deathCount;

	public int timesBeforeAd;

	void OnEnable ()
	{
	}

	void Start ()
	{

		Boat.player.onBoatDeath += PlayerDeath;
	}

	void OnDisable ()
	{
		Boat.player.onBoatDeath -= PlayerDeath;
	}

	void Awake ()
	{
		DontDestroyOnLoad (this);
		if (controller == null)
			controller = this;
		else if (controller != this)
			Destroy (gameObject);
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
