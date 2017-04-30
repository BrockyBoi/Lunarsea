using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public static TutorialController controller;

    public enum TutorialStage { MOVEMENT, SPAWN_MOON, RETRACT_MOON, DONE }
	TutorialStage currentStage;

    public delegate void OnStartTutorial();
    public event OnStartTutorial onStartTutorial;

    public delegate void OnFinishTutorial();
    public event OnFinishTutorial onFinishTutorial;

    public bool tutorialMode;

    void OnEnable()
    {
    }

    void Awake()
    {
        controller = this;
    }

    void Start()
    {
        Boat.player.onFinishedSailingIn += SetUpTutorial;
        tutorialMode = PlayerInfo.controller.CheckIfFirstTime();
    }

	void OnDisable()
	{
		Boat.player.onFinishedSailingIn -= SetUpTutorial;
	}

    void SetUpTutorial()
    {
        if (!tutorialMode)
        {
            SpeechController.controller.CloseWindow();
            SetStage(TutorialStage.DONE);
        }
        else
        {
            onStartTutorial();

            SetStage(TutorialStage.MOVEMENT);
        }
    }

    public bool CheckIfTutorialMode()
    {
        return tutorialMode;
    }

    public bool CheckIfOnStage(TutorialStage t)
    {
		return currentStage == t;
    }

    public void SetStage(TutorialStage t)
    {
        if (t == TutorialStage.DONE)
        {
            tutorialMode = false;

            onFinishTutorial();

            return;
        }

        currentStage = t;

        if(t != TutorialStage.MOVEMENT)
            SpeechController.controller.NextPhrase();

    }
}
