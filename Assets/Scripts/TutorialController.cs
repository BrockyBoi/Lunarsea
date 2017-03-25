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
        tutorialMode = PlayerInfo.controller.firstTimeEver;
    }

    public void SetUpTutorial()
    {
        if (!tutorialMode)
        {
            SpeechController.controller.CloseWindow();
            SetStage(TutorialStage.DONE);
        }
        else
        {
            Boat.player.SetTutorialMode(true);
            SpeechController.controller.FirstPhrase();
            SetStage(TutorialStage.MOVEMENT);
        }
    }

    public bool CheckIfOnStage(TutorialStage t)
    {
        if (currentStage == (int)t)
            return true;
        return false;
    }

    public void SetStage(TutorialStage t)
    {
        if (t == TutorialStage.DONE)
        {
            tutorialMode = false;
            Boat.player.SetTutorialMode(false);
            MillileSpawner.controller.StartGame();
            MainCanvas.controller.StartLevel();
            SpeechController.controller.CloseWindow();
            return;
        }

        currentStage = (int)t;

        if(t != TutorialStage.MOVEMENT)
            SpeechController.controller.NextPhrase();

    }
}
