using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempGoalController : MonoBehaviour
{
    public static TempGoalController controller;

    //YOU REALLY GOTTA FIX THIS SHIT BROCK HALF OF IT IS A LIST AND THE OTHER HALF IS AN ARRAY!  YOU GOTTA FIX IT!
    List<TempGoal> goals = new List<TempGoal>();

    public bool[] hasGoals = new bool[3];
    public const int hasDistance = 0;
    public const int hasCoin = 1;
    public const int hasMissile = 2;
    int missileCount;

	int maxGoals;
    void Awake()
    {
        DontDestroyOnLoad(this);
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
        goals.Add(new TempGoal(0, TempGoal.Goal.Distance, 20, false, 500));
        hasGoals[hasDistance] = true;

        goals.Add(new TempGoal(1, TempGoal.Goal.Coin, 50, true, 100));
        hasGoals[hasCoin] = true;

        goals.Add(new TempGoal(2, TempGoal.Goal.MissilesDestroyed, 20, false, 500));
        hasGoals[hasMissile] = true;

    }

    public List<TempGoal> GetGoals()
    {
        return goals;
    }

    public void SetGoals(List<TempGoal> tG)
    {
        goals = new List<TempGoal>(tG);
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
        if (!hasGoals[hasDistance])
            return;

        for (int i = 0; i < goals.Count; i++)
        {
            if (goals[i].CheckIfGoalExists(TempGoal.Goal.Distance))
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

	public void UpdateMaxGoals(int value)
	{
		maxGoals = 1 + value;
	}

    public void FinishGoal(int spot, int rewardValue)
    {
        CoinController.controller.ReceiveReward(rewardValue);

        hasGoals[goals[spot].GetGoalType()] = false;

        goals.RemoveAt(spot);
		RedoSpotsInList();
    }

	void RedoSpotsInList()
	{
		for(int i = 0; i < goals.Count; i++)
		{
			goals[i].UpdateSpotInList(i);
		}
	}

    public void PlayerDied()
    {
        for (int i = 0; i < goals.Count; i++)
        {
            goals[i].PlayerDied();
        }
    }
}
