using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeController : MonoBehaviour
{

    #region Variables
    public static UpgradeController controller;
    public enum Upgrade { MaxHealth, HealthDrop, CoinDrop, InvulTime, BoatSpeed, CoinMagnet, MaxGoals, UPGRADE_COUNT }

    int[] upgradeValues = new int[(int)Upgrade.UPGRADE_COUNT];
    public int[] coinAmounts = new int[5];

    bool currentlyUpgrading;

    public delegate void NotUpgrading();
    public event NotUpgrading notUpgrading;
    #endregion

    void Awake()
    {
        controller = this;
        currentlyUpgrading = true;
    }

    void Start()
    {

    }

    public int GetUpgradeValue(Upgrade u)
    {
        return upgradeValues[(int)u];
    }
    public int[] GetUpgradeArray()
    {
        return upgradeValues;
    }
    public void GiveUpgradeArray(int[] upgrades)
    {
        upgrades.CopyTo(upgradeValues, 0);

        UpdateAll();
    }

    public void BuyUpgrade(int upgrade)
    {
        if (upgradeValues[upgrade] >= 5  || CoinController.controller.getCoinNum() < coinAmounts[upgradeValues[upgrade]])
            return;

        CoinController.controller.MakePurchase(coinAmounts[upgradeValues[upgrade]]);

        upgradeValues[upgrade]++;
        switch (upgrade)
        {
            case (int)Upgrade.MaxHealth:
                UpdateMaxHealth();
                break;
            case (int)Upgrade.HealthDrop:
                UpdateHealthDrop();
                break;
            case (int)Upgrade.CoinDrop:
                UpdateCoinDrop();
                break;
            case (int)Upgrade.InvulTime:
                UpdateInvulTime();
                break;
            case (int)Upgrade.BoatSpeed:
                UpdateBoatSpeed();
                break;
            case (int)Upgrade.CoinMagnet:
                UpdateCoinMagnet();
                break;
            case (int)Upgrade.MaxGoals:
                UpdateMaxGoals();
                break;
            default:
                break;
        }
        MainCanvas.controller.UpdateUpgradePrice(upgrade, coinAmounts[upgradeValues[upgrade]]);
        PlayerInfo.controller.Save();
    }

    public void BuyUpgrade(Upgrade u)
    {
        upgradeValues[(int)u]++;
    }

    void UpdateMaxHealth()
    {
        Boat.player.UpdateMaxHealth(upgradeValues[(int)Upgrade.MaxHealth]);
    }

    void UpdateHealthDrop()
    {
        MillileSpawner.controller.UpgradeHealthDrop(upgradeValues[(int)Upgrade.HealthDrop]);
    }

    void UpdateCoinDrop()
    {
        MillileSpawner.controller.UpdateCoinRate(upgradeValues[(int)Upgrade.CoinDrop]);
    }

    void UpdateBoatSpeed()
    {
        Boat.player.UpdateBoatSpeed(upgradeValues[(int)Upgrade.BoatSpeed]);
    }

    void UpdateInvulTime()
    {
        Boat.player.UpdateInvulTime(upgradeValues[(int)Upgrade.InvulTime]);
    }

    void UpdateCoinMagnet()
    {
        Boat.player.UpdateMagnetSize(upgradeValues[(int)Upgrade.CoinMagnet]);
    }

    void UpdateMaxGoals()
    {
        TempGoalController.controller.UpdateMaxGoals(upgradeValues[(int)Upgrade.MaxGoals]);
    }

    void UpdateAll()
    {
        UpdateBoatSpeed();
        UpdateCoinDrop();
        UpdateCoinMagnet();
        UpdateHealthDrop();
        UpdateInvulTime();
        UpdateMaxHealth();
        UpdateMaxGoals();

        for (int i = 0; i < (int)Upgrade.UPGRADE_COUNT; i++)
        {
            if (upgradeValues[i] < 5)
                MainCanvas.controller.UpdateUpgradePrice(i, coinAmounts[upgradeValues[i]]);
            else MainCanvas.controller.UpdateUpgradePrice(i, 0);
        }
    }

    public void StartUpgrading()
    {
        currentlyUpgrading = true;
    }

    public void NoLongerUpgrading()
    {
        AudioController.controller.ClickUI();
        currentlyUpgrading = false;
        MainCanvas.controller.upgradeScreen.SetActive(false);
        notUpgrading();
    }

    public bool CheckIfUpgrading()
    {
        return currentlyUpgrading;
    }

}
