using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour {
	public static TutorialController controller;

	public enum TutorialStage{MOVEMENT, SPAWN_MOON,RETRACT_MOON, DONE}
	int currentStage;

	public bool tutorialMode;

	void Awake()
	{
		controller = this;

		currentStage = (int)TutorialStage.MOVEMENT;

	}

	void Start()
	{
		Boat.player.SetTutorialMode (tutorialMode);

		if (!tutorialMode)
			SpeechController.controller.CloseWindow ();
	}

	public bool CheckIfOnStage(TutorialStage t)
	{
		if (currentStage == (int)t)
			return true;
		return false;
	}

	public void SetStage(TutorialStage t)
	{
		currentStage = (int)t;
		SpeechController.controller.NextPhrase ();

		if (t == TutorialStage.DONE)
		{
			tutorialMode = false;
			Boat.player.SetTutorialMode (false);
		}
	}
}
