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

    public Image blackScreen;
    float score = 0.0f;
    float highScore;

    public float speedMult;

    bool levelEnded;

    public GameObject tempGoalDisplay;

    Queue<string> completedGoalDescs = new Queue<string>();

    bool displayingGoals;

    bool bossBattle;
    public GameObject bossHealthSlider;

    public Transform particles;

    bool storyMode;

    void OnEnable()
    {

    }

    void Awake()
    {
        controller = this;
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

        CheckToTurnOffScoreTexts();
    }

    void OnDisable()
    {
        Boat.player.onBoatDeath -= HealthChange;
        Boat.player.onBoatDeath -= DeathScreen;
        Boat.player.onBoatDeath -= EndLevel;

        MillileSpawner.controller.onWavesCleared -= EndLevel;

        TutorialController.controller.onFinishTutorial -= StartLevel;
    }


    void FixedUpdate()
    {
        UpdateScore();
    }

    void UpdateScore()
    {
        if (storyMode && (levelEnded || bossBattle))
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

    void CheckToTurnOffScoreTexts()
    {
        if(GameModeController.controller.CheckCurrentMode(GameModeController.Mode.Story))
        {
            storyMode = true;
            scoreText.enabled = false;
            highScoreText.enabled = false;

            coinText.GetComponent<RectTransform>().localPosition -= new Vector3(0, -169, 0);
        }
        else
        {
            storyMode = false;
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
        AudioController.controller.ClickUI();
        SceneManager.LoadScene("MainScene");
    }

    public void PressMainMenu()
    {
        AudioController.controller.ClickUI();
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
		PlayerInfo.controller.Save ();
        SceneManager.LoadScene("Main Menu");
    }

    public void PressStore()
    {
        AudioController.controller.ClickUI();
        monetizationScreen.SetActive(true);
        upgradeScreen.SetActive(false);
    }

    public void PressBackMonetization()
    {
        AudioController.controller.ClickUI();
        monetizationScreen.SetActive(false);
        upgradeScreen.SetActive(true);
    }

    #endregion

    public void UpdateUpgradePrice(int button, int price)
    {
        upgradeButtons[button].GetComponentInChildren<UpgradePriceUI>().SetPrice(price);
    }

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

        int level = GameModeController.controller.GetCurrentLevel();
        if(level < 5)
            GameModeController.controller.ChooseLevel(level + 1);

        SceneManager.LoadScene("MainScene");
    }

    IEnumerator ShowGoalsAtStart()
    {
		Debug.Log (TempGoalController.controller.GetGoalListCount());
        float time = 0;
        for (int i = 0; i < TempGoalController.controller.GetGoalListCount(); i++)
        {
			Debug.Log ("Show goal: " + i);
            AudioController.controller.PlayFX(AudioController.controller.woodSignDrop);
            tempGoalDisplay.GetComponentInChildren<Text>().text = TempGoalController.controller.GetGoalDescription(i);
            tempGoalDisplay.transform.GetChild(1).GetComponent<Image>().sprite = TempGoalController.controller.goalImages[TempGoalController.controller.GetGoal(i).GetGoalType()];

            RectTransform trans = tempGoalDisplay.GetComponent<RectTransform>();
            Vector3 startingPoint = trans.anchoredPosition;
            while (time < 1)
            {
                trans.anchoredPosition = Vector3.Lerp(startingPoint, new Vector3(startingPoint.x, 20), time);
                time += Time.deltaTime;
                yield return null;
            }

            Vector3 endPoint = trans.anchoredPosition;
            time = 0;

            yield return new WaitForSeconds(1);

            AudioController.controller.PlayFX(AudioController.controller.woodSignPullUp);
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

        while(displayingGoals)
            yield return null;

        float time = 0;
        displayingGoals = true;
        do
        {
            AudioController.controller.PlayFX(AudioController.controller.woodSignDrop);
            tempGoalDisplay.GetComponentInChildren<Text>().text = completedGoalDescs.Peek();

            RectTransform trans = tempGoalDisplay.GetComponent<RectTransform>();
            Vector3 startingPoint = trans.anchoredPosition;
            while (time < 1)
            {
                trans.anchoredPosition = Vector3.Lerp(startingPoint, new Vector3(startingPoint.x, 20), time);
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

        displayingGoals = false;

    }

    public void CompleteGoal(string desc)
    {
        completedGoalDescs.Enqueue(desc);
        if (!displayingGoals)
            StartCoroutine(ShowCompletedGoal());
    }

    public void UpdateCoinString(int coinAmount)
    {
        coinText.text = "Coins: " + string.Format("{0:N0}", coinAmount);
    }


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
