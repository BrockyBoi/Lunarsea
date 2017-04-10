using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempGoalController : MonoBehaviour
{
    public static TempGoalController controller;

    List<TempGoal> goals = new List<TempGoal>();

    bool[] hasGoals = new bool[(int)TempGoal.Goal.MAX_GOALS];
    public const int hasDistance = 0;
    public const int hasCoin = 1;
    public const int hasMissile = 2;
    public const int hasTimesPlayed = 3;
    public const int hasAvoidDamage = 4;
    int missileCount;

    [SerializeField]
    int maxGoals;
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
        if (PlayerInfo.controller.DontLoadOnStart)
            NewGoals();
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
        if (!hasGoals[hasMissile])
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
        if (!hasGoals[hasDistance] && !hasGoals[hasAvoidDamage])
            return;

        for (int i = 0; i < goals.Count; i++)
        {
            if (goals[i].CheckIfGoalExists(TempGoal.Goal.Distance) || goals[i].CheckIfGoalExists(TempGoal.Goal.AvoidDamage))
                goals[i].UpdateProgress(0);
        }
    }

    public void UpdateCoinGoals()
    {
        if (!hasGoals[hasCoin])
            return;

        for (int i = 0; i < goals.Count; i++)
        {
            if (goals[i].CheckIfGoalExists(TempGoal.Goal.Coin))
                goals[i].UpdateProgress(0);
        }
    }

    public void UpdateTimesPlayedGoals()
    {
        if (!hasGoals[hasTimesPlayed])
            return;

        for (int i = 0; i < goals.Count; i++)
        {
            if (goals[i].CheckIfGoalExists(TempGoal.Goal.TimesPlayed))
                goals[i].UpdateProgress(0);
        }
    }

    public void UpdateMaxGoals(int value)
    {
        maxGoals = 1 + value;
        NewGoals();
    }
    #endregion

    public void FinishGoal(int spot, int rewardValue)
    {
        CoinController.controller.ReceiveReward(rewardValue);

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
                newGoal = Random.Range(0, 8);
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
                default:
                    break;
            }
        }

        CheckCurrentGoals();
    }

    public void PlayerDied()
    {
        for (int i = 0; i < goals.Count; i++)
        {
            goals[i].PlayerDied();
        }
        NewGoals();
    }
}