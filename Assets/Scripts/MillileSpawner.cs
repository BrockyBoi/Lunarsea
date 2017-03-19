using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MillileSpawner : MonoBehaviour
{
    public static MillileSpawner controller;
    public GameObject missilePrefab;
    public GameObject blueMissilePrefab;
    public GameObject trackerMissile;
    public GameObject rockPrefab;
    public GameObject healthPrefab;

    WaitForSeconds waitForRocks;
    WaitForSeconds waitForMissiles;
    WaitForSeconds waitForCoins;
    float hTimer;
    float hNextTime;

    float speedMultiplier;

    float healthRate;

    int waveCounter;

    int coinDropRate;

    void Awake()
    {
        controller = this;
    }

    // Use this for initialization
    void Start()
    {
        hNextTime = 5;
        healthRate = 20;
        waitForRocks = new WaitForSeconds(4f);
        waitForMissiles = new WaitForSeconds(3f);
        waitForCoins = new WaitForSeconds(2.5f);

        if (!TutorialController.controller.tutorialMode && !UpgradeController.controller.CheckIfUpgrading())
        {
            StartGame();
        }
    }

    #region Waves

    public void StartGame()
    {
        StartCoroutine(Wave1());
        if(Boat.player.GetMaxHealth() > 1)
            Invoke("SpawnHealth", 30);
    }

    IEnumerator Wave1()
    {
               TrackingMissileVolley(4);
        if (TutorialController.controller.tutorialMode)
        {
            while (TutorialController.controller.tutorialMode)
            {
                yield return null;
            }
        }
        else yield return new WaitForSeconds(2.5f);

        MissileVolley();
        yield return waitForMissiles;

        SpawnRocks(4);
        yield return waitForRocks;

        MissileVolley();
        yield return waitForMissiles;

        BlueMissileVolleyLow();
        yield return waitForMissiles;

        BlueMissileVolleyHigh();
        yield return waitForMissiles;

        TrackingMissileVolley(4);
        yield return waitForMissiles;

        waveCounter++;
        if(waveCounter < 5)
        {
            StartCoroutine(UpdateSpeedMultiplier(1f));
            StartCoroutine(Wave2());
        }
        else
        {
            Boat.player.SailOffScreen();
        }
        
    }

    IEnumerator Wave2()
    {
        SpawnCoinBlock();
        yield return waitForCoins;
        SpawnCoinLine();
        yield return waitForCoins;
        yield return waitForCoins;
        SpawnCoinZig();
        yield return waitForCoins;

        StartCoroutine(Wave1());
    }
    #endregion

    #region Coin Spawns
    void SpawnCoinBlock()
    {
        CoinController.controller.coinSpawnBlock(5 + coinDropRate, 5 + coinDropRate, -1, 20, .15f);
    }

    void SpawnCoinLine()
    {
        CoinController.controller.coinSpawnLine(15 + coinDropRate * 5, -2, 20, .1f);
    }

    void SpawnCoinZig()
    {
        CoinController.controller.coinSpawnZig(20 + coinDropRate * 5, 6, -1.5f, 20, .05f);
    }

    public void UpdateCoinRate(int value)
    {
        coinDropRate = value;
    }
    #endregion

    IEnumerator UpdateSpeedMultiplier(float amount)
    {
        float time = 0;
        while (time < amount)
        {
            speedMultiplier += Time.deltaTime;
            MainCanvas.controller.speedMult += Time.deltaTime;
            BackgroundConroller.controller.UpdateSpeedMult(Time.deltaTime);

            time += Time.deltaTime;
            yield return null;
        }
    }
    #region Missiles
    void SpawnMissile()
    {
        float minHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .65f)).y;
        float maxHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .9f)).y;
        Vector2 offScreen = new Vector2(Camera.main.ViewportToWorldPoint(new Vector3(1, .5f)).x + 5, Random.Range(minHeight, maxHeight));

        GameObject missile = Instantiate(missilePrefab, offScreen, Quaternion.identity) as GameObject;
        missile.GetComponent<Missile>().GiveSpeedMultiplier(speedMultiplier);
    }

    void MissileVolley()
    {
        float minHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .4f)).y;
        float maxHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .85f)).y;
        float offScreenX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0)).x;

        for (int i = 0; i < 5; i++)
        {
            GameObject missile = Instantiate(missilePrefab, new Vector3(offScreenX + 5, minHeight + (i * ((maxHeight - minHeight) / 5.0f))), Quaternion.identity) as GameObject;
            missile.GetComponent<Missile>().GiveSpeedMultiplier(speedMultiplier);
        }
    }

    void BlueMissileVolleyHigh()
    {
        float minHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .4f)).y;
        float maxHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .8f)).y;
        float offScreenX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0)).x;

        for (int i = 0; i < 4; i++)
        {
            GameObject missile = Instantiate(blueMissilePrefab, new Vector3(offScreenX + 5, minHeight + (i * ((maxHeight - minHeight) / 3.0f))), Quaternion.identity) as GameObject;
            missile.GetComponent<Missile>().GiveSpeedMultiplier(speedMultiplier);
        }
    }

    void BlueMissileVolleyLow()
    {
        float minHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .075f)).y;
        float maxHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .4f)).y;
        float offScreenX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0)).x;

        for (int i = 0; i < 4; i++)
        {
            GameObject missile = Instantiate(blueMissilePrefab, new Vector3(offScreenX + 5, minHeight + (i * ((maxHeight - minHeight) / 3.0f))), Quaternion.identity) as GameObject;
            missile.GetComponent<Missile>().GiveSpeedMultiplier(speedMultiplier);
        }
    }

    void TrackingMissileVolley(int amount)
    {
        StartCoroutine(SpawnTrackerMissiles(amount));
    }

    IEnumerator SpawnTrackerMissiles(int missileCount)
    {
        Vector2 offScreen = Camera.main.ViewportToWorldPoint(new Vector2(Random.Range(0.0f, 1.0f), 1.2f));

        for (int i = 0; i < missileCount; i++)
        {
            GameObject trackMissile = Instantiate(trackerMissile, offScreen, Quaternion.identity) as GameObject;
            trackMissile.GetComponent<TrackingMissile>().GiveSpeedMultiplier(speedMultiplier);
            yield return new WaitForSeconds(2.5f);
        }
    }
    #endregion

    #region Rocks
    void SpawnRocks(int amount)
    {
        float minRockHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .15f)).y;
        float maxRockHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.15f)).y;
        float offScreen = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0)).x;

        for (int i = 0; i < amount; i++)
        {
            GameObject rock = Instantiate(rockPrefab, new Vector3(offScreen + (10 * i), Random.Range(minRockHeight, maxRockHeight)), Quaternion.identity) as GameObject;
            rock.GetComponent<Rock>().GiveSpeedMultiplier(speedMultiplier);
        }
    }
    #endregion

    #region Health
    void SpawnHealth()
    {
        float minHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .35f)).y;
        float maxHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .55f)).y;
        Vector2 offScreen = new Vector2(Camera.main.ViewportToWorldPoint(new Vector3(1, .5f)).x + 5, Random.Range(minHeight, maxHeight));

        GameObject health = Instantiate(healthPrefab, offScreen, Quaternion.identity) as GameObject;
        health.GetComponent<HealthPickup>().GiveSpeedMultiplier(speedMultiplier);

        Invoke("SpawnHealth", healthRate);
    }
    #endregion

    #region Upgrades
    public void UpgradeCoinDrop(float rate)
    {

    }

    public void UpgradeHealthDrop(float rate)
    {
        healthRate = 20 - rate * 1.5f;
    }

    #endregion
}
