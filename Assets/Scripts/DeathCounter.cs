using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCounter : MonoBehaviour
{

    public static DeathCounter controller;

    int deathCount;

    void Awake()
    {
        DontDestroyOnLoad(this);
        if (controller == null)
            controller = this;
        else if (controller != this)
            Destroy(gameObject);
    }

    public void PlayerDeath()
    {
        deathCount++;

        if (!MonetizationController.controller.CheckIfAdsTurnedOff() && deathCount % 2 == 0)
        {
            MonetizationController.controller.ShowNormalAd();
        }
		TempGoalController.controller.UpdateTimesPlayedGoals();
    }

	public int GetDeathCount()
	{
		return deathCount;
	}

}
