using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo controller;
    public bool DontLoadOnStart;

    float highScore;
    int coinCount;
    int[] playerUpgrades = new int[(int)UpgradeController.Upgrade.UPGRADE_COUNT];

	List<TempGoal> goals = new List<TempGoal>();
    void Awake()
    {
        if (controller == null)
        {
            //DontDestroyOnLoad(gameObject);
            controller = this;
        }
        else if (controller != this)
            Destroy(this);


        //newPlayer = true;
    }

    void Start()
    {
        if (!DontLoadOnStart)
            Load();
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        PlayerData data = new PlayerData();
        data.highScore = MainCanvas.controller.GetHighScore();
        data.coinCount = CoinController.controller.getCoinNum();
        UpgradeController.controller.GetUpgradeArray().CopyTo(data.playerUpgrades, 0);
		data.goals = new List<TempGoal>(TempGoalController.controller.GetGoals());
        bf.Serialize(file, data);
        file.Close();

    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            highScore = data.highScore;
            coinCount = data.coinCount;
            data.playerUpgrades.CopyTo(playerUpgrades,0);
			goals = new List<TempGoal>(data.goals);

            MainCanvas.controller.SetHighScore(highScore);
            CoinController.controller.setCoinNum(coinCount);
            UpgradeController.controller.GiveUpgradeArray(playerUpgrades);
			TempGoalController.controller.SetGoals(goals);
        }
    }

	public List<TempGoal> GetGoals()
	{
		return goals;
	}

	public float GetHighScore()
	{
		return highScore;
	}	

	public int GetCoinCount()
	{
		return coinCount;
	}

	public int[] GetUpgradeArray()
	{
		return playerUpgrades;
	}

    [Serializable]
    class PlayerData
    {
        public float highScore;
        public int coinCount;
        public int[] playerUpgrades = new int[(int)UpgradeController.Upgrade.UPGRADE_COUNT];
		public List<TempGoal> goals = new List<TempGoal>();
    }
}
