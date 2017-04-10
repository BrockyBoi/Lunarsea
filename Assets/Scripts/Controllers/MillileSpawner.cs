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

    List<GameObject> missileList = new List<GameObject>();
    List<GameObject> torpedoList = new List<GameObject>();
    List<GameObject> rockList = new List<GameObject>();
    List<GameObject> trackingMissileList = new List<GameObject>();

    GameObject healthItem;

    bool bossBattle;

    public delegate void OnWavesCleared();
    public event OnWavesCleared onWavesCleared;

    delegate void EndlessObstacleSpawn();
    EndlessObstacleSpawn ObstacleSpawn1;
    EndlessObstacleSpawn ObstacleSpawn2;
    EndlessObstacleSpawn CoinSpawner;

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
        Boat.player.onBoatDeath += EndLevel;

        TutorialController.controller.onFinishTutorial += StartGame;
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
        if (GameModeController.controller.CheckCurrentMode(GameModeController.Mode.Story))
            StartCoroutine(Wave1());
        else
            StartCoroutine(EndlessWave());
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
            onWavesCleared();

            PlayerInfo.controller.Save();
            StopAllCoroutines();
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

    IEnumerator EndlessWave()
    {
        float timeSpeedUp = 0;
        ObstacleSpawn1 += MissileVolley;
        ObstacleSpawn2 += TorpedoVolleyLow;
        CoinSpawner += SpawnCoinBlock;
        while (true)
        {
            ObstacleSpawn1();
            yield return new WaitForSeconds(3.5f - timeSpeedUp);

            ObstacleSpawn2();
            yield return new WaitForSeconds(4 - timeSpeedUp);

            CoinSpawner();
            yield return new WaitForSeconds(4 + timeSpeedUp * 2);

            waveCounter++;

            if (waveCounter == 2)
            {
                ObstacleSpawn1 += SpawnRocks;
            }
            else if (waveCounter == 3)
            {
                CoinSpawner = SpawnBlockAndLine;
            }
            else if (waveCounter == 4)
            {
                ObstacleSpawn2 -= TorpedoVolleyLow;
                ObstacleSpawn2 = TorpedoVolley;
            }
            else if (waveCounter == 5)
            {
                ObstacleSpawn1 += TrackingMissileVolley;
            }
            else if (waveCounter == 8)
            {
                ObstacleSpawn2 += TrackingMissileVolley;
            }
            else if (waveCounter == 10)
            {
                CoinSpawner = SpawnAllCoins;
            }
            else if (waveCounter == 12)
            {
                ObstacleSpawn2 += SpawnRocks;
            }
            else if (waveCounter == 15)
            {
                ObstacleSpawn1 += TrackingMissileVolley;
                ObstacleSpawn2 += TrackingMissileVolley;
                CoinSpawner += TrackingMissileVolley;
            }

            if (waveCounter < 20)
                timeSpeedUp += .15f;

            UpdateSpeedMultiplier(1.75f);
            yield return null;
        }
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

    void SpawnBlockAndLine()
    {
        CoinController.controller.coinSpawnBlock(3 + coinDropRate, 3 + coinDropRate, -1, 20, .15f);
        CoinController.controller.coinSpawnLine(8 + coinDropRate * 3, -2, 35, .1f);
    }

    void SpawnAllCoins()
    {
        CoinController.controller.coinSpawnBlock(3 + coinDropRate, 3 + coinDropRate, -1, 20, .15f);
        CoinController.controller.coinSpawnLine(8 + coinDropRate * 3, -2, 35, .1f);
        CoinController.controller.coinSpawnZig(10 + coinDropRate * 4, 6, -1.5f, 55, .05f);
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

    void TorpedoVolley()
    {
        float minHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .075f)).y;
        float maxHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .45f)).y;
        float offScreenX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0)).x;

        //Spawn high
        for (int i = 0; i < 4; i++)
        {
            int j = 0;
            while (torpedoList[j].activeInHierarchy)
                j++;

            GameObject missile = torpedoList[j].gameObject;
            missile.transform.position = new Vector3(offScreenX + 5, minHeight + (i * ((maxHeight - minHeight) / 5.0f)));
            missile.SetActive(true);
        }

        minHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .4f)).y;
        maxHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .8f)).y;
        offScreenX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0)).x;
        //Spawn low
        for (int i = 0; i < 4; i++)
        {
            int j = 0;
            while (torpedoList[j].activeInHierarchy)
                j++;

            GameObject missile = torpedoList[j].gameObject;
            missile.transform.position = new Vector3(offScreenX + 20, minHeight + (i * ((maxHeight - minHeight) / 5.0f)));
            missile.SetActive(true);
        }
    }

    void TorpedoVolleyHigh()
    {
        float minHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .4f)).y;
        float maxHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .8f)).y;
        float offScreenX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0)).x;

        for (int i = 0; i < 5; i++)
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
        float maxHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .45f)).y;
        float offScreenX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0)).x;

        for (int i = 0; i < 5; i++)
        {
            int j = 0;
            while (torpedoList[j].activeInHierarchy)
                j++;

            GameObject missile = torpedoList[j].gameObject;
            missile.transform.position = new Vector3(offScreenX + 5, minHeight + (i * ((maxHeight - minHeight) / 5.0f)));
            missile.SetActive(true);
        }
    }

    void TrackingMissileVolley()
    {
        StartCoroutine(SpawntrackingMissilePrefabs(4));
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
        for (int i = 0; i < 10; i++)
        {
            GameObject rock = Instantiate(rockPrefab, new Vector2(100, 100), Quaternion.identity) as GameObject;
            rockList.Add(rock);
            rock.SetActive(false);
            rock.transform.SetParent(rocksParent);
        }
    }

    void SpawnRocks()
    {
        float minRockHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .15f)).y;
        float maxRockHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.15f)).y;
        float offScreen = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0)).x;

        for (int i = 0; i < 4; i++)
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
    void SpawnRocks(int amount = 5)
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
        Instantiate(pirateShipPrefab, Camera.main.ViewportToWorldPoint(new Vector2(1.1f, .6f)), Quaternion.identity);

        while (EnemyBoat.currentBoss.GetPhase() != 2)
        {
            while (EnemyBoat.currentBoss.GetPhase() == 0)
            {
                yield return new WaitForSeconds(4);
                SpawnRocks(5);
            }
            while (EnemyBoat.currentBoss.GetPhase() == 1)
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
        //MainCanvas.controller.EndLevel();
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
