using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainCanvas : MonoBehaviour
{

    public static MainCanvas controller;
    public GameObject deathScreen;
    public GameObject upgradeScreen;
    public GameObject monetizationScreen;
    public Text scoreText;
    public Text highScoreText;
    public Text coinText;
    public List<Image> healthImages;
    public List<Button> upgradeButtons;
    bool[] canUseButton;

    public Image blackScreen;
    float score = 0.0f;
    float highScore;

    public float speedMult;

    bool levelEnded;

    public GameObject tempGoalDisplay;

    Queue<string> completedGoalDescs = new Queue<string>();

    bool displayingCompletedGoals;

    bool bossBattle;
    public GameObject bossHealthSlider;

    public Transform particles;

    void OnEnable()
    {

    }

    void Awake()
    {
        controller = this;
        canUseButton = new bool[upgradeButtons.Count];
        for (int i = 0; i < canUseButton.Length; i++)
        {
            canUseButton[i] = true;
        }
    }

    void Start()
    {
        Boat.player.onBoatDeath += HealthChange;
        Boat.player.onBoatDeath += DeathScreen;
        Boat.player.onBoatDeath += EndLevel;

        MillileSpawner.controller.onWavesCleared += EndLevel;

        TutorialController.controller.onFinishTutorial += StartLevel;
        levelEnded = true;
        deathScreen.SetActive(false);
        bossHealthSlider.SetActive(false);
        monetizationScreen.SetActive(false);
    }


    void FixedUpdate()
    {
        UpdateScore();
    }

    void UpdateScore()
    {
        if (levelEnded || bossBattle)
            return;


        score += Time.fixedDeltaTime + (Time.deltaTime * speedMult);

        scoreText.text = "Score: " + string.Format("{0:0.0}", score) + " m";

        TempGoalController.controller.UpdateDistanceGoals();

        if (score > highScore)
        {
            highScore = score;
            highScoreText.text = "High Score: " + string.Format("{0:0.0}", highScore) + " m";
        }

    }

    void StartLevel()
    {
        StartCoroutine(ShowGoalsAtStart());
        TempGoalController.controller.UpdateTimesPlayedGoals();
        levelEnded = false;
    }
    void EndLevel()
    {
        levelEnded = true;
    }

    void DeathScreen()
    {
        deathScreen.SetActive(true);
    }

    #region Press Button
    public void PressRetry()
    {
        PlayerInfo.controller.Save();
        SceneManager.LoadScene("MainScene");
    }

    public void PressMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void PressStore()
    {
        monetizationScreen.SetActive(true);
        upgradeScreen.SetActive(false);
    }

    public void PressBackMonetization()
    {
        monetizationScreen.SetActive(false);
        upgradeScreen.SetActive(true);
    }

    #endregion

    public void HealthChange()
    {

        int health = Boat.player.GetHealth();
        for (int i = 0; i < healthImages.Count; i++)
        {
            if (i < health)
            {
                healthImages[i].gameObject.SetActive(true);
            }
            else
                healthImages[i].gameObject.SetActive(false);
        }
    }
    #region High Score Accessors
    public float GetScore()
    {
        return score;
    }

    public float GetHighScore()
    {
        return highScore;
    }

    public void SetHighScore(float f)
    {
        highScore = f;
        highScoreText.text = "High Score: " + string.Format("{0:0.0}", highScore) + " m";
    }
    #endregion

    public void FinishLevel()
    {
        StartCoroutine(FadeToBlack());
    }

    IEnumerator FadeToBlack()
    {
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * .5f;
            Color c = blackScreen.color;
            blackScreen.color = new Color(c.r, c.b, c.g, time);
            yield return null;
        }
    }

    IEnumerator ShowGoalsAtStart()
    {
        float time = 0;
        for (int i = 0; i < TempGoalController.controller.GetGoalListCount(); i++)
        {
            tempGoalDisplay.GetComponentInChildren<Text>().text = TempGoalController.controller.GetGoalDescription(i);
            tempGoalDisplay.transform.GetChild(1).GetComponent<Image>().sprite = TempGoalController.controller.goalImages[TempGoalController.controller.GetGoal(i).GetGoalType()];
            //tempGoalDisplay.GetComoponentInChildren<Image>() = Some Image that I'll store somewhere

            RectTransform trans = tempGoalDisplay.GetComponent<RectTransform>();
            Vector3 startingPoint = trans.anchoredPosition;
            while (time < 1)
            {
                trans.anchoredPosition = Vector3.Lerp(startingPoint, new Vector3(startingPoint.x, 0), time);
                time += Time.deltaTime;
                yield return null;
            }

            Vector3 endPoint = trans.anchoredPosition;
            time = 0;

            yield return new WaitForSeconds(1);
            while (time < 1)
            {
                trans.anchoredPosition = Vector3.Lerp(endPoint, startingPoint, time);
                time += Time.deltaTime;
                yield return null;
            }
            trans.anchoredPosition = startingPoint;

            time = 0;

            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator ShowCompletedGoal()
    {
        float time = 0;
        displayingCompletedGoals = true;
        do
        {
            tempGoalDisplay.GetComponentInChildren<Text>().text = completedGoalDescs.Peek();
            //tempGoalDisplay.GetComoponentInChildren<Image>() = Some Image that I'll store somewhere

            RectTransform trans = tempGoalDisplay.GetComponent<RectTransform>();
            Vector3 startingPoint = trans.anchoredPosition;
            while (time < 1)
            {
                trans.anchoredPosition = Vector3.Lerp(startingPoint, new Vector3(startingPoint.x, 0), time);
                time += Time.deltaTime;
                yield return null;
            }

            Vector3 endPoint = trans.anchoredPosition;
            time = 0;

            yield return new WaitForSeconds(1);
            while (time < 1)
            {
                trans.anchoredPosition = Vector3.Lerp(endPoint, startingPoint, time);
                time += Time.deltaTime;
                yield return null;
            }
            trans.anchoredPosition = startingPoint;

            time = 0;

            yield return new WaitForSeconds(1);
            completedGoalDescs.Dequeue();
        }
        while (completedGoalDescs.Count != 0);

        displayingCompletedGoals = false;

    }

    public void CompleteGoal(string desc)
    {
        completedGoalDescs.Enqueue(desc);
        if (!displayingCompletedGoals)
            StartCoroutine(ShowCompletedGoal());
    }

    public void UpdateCoinString(int coinAmount)
    {
        coinText.text = "Coins: " + coinAmount.ToString();
    }

    #region Upgrade Buttons
    public void DisableUpgradeButton(int buttonNum)
    {
        upgradeButtons[buttonNum].gameObject.GetComponent<Image>().color = new Color(100 / 255f, 100 / 255f, 100 / 255f, 168 / 255f);
        canUseButton[buttonNum] = false;
    }

    public void EnableUpgradeButton(int buttonNum)
    {
        upgradeButtons[buttonNum].gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 168 / 255f);
        canUseButton[buttonNum] = true;
    }

    public bool CheckIfCanUseButton(int buttonNum)
    {
        if (buttonNum >= canUseButton.Length)
            return false;

        return canUseButton[buttonNum];
    }
    #endregion

    #region Boss Related
    public void StartBossBattle(int maxVal)
    {
        bossBattle = true;
        bossHealthSlider.SetActive(true);
        bossHealthSlider.GetComponent<Slider>().maxValue = maxVal;
        bossHealthSlider.GetComponent<Slider>().value = maxVal;
    }

    public void BossBeaten()
    {
        bossHealthSlider.SetActive(false);
    }

    public void BossTakesDamage(int bossHealth)
    {
        bossHealthSlider.GetComponent<Slider>().value = bossHealth;
    }
    #endregion
}
