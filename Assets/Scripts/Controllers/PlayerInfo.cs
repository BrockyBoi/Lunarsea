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
    public bool ResetSaveFile;

    float highScore;
    int coinCount;
    int[] playerUpgrades = new int[(int)UpgradeController.Upgrade.UPGRADE_COUNT];

    bool firstTimeEver;

    List<TempGoal> goals = new List<TempGoal>();

    bool adsTurnedOff;

    public bool DeleteFirst;

    public delegate void OnLoadF(float f);
    public event OnLoadF onLoadF;

    void OnEnable()
    {
    }
    void Awake()
    {
        if (controller == null)
        {
            //DontDestroyOnLoad(gameObject);
            controller = this;
        }
        else if (controller != this)
            Destroy(this);


        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            firstTimeEver = false;
        }
        else firstTimeEver = true;
    }

    void Start()
    {
        Boat.player.onFinishedSailingIn += NoLongerFirstTime;
        if (DeleteFirst)
            DeleteFile();
        if (!ResetSaveFile)
            Load();
        else Save();
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
        data.adsTurnedOff = MonetizationController.controller.CheckIfAdsTurnedOff();
        data.scoreMultiplier = TempGoalController.controller.GetScoreMultiplier();
        data.firstTimeEver = firstTimeEver;
        data.oldDate = MonetizationController.controller.GetOldDate();

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

            data.playerUpgrades.CopyTo(playerUpgrades, 0);
            goals = new List<TempGoal>(data.goals);

            Debug.Log("High Score: " + data.highScore);
            MonetizationController.controller.DayCheck();
            MainCanvas.controller.SetHighScore(data.highScore);
            CoinController.controller.setCoinNum(data.coinCount);
            UpgradeController.controller.GiveUpgradeArray(playerUpgrades);
            TempGoalController.controller.SetGoals(goals);
            MonetizationController.controller.UpdateAdsTurnedOff(data.adsTurnedOff);
            TempGoalController.controller.SetScoreMultiplier(data.scoreMultiplier);

            MonetizationController.controller.SetOldDate(data.oldDate);
        }
        else
        {
            firstTimeEver = true;
        }
    }

    public bool CheckIfFirstTime()
    {
        return firstTimeEver;
    }

    void NoLongerFirstTime()
    {
        firstTimeEver = false;
    }

    void DeleteFile()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            File.Delete(Application.persistentDataPath + "/playerInfo.dat");

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
    }

    public void ResetAllValues()
    {
        highScore = 0;
        coinCount = 0;

        for(int i = 0; i < playerUpgrades.Length; i++)
        {
            playerUpgrades[i] = 0;
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

    [Serializable]
    class PlayerData
    {
        public float highScore, scoreMultiplier;
        public int coinCount;
        public int[] playerUpgrades = new int[(int)UpgradeController.Upgrade.UPGRADE_COUNT];
        public List<TempGoal> goals = new List<TempGoal>();

        public bool firstTimeEver, adsTurnedOff;

        public DateTime oldDate;
    }
}
