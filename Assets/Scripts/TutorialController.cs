using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public static TutorialController controller;

    public enum TutorialStage { MOVEMENT, SPAWN_MOON, RETRACT_MOON, DONE }
    int currentStage;

    public bool tutorialMode;

    void Awake()
    {
        controller = this;
    }

    void Start()
    {
		
    }

    public void SetUpTutorial()
    {
        if (!tutorialMode)
		{
            SpeechController.controller.CloseWindow();
			SetStage(TutorialStage.DONE);
		}
		else SetStage(TutorialStage.MOVEMENT);
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
        SpeechController.controller.NextPhrase();

        if (t == TutorialStage.DONE)
        {
            tutorialMode = false;
            Boat.player.SetTutorialMode(false);
            MillileSpawner.controller.StartGame();
        }
    }
}
