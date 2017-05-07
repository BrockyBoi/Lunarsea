using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AchievementController : MonoBehaviour
{
	public static AchievementController controller;

	public enum AchievementType
	{
		Distance,
		LevelsBeat,
		Coins,
		TimePlayed,
		UpgradesBought,
		TempGoalsBeaten,
		ACHIEVEMENT_COUNT
	}

	#region Achievement Strings

	List<string> distanceStrings = new List<string> ();
	List<string> levelStrings = new List<string> ();
	List<string> coinStrings = new List<string> ();
	List<string> timeStrings = new List<string> ();
	List<string> upgradeStrings = new List<string> ();
	List<string> tempGoalStrings = new List<string> ();
	Dictionary<AchievementType, List<string>> AchievementStrings = new Dictionary<AchievementType, List<string>> ();

	#endregion


	#region AchievementNeeds

	readonly int[] distanceNeeds = new int[4]{ 500, 1000, 5000, 20000 };
	readonly int[] coinNeeds = new int[5]{ 100, 250, 750, 2000, 10000 };
	readonly int[] upgradeNeeds = new int[4]{ 1, 5, 20, 35 };
	readonly int[] tempGoalNeeds = new int[5]{ 1, 20, 100, 500, 2000 };
	readonly int[] timeNeeds = new int[5]{ 1, 5, 12, 24, 48 };
	Dictionary<AchievementType, int[]> AchievementNeeds = new Dictionary<AchievementType, int[]> ();

	#endregion

	//All of these are hours needed
	DateTime timePlayed;

	void Awake ()
	{
		controller = this;

		InitializeDict ();
		InitializeStringLists ();
	}


	void InitializeStringLists ()
	{
		#region DistanceStrings
		distanceStrings.Add (LunarseaResources.achievement_travel_500_m);
		distanceStrings.Add (LunarseaResources.achievement_travel_1000_m);
		distanceStrings.Add (LunarseaResources.achievement_travel_5000_m);
		distanceStrings.Add (LunarseaResources.achievement_travel_10000_m);
		#endregion
		#region LevelStrings
		levelStrings.Add (LunarseaResources.achievement_beat_level_1);
		levelStrings.Add (LunarseaResources.achievement_beat_level_2);
		levelStrings.Add (LunarseaResources.achievement_beat_level_3);
		levelStrings.Add (LunarseaResources.achievement_beat_level_4);
		levelStrings.Add (LunarseaResources.achievement_beat_level_5);
		#endregion
		#region CoinStrings
		coinStrings.Add (LunarseaResources.achievement_collect_100_coins);
		coinStrings.Add (LunarseaResources.achievement_collect_250_coins);
		coinStrings.Add (LunarseaResources.achievement_collect_750_coins);
		coinStrings.Add (LunarseaResources.achievement_collect_2000_coins);
		coinStrings.Add (LunarseaResources.achievement_collect_10000_coins);
		#endregion
		#region TimeStrings
		timeStrings.Add (LunarseaResources.achievement_1_hour);
		timeStrings.Add (LunarseaResources.achievement_5_hours);
		timeStrings.Add (LunarseaResources.achievement_12_hours);
		timeStrings.Add (LunarseaResources.achievement_1_day);
		timeStrings.Add (LunarseaResources.achievement_2_days);
		#endregion
		#region UpgradeStrings
		upgradeStrings.Add (LunarseaResources.achievement_first_upgrade);
		upgradeStrings.Add (LunarseaResources.achievement_5_upgrades);
		upgradeStrings.Add (LunarseaResources.achievement_20_upgrades);
		upgradeStrings.Add (LunarseaResources.achievement_all_upgrades_unlocked);
		#endregion
		#region TempGoalStrings
		tempGoalStrings.Add (LunarseaResources.achievement_temp_goal_beat);
		tempGoalStrings.Add (LunarseaResources.achievement_20_temp_goals_beat);
		tempGoalStrings.Add (LunarseaResources.achievement_100_temp_goals);
		tempGoalStrings.Add (LunarseaResources.achievement_500_temp_goals);
		tempGoalStrings.Add (LunarseaResources.achievement_2000_temp_goals);
		#endregion

	}

	void InitializeDict ()
	{
		AchievementNeeds.Add (AchievementType.Distance, distanceNeeds);
		AchievementNeeds.Add (AchievementType.Coins, coinNeeds);
		AchievementNeeds.Add (AchievementType.UpgradesBought, upgradeNeeds);
		AchievementNeeds.Add (AchievementType.TempGoalsBeaten, tempGoalNeeds);
		AchievementNeeds.Add (AchievementType.TimePlayed, timeNeeds);

		AchievementStrings.Add (AchievementType.Distance, distanceStrings);
		AchievementStrings.Add (AchievementType.Coins, coinStrings);
		AchievementStrings.Add (AchievementType.TempGoalsBeaten, tempGoalStrings);
		AchievementStrings.Add (AchievementType.UpgradesBought, upgradeStrings);
		AchievementStrings.Add (AchievementType.TimePlayed, timeStrings);
	}


	public void AddToAchievementProgress (float progress, AchievementType ach)
	{
		if (!Social.localUser.authenticated)
			return;
		
		for (int i = 0; i < AchievementStrings [ach].Count; i++) {
			Social.ReportProgress (AchievementStrings [ach] [i], progress, (bool success) => {

			});
		}
	}


	public void BeatLevel (int levelBeat)
	{
		if (!Social.localUser.authenticated)
			return;
		
		Social.ReportProgress (AchievementStrings [AchievementType.LevelsBeat] [levelBeat - 1], 100.0f, (bool success) => {
			
		});
	}

	public void UpdateLeaderboard (long highScore)
	{
		if (!Social.localUser.authenticated)
			return;

		Social.ReportScore (highScore, LunarseaResources.leaderboard_highest_distance_traveled_endless_mode, (bool success) => {
			
		});
	}
		
}
