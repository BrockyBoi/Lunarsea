using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TempGoal : System.Object
{
	public enum Goal
	{
		Distance,
		Coin,
		MissilesDestroyed,
		TimesPlayed,
		AvoidDamage,
		MAX_GOALS

	}

	float currentProgress, lastLifeProgress, targetGoal;
	bool perLife;
	int rewardValue;

	bool rewardFulfilled;
	int spot;
	Goal myGoal;
	int goalNum;

	public TempGoal (int spotInArray, Goal g, float target, bool oncePerLife, int reward, int goalNum)
	{
		spot = spotInArray;
		targetGoal = target;
		perLife = oncePerLife;
		rewardValue = reward;
		myGoal = g;
		this.goalNum = goalNum;
	}

	public void UpdateProgress (float missileCount)
	{
		if (rewardFulfilled)
			return;

		switch (myGoal) {
		case Goal.Distance:
			CheckIfGoalIsMet (MainCanvas.controller.GetScore ());
			break;
		case Goal.Coin:
			CheckIfGoalIsMet (CoinController.controller.GetTempCoinNum ());
			break;
		case Goal.MissilesDestroyed:
			CheckIfGoalIsMet (missileCount);
			break;
		case Goal.TimesPlayed:
			lastLifeProgress++;
			CheckIfGoalIsMet (0);
			break;
		case Goal.AvoidDamage:
			if (!Boat.player.CheckIfTookDamage ())
				CheckIfGoalIsMet (MainCanvas.controller.GetScore ());
			break;
		default:
			break;

		}
	}

	public void UpdateSpotInList (int spot)
	{
		this.spot = spot;
	}

	void CheckIfGoalIsMet (float input)
	{
		currentProgress = input;
		if (rewardFulfilled)
			return;

		if (perLife) {
			if (currentProgress >= targetGoal)
				FulfillGoal ();
		} else {
			if (currentProgress + lastLifeProgress >= targetGoal)
				FulfillGoal ();
		}
	}


	void FulfillGoal ()
	{
		rewardFulfilled = true;
		TempGoalController.controller.FinishGoal (spot, rewardValue);
		MainCanvas.controller.CompleteGoal (GetCompletedDescription ());
		AchievementController.controller.AddToAchievementProgress (1f, AchievementController.AchievementType.TempGoalsBeaten);
	}

	public void PlayerDied ()
	{
		if (perLife)
			return;

		switch (myGoal) {
		case Goal.Distance:
			lastLifeProgress += MainCanvas.controller.GetScore ();
			break;
		case Goal.Coin:
			lastLifeProgress += CoinController.controller.GetTempCoinNum ();
			break;
		case Goal.MissilesDestroyed:
			lastLifeProgress += TempGoalController.controller.GetMissileCount ();
			break;
		default:
			break;
		}
	}

	public int GetGoalNum ()
	{
		return goalNum;
	}

	public int GetGoalType ()
	{
		return (int)myGoal;
	}

	public bool CheckIfGoalExists (Goal g)
	{
		if (myGoal == g)
			return true;

		return false;
	}

	public string GetDescription ()
	{
		string inProgress = "";
		if (currentProgress + lastLifeProgress == 0 || perLife)
			inProgress = " ";
		else
			inProgress = "ed ";

		string time = "";
		if (perLife)
			time = " in one life";
		else
			time = "";

		string goal = "";
		if (perLife)
			goal = ((int)targetGoal).ToString ();
		else
			goal = ((int)currentProgress + (int)lastLifeProgress).ToString () + " / " + targetGoal.ToString ();

		switch (myGoal) {
		case Goal.Distance:
			return "Travel" + inProgress + goal + " meters" + time + ".";
		case Goal.Coin:
			return "Collect" + inProgress + goal + " coins" + time + ".";
		case Goal.MissilesDestroyed:
			return "Destroy" + inProgress + goal + " missiles" + time + ".";
		case Goal.AvoidDamage:
			return "Travel" + inProgress + goal + " meters without taking damage.";
		case Goal.TimesPlayed:
			return "Play" + inProgress + goal + " games.";
		default:
			return "";
		}
	}

	string GetCompletedDescription ()
	{
		string time = "";
		if (perLife)
			time = " in one life";
		else
			time = "";

		string goal = "";

		string reward = " for " + (Mathf.RoundToInt (rewardValue * TempGoalController.controller.GetAllMultipliers ())).ToString () + " coins.";
        
		goal = targetGoal.ToString ();

		switch (myGoal) {
		case Goal.Distance:
			return "Traveled " + goal + " meters" + time + reward;
		case Goal.Coin:
			return "Collected " + goal + " coins" + time + reward;
		case Goal.MissilesDestroyed:
			return "Destroyed " + goal + " missiles" + time + reward;
		case Goal.AvoidDamage:
			return "Traveled " + goal + " meters without taking damage" + reward;
		case Goal.TimesPlayed:
			return "Played " + goal + " games.";
		default:
			return "";
		}
	}
}
