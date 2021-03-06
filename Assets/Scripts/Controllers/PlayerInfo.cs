﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class PlayerInfo : MonoBehaviour
{
	public static PlayerInfo controller;
	public bool ResetSaveFile;

	public bool DeleteFirst;

	int levelsBeaten;
	int[] playerUpgrades = new int[(int)UpgradeController.Upgrade.UPGRADE_COUNT];
	List<TempGoal> goals = new List<TempGoal> ();

	bool firstTimeEver;

	public DateTime oldDate;

	void OnEnable ()
	{
	}

	void Awake ()
	{
		if (controller == null) {
			controller = this;
		} else if (controller != this)
			Destroy (this);


		if (File.Exists (Application.persistentDataPath + "/playerInfo.dat")) {
			firstTimeEver = false;
		} else
			firstTimeEver = true;
	}

	void Start ()
	{
		if (Boat.player != null)
			Boat.player.onFinishedSailingIn += NoLongerFirstTime;

		if (DeleteFirst)
			DeleteFile ();
		if (!ResetSaveFile)
			Load ();
		else
			Save ();
	}

	void OnDisable ()
	{
		if (Boat.player != null)
			Boat.player.onFinishedSailingIn -= NoLongerFirstTime;
	}

	public void Save ()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/playerInfo.dat");

		PlayerData data = new PlayerData ();
		data.firstTimeEver = firstTimeEver;
		data.levelsBeaten = levelsBeaten;

		if (MainCanvas.controller != null) {
			data.highScore = MainCanvas.controller.GetHighScore ();
		} 

		if (CoinController.controller != null)
			data.coinCount = CoinController.controller.getCoinNum ();

		if (UpgradeController.controller != null)
			UpgradeController.controller.GetUpgradeArray ().CopyTo (data.playerUpgrades, 0);

		if (TempGoalController.controller != null) {
			data.scoreMultiplier = TempGoalController.controller.GetScoreMultiplier ();
			data.goals = new List<TempGoal> (TempGoalController.controller.GetGoals ());
		}

		if (MonetizationController.controller != null) {
			data.adsTurnedOff = MonetizationController.controller.CheckIfAdsTurnedOff ();
			data.oldDate = MonetizationController.controller.GetOldDate ();
		}

		if (AudioController.controller != null) {
			data.musicVolume = AudioController.controller.GetMusicVolume ();
			data.fxVolume = AudioController.controller.GetFXVolume ();
		}

		bf.Serialize (file, data);
		file.Close ();
	}

	public void Load ()
	{
		if (File.Exists (Application.persistentDataPath + "/playerInfo.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
			PlayerData data = (PlayerData)bf.Deserialize (file);
			file.Close ();

			data.playerUpgrades.CopyTo (playerUpgrades, 0);
			goals = new List<TempGoal> (data.goals);

			levelsBeaten = data.levelsBeaten;

			if (MainCanvas.controller != null) {
				MainCanvas.controller.SetHighScore (data.highScore);
			}
			if (MainMenu.controller != null)
				MainMenu.controller.InitializeLevelButtons (data.levelsBeaten);

			if (CoinController.controller != null)
				CoinController.controller.setCoinNum (data.coinCount);
			if (UpgradeController.controller != null)
				UpgradeController.controller.GiveUpgradeArray (playerUpgrades);
			if (TempGoalController.controller != null) {
				TempGoalController.controller.SetGoals (goals);
				TempGoalController.controller.SetScoreMultiplier (data.scoreMultiplier);
			}

			if (MonetizationController.controller != null) {
				MonetizationController.controller.SetOldDate (data.oldDate);
				MonetizationController.controller.SetAdsTurnedOff (data.adsTurnedOff);
			}

			if (AudioController.controller != null) {
				AudioController.controller.ChangeMusicVolume (data.musicVolume, true);
				AudioController.controller.ChangeFXVolume (data.fxVolume, true);
			}
		} else {
			firstTimeEver = true;
		}
	}

	public bool CheckIfFirstTime ()
	{
		return firstTimeEver;
	}

	void NoLongerFirstTime ()
	{
		firstTimeEver = false;
	}

	void DeleteFile ()
	{
		if (File.Exists (Application.persistentDataPath + "/playerInfo.dat")) {
			File.Delete (Application.persistentDataPath + "/playerInfo.dat");

#if UNITY_EDITOR
			UnityEditor.AssetDatabase.Refresh ();
#endif
		}
	}

	public void BeatLevel (int level)
	{
		if (level > levelsBeaten) {
			levelsBeaten++;
			AchievementController.controller.BeatLevel (level);
			Save ();
		}
	}

	public int GetLevelsBeaten ()
	{
		return levelsBeaten;
	}

	[Serializable]
	class PlayerData
	{
		public float musicVolume, fxVolume;
		public float highScore, scoreMultiplier;
		public int coinCount, levelsBeaten;
		public int[] playerUpgrades = new int[(int)UpgradeController.Upgrade.UPGRADE_COUNT];
		public List<TempGoal> goals = new List<TempGoal> ();

		public bool firstTimeEver, adsTurnedOff;

		public DateTime oldDate;
	}
}
