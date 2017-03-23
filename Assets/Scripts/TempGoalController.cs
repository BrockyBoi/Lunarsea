using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempGoalController : MonoBehaviour
{
    public static TempGoalController controller;

    List<TempGoal> goals = new List<TempGoal>();

    public bool[] hasGoals = new bool[3];
    public const int hasDistance = 0;
    public const int hasCoin = 1;
    public const int hasMissile = 2;
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
        for (int i = 0; i < goals.Count; i++)
        {
            goals[i].UpdateSpotInList(i);
        }
    }

    void NewGoals()
    {
		int max = maxGoals - goals.Count;
        for (int i = 0; i < max; i++)
        {
            int newGoal = Random.Range(0, 3);
			int currentCount = goals.Count;
			Debug.Log(newGoal);

            switch (newGoal)
            {
                case 0:
                    goals.Add(new TempGoal(currentCount, TempGoal.Goal.Distance, 5, false, 500));
                    break;
                case 1:
                    goals.Add(new TempGoal(currentCount, TempGoal.Goal.Coin, 5, true, 100));
                    break;
                case 2:
                    goals.Add(new TempGoal(currentCount, TempGoal.Goal.MissilesDestroyed, 2, false, 500));
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
