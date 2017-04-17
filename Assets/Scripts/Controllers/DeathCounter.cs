using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCounter : MonoBehaviour
{

    public static DeathCounter controller;

    int deathCount;

    public int timesBeforeAd;

    void OnEnable()
    {
        Boat.player.onBoatDeath += PlayerDeath;
    }
    void Awake()
    {
        DontDestroyOnLoad(this);
        if (controller == null)
            controller = this;
        else if (controller != this)
            Destroy(gameObject);
    }

    void PlayerDeath()
    {
        deathCount++;

//         if (!MonetizationController.controller.CheckIfAdsTurnedOff() && deathCount % timesBeforeAd == 0)
//         {
// #if UNITY_ADS
//             MonetizationController.controller.ShowNormalAd();
// #endif
//         }
    }

	public int GetDeathCount()
	{
		return deathCount;
	}

}
