using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TempGoalController : MonoBehaviour
{
    public static TempGoalController controller;

    List<TempGoal> goals = new List<TempGoal>();

    bool[] hasGoals = new bool[(int)TempGoal.Goal.MAX_GOALS];
    public Sprite[] goalImages = new Sprite[(int)TempGoal.Goal.MAX_GOALS];

    int missileCount;

    float totalScoreMultiplier = 1;
    float tempScoreMultiplier = 0;
    float rewardScoreMults = 0;

    [SerializeField]
    int maxGoals;

    void OnEnable()
    {
    }
    void Awake()
    {
        //DontDestroyOnLoad(this);
        if (controller == null)
        {
            controller = this;
        }
        else if (controller != this)
        {
            Destroy(gameObject);
        }
    }
    // Use this for initialization
    void Start()
    {
        Boat.player.onBoatDeath += PlayerDied;
        if (PlayerInfo.controller.ResetSaveFile)
            NewGoals();
    }

    public TempGoal GetGoal(int spot)
    {
        return goals[spot];
    }

    public List<TempGoal> GetGoals()
    {
        return goals;
    }

    public void SetGoals(List<TempGoal> tG)
    {
        goals = new List<TempGoal>(tG);
        CheckCurrentGoals();
    }

    public int GetGoalListCount()
    {
        return goals.Count;
    }

    void CheckCurrentGoals()
    {
        for (int i = 0; i < goals.Count; i++)
        {
            hasGoals[goals[i].GetGoalType()] = true;
        }
    }

    public void MissileDestryoed()
    {
        missileCount++;
        UpdateMissileGoals();
    }

    public int GetMissileCount()
    {
        return missileCount;
    }

    public string GetGoalDescription(int goal)
    {
        return goals[goal].GetDescription();
    }

    #region Update Goals
    void UpdateMissileGoals()
    {
        if (!hasGoals[(int)TempGoal.Goal.MissilesDestroyed])
            return;

        for (int i = 0; i < goals.Count; i++)
        {
            if (goals[i].CheckIfGoalExists(TempGoal.Goal.MissilesDestroyed))
            {
                goals[i].UpdateProgress(missileCount);
            }
        }
    }

    public void UpdateDistanceGoals()
    {
        if (!hasGoals[(int)TempGoal.Goal.Distance] && !hasGoals[(int)TempGoal.Goal.AvoidDamage])
            return;

        for (int i = 0; i < goals.Count; i++)
        {
            if (goals[i].CheckIfGoalExists(TempGoal.Goal.Distance) || goals[i].CheckIfGoalExists(TempGoal.Goal.AvoidDamage))
                goals[i].UpdateProgress(0);
        }
    }

    public void UpdateCoinGoals()
    {
        if (!hasGoals[(int)TempGoal.Goal.Coin])
            return;

        for (int i = 0; i < goals.Count; i++)
        {
            if (goals[i].CheckIfGoalExists(TempGoal.Goal.Coin))
                goals[i].UpdateProgress(0);
        }
    }

    public void UpdateTimesPlayedGoals()
    {
        if (!hasGoals[(int)TempGoal.Goal.TimesPlayed])
            return;

        for (int i = 0; i < goals.Count; i++)
        {
            if (goals[i].CheckIfGoalExists(TempGoal.Goal.TimesPlayed))
                goals[i].UpdateProgress(0);
        }
    }

    public void UpdateMaxGoals(int value)
    {
        maxGoals = 3 + value;
        NewGoals();
    }
    #endregion

    public void FinishGoal(int spot, int rewardValue)
    {
        CoinController.controller.ReceiveReward(rewardValue * Mathf.RoundToInt(totalScoreMultiplier));

        hasGoals[goals[spot].GetGoalType()] = false;

        goals.RemoveAt(spot);
        RedoSpotsInList();
    }

    void RedoSpotsInList()
    {
        for (int i = 0; i < goals.Count; i++)
        {
            goals[i].UpdateSpotInList(i);
        }
    }

    bool CheckIfGoalNumExists(int goalNum)
    {
        for (int i = 0; i < goals.Count; i++)
        {
            if (goals[i].GetGoalNum() == goalNum)
            {
                return true;
            }
        }

        return false;
    }

    void NewGoals()
    {
        int max = maxGoals - goals.Count;
        for (int i = 0; i < max; i++)
        {
            int newGoal = 0;
            do
            {
                newGoal = Random.Range(0, 11);
            } while (CheckIfGoalNumExists(newGoal));

            int currentCount = goals.Count;

            switch (newGoal)
            {
                case 0:
                    goals.Add(new TempGoal(currentCount, TempGoal.Goal.Distance, 300, false, 100, newGoal));
                    break;
                case 1:
                    goals.Add(new TempGoal(currentCount, TempGoal.Goal.Coin, 50, true, 50, newGoal));
                    break;
                case 2:
                    goals.Add(new TempGoal(currentCount, TempGoal.Goal.MissilesDestroyed, 30, false, 150, newGoal));
                    break;
                case 3:
                    goals.Add(new TempGoal(currentCount, TempGoal.Goal.TimesPlayed, 3, false, 150, newGoal));
                    break;
                case 4:
                    goals.Add(new TempGoal(currentCount, TempGoal.Goal.Distance, 100, true, 100, newGoal));
                    break;
                case 5:
                    goals.Add(new TempGoal(currentCount, TempGoal.Goal.Coin, 200, false, 200, newGoal));
                    break;
                case 6:
                    goals.Add(new TempGoal(currentCount, TempGoal.Goal.MissilesDestroyed, 10, true, 75, newGoal));
                    break;
                case 7:
                    goals.Add(new TempGoal(currentCount, TempGoal.Goal.AvoidDamage, 50, true, 75, newGoal));
                    break;
                case 8:
                    goals.Add(new TempGoal(currentCount, TempGoal.Goal.MissilesDestroyed, 75, false, 250, newGoal));
                    break;
                case 9:
                    goals.Add(new TempGoal(currentCount, TempGoal.Goal.Distance, 1000, false, 300, newGoal));
                    break;
                case 10:
                    goals.Add(new TempGoal(currentCount, TempGoal.Goal.AvoidDamage, 100, true, 150, newGoal));
                    break;
                default:
                    break;
            }
        }

        CheckCurrentGoals();
    }

    void PlayerDied()
    {
        for (int i = 0; i < goals.Count; i++)
        {
            goals[i].PlayerDied();
        }
        NewGoals();
    }

    public float GetScoreMultiplier()
    {
        return totalScoreMultiplier;
    }
    public void SetScoreMultiplier(float f)
    {
        totalScoreMultiplier = f + rewardScoreMults + tempScoreMultiplier;
    }

    public void AddRewardScoreMultipliers(float f)
    {
        rewardScoreMults += f;
        SetScoreMultiplier(0);
    }

    public void AddTempScoreMultiplier(float f)
    {
        tempScoreMultiplier += f;
        SetScoreMultiplier(0);
    }

    public void ResetTempMults()
    {
        tempScoreMultiplier = 0;
        SetScoreMultiplier(0);
    }
}
