using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class PlayerInfo : MonoBehaviour {
	public static PlayerInfo controller; 
	public bool DontLoadOnStart;

	int highScore;
	float[] playerUpgrades = new float[(int)UpgradeController.Upgrade.UPGRADE_COUNT];
	void Awake()
	{
		if (controller == null)
			controller = this;
		else if(controller != this)
			Destroy (this);


		//newPlayer = true;
	}
		
	void Start () {

		if (!DontLoadOnStart)
			Load ();
	}

	public void Save()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/playerInfo.dat");

		PlayerData data = new PlayerData ();
		data.highScore = highScore;
		data.playerUpgrades = playerUpgrades;
		bf.Serialize (file, data);
		file.Close ();

	}

	public void Load()
	{
		if (File.Exists (Application.persistentDataPath + "/playerInfo.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
			PlayerData data = (PlayerData)bf.Deserialize (file);
			file.Close ();

			highScore = data.highScore;
			playerUpgrades = data.playerUpgrades;
		}
	}

	[Serializable]
	class PlayerData
	{
		public int highScore;
		public float[] playerUpgrades = new float[(int)UpgradeController.Upgrade.UPGRADE_COUNT];
	}

	public int GetHighScore()
	{
		return highScore;
	}

	public void SetHighScore(int score)
	{
		highScore = score;
	}

	public void GiveUpgrades(float[] upgrades)
	{
		upgrades.CopyTo(playerUpgrades, 0);
	}
}
 