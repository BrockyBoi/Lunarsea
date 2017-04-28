using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MillileSpawner : MonoBehaviour
{
    #region variables
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

	Queue<GameObject> missileList = new Queue<GameObject>();
	Queue<GameObject> torpedoList = new Queue<GameObject>();
	Queue<GameObject> rockList = new Queue<GameObject>();
    Queue<GameObject> trackingMissileList = new Queue<GameObject>();

    GameObject healthItem;

    bool bossBattle;
    int diff;
    int[] ran = new int[1] { -1 };

    public delegate void OnWavesCleared();
    public event OnWavesCleared onWavesCleared;
    delegate void CoinSpawn();
    CoinSpawn CoinSpawner;

    #endregion

    void Awake()
    {
        if (controller == null)
            controller = this;
        else if (controller != this)
            this.enabled = false;
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

    void OnDisable()
    {
        Boat.player.onBoatDeath -= EndLevel;

        TutorialController.controller.onFinishTutorial -= StartGame;
    }

    #region Level and Wave Management

    void StartGame()
    {
        if (GameModeController.controller.CheckCurrentMode(GameModeController.Mode.Story))
            StartCoroutine("testWave");
        //StartCoroutine("Level" + GameModeController.controller.GetCurrentLevel().ToString());
        else {
            diff = 0;
            StartCoroutine(EndlessWave());
        }
        if (Boat.player.GetMaxHealth() > 1)
            Invoke("SpawnHealth", 45);
    }

    IEnumerator testWave()
    {
        yield return new WaitForSecondsRealtime(2);
        StartCoroutine(SpawnPirateShip());
    }
	#endregion

    #region Endless Mode
    IEnumerator EndlessWave()
    {
        StopCoroutine(transition());
        Debug.Log("diff: " + diff);
        yield return waitForCoins;

        #region diff 1
        if (diff <= 2)
        {
            SpawnRocks((int)Random.Range(1, 2), ran);
            yield return waitForRocks;
            MissileVolley((int)Random.Range(1, 3), ran);
            yield return waitForMissiles;

            float rand = Random.Range(0, 10);
            //Coins
            rand = Random.Range(0, 10);
            if (rand >= 4)
            {
                SpawnCoinLine((int)Random.Range(3, 10), (int)Random.Range(-4, 4));
                yield return waitForCoins;
            }
            else if (rand >= 1)
            {
                SpawnCoinZig((int)Random.Range(3, 12), (int)Random.Range(2, 8));
                yield return waitForCoins;
            }
        }
        #endregion
        #region diff2
        else if (diff <= 4){
            float rand = Random.Range(0, 10);

            if (rand >= 7)
            {
                SpawnRocks((int)Random.Range(1, 4), ran);
                yield return waitForRocks;
                MissileVolley((int)Random.Range(2, 6), ran);
                yield return waitForMissiles;
            }
            else if (rand >= 4)
            {
                MissileVolley((int)Random.Range(2, 6), ran);
                yield return waitForMissiles;
                SpawnRocks((int)Random.Range(1, 4), ran);
                yield return waitForRocks;
            }
            else
            {
                SpawnRocks((int)Random.Range(1, 4), ran);
                yield return waitForRocks;
                MissileVolley((int)Random.Range(2, 6), ran);
                yield return waitForMissiles;
                MissileVolley((int)Random.Range(2, 6), ran);
                yield return waitForMissiles;
            }

            //Coins
            rand = Random.Range(0, 10);
            if (rand >= 4)
            {
                SpawnCoinLine((int)Random.Range(3, 10), (int)Random.Range(-4, 4));
                yield return waitForCoins;
            }
            else if (rand >= 1)
            {
                SpawnCoinZig((int)Random.Range(3, 12), (int)Random.Range(2, 8));
                yield return waitForCoins;
            }
            else
            {
                SpawnCoinBlock((int)Random.Range(3, 6), (int)Random.Range(-4, 4));
            }

        }
        #endregion
        #region diff 3
        else if (diff <= 15)
        {
            float rand = Random.Range(0, 10);

            SpawnRocks((int)Random.Range(1, 4), ran);
            yield return waitForRocks;
            MissileVolley((int)Random.Range(2, 6), ran);
            yield return waitForMissiles;
            TorpedoVolleyHigh((int)Random.Range(1, 2), ran);
            TorpedoVolleyLow((int)Random.Range(1, 2), ran);
            yield return waitForMissiles;

            //Coins
            rand = Random.Range(0, 10);
            if (rand >= 4)
            {
                SpawnCoinLine((int)Random.Range(5, 12), (int)Random.Range(-4, 4));
            }
            else if (rand >= 1)
            {
                SpawnCoinZig((int)Random.Range(5, 14), (int)Random.Range(2, 8));
            }
            else
            {
                SpawnCoinBlock((int)Random.Range(3, 6), (int)Random.Range(-4, 4));
            }
            yield return waitForCoins;
        }
        else if (diff <= 30)
        {
            float rand = Random.Range(0, 10);

            SpawnRocks((int)Random.Range(2, 4));
            yield return waitForRocks;
            MissileVolley((int)Random.Range(3, 6));
            yield return waitForMissiles;
            TorpedoVolleyHigh((int)Random.Range(2, 3));
            TorpedoVolleyLow((int)Random.Range(1, 3));
            yield return waitForMissiles;

            //Coins
            rand = Random.Range(0, 10);
            if (rand >= 4)
            {
                SpawnCoinLine((int)Random.Range(6, 16), (int)Random.Range(-4, 4));
            }
            else if (rand >= 1)
            {
                SpawnCoinZig((int)Random.Range(6, 20), (int)Random.Range(2, 8));
            }
            else
            {
                SpawnCoinBlock((int)Random.Range(5, 10), (int)Random.Range(-4, 4));
            }
            yield return waitForCoins;

        }
        #endregion
        #region diff 4
        else if (diff <= 30)
        {
            float rand = Random.Range(0, 10);

            SpawnRocks((int)Random.Range(3, 4),ran);
            yield return waitForRocks;
            MissileVolley((int)Random.Range(4, 6),ran);
            yield return waitForMissiles;
            TorpedoVolleyHigh((int)Random.Range(3, 4),ran);
            TorpedoVolleyLow((int)Random.Range(2, 4),ran);
            yield return waitForMissiles;

            //Coins
            rand = Random.Range(0, 10);
            if (rand >= 4)
            {
                SpawnCoinLine((int)Random.Range(6, 13), (int)Random.Range(-4, 4));
            }
            else if (rand >= 1)
            {
                SpawnCoinZig((int)Random.Range(6, 15), (int)Random.Range(2, 8));
            }
            else
            {
                SpawnCoinBlock((int)Random.Range(4, 10), (int)Random.Range(-4, 4));
            }
            yield return waitForCoins;
        }
        #endregion

        StartCoroutine(transition());
    }

    IEnumerator transition()
    {
        StopCoroutine(EndlessWave());
        diff = diff + 1;
        yield return new WaitForEndOfFrame();
        StartCoroutine(EndlessWave());
    }
    #endregion

    #region Levels

    #region Level 1
    IEnumerator Level1()
    {
        SpawnRocks(3, new int[3] { 0, 0, 0 });
        yield return waitForRocks;

        MissileVolley(1, new int[1] { 2 });
        yield return waitForMissiles;

        SpawnRocks(2, new int[2] { 0, 0 });
        yield return waitForRocks;

        MissileVolley(2, new int[2] { 0, 1 }, new int[2] { 0, 1 });
        yield return waitForMissiles;

        SpawnRocks(1, new int[1] { 1 });
        yield return waitForRocks;

        MissileVolley(2, new int[2] { 1, 2 });
        yield return waitForMissiles;

        SpawnRocks(2, new int[2] { 1, 0 });
        yield return waitForRocks;

        SpawnRocks(2, new int[2] { 1, 1 });
        yield return waitForRocks;

        MissileVolley(3, new int[3] { 0, 1, 0 }, new int[3] { 0, 1, 2 });
        yield return waitForMissiles;

        SpawnRocks(4, new int[4] { 1, 1, 0, 0 });
        yield return waitForRocks;

        MissileVolley(3, new int[3] { 1, 2, 2 }, new int[3] { 0, 0, 1 });
        yield return waitForMissiles;

        //coins - block
        SpawnCoinBlock();
        yield return waitForCoins;

        SpawnRocks(5, new int[5] { 1, 2, 0, 1, 2 });
        yield return waitForRocks;

        MissileVolley(6, new int[6] { 2, 3, 1, 3, 1, 3 }, new int[6] { 0, 1, 2, 3, 4, 5 });
        yield return waitForMissiles;

        yield return new WaitForSecondsRealtime(5);

        BeatLevel(1);
    }
    #endregion

    #region Level 2
    IEnumerator Level2()
    {
        //storm active

        MissileVolley(2, new int[2] { 2, 3 }, new int[2] { 0, 1 });
        yield return waitForMissiles;

        MissileVolley(2, new int[2] { 0, 1 });
        yield return waitForMissiles;

        SpawnRocks(4, new int[4] { 1, 1, 0, 2 });
        yield return waitForRocks;

        //coins - line
        SpawnCoinLine();
        yield return waitForCoins;

        SpawnRocks(5, new int[5] { 1, 1, 3, 1, 1 });
        yield return waitForRocks;

        MissileVolley(5, new int[5] { 1, 2, 3, 4, 5 });
        yield return waitForMissiles;

        SpawnRocks(2, new int[2] { 3, 3 });
        yield return waitForRocks;

        MissileVolley(1, new int[1] { 3 });
        yield return waitForMissiles;

        SpawnRocks(3, new int[3] { 1, 2, 0 });
        yield return waitForRocks;

        MissileVolley(4, new int[4] { 2, 4, 2, 4 }, new int[4] { 0, 1, 2, 3 });
        yield return waitForMissiles;

        SpawnRocks(3, new int[3] { 1, 2, 1 });
        yield return waitForRocks;

        MissileVolley(3, new int[3] { 1, 2, 3 });
        yield return waitForMissiles;

        SpawnRocks(2, new int[2] { 2, 1 });
        yield return waitForRocks;

        MissileVolley(3, new int[3] { 1, 2, 3 }, new int[3] { 1, 0, 1 });
        yield return waitForMissiles;

        //coins- zigzag
        SpawnCoinZig();
        yield return waitForCoins;

        SpawnRocks(2, new int[2] { 2, 3 });
        yield return waitForRocks;

        MissileVolley(1, new int[1] { 2 });
        yield return waitForMissiles;

        SpawnRocks(1, new int[1] { 3 });
        yield return waitForRocks;

        yield return new WaitForSecondsRealtime(5);

        BeatLevel(2);
    }
    #endregion

    #region Level 3
    IEnumerator Level3()
    {
        SpawnRocks(1, new int[1] { 1 });
        yield return waitForRocks;

        MissileVolley(1, new int[1] { 2 });
        yield return waitForMissiles;

        SpawnRocks(2, new int[2] { 2, 1 });
        yield return waitForRocks;

        MissileVolley(3, new int[3] { 1, 2, 3 }, new int[3] { 1, 0, 1 });
        yield return waitForMissiles;

        SpawnRocks(2, new int[2] { 2, 1 });
        yield return waitForRocks;

        TrackingMissileVolley(1);
        yield return waitForMissiles;

        MissileVolley(3, new int[3] { 1, 2, 3 });
        yield return waitForMissiles;

        SpawnRocks(3, new int[3] { 1, 2, 1 });
        yield return waitForRocks;

        MissileVolley(4, new int[4] { 2, 4, 2, 4 }, new int[4] { 0, 1, 2, 3 });
        yield return waitForMissiles;

        SpawnRocks(3, new int[3] { 1, 2, 0 });
        yield return waitForRocks;

        //coins - line
        SpawnCoinLine();
        yield return waitForCoins;

        MissileVolley(1, new int[1] { 3 });
        yield return waitForMissiles;

        TrackingMissileVolley(2);
        yield return waitForMissiles;

        SpawnRocks(2, new int[2] { 3, 3 });
        yield return waitForRocks;

        MissileVolley(5, new int[5] { 1, 2, 3, 4, 5 });
        yield return waitForMissiles;

        SpawnRocks(5, new int[5] { 1, 1, 3, 1, 1 });
        yield return waitForRocks;

        //coins - zigzag
        SpawnCoinZig();
        yield return waitForCoins;

        TrackingMissileVolley(3);
        yield return waitForMissiles;

        SpawnRocks(4, new int[4] { 1, 1, 0, 2 });
        yield return waitForRocks;


        MissileVolley(2, new int[2] { 2, 3 });
        yield return waitForMissiles;

        MissileVolley(2, new int[2] { 2, 3 }, new int[2] { 0, 1 });
        yield return waitForMissiles;

        yield return new WaitForSecondsRealtime(5);

        BeatLevel(3);
    }
    #endregion

    #region Level 4
    IEnumerator Level4()
    {
        SpawnRocks(3, new int[3] { 1, 2, 2 });
        yield return waitForRocks;

        MissileVolley(6, new int[6] { 0, 1, 2, 3, 3, 4 }, new int[6] { 0, 1, 4, 0, 4, 4 });
        yield return waitForMissiles;

        SpawnRocks(4, new int[4] { 1, 1, 3, 2 });
        yield return waitForRocks;

        MissileVolley(4, new int[4] { 0, 1, 3, 3 }, new int[4] { 0, 1, 2, 3 });
        yield return waitForMissiles;

        SpawnRocks(3, new int[3] { 3, 2, 1 });
        yield return waitForRocks;

        MissileVolley(6, new int[6] { 0, 1, 2, 3, 4, 5 }, new int[6] { 0, 0, 0, 3, 3, 3 });
        yield return waitForMissiles;

        SpawnRocks(3, new int[3] { 0, 0, 1 });
        yield return waitForRocks;

        //coins - zigzag
        SpawnCoinZig();
        yield return waitForCoins;

        TorpedoVolleyHigh(1, new int[1] { 0 });
        TorpedoVolleyLow(1, new int[1] { 4 });
        yield return waitForMissiles;

        MissileVolley(2, new int[2] { 1, 2 }, new int[2] { 0, 1 });
        yield return waitForMissiles;

        TorpedoVolleyHigh(2, new int[2] { 0, 1 });
        yield return waitForMissiles;

        SpawnRocks(3, new int[3] { 1, 2, 1 });
        yield return waitForRocks;

        SpawnRocks(2, new int[2] { 1, 1 });
        TorpedoVolleyHigh(2, new int[2] { 3, 4 });
        yield return waitForMissiles;

        MissileVolley(3, new int[3] { 1, 1, 2 }, new int[3] { 0, 1, 0 });
        yield return waitForMissiles;

        //coins - block
        SpawnCoinBlock();
        yield return waitForCoins;

        TorpedoVolleyHigh(2, new int[2] { 3, 4 }, new int[2] { 1, 2 });
        TorpedoVolleyLow(2, new int[2] { 4, 3 }, new int[2] { 0, 1 });
        yield return waitForMissiles;

        MissileVolley(6, new int[6] { 0, 1, 2, 3, 4, 5 });
        yield return waitForMissiles;

        SpawnRocks(4, new int[4] { 2, 2, 0, 1 });
        yield return waitForRocks;

        TorpedoVolleyHigh(4, new int[4] { 0, 1, 1, 4 }, new int[4] { 0, 1, 2, 4 });
        TorpedoVolleyLow(1, new int[1] { 4 });
        yield return waitForMissiles;

        SpawnRocks(3, new int[3] { 3, 1, 1 });
        yield return waitForRocks;

        MissileVolley(3, new int[3] { 1, 2, 3 }, new int[3] { 0, 1, 2 });
        yield return waitForMissiles;

        yield return new WaitForSecondsRealtime(5);

        BeatLevel(4);
    }
    #endregion

    #region Level 5
    IEnumerator Level5()
    {
        MissileVolley(3, new int[3] { 1, 2, 2 }, new int[3] { 0, 1, 2 });
        yield return waitForMissiles;

        SpawnRocks(3, new int[3] { 2, 2, 1 });
        yield return waitForRocks;

        MissileVolley(3, new int[3] { 3, 4, 5 }, new int[3] { 0, 1, 2 });
        TorpedoVolleyHigh(1, new int[1] { 0 }, new int[1] { 2 });
        TorpedoVolleyLow(1, new int[1] { 4 }, new int[1] { 2 });
        yield return waitForMissiles;

        SpawnRocks(4, new int[4] { 2, 2, 2, 2 });
        yield return waitForRocks;

        TorpedoVolleyHigh(2, new int[2] { 1, 0 }, new int[2] { 0, 1 });
        TorpedoVolleyLow(2, new int[2] { 4, 3 }, new int[2] { 2, 3 });
        yield return waitForMissiles;

        MissileVolley(4, new int[4] { 2, 3, 3, 3 }, new int[4] { 0, 1, 2, 3 });
        yield return waitForMissiles;

        SpawnRocks(3, new int[3] { 2, 2, 3 });
        yield return waitForRocks;

        MissileVolley(6, new int[6] { 0, 1, 2, 3, 4, 5 });
        yield return waitForMissiles;

        SpawnRocks(2, new int[2] { 1, 1 });
        yield return waitForRocks;

        TorpedoVolleyHigh(5, new int[5] { 0, 1, 2, 3, 4 }, new int[5] { 0, 0, 0, 3, 4 });
        yield return waitForMissiles;

        SpawnRocks(4, new int[4] { 0, 2, 1, 2 });
        yield return waitForRocks;

        MissileVolley(4, new int[4] { 0, 1, 2, 3 }, new int[4] { 0, 1, 0, 3 });
        yield return waitForMissiles;

        //coins - zigzag
        SpawnCoinZig();
        yield return waitForCoins;

        SpawnRocks(4, new int[4] { 1, 1, 0, 2 });
        yield return waitForRocks;

        TorpedoVolleyHigh(3, new int[3] { 0, 3, 4 }, new int[3] { 0, 1, 1 });
        yield return waitForMissiles;

        MissileVolley(6, new int[6] { 2, 2, 2, 3, 3, 3 }, new int[6] { 0, 1, 2, 0, 1, 2 });
        yield return waitForMissiles;

        TorpedoVolleyHigh(5, new int[5] { 0, 2, 4, 1, 3 }, new int[5] { 0, 0, 0, 3, 3 });
        yield return waitForMissiles;

        //coins - block
        SpawnCoinBlock();
        yield return waitForCoins;

        SpawnRocks(4, new int[4] { 1, 1, 2, 2 });
        yield return waitForRocks;

        TorpedoVolleyHigh(3, new int[3] { 1, 2, 3 });
        yield return waitForMissiles;

        yield return new WaitForSecondsRealtime(5);

        StartCoroutine(SpawnPirateShip());
    }
    #endregion
    #endregion

    #region Level meta
    void BeatLevel(int levelBeat)
    {
        int coinReward;
        switch (levelBeat)
        {
            case 1:
                coinReward = 500;
                break;
            case 2:
                coinReward = 1000;
                break;
            case 3:
                coinReward = 1500;
                break;
            case 4:
                coinReward = 2500;
                break;
            case 5:
                coinReward = 7500;
                break;
            default:
                coinReward = 0;
                break;
        }
        CoinController.controller.ReceiveReward(coinReward);
        PlayerInfo.controller.BeatLevel(levelBeat);
        onWavesCleared();
        EndLevel();
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
    void SpawnCoinBlock(int num = 3, int height = -1)
    {
        CoinController.controller.coinSpawnBlock(num + coinDropRate, num + coinDropRate, height, 20, .15f);
    }

    void SpawnCoinLine(int num = 8, int height = -2)
    {
        CoinController.controller.coinSpawnLine(num + coinDropRate * 3, height, 20, .1f);
    }

    void SpawnCoinZig(int num = 10, int height = 6)
    {
        CoinController.controller.coinSpawnZig(num + coinDropRate * 4, height, -1.5f, 20, .05f);
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

    #region Speed
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
    #endregion

    #region Missiles

	public void EnqueueMissile(GameObject missile)
	{
		missileList.Enqueue (missile);
	}

	public void EnqueueTorpedo(GameObject torpedo)
	{
		torpedoList.Enqueue (torpedo);
	}

	public void EnqueueTrackingMissile(GameObject tM)
	{
		trackingMissileList.Enqueue (tM);
	}

    void InitializeMissiles()
    {
        Vector2 farAway = new Vector2(100, 100);
        for (int i = 0; i < 12; i++)
        {
            GameObject missile = Instantiate(missilePrefab, farAway, Quaternion.identity) as GameObject;
            missile.SetActive(false);
            missile.transform.SetParent(missilesParent);
        }

        for (int i = 0; i < 12; i++)
        {
            GameObject torpedo = Instantiate(torpedoPrefab, farAway, Quaternion.identity) as GameObject;
            torpedo.SetActive(false);
            torpedo.transform.SetParent(missilesParent);
        }

        for (int i = 0; i < 12; i++)
        {
            GameObject trackingMissile = Instantiate(trackingMissilePrefab, farAway, Quaternion.identity) as GameObject;
            trackingMissile.SetActive(false);
            trackingMissile.transform.SetParent(missilesParent);
        }
    }

    /// <summary>
    /// MissileVolley sends out a specified number of missiles that can be at specified heights and order
    /// </summary>
    ///<param name="numMissiles"> 
    ///numMissiles specifies how many missliles you want to spawn. If nothing is put, the default is 6
    ///</param>
    ///<param name="vStates"> 
    ///vStates takes in an integer array of the same length as the number of missiles you are spawning
    ///each integer specifies how high you want to place the missile on a scale from 0 to 5.
    ///0 being the minimum value and 5 being the largest. If no array is given, the height of the missile is chosen at random
    ///</param>
    ///<param name="hStates">
    ///hStates takes in an integer array of the same length as the number of missiles you are spawning
    ///each integer specifies where horizonally you want each missile to be spawned from a scale of 0 to 5.
    ///0 being the leftmost and 5 being the rightmost. If no array is given, the default position is 0, which represents offScreenX + 5.
    /// </param>
    void MissileVolley(int numMissiles = 6, int[] vStates = null, int[] hStates = null)
    {
        AudioController.controller.PlayFX(AudioController.controller.missileFire);
        float minHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .4f)).y;
        float maxHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .85f)).y;
        float offScreenX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0)).x;

        if(vStates == null)
        {
            //nothin
        }
        else if (vStates[0] == -1)
        {
            hStates = new int[numMissiles];
            vStates = new int[numMissiles];
            for (int i = 0; i < numMissiles; i++)
            {
                hStates[i] = (int)Random.Range(0, 6);
                vStates[i] = (int)Random.Range(.5f, 6);
            }
        }

        for (int i = 0; i < numMissiles; i++)
        {
            float tempY;
            float tempX;

            if (vStates == null)
            {
                tempY = Random.Range(minHeight, maxHeight);
            }
            else
            {

                switch (vStates[i])
                {
                    case 0:
                        tempY = minHeight;
                        break;
                    case 1:
                        tempY = minHeight + 1 * ((maxHeight - minHeight) / 5.0f);
                        break;
                    case 2:
                        tempY = minHeight + 2 * ((maxHeight - minHeight) / 5.0f);
                        break;
                    case 3:
                        tempY = minHeight + 3 * ((maxHeight - minHeight) / 5.0f);
                        break;
                    case 4:
                        tempY = minHeight + 4 * ((maxHeight - minHeight) / 5.0f);
                        break;
                    case 5:
                        tempY = minHeight + 5 * ((maxHeight - minHeight) / 5.0f);
                        break;
                    default:
                        tempY = Random.Range(minHeight, maxHeight);
                        break;
                }
            }
            if (hStates == null)
            {
                tempX = offScreenX + 5;
            }
            else
            {
                switch (hStates[i])
                {
                    case 0:
                        tempX = offScreenX + 5;
                        break;
                    case 1:
                        tempX = offScreenX + 10;
                        break;
                    case 2:
                        tempX = offScreenX + 15;
                        break;
                    case 3:
                        tempX = offScreenX + 20;
                        break;
                    case 4:
                        tempX = offScreenX + 25;
                        break;
                    case 5:
                        tempX = offScreenX + 30;
                        break;
                    default:
                        tempX = offScreenX + 5;
                        break;
                }
            }

			GameObject missile = missileList.Dequeue();
            missile.transform.position = new Vector3(tempX, tempY);
            missile.SetActive(true);
        }
    }

    /// <summary>
    /// TorpedoVolleyHigh sends out a specified number of torpedoes that can be at specified heights and order in the upper area of the screen
    /// </summary>
    ///<param name="num"> 
    ///numMissiles specifies how many torpedoes you want to spawn. If nothing is put, the default is 4
    ///</param>
    ///<param name="vStates"> 
    ///vStates takes in an integer array of the same length as the number of missiles you are spawning
    ///each integer specifies how high you want to place the torpedo on a scale from 0 to 5.
    ///0 being the minimum value and 5 being the largest. If no array is given, the height of the torpedo is chosen at random
    ///</param>
    ///<param name="hStates">
    ///hStates takes in an integer array of the same length as the number of torpedoes you are spawning
    ///each integer specifies where horizonally you want each torpedo to be spawned from a scale of 0 to 5.
    ///0 being the leftmost and 5 being the rightmost. If no array is given, the default position is 0, which represents offScreenX + 5.
    /// </param>
    void TorpedoVolleyHigh(int num = 4, int[] vStates = null, int[] hStates = null)
    {
        AudioController.controller.PlayFX(AudioController.controller.sonarPing);
        float minHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .4f)).y;
        float maxHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .85f)).y;
        float offScreenX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0)).x;

        if (vStates == null)
        {
            //nothin
        }
        else if (vStates[0] == -1)
        {
            hStates = new int[num];
            vStates = new int[num];
            for (int i = 0; i < num; i++)
            {
                hStates[i] = (int)Random.Range(0, 6);
                vStates[i] = (int)Random.Range(0, 6);
            }
        }

        for (int i = 0; i < num; i++)
        {
            float tempY;
            float tempX;

            if (vStates == null)
            {
                tempY = Random.Range(minHeight, maxHeight);
            }
            else
            {
                switch (vStates[i])
                {
                    case 0:
                        tempY = minHeight;
                        break;
                    case 1:
                        tempY = minHeight + 1 * ((maxHeight - minHeight) / 5.0f);
                        break;
                    case 2:
                        tempY = minHeight + 2 * ((maxHeight - minHeight) / 5.0f);
                        break;
                    case 3:
                        tempY = minHeight + 3 * ((maxHeight - minHeight) / 5.0f);
                        break;
                    case 4:
                        tempY = minHeight + 4 * ((maxHeight - minHeight) / 5.0f);
                        break;
                    case 5:
                        tempY = minHeight + 5 * ((maxHeight - minHeight) / 5.0f);
                        break;
                    default:
                        tempY = Random.Range(minHeight, maxHeight);
                        break;
                }
            }
            if (hStates == null)
            {
                tempX = offScreenX + 5;
            }
            else
            {
                switch (hStates[i])
                {
                    case 0:
                        tempX = offScreenX + 5;
                        break;
                    case 1:
                        tempX = offScreenX + 10;
                        break;
                    case 2:
                        tempX = offScreenX + 15;
                        break;
                    case 3:
                        tempX = offScreenX + 20;
                        break;
                    case 4:
                        tempX = offScreenX + 25;
                        break;
                    case 5:
                        tempX = offScreenX + 30;
                        break;
                    default:
                        tempX = offScreenX + 5;
                        break;
                }
            }

			GameObject missile = torpedoList.Dequeue();
            missile.transform.position = new Vector3(tempX, tempY);
            missile.SetActive(true);
        }
    }

    /// <summary>
    /// TorpedoVolleyLow sends out a specified number of torpedoes that can be at specified heights and order in the lower area of the screen
    /// </summary>
    ///<param name="num"> 
    ///numMissiles specifies how many torpedoes you want to spawn. If nothing is put, the default is 4
    ///</param>
    ///<param name="vStates"> 
    ///vStates takes in an integer array of the same length as the number of torpedoes you are spawning
    ///each integer specifies how high you want to place the torpedo on a scale from 0 to 5.
    ///0 being the minimum value and 5 being the largest. If no array is given, the height of the torpedo is chosen at random
    ///</param>
    ///<param name="hStates">
    ///hStates takes in an integer array of the same length as the number of torpedoes you are spawning
    ///each integer specifies where horizonally you want each torpedo to be spawned from a scale of 0 to 5.
    ///0 being the leftmost and 5 being the rightmost. If no array is given, the default position is 0, which represents offScreenX + 5.
    /// </param>
    void TorpedoVolleyLow(int num = 4, int[] vStates = null, int[] hStates = null)
    {
        AudioController.controller.PlayFX(AudioController.controller.sonarPing);
        float minHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .075f)).y;
        float maxHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, .4f)).y;
        float offScreenX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0)).x;

        if (vStates == null)
        {
            //nothin
        }
        else if (vStates[0] == -1)
        {
            hStates = new int[num];
            vStates = new int[num];
            for (int i = 0; i < num; i++)
            {
                hStates[i] = (int)Random.Range(0, 6);
                vStates[i] = (int)Random.Range(3, 6);
            }
        }

        for (int i = 0; i < num; i++)
        {
            float tempY;
            float tempX;

            if (vStates == null)
            {
                tempY = Random.Range(minHeight, maxHeight);
            }
            else
            {
                switch (vStates[i])
                {
                    case 0:
                        tempY = minHeight;
                        break;
                    case 1:
                        tempY = minHeight + 1 * ((maxHeight - minHeight) / 5.0f);
                        break;
                    case 2:
                        tempY = minHeight + 2 * ((maxHeight - minHeight) / 5.0f);
                        break;
                    case 3:
                        tempY = minHeight + 3 * ((maxHeight - minHeight) / 5.0f);
                        break;
                    case 4:
                        tempY = minHeight + 4 * ((maxHeight - minHeight) / 5.0f);
                        break;
                    case 5:
                        tempY = minHeight + 5 * ((maxHeight - minHeight) / 5.0f);
                        break;
                    default:
                        tempY = Random.Range(minHeight, maxHeight);
                        break;
                }
            }
            if (hStates == null)
            {
                tempX = offScreenX + 5;
            }
            else
            {
                switch (hStates[i])
                {
                    case 0:
                        tempX = offScreenX + 5;
                        break;
                    case 1:
                        tempX = offScreenX + 10;
                        break;
                    case 2:
                        tempX = offScreenX + 15;
                        break;
                    case 3:
                        tempX = offScreenX + 20;
                        break;
                    case 4:
                        tempX = offScreenX + 25;
                        break;
                    case 5:
                        tempX = offScreenX + 30;
                        break;
                    default:
                        tempX = offScreenX + 5;
                        break;
                }
            }

			GameObject missile = torpedoList.Dequeue();
            missile.transform.position = new Vector3(tempX, tempY);
            missile.SetActive(true);
        }
    }

	#endregion

    #region Tracking Missiles
    void TrackingMissileVolley(int amount, int leftOrRight = 0)
    {
        StartCoroutine(SpawntrackingMissilePrefabs(amount, leftOrRight));
    }
    IEnumerator SpawntrackingMissilePrefabs(int missileCount, int leftOrRight = 0)
    {
		Debug.Log ("Times spawn tracking missiles is called");
        Vector2 offScreen;
        if (leftOrRight == 0)
        {
            offScreen = Camera.main.ViewportToWorldPoint(new Vector2(Random.Range(0.0f, 1.0f), 1.2f));
        }
        else if( leftOrRight > 0 )
        {
            offScreen = Camera.main.ViewportToWorldPoint(new Vector2(Random.Range(0.7f, 1.4f), 1.2f));
        }
        else
        {
           offScreen = Camera.main.ViewportToWorldPoint(new Vector2(Random.Range(-0.4f, 0.3f), 1.2f));
        }

        for (int i = 0; i < missileCount; i++)
        {
			GameObject trackMissile = trackingMissileList.Dequeue();
            trackMissile.transform.position = offScreen;
            trackMissile.SetActive(true);

            yield return new WaitForSeconds(2.5f);
        }
    }
    #endregion

    #region Rocks
	public void EnqueueRock(GameObject rock)
	{
		rockList.Enqueue (rock);
	}
    void InitializeRocks()
    {
        for (int i = 0; i < 12; i++)
        {
            GameObject rock = Instantiate(rockPrefab, new Vector2(100, 100), Quaternion.identity) as GameObject;
            rock.SetActive(false);
            rock.transform.SetParent(rocksParent);
        }
    }

    /// <summary>
    /// SpawnRocks is used to spawn a cluster of rocks of various amount, height, and distance from each other
    /// </summary>
    /// <param name="amount">
    /// amount specifies how many rocks you want to spawn
    /// </param>
    /// <param name="states">
    /// states takes in an integer array of the same length of the ammount of rocks you want to spawn.
    /// Each integer represents how high you want the rock to spawn in on a scale from 0 to 4.
    /// If no array is given, the rocks will spawn at random height.
    /// </param>
    /// <param name="dist">
    /// dist specifies how far apart the rocks will be. If no value is given, the default is 10.
    /// </param>
    void SpawnRocks(int amount, int[] states = null, int dist = 10)
    {
        float minRockHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.03f)).y;
        float maxRockHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.25f)).y;
        float offScreen = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0)).x;

        float height1 = Camera.main.ViewportToWorldPoint(new Vector3(0, .10f)).y;
        float height2 = Camera.main.ViewportToWorldPoint(new Vector3(0, .15f)).y;
        float height3 = Camera.main.ViewportToWorldPoint(new Vector3(0, .20f)).y;

        if (states == null)
        {
            //nothin
        }
        else if(states[0] == -1){
            states = new int[amount];
            for (int i = 0; i < amount; i++)
            {
                states[i] = (int)Random.Range(0, 4);
            }
        }

        for (int i = 0; i < amount; i++)
        {
			GameObject rock = rockList.Dequeue();
            rock.SetActive(true);
            if (states == null || states.Length > amount)
            {
                rock.transform.position = new Vector3(offScreen + (dist * i), Random.Range(minRockHeight, maxRockHeight));
            }
            else
            {
                switch (states[i])
                {
                    case 0:
                        rock.transform.position = new Vector3(offScreen + (dist * i), minRockHeight);           //When water is level, boat will not run into it
                        break;
                    case 1:
                        rock.transform.position = new Vector3(offScreen + (dist * i), height1);                 //When water is level, boat will just barely run into it
                        break;
                    case 2:
                        rock.transform.position = new Vector3(offScreen + (dist * i), height2);                 //Sticks out of the water a bit
                        break;
                    case 3:
                        rock.transform.position = new Vector3(offScreen + (dist * i), height3);                 //Sticks out of the water a fair ammount
                        break;
                    case 4:
                        rock.transform.position = new Vector3(offScreen + (dist * i), maxRockHeight);           //Sticks out of the water a lot
                        break;
                    default:
                        rock.transform.position = new Vector3(offScreen + (dist * i), Random.Range(minRockHeight, maxRockHeight));
                        break;
                }
            }
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
        Instantiate(pirateShipPrefab, Camera.main.ViewportToWorldPoint(new Vector2(1.2f, .5f)), Quaternion.identity);

		while (EnemyBoat.currentBoss.CheckIfAlive() && EnemyBoat.currentBoss.GetPhase() != 2)
        {
            while (EnemyBoat.currentBoss.GetPhase() == 1)
            {
                yield return new WaitForSecondsRealtime(3);
                if (Random.Range(0, 10) > 3)
                {
                    SpawnRocks(3, new int[3] { 0, 0, 0 });
                    yield return waitForRocks;
                }
                else
                {
                    SpawnRocks(1, new int[1] { 0});
                    yield return waitForRocks;
                }
            }
            while (EnemyBoat.currentBoss.GetPhase() == 0)
            {
				TrackingMissileVolley(5, -1);
                yield return new WaitForSecondsRealtime(10);
            }
            while (EnemyBoat.currentBoss.GetPhase() == 2)
            {
				yield return null;
            }
            yield return null;
        }

        BeatLevel(5);
    }
    #endregion                                  ///STUFF TO DO HERE

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

    void UpdateAllSpeedMults(float mult)
    {
        Missile.GiveSpeedMultiplier(mult);
        Rock.GiveSpeedMultiplier(mult);
        Coin.GiveSpeedMultiplier(mult);
        HealthPickup.GiveSpeedMultiplier(mult);
    }


}