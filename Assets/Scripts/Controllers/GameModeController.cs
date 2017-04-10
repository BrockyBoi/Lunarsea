using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeController : MonoBehaviour {

	#region Variables
	public static GameModeController controller;

	public enum Mode{Story, Endless}
	[SerializeField]
	Mode currentMode;
	#endregion
	void Awake()
	{
		controller = this;
	}

	public bool CheckCurrentMode(Mode m)
	{
		if(m == currentMode)
		return true;

		return false;
	}

	public void SetGameMode(Mode m)
	{
		currentMode = m;
	}
}
