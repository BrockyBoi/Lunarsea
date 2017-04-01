using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeController : MonoBehaviour {

	#region Variables
	public static GameModeController controller;

	public enum Mode{Story, Endless}

	int currentMode;
	#endregion
	void Awake()
	{
		controller = this;
	}

	public bool CheckCurrentMode(Mode m)
	{
		if((int)m == currentMode)
		return true;

		return false;
	}

	public void SetGameMode(Mode m)
	{
		currentMode = (int)m;
	}
}
