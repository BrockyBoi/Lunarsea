using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameModeController : MonoBehaviour
{

	#region Variables

	public static GameModeController controller;

	public enum Mode
	{
		Story,
		Endless
	}

	[SerializeField]
	Mode currentMode;

	int chosenLevel = 1;
	bool gameStarted;

	bool gyro = true;

	#endregion

	void Awake ()
	{
		if (controller == null)
			controller = this;
		else if (controller != this)
			Destroy (gameObject);

		DontDestroyOnLoad (this);

		gameStarted = true;
	}

	void OnEnable ()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable ()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
		if (MainMenu.controller != null)
			MainMenu.controller.PressedLevel -= ChooseLevel;
	}

	public bool CheckIfGameStarted ()
	{
		return gameStarted;
	}

	public void SetGameStarted (bool b)
	{
		gameStarted = b;
	}

	void OnLevelFinishedLoading (Scene scene, LoadSceneMode mode)
	{
		if (MainMenu.controller != null)
			MainMenu.controller.PressedLevel += ChooseLevel;

		Coin.ResetStaticVariables ();
		WaterJoint.ResetStaticVariables ();
	}

	void Start ()
	{
		if (MainMenu.controller != null)
			MainMenu.controller.PressedLevel += ChooseLevel;
	}


	public bool CheckCurrentMode (Mode m)
	{
		if (m == currentMode)
			return true;

		return false;
	}

	public void SetGameMode (Mode m)
	{
		currentMode = m;
	}

	public void ChooseLevel (int level)
	{
		chosenLevel = level;
	}

	public int GetCurrentLevel ()
	{
		return chosenLevel;
	}

	public bool GetGyro ()
	{
		return gyro;
	}

	public void SetGyro (bool b)
	{
		gyro = b;
	}

	public void ToggleGyro ()
	{
		gyro = !gyro;
	}
}
