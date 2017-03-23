using UnityEngine;
[System.Serializable]
public class TempGoal : System.Object
{

    public enum Goal { Distance, Coin, MissilesDestroyed, MAX_GOALS }
    int goalType;
    float currentProgress, targetGoal;
    bool perLife;
    int rewardValue;

    bool rewardFulfilled;
    int spot;
    public TempGoal(int spotInArray, Goal g, float target, bool oncePerLife, int reward)
    {
        spot = spotInArray;
        goalType = (int)g;
        targetGoal = target;
        perLife = oncePerLife;
        rewardValue = reward;
    }

    public void UpdateProgress(float missileCount)
    {
        if (rewardFulfilled)
            return;

        switch (goalType)
        {
            case (int)Goal.Distance:
                CheckIfGoalIsMet(MainCanvas.controller.GetScore());
                break;
            case (int)Goal.Coin:
                CheckIfGoalIsMet(CoinController.controller.GetTempCoinNum());
                break;
            case (int)Goal.MissilesDestroyed:
                CheckIfGoalIsMet(missileCount);
                break;
            default:
                break;
        }
    }

    public void UpdateSpotInList(int spot)
    {
        this.spot = spot;
    }

    void CheckIfGoalIsMet(float input)
    {
        if (rewardFulfilled)
            return;
        Debug.Log((input + currentProgress) + " out of " + targetGoal);
        if (perLife)
        {
            if (input >= targetGoal)
                FulfillGoal();
        }
        else
        {
            if (input + currentProgress >= targetGoal)
                FulfillGoal();
        }
    }


    void FulfillGoal()
    {
        rewardFulfilled = true;
        Debug.Log("Money before: " + CoinController.controller.getCoinNum());
        TempGoalController.controller.FinishGoal(spot, rewardValue);
        Debug.Log("Money after: " + CoinController.controller.getCoinNum());
    }

    public void PlayerDied()
    {
        if (perLife)
            return;

        switch (goalType)
        {
            case (int)Goal.Distance:
                currentProgress += MainCanvas.controller.GetScore();
                break;
            case (int)Goal.Coin:
                currentProgress += CoinController.controller.GetTempCoinNum();
                break;
            case (int)Goal.MissilesDestroyed:
                currentProgress += TempGoalController.controller.GetMissileCount();
                break;
            default:
                break;
        }
    }

    public int GetGoalType()
    {
        return goalType;
    }
    public bool CheckIfGoalExists(Goal g)
    {
        if (goalType == (int)g)
            return true;

        return false;
    }
}
