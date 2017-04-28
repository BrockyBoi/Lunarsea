using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public static CoinController controller;
    public GameObject coinPrefab;
    public Transform coinsParent;

    //Temp coins are just the coins that you get during that specific life
    int tempCoins;
    int numCoins;

    Queue<GameObject> disabledCoins = new Queue<GameObject>();
    public bool FullCoins;

    [SerializeField]
    int[] dailyCoinRewards = new int[7];
    int currentDayOfDailyReward;

    void OnEnable()
    {
    }

    void Awake()
    {
        controller = this;
    }
    // Use this for initialization
    void Start()
    {
        Boat.player.onFinishedSailingIn += StartGame;
        if (FullCoins)
        {
            numCoins = 50000000;
            MainCanvas.controller.UpdateCoinString(numCoins);
        }
        InitializeCoins();
    }

    void OnDisable()
    {
        Boat.player.onFinishedSailingIn -= StartGame;
    }

    #region Coin Spawners

    void InitializeCoins()
    {
        Vector2 farAway = new Vector2(100, 100);
        for (int i = 0; i < 250; i++)
        {
            GameObject coin = Instantiate(coinPrefab, farAway, Quaternion.identity) as GameObject;
            coin.SetActive(false);
            disabledCoins.Enqueue(coin);
            coin.transform.SetParent(coinsParent);
        }
    }
    //Spawns coins in a straight line with length coins
    public void coinSpawnLine(int length, float yPos, float xPos, float gap = 0.5f)
    {
        for (int i = 0; i < length; i++)
        {
            GameObject coin = disabledCoins.Dequeue();
            float x = xPos + (1.15f + gap) * i;
            coin.transform.position = new Vector3(x, yPos);
            coin.SetActive(true);
        }
    }

    //Spawns coins in a zig-zag with length coins with a height coins going up then down
    public void coinSpawnZig(int length, int height, float yPos, float xPos, float gap = 0.5f)
    {
        int h = 0;
        float x;
        float y = yPos - (1.15f + gap);
        int up = 1;
        for (int i = 0; i < length; i++)
        {
            x = xPos + (1.15f + gap) * i;
            if (h < height)
            {
                y = y + (1.15f + gap) * up;
                h++;
            }
            else
            {
                y = y - (1.15f + gap);
                h = 1;
                up = 0 - up;
            }
            GameObject coin = disabledCoins.Dequeue();
            coin.transform.position = new Vector3(x, y);
            coin.SetActive(true);
        }
    }

    //Spawns a block of coins of length by height
    public void coinSpawnBlock(int length, int height, float yPos, float xPos, float gap = 0.5f)
    {
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float x = xPos + (1.15f + gap) * i;
                float y = yPos + (1.15f + gap) * j;

                GameObject coin = disabledCoins.Dequeue();
                coin.transform.position = new Vector3(x, y);
                coin.SetActive(true);
            }
        }
    }
    #endregion

    #region Accessors

    void StartGame()
    {
        MainCanvas.controller.UpdateCoinString(tempCoins);
    }
    public int getCoinNum()
    {
        return numCoins;
    }

    public int GetTempCoinNum()
    {
        return tempCoins;
    }
    public void setCoinNum(int num)
    {
        numCoins = num;
        MainCanvas.controller.UpdateCoinString(numCoins);
    }
    public void addCoin()
    {
        tempCoins++;
        numCoins++;
        MainCanvas.controller.UpdateCoinString(tempCoins);
        TempGoalController.controller.UpdateCoinGoals();
    }

    public void BuyCoins(int amount)
    {
        numCoins += amount;
        MainCanvas.controller.UpdateCoinString(numCoins);
    }

    public void ReceiveReward(int amount)
    {
        numCoins += amount;
    }

    public void MakePurchase(int amount)
    {
        numCoins -= amount;
        MainCanvas.controller.UpdateCoinString(numCoins);
    }
    #endregion

    #region Daily Rewards
    public void GiveDailyReward()
    {
        if (currentDayOfDailyReward < dailyCoinRewards.Length - 1)
        {
            ReceiveReward(dailyCoinRewards[currentDayOfDailyReward]);
            currentDayOfDailyReward++;
        }
        else
        {
            //Add permanent .25% multiplier to temp goals and set day back to zero
            currentDayOfDailyReward = 0;
            TempGoalController.controller.AddRewardScoreMultipliers(.25f); 
        }
    }
    #endregion
}
