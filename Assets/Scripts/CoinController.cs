using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public static CoinController controller;
    public GameObject coin;

    //Temp coins are just the coins that you get during that specific life
    int tempCoins;
    int numCoins;

    public bool FullCoins;

    void Awake()
    {
        controller = this;
    }
    // Use this for initialization
    void Start()
    {
        if(FullCoins)
        {
            numCoins = 50000000;
            MainCanvas.controller.UpdateCoinString(numCoins);
        }
    }
    // Update is called once per frame
    void Update()
    {
    }

    #region Coin Spawners
    //Spawns coins in a straight line with length coins
    public void coinSpawnLine(int length, float yPos, float xPos, float gap = 0.5f)
    {
        for (int i = 0; i < length; i++)
        {
            float x = xPos + (1.15f + gap) * i;
            Instantiate(coin, new Vector3(x, yPos), Quaternion.identity);
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
            Instantiate(coin, new Vector3(x, y), Quaternion.identity);
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
                Instantiate(coin, new Vector3(x, y), Quaternion.identity);
            }
        }
    }
    #endregion

    #region Accessors

    public void StartGame()
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
}
