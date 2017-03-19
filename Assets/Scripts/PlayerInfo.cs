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
            playerUpgrades = data.playerUpgrades;

            MainCanvas.controller.SetHighScore(highScore);
            CoinController.controller.setCoinNum(coinCount);
            UpgradeController.controller.GiveUpgradeArray(playerUpgrades);
        }
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
    }
}
