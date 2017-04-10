using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MillileSpawner : MonoBehaviour
{
    public static MillileSpawner controller;
    public GameObject missilePrefab;
    public GameObject torpedoPrefab;
    public GameObject trackingMissilePrefab;
    public GameObject rockPrefab;
    public GameObject healthPrefab;

    public GameObject pirateShipPrefab;

    WaitForSeconds waitForRocks;
    WaitForSeconds waitForMissiles;
    WaitForSeconds waitForCoins;

    public Transform missilesParent;
    public Transform rocksParent;


    float speedMultiplier;

    float healthRate;

    int waveCounter;

    int coinDropRate;

    List<Rock> currentRocks = new List<Rock>();

    [SerializeField]
    List<GameObject> missileList = new List<GameObject>();
    [SerializeField]
    List<GameObject> torpedoList = new List<GameObject>();
    [SerializeField]
    List<GameObject> rockList = new List<GameObject>();
    [SerializeField]
    List<GameObject> trackingMissileList = new List<GameObject>();

    GameObject healthItem;

    bool bossBattle;

    void Awake()
    {
        controller = this;
    }

    // Use this for initialization
    void Start()
    {
        healthRate = 45;
        waitForRocks = new WaitForSeconds(4f);
        waitForMissiles = new WaitForSeconds(3f);
        waitForCoins = new WaitForSeconds(2.5f);

        InitializeMissiles();
        InitializeRocks();
        healthItem = Instantiate(healthPrefab, new Vector2(100, 100), Quaternion.identity) as GameObject;
        healthItem.SetActive(false);
    }

    #region Waves

    public void StartGame()
    {
        StartCoroutine(Wave1());
        //StartCoroutine(SpawnPirateShip());
        if (Boat.player.GetMaxHealth() > 1)
            Invoke("SpawnHealth", 45);
    }

    IEnumerator Wave1()
    {
        MissileVolley();
        yield return waitForMissiles;

        SpawnRocks(4);
        yield return waitForRocks;

        MissileVolley();
        yield return waitForMissiles;

        TorpedoVolleyLow();
        yield return waitForMissiles;

        TorpedoVolleyHigh();
        yield return waitForMissiles;

        TrackingMissileVolley(4);
        yield return waitForMissiles;

        waveCounter++;
        if (waveCounter < 5)
        {
            StartCoroutine(UpdateSpeedMultiplier(1.5f));
            StartCoroutine(Wave2());
        }
        else
        {
            Boat.player.SailOffScreen();
            PlayerInfo.controller.Save();
            StopAllCoroutines();
            MainCanvas.controller.EndLevel();
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

    public void EndLevel()
    {
        StopAllCoroutines();

        for (int i = 0; i < currentRocks.Count; i++)
        {
            currentRocks[i].StopObject();
        }
    }
    #endregion

    #region Coin Spawns
    void SpawnCoinBlock()
    {
        CoinController.controller.coinSpawnBlock(3 + coinDropRate, 3 + coinDropRate, -1, 20, .15f);
    }

    void SpawnCoinLine()
    {
        CoinController.controller.coinSpawnLine(8 + coinDropRate * 3, -2, 20, .1f);
    }

    void SpawnCoinZig()
    {
        CoinController.controller.coinSpawnZig(10 + coinDropRate * 4, 6, -1.5f, 20, .05f);
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

    public float GetSpeedMultiplier()
    {
        return speedMultiplier;
    }
    #region Missiles

    void InitializeMissiles()
    {
        Vector2 farAway = new Vector2(100, 100);
        for (int i = 0; i < 12; i++)
        {
            GameObject missile = Instantiate(missilePrefab, farAway, Quaternion.identity) as GameObject;
            missile.SetActive(false);
            missileList.Add(missile);
            missile.transform.SetParent(missilesParent);
        }

        for (int i = 0; i < 12; i++)
        {
            GameObject torpedo = Instantiate(torpedoPrefab, farAway, Quaternion.identity) as GameObject;
            torpedo.SetActive(false);
            torpedoList.Add(torpedo);
            torpedo.transform.SetParent(missilesParent);
        }

        for (int i = 0; i < 6; i++)
        {
            GameObject trackingMissile = Instantiate(trackingMissilePrefab, farAway, Quaternion.identity) as GameObject;
            trackingMissile.SetActive(false);
            trackingMissileList.Add(trackingMissile);
            trackingMissile.transform.SetParent(missilesParent);
        }
    }

    void MissileVolley()
    {
        float minHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .4f)).y;
        float maxHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .85f)).y;
        float offScreenX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0)).x;

        for (int i = 0; i < 5; i++)
        {
            int j = 0;
            while (missileList[j].activeInHierarchy)
                j++;

            GameObject missile = missileList[j].gameObject;
            missile.transform.position = new Vector3(offScreenX + 5, minHeight + (i * ((maxHeight - minHeight) / 5.0f)));
            missile.SetActive(true);
        }
    }

    void TorpedoVolleyHigh()
    {
        float minHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .4f)).y;
        float maxHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .8f)).y;
        float offScreenX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0)).x;

        for (int i = 0; i < 4; i++)
        {
            int j = 0;
            while (torpedoList[j].activeInHierarchy)
                j++;

            GameObject missile = torpedoList[j].gameObject;
            missile.transform.position = new Vector3(offScreenX + 5, minHeight + (i * ((maxHeight - minHeight) / 5.0f)));
            missile.SetActive(true);
        }
    }

    void TorpedoVolleyLow()
    {
        float minHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .075f)).y;
        float maxHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .4f)).y;
        float offScreenX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0)).x;

        for (int i = 0; i < 4; i++)
        {
            int j = 0;
            while (torpedoList[j].activeInHierarchy)
                j++;

            GameObject missile = torpedoList[j].gameObject;
            missile.transform.position = new Vector3(offScreenX + 5, minHeight + (i * ((maxHeight - minHeight) / 5.0f)));
            missile.SetActive(true);
        }
    }

    void TrackingMissileVolley(int amount)
    {
        StartCoroutine(SpawntrackingMissilePrefabs(amount));
    }

    IEnumerator SpawntrackingMissilePrefabs(int missileCount)
    {
        Vector2 offScreen = Camera.main.ViewportToWorldPoint(new Vector2(Random.Range(0.0f, 1.0f), 1.2f));

        for (int i = 0; i < missileCount; i++)
        {
            int j = 0;
            while (trackingMissileList[j].activeInHierarchy)
                j++;

            GameObject trackMissile = trackingMissileList[j].gameObject;
            trackMissile.transform.position = offScreen;
            trackMissile.SetActive(true);

            yield return new WaitForSeconds(2.5f);
        }
    }
    #endregion

    #region Rocks

    void InitializeRocks()
    {
        for (int i = 0; i < 6; i++)
        {
            GameObject rock = Instantiate(rockPrefab, new Vector2(100, 100), Quaternion.identity) as GameObject;
            rockList.Add(rock);
            rock.SetActive(false);
            rock.transform.SetParent(rocksParent);
        }
    }
    void SpawnRocks(int amount)
    {
        float minRockHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .15f)).y;
        float maxRockHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.15f)).y;
        float offScreen = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0)).x;

        for (int i = 0; i < amount; i++)
        {
            int j = 0;
            while (rockList[j].activeInHierarchy)
                j++;

            GameObject rock = rockList[j];
            rock.transform.position = new Vector3(offScreen + (10 * i), Random.Range(minRockHeight, maxRockHeight));
            rock.SetActive(true);
            currentRocks.Add(rock.GetComponent<Rock>());
        }
    }

    public void GetOutofCurrentRockList(Rock rock)
    {
        currentRocks.Remove(rock);
    }
    #endregion

    #region Health
    void SpawnHealth()
    {
        float minHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .35f)).y;
        float maxHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .55f)).y;
        Vector2 offScreen = new Vector2(Camera.main.ViewportToWorldPoint(new Vector3(1, .5f)).x + 5, Random.Range(minHeight, maxHeight));

        healthItem.transform.position = offScreen;
        healthItem.SetActive(true);

        Invoke("SpawnHealth", healthRate);
    }
    #endregion

    #region Boss Battles
    IEnumerator SpawnPirateShip()
    {
        Instantiate(pirateShipPrefab, Camera.main.ViewportToWorldPoint(new Vector2(1.1f, .5f)), Quaternion.identity);

        while (EnemyBoat.currentBoss.GetPhase() != 2) 
        {
            while(EnemyBoat.currentBoss.GetPhase() == 0)
            {
                yield return new WaitForSeconds(8);
                SpawnRocks(5);
            }
            while(EnemyBoat.currentBoss.GetPhase() == 1)
            {
                yield return new WaitForSeconds(10);
                TrackingMissileVolley(5);
            }
            yield return null;
        }

    }

    public void BossBeaten()
    {
        StopAllCoroutines();
        Boat.player.SailOffScreen();
        MainCanvas.controller.EndLevel();
    }
    #endregion

    #region Upgrades

    public void UpgradeHealthDrop(float rate)
    {
        healthRate = 45 - rate * 2.5f;
    }

    public void UpdateCoinRate(int value)
    {
        coinDropRate = value;
    }

    #endregion
}
