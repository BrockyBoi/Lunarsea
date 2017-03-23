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
        deathScreen.SetActive(false);
		monetizationScreen.SetActive(false);
        SetHighScore(PlayerInfo.controller.GetHighScore());
		
    }


    void FixedUpdate()
    {
        UpdateScore();
    }

    void UpdateScore()
    {
        if (Boat.player.CheckIfAlive() && !SpeechController.controller.CheckTextTime() && !UpgradeController.controller.CheckIfUpgrading())
        {
            score += Time.fixedDeltaTime + (Time.deltaTime * speedMult);

            scoreText.text = "Score: " + string.Format("{0:0.0}", score) + " m";

			TempGoalController.controller.UpdateDistanceGoals();

            if (score > highScore)
            {
                highScore = score;
                highScoreText.text = "High Score: " + string.Format("{0:0.0}", highScore) + " m";
            }
        }
    }

    public void DeathScreen()
    {
        deathScreen.SetActive(true);
    }

    #region Press Button
    public void PressRetry()
    {
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

    public void UpdateCoinString(int coinAmount)
    {
        coinText.text = "Coins: " + coinAmount.ToString();
    }

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
}
