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
	public GameObject levelScreen;
	public GameObject monetizationScreen;
	public GameObject startScreen;
	public GameObject mobileButtons;
	public Text scoreText;
	public Text highScoreText;
	public Text coinText;
	public Text levelTitle;
	public List<Image> healthImages;
	public List<Button> upgradeButtons;

	public List<Image> UpgradeIcons;
	int[] IconStatus = new int[7] { 0, 0, 0, 0, 0, 0, 0 };
	public List<Sprite> healthUpgradeIcons;
	public List<Sprite> goalUpgradeIcons;
	public List<Sprite> magnetUpgradeIcons;
	public List<Sprite> cDropUpgradeIcons;
	public List<Sprite> speedUpgradeIcons;
	public List<Sprite> invulUpgradeIcons;
	public List<Sprite> hDropUpgradeIcons;

	public GameObject winLevelDropMenu;
	public Text winLevelText;


	public Image blackScreen;
	float score = 0.0f;
	float highScore;

	public float speedMult;

	bool levelEnded;

	public GameObject tempGoalDisplay;

	Queue<string> completedGoalDescs = new Queue<string> ();

	bool displayingGoals;

	bool bossBattle;
	public GameObject bossHealthSlider;

	public Transform particles;

	bool storyMode;

	void OnEnable ()
	{

	}

	void Awake ()
	{
		controller = this;
		UpdateCoinString (0);
		highScoreText.text = "Record: " + 0;
	}

	void Start ()
	{
		Boat.player.onBoatDeath += HealthChange;
		Boat.player.onBoatDeath += DeathScreen;
		Boat.player.onBoatDeath += EndLevel;

		MillileSpawner.controller.onWavesCleared += EndLevel;

		TutorialController.controller.onFinishTutorial += StartLevel;

		levelEnded = true;
		deathScreen.SetActive (false);
		bossHealthSlider.SetActive (false);
		monetizationScreen.SetActive (false);

		if (GameModeController.controller.CheckCurrentMode (GameModeController.Mode.Story)) {
			levelTitle.text = string.Format ("Level " + GameModeController.controller.GetCurrentLevel ().ToString ());
		} else {
			levelTitle.text = string.Format ("Endless Mode");
		}

		CheckToTurnOffScoreTexts ();
	
		if (GameModeController.controller.GetGyro ()) {
			mobileButtons.SetActive (false);
		} else {
			mobileButtons.SetActive (true);
		}

		CheckToTurnOffScoreTexts ();
	}

	void OnDisable ()
	{
		Boat.player.onBoatDeath -= HealthChange;
		Boat.player.onBoatDeath -= DeathScreen;
		Boat.player.onBoatDeath -= EndLevel;

		MillileSpawner.controller.onWavesCleared -= EndLevel;

		TutorialController.controller.onFinishTutorial -= StartLevel;
	}


	void FixedUpdate ()
	{
		UpdateScore ();
	}

	void UpdateScore ()
	{
		if (levelEnded || bossBattle)
			return;


		score += Time.fixedDeltaTime + (Time.deltaTime * speedMult);
		TempGoalController.controller.UpdateDistanceGoals ();

		if (!storyMode) {
			//scoreText.text = "Score: " + string.Format ("{0:0.0}", score) + " m";
			scoreText.text = string.Format ("{0:0.0}", score) + " m";


			if (score > highScore) {
				highScore = score;
				//highScoreText.text = "High Score: " + string.Format ("{0:0.0}", highScore) + " m";
				highScoreText.text = "Best: " + string.Format ("{0:0.0}", highScore) + " m";
			}
		}

	}

	void CheckToTurnOffScoreTexts ()
	{
		if (GameModeController.controller.CheckCurrentMode (GameModeController.Mode.Story)) {
			storyMode = true;
			scoreText.enabled = false;
			highScoreText.enabled = false;

			//coinText.GetComponent<RectTransform>().localPosition -= new Vector3(0, -169, 0);
		} else {
			storyMode = false;
		}
	}


	void StartLevel ()
	{
		StartCoroutine (ShowGoalsAtStart ());
		TempGoalController.controller.UpdateTimesPlayedGoals ();
		levelEnded = false;
	}

	void EndLevel ()
	{
		levelEnded = true;
	}

	void DeathScreen ()
	{
		deathScreen.SetActive (true);
	}


	#region Press Button

	public void PressRetry ()
	{
		AudioController.controller.ClickUI ();
		SceneManager.LoadScene ("MainScene");
	}

	public void PressMainMenu ()
	{
		AudioController.controller.ClickUI ();
		Time.timeScale = 1;
		Time.fixedDeltaTime = 0.02F * Time.timeScale;
		PlayerInfo.controller.Save ();
		SceneManager.LoadScene ("Main Menu");
	}

	public void PressStore ()
	{
		AudioController.controller.ClickUI ();
		monetizationScreen.SetActive (true);
		upgradeScreen.SetActive (false);
		levelScreen.SetActive (false);
	}

	public void PressBackMonetization ()
	{
		AudioController.controller.ClickUI ();
		monetizationScreen.SetActive (false);
		startScreen.SetActive (false);
		upgradeScreen.SetActive (true);
		levelScreen.SetActive (true);
	}

	public void PressStartScreen ()
	{
		AudioController.controller.ClickUI ();
		upgradeScreen.SetActive (false);
		startScreen.SetActive (true);
	}

	#endregion

	public void UpdateUpgradePrice (int button, int price)
	{
		upgradeButtons [button].GetComponentInChildren<UpgradePriceUI> ().SetPrice (price);


		//Updating Icon
		switch (button) {
		case 0:
			if (IconStatus [button] < healthUpgradeIcons.Count)
				UpgradeIcons [button].overrideSprite = healthUpgradeIcons [Mathf.Min (++IconStatus [button], healthUpgradeIcons.Count - 1)];
			break;
		case 1:
			if (IconStatus [button] < healthUpgradeIcons.Count)
				UpgradeIcons [button].overrideSprite = healthUpgradeIcons [Mathf.Min (++IconStatus [button], healthUpgradeIcons.Count - 1)];
			break;
		case 2:
			if (IconStatus [button] < healthUpgradeIcons.Count)
				UpgradeIcons [button].overrideSprite = healthUpgradeIcons [Mathf.Min (++IconStatus [button], healthUpgradeIcons.Count - 1)];
			break;
		case 3:
			if (IconStatus [button] < healthUpgradeIcons.Count)
				UpgradeIcons [button].overrideSprite = healthUpgradeIcons [Mathf.Min (++IconStatus [button], healthUpgradeIcons.Count - 1)];
			break;
		case 4:
			if (IconStatus [button] < healthUpgradeIcons.Count)
				UpgradeIcons [button].overrideSprite = healthUpgradeIcons [Mathf.Min (++IconStatus [button], healthUpgradeIcons.Count - 1)];
			break;
		case 5:
			if (IconStatus [button] < healthUpgradeIcons.Count)
				UpgradeIcons [button].overrideSprite = healthUpgradeIcons [Mathf.Min (++IconStatus [button], healthUpgradeIcons.Count - 1)];
			break;
		case 6:
			if (IconStatus [button] < healthUpgradeIcons.Count)
				UpgradeIcons [button].overrideSprite = healthUpgradeIcons [Mathf.Min (++IconStatus [button], healthUpgradeIcons.Count - 1)];
			break;
		default:
			break;
		}
        
	}

	public void HealthChange ()
	{

		int health = Boat.player.GetHealth ();
		for (int i = 0; i < healthImages.Count; i++) {
			if (i < health) {
				healthImages [i].gameObject.SetActive (true);
			} else
				healthImages [i].gameObject.SetActive (false);
		}
	}

	#region High Score Accessors

	public float GetScore ()
	{
		return score;
	}

	public float GetHighScore ()
	{
		return highScore;
	}

	public void SetHighScore (float f)
	{
		highScore = f;
		highScoreText.text = "Best: " + string.Format ("{0:0.0}", highScore) + " m";
	}

	#endregion

	public void FinishLevel (int level, int coinReward)
	{
		winLevelDropMenu.gameObject.SetActive (true);
		StartCoroutine (DisplayEndLevelReward (level, coinReward));
	}

	IEnumerator FadeToBlack ()
	{
		float time = 0;
		while (time < 1) {
			time += Time.deltaTime * .5f;
			Color c = blackScreen.color;
			blackScreen.color = new Color (c.r, c.b, c.g, time);
			yield return null;
		}

		int level = GameModeController.controller.GetCurrentLevel ();
		if (level < 5)
			GameModeController.controller.ChooseLevel (level + 1);

		yield return new WaitForSeconds (1.5f);
		SceneManager.LoadScene ("MainScene");
	}

	IEnumerator DisplayEndLevelReward (int level, int coinReward)
	{
		RectTransform rect = winLevelDropMenu.GetComponent<RectTransform> ();
		Vector3 startingPoint = rect.anchoredPosition;
		winLevelText.text = "Won level " + level + "\nReceive " + coinReward + " coins"; 
		float time = 0;
		while (time < 1) {
			rect.anchoredPosition = Vector3.Lerp (startingPoint, new Vector3 (startingPoint.x, -350), time);
			time += Time.deltaTime * .5f;
			yield return null;
		}

		StartCoroutine (FadeToBlack ());
	}

	IEnumerator ShowGoalsAtStart ()
	{
		float time = 0;
		for (int i = 0; i < TempGoalController.controller.GetGoalListCount (); i++) {
			AudioController.controller.PlayFX (AudioController.controller.woodSignDrop);
			tempGoalDisplay.GetComponentInChildren<Text> ().text = TempGoalController.controller.GetGoalDescription (i);
			tempGoalDisplay.transform.GetChild (1).GetComponent<Image> ().sprite = TempGoalController.controller.goalImages [TempGoalController.controller.GetGoal (i).GetGoalType ()];

			RectTransform trans = tempGoalDisplay.GetComponent<RectTransform> ();
			Vector3 startingPoint = trans.anchoredPosition;
			while (time < 1) {
				trans.anchoredPosition = Vector3.Lerp (startingPoint, new Vector3 (startingPoint.x, 75), time);
				time += Time.deltaTime;
				yield return null;
			}

			Vector3 endPoint = trans.anchoredPosition;
			time = 0;

			yield return new WaitForSeconds (1);

			AudioController.controller.PlayFX (AudioController.controller.woodSignPullUp);
			while (time < 1) {
				trans.anchoredPosition = Vector3.Lerp (endPoint, startingPoint, time);
				time += Time.deltaTime;
				yield return null;
			}
			trans.anchoredPosition = startingPoint;

			time = 0;

			yield return new WaitForSeconds (1);
		}
	}

	IEnumerator ShowCompletedGoal ()
	{

		while (displayingGoals)
			yield return null;

		float time = 0;
		displayingGoals = true;
		do {
			AudioController.controller.PlayFX (AudioController.controller.woodSignDrop);
			tempGoalDisplay.GetComponentInChildren<Text> ().text = completedGoalDescs.Peek ();

			RectTransform trans = tempGoalDisplay.GetComponent<RectTransform> ();
			Vector3 startingPoint = trans.anchoredPosition;
			while (time < 1) {
				trans.anchoredPosition = Vector3.Lerp (startingPoint, new Vector3 (startingPoint.x, 20), time);
				time += Time.deltaTime;
				yield return null;
			}

			Vector3 endPoint = trans.anchoredPosition;
			time = 0;

			yield return new WaitForSeconds (1);
			while (time < 1) {
				trans.anchoredPosition = Vector3.Lerp (endPoint, startingPoint, time);
				time += Time.deltaTime;
				yield return null;
			}
			trans.anchoredPosition = startingPoint;

			time = 0;

			yield return new WaitForSeconds (1);
			completedGoalDescs.Dequeue ();
		} while (completedGoalDescs.Count != 0);

		displayingGoals = false;

	}

	public void CompleteGoal (string desc)
	{
		completedGoalDescs.Enqueue (desc);
		if (!displayingGoals)
			StartCoroutine (ShowCompletedGoal ());
	}

	public void UpdateCoinString (int coinAmount)
	{
		coinText.text = string.Format ("{0:N0}", coinAmount);
	}


	#region Boss Related

	public void StartBossBattle (int maxVal)
	{
		bossBattle = true;
		bossHealthSlider.SetActive (true);
		bossHealthSlider.GetComponent<Slider> ().maxValue = maxVal;
		bossHealthSlider.GetComponent<Slider> ().value = maxVal;
	}

	public void BossBeaten ()
	{
		bossHealthSlider.SetActive (false);
	}

	public void BossTakesDamage (int bossHealth)
	{
		bossHealthSlider.GetComponent<Slider> ().value = bossHealth;
	}

	#endregion
}
