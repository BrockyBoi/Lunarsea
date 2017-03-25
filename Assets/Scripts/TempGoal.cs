using UnityEngine;
[System.Serializable]
public class TempGoal : System.Object
{

    public enum Goal { Distance, Coin, MissilesDestroyed, TimesPlayed, AvoidDamage, MAX_GOALS }
    int goalType;
    float currentProgress, targetGoal;
    bool perLife;
    int rewardValue;

    bool rewardFulfilled;
    int spot;
    Goal myGoal;
    public TempGoal(int spotInArray, Goal g, float target, bool oncePerLife, int reward)
    {
        spot = spotInArray;
        targetGoal = target;
        perLife = oncePerLife;
        rewardValue = reward;
        myGoal = g;
        goalType = (int)g;
    }

    public void UpdateProgress(float missileCount)
    {
        if (rewardFulfilled)
            return;

        switch (myGoal)
        {
            case Goal.Distance:
                CheckIfGoalIsMet(MainCanvas.controller.GetScore());
                break;
            case Goal.Coin:
                CheckIfGoalIsMet(CoinController.controller.GetTempCoinNum());
                break;
            case Goal.MissilesDestroyed:
                CheckIfGoalIsMet(missileCount);
                break;
            case Goal.TimesPlayed:
                CheckIfGoalIsMet(DeathCounter.controller.GetDeathCount());
                break;
            case Goal.AvoidDamage:
                if (!Boat.player.CheckIfTookDamage())
                    CheckIfGoalIsMet(MainCanvas.controller.GetScore());
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
        TempGoalController.controller.FinishGoal(spot, rewardValue);
        Debug.Log("Fulfilled goal # " + spot);
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
