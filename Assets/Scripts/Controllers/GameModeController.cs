using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeController : MonoBehaviour
{

    #region Variables
    public static GameModeController controller;

    public enum Mode { Story, Endless }
    [SerializeField]
    Mode currentMode;

    int chosenLevel;
    #endregion
    void Awake()
    {
        if (controller == null)
            controller = this;
        else if (controller != this)
            this.enabled = false;

        DontDestroyOnLoad(this);
    }


    void Start()
    {
        if (MainMenu.controller != null)
            MainMenu.controller.PressedLevel += ChooseLevel;
    }

    public bool CheckCurrentMode(Mode m)
    {
        if (m == currentMode)
            return true;

        return false;
    }

    public void SetGameMode(Mode m)
    {
        currentMode = m;
    }

    public void ChooseLevel(int level)
    {
        chosenLevel = level;
    }

    public int GetCurrentLevel()
    {
        return chosenLevel;
    }
}
