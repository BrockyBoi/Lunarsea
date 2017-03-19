using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeController : MonoBehaviour
{

    #region Variables
    public static UpgradeController controller;
    public enum Upgrade { MaxHealth, HealthDrop, CoinDrop, InvulTime, BoatSpeed, UPGRADE_COUNT }

    int[] upgradeValues = new int[(int)Upgrade.UPGRADE_COUNT];
    public int[] coinAmounts = new int[(int)Upgrade.UPGRADE_COUNT];

    bool currentlyUpgrading;
    #endregion

    void Awake()
    {
        controller = this;
    }

    void Start()
    {
        currentlyUpgrading = true;
    }

    public int[] GetUpgradeArray()
    {
        return upgradeValues;
    }
    public void GiveUpgradeArray(int[] upgrades)
    {
        upgrades.CopyTo(upgradeValues, 0);

		CheckDisabledButtons();
    }

    void CheckDisabledButtons()
    {
        if (upgradeValues[(int)Upgrade.MaxHealth] == 0)
        {
            MainCanvas.controller.DisableUpgradeButton((int)Upgrade.HealthDrop);
            MainCanvas.controller.DisableUpgradeButton((int)Upgrade.InvulTime);
        }
		else if(upgradeValues[(int)Upgrade.MaxHealth] == 1)
		{
			MainCanvas.controller.EnableUpgradeButton((int)Upgrade.HealthDrop);
            MainCanvas.controller.EnableUpgradeButton((int)Upgrade.InvulTime);
		}
    }

    public void BuyUpgrade(int upgrade)
    {
        if (upgradeValues[upgrade] >= 5 || !MainCanvas.controller.CheckIfCanUseButton(upgrade) || CoinController.controller.getCoinNum() < coinAmounts[upgradeValues[upgrade]])
            return;

		CoinController.controller.MakePurchase(coinAmounts[upgrade]);

        upgradeValues[upgrade]++;
		CheckDisabledButtons();
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
            default:
                break;
        }
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

    public void StartUpgrading()
    {
        currentlyUpgrading = true;
    }

    public void NoLongerUpgrading()
    {
        currentlyUpgrading = false;
        MainCanvas.controller.upgradeScreen.SetActive(false);
        TutorialController.controller.SetUpTutorial();
    }

    public bool CheckIfUpgrading()
    {
        return currentlyUpgrading;
    }

}
