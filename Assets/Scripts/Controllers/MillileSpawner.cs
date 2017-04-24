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

    int levelsBeaten;


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
    delegate void MissileDelegate(int numMissiles, int[] vStates, int[] hStates);

    MissileDelegate MissileDel;
    EndlessObstacleSpawn ObstacleSpawn1;
    EndlessObstacleSpawn ObstacleSpawn2;
    EndlessObstacleSpawn CoinSpawner;

    #endregion

    void OnEnable()
    {
    }

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

    #region Level and Wave Management

    public void StartGame()//int level = 0)
    {
        if (GameModeController.controller.CheckCurrentMode(GameModeController.Mode.Story))
            StartCoroutine(Level1());
        else
            StartCoroutine(EndlessWave());

        //Dont want to mess with what you currently have, but want to show what I had planned for level selection
        //Might be easier to just call Start game with a level parameter (default 0 = endless) Code:
        /*  Code to swap in:
        switch (level)
        {
            case 0:
                StartCoroutine(EndlessWave());
                break;
            case 1:
                StartCoroutine(Level1());
                break;
            case 2:
                StartCoroutine(Level2());
                break;
            case 3:
                StartCoroutine(Level3());
                break;
            case 4:
                StartCoroutine(Level4());
                break;
            case 5:
                StartCoroutine(Level5());
                break;
        }
        */

        if (Boat.player.GetMaxHealth() > 1)
            Invoke("SpawnHealth", 45);
    }

    #region Waves
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
        // float timeSpeedUp = 0;

        // MissileDel += MissileVolley(5, new int[] {1},)
        // ObstacleSpawn1 += MissileVolley;
        // ObstacleSpawn2 += TorpedoVolleyLow;
        // CoinSpawner += SpawnCoinBlock;
        // while (true)
        // {
        //     ObstacleSpawn1();
        //     yield return new WaitForSeconds(3.5f - timeSpeedUp);

        //     ObstacleSpawn2();
        //     yield return new WaitForSeconds(4 - timeSpeedUp);

        //     CoinSpawner();
        //     yield return new WaitForSeconds(4 + timeSpeedUp * 2);

        //     waveCounter++;

        //     if (waveCounter == 2)
        //     {
        //         ObstacleSpawn1 += SpawnRocks;
        //     }
        //     else if (waveCounter == 3)
        //     {
        //         CoinSpawner = SpawnBlockAndLine;
        //     }
        //     else if (waveCounter == 4)
        //     {
        //         ObstacleSpawn2 -= TorpedoVolleyLow;
        //         ObstacleSpawn2 = TorpedoVolley;
        //     }
        //     else if (waveCounter == 5)
        //     {
        //         ObstacleSpawn1 += TrackingMissileVolley;
        //     }
        //     else if (waveCounter == 8)
        //     {
        //         ObstacleSpawn2 += TrackingMissileVolley;
        //     }
        //     else if (waveCounter == 10)
        //     {
        //         CoinSpawner = SpawnAllCoins;
        //     }
        //     else if (waveCounter == 12)
        //     {
        //         ObstacleSpawn2 += SpawnRocks;
        //     }
        //     else if (waveCounter == 15)
        //     {
        //         ObstacleSpawn1 += TrackingMissileVolley;
        //         ObstacleSpawn2 += TrackingMissileVolley;
        //         CoinSpawner += TrackingMissileVolley;
        //     }

        //     if (waveCounter < 20)
        //         timeSpeedUp += .15f;

        //     UpdateSpeedMultiplier(1.75f);
        //     yield return null;
        // }
        yield return null;
    }
    #endregion

    #region Levels
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

        SpawnRocks(5, new int[5] { 1, 2, 0, 1 ,2});
        yield return waitForRocks;

        MissileVolley(6, new int[6] { 2, 3, 1, 3, 1, 3 }, new int[6] { 0, 1, 2, 3, 4, 5 });
        yield return waitForMissiles;

        yield return new WaitForSeconds(5);

        BeatLevel(1);
        EndLevel();

    }

    IEnumerator Level2()
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

        SpawnRocks(2, new int[2] { 3, 3 });
        yield return waitForRocks;

        MissileVolley(5, new int[5] { 1, 2, 3, 4, 5 });
        yield return waitForMissiles;

        SpawnRocks(5, new int[5] { 1, 1, 3, 1, 1 });
        yield return waitForRocks;

        //coins - zigzag
        SpawnCoinZig();
        yield return waitForCoins;

        SpawnRocks(4, new int[4] { 1, 1, 0, 2 });
        yield return waitForRocks;

        MissileVolley(2, new int[2] { 2, 3 });
        yield return waitForMissiles;

        MissileVolley(2, new int[2] { 2, 3 }, new int[2] { 0, 1 });
        yield return waitForMissiles;

        BeatLevel(2);
        EndLevel();
    }

    IEnumerator Level3()
    {
        //storm active

        MissileVolley(2, new int[2] { 2, 3 }, new int[2] { 0, 1 });
        yield return waitForMissiles;

        MissileVolley(2, new int[2] { 2, 3 });
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

        BeatLevel(3);
        EndLevel();
    }

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

        BeatLevel(4);
        EndLevel();
    }

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

        TorpedoVolleyHigh(4, new int[4] { 1, 2, 3, 4 });
        yield return waitForMissiles;

        StartCoroutine(SpawnPirateShip());
    }
    #endregion

    public int GetLevelsBeaten()
    {
        return levelsBeaten;
    }

    public void SetLevelsBeaten(int levelsBeat)
    {
        levelsBeaten = levelsBeat;
    }

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

        for (int i = 0; i < numMissiles; i++)
        {
            int j = 0;
            while (missileList[j].activeInHierarchy)
                j++;

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

            GameObject missile = missileList[j].gameObject;
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

        for (int i = 0; i < num; i++)
        {
            int j = 0;
            while (torpedoList[j].activeInHierarchy)
                j++;

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

            GameObject missile = torpedoList[j].gameObject;
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

        for (int i = 0; i < num; i++)
        {
            int j = 0;
            while (torpedoList[j].activeInHierarchy)
                j++;

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

            GameObject missile = torpedoList[j].gameObject;
            missile.transform.position = new Vector3(tempX, tempY);
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
        for (int i = 0; i < 12; i++)
        {
            GameObject rock = Instantiate(rockPrefab, new Vector2(100, 100), Quaternion.identity) as GameObject;
            rockList.Add(rock);
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

        for (int i = 0; i < amount; i++)
        {
            int j = 0;
            while (rockList[j].activeInHierarchy)
                j++;

            GameObject rock = rockList[j];
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
        Instantiate(pirateShipPrefab, Camera.main.ViewportToWorldPoint(new Vector2(1.1f, .6f)), Quaternion.identity);

        while (EnemyBoat.currentBoss.GetPhase() != 2 && EnemyBoat.currentBoss.CheckIfAlive())
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

        BeatLevel(5);
        EndLevel();

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