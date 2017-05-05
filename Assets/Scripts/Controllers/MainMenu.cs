using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	public static MainMenu controller;
	public Text credits;
	public Text loading;

	public delegate void LevelPressed (int num);

	public event LevelPressed PressedLevel;

	public GameObject creditsParent;
	public GameObject mainMenuParent;
	public GameObject levelSelectParent;
	public GameObject playSelectParent;
	public GameObject optionsParent;
	public GameObject introParent;

	public GameObject toggleGyroButton;
	public Text toggleGyroText;
	public List<Button> levelButtons;

	public Slider musicSlider;
	public Slider fxSlider;

	public GameObject quitButton;

	void Awake ()
	{
		controller = this;
		if (PlayerInfo.controller.CheckIfFirstTime () || PlayerInfo.controller.DeleteFirst || PlayerInfo.controller.ResetSaveFile)
			InitializeLevelButtons (0);

	}
	// Use this for initialization
	void Start ()
	{
		if (GameModeController.controller.CheckIfGameStarted ()) {
			introParent.SetActive (true);
			mainMenuParent.SetActive (false);
			SpeechController.controller.DisplayStory ();
		} else {
			introParent.SetActive (false);
			mainMenuParent.SetActive (true);
		}

		creditsParent.SetActive (false);
		levelSelectParent.SetActive (false);
		playSelectParent.SetActive (false);
		loading.gameObject.SetActive (false);
		optionsParent.gameObject.SetActive (false);

		AudioController.controller.MusicChange += MusicSliderUpdate;
		AudioController.controller.FXChange += FXSliderUpdate;

		MusicSliderUpdate (AudioController.controller.GetMusicVolume ());
		FXSliderUpdate (AudioController.controller.GetFXVolume ());

#if UNITY_STANDALONE || UNITY_WEBGL
		toggleGyroButton.SetActive (false);
		quitButton.SetActive (true);
#elif UNITY_IOS || UNITY_ANDROID
		toggleGyroButton.SetActive (true);
		quitButton.SetActive (false);
#endif
		if (GameModeController.controller.GetGyro ()) {
			toggleGyroText.text = "Gyro:\nOn";
		} else {
			toggleGyroText.text = "Gyro:\nOff";
		}
	}

	public void InitializeLevelButtons (int levelsBeaten)
	{
		for (int i = levelsBeaten + 1; i < levelButtons.Count; i++) {
			levelButtons [i].GetComponent<Image> ().color = new Color (Color.gray.r, Color.gray.g, Color.gray.b, .5f);
		}
	}


	#region Pressed Button Functions

	public void PressContinue ()
	{
		AudioController.controller.ClickUI ();
		introParent.SetActive (false);
		mainMenuParent.SetActive (true);
		GameModeController.controller.SetGameStarted (false);
		SpeechController.controller.StopAllCoroutines ();
	}

	public void PressLevel (int level)
	{
		AudioController.controller.ClickUI ();
		if (level - 1 <= PlayerInfo.controller.GetLevelsBeaten ()) {
			GameModeController.controller.SetGameMode (GameModeController.Mode.Story);
			PressedLevel (level);
			loading.gameObject.SetActive (true);
			SceneManager.LoadScene ("MainScene");
		}
	}

	public void PressPlay ()
	{
		AudioController.controller.ClickUI ();
		playSelectParent.SetActive (true);
		mainMenuParent.SetActive (false);
	}

	public void PressEndlessMode ()
	{
		AudioController.controller.ClickUI ();
		GameModeController.controller.SetGameMode (GameModeController.Mode.Endless);
		loading.gameObject.SetActive (true);
		SceneManager.LoadScene ("MainScene");
	}

	public void PressBackPlaySelect ()
	{
		AudioController.controller.ClickUI ();
		playSelectParent.SetActive (false);
		mainMenuParent.SetActive (true);
	}

	public void PressOptions ()
	{
		AudioController.controller.ClickUI ();
		optionsParent.SetActive (true);
		mainMenuParent.SetActive (false);
	}

	public void PressBackOptions ()
	{
		AudioController.controller.ClickUI ();
		optionsParent.SetActive (false);
		mainMenuParent.SetActive (true);
	}

	public void PressCredits ()
	{
		AudioController.controller.ClickUI ();
		creditsParent.SetActive (true);
		mainMenuParent.SetActive (false);
	}

	public void PressBackCredits ()
	{
		AudioController.controller.ClickUI ();
		creditsParent.SetActive (false);
		mainMenuParent.SetActive (true);
	}

	public void PressStoryMode ()
	{
		AudioController.controller.ClickUI ();
		levelSelectParent.SetActive (true);
		playSelectParent.SetActive (false);
	}

	public void PressBackLevelSelect ()
	{
		AudioController.controller.ClickUI ();
		levelSelectParent.SetActive (false);
		playSelectParent.SetActive (true);
	}

	public void PressQuit ()
	{
		Application.Quit ();
	}

	public void PressToggleGyro ()
	{
		GameModeController.controller.ToggleGyro ();
		if (GameModeController.controller.GetGyro ()) {
			toggleGyroText.text = "Gyro:\nOn";
		} else {
			toggleGyroText.text = "Gyro:\nOff";
		}
	}

	#endregion

	#region Sliders

	public void MusicSliderUpdate (float f)
	{
		musicSlider.value = f;
	}

	public void FXSliderUpdate (float f)
	{
		fxSlider.value = f;
	}

	#endregion
}

