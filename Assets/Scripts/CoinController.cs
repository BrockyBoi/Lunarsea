﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public static CoinController controller;
    public GameObject coinPrefab;

    //Temp coins are just the coins that you get during that specific life
    int tempCoins;
    int numCoins;

    List<GameObject> coinList = new List<GameObject>();
    public bool FullCoins;

    void Awake()
    {
        controller = this;
    }
    // Use this for initialization
    void Start()
    {
        if (FullCoins)
        {
            numCoins = 50000000;
            MainCanvas.controller.UpdateCoinString(numCoins);
        }
        InitializeCoins();
    }
    // Update is called once per frame
    void Update()
    {
    }

    #region Coin Spawners

    void InitializeCoins()
    {
        Vector2 farAway = new Vector2(100, 100);
        for (int i = 0; i < 75; i++)
        {
            GameObject coin = Instantiate(coinPrefab, farAway, Quaternion.identity) as GameObject;
            coin.SetActive(false);
            coinList.Add(coin);
        }
    }
    //Spawns coins in a straight line with length coins
    public void coinSpawnLine(int length, float yPos, float xPos, float gap = 0.5f)
    {
        for (int i = 0; i < length; i++)
        {
            float x = xPos + (1.15f + gap) * i;
            //Instantiate(coinPrefab, new Vector3(x, yPos), Quaternion.identity);
            int j = 0;
            while (coinList[j].activeInHierarchy)
                j++;

            GameObject coin = coinList[j];
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
            // Instantiate(coinPrefab, new Vector3(x, y), Quaternion.identity);
            int j = 0;

            while (coinList[j].activeInHierarchy)
                j++;

            GameObject coin = coinList[j];
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

                int z = 0;
                while (coinList[z].activeInHierarchy)
                    z++;

                GameObject coin = coinList[z];
                coin.transform.position = new Vector3(x, y);
                coin.SetActive(true);
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
