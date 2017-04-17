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
    public Button back;

    public delegate void LevelPressed(int num);
    public event LevelPressed PressedLevel;

	public GameObject creditsParent;
	public GameObject mainMenuParent;
    public GameObject levelSelectParent;
    public GameObject playSelectParent;
	public GameObject optionsParent;
    public List<Button> levelButtons;

	public Slider musicSlider;
	public Slider fxSlider;

    void Awake()
    {
        controller = this;
    }
    // Use this for initialization
    void Start()
    {
		mainMenuParent.SetActive(true);
		creditsParent.SetActive(false);
		levelSelectParent.SetActive(false);
		playSelectParent.SetActive(false);
		loading.gameObject.SetActive(false);

        InitializeLevelButtons();
    }

    void InitializeLevelButtons()
    {
        for (int i = PlayerInfo.controller.GetLevelsBeaten() + 1; i < 10; i++)
        {
            levelButtons[i].GetComponent<Image>().color = new Color(Color.gray.r, Color.gray.g, Color.gray.b, .5f);

        }
    }
    #region Pressed Button Functions
    public void PressLevel(int level)
    {
        if (level <= PlayerInfo.controller.GetLevelsBeaten())
        {
            GameModeController.controller.SetGameMode(GameModeController.Mode.Story);
            PressedLevel(level);
			loading.gameObject.SetActive(true);
			SceneManager.LoadScene("MainScene");
        }
    }

    public void PressPlay()
    {
        playSelectParent.SetActive(true);
		mainMenuParent.SetActive(false);
    }

    public void PressEndlessMode()
    {
        GameModeController.controller.SetGameMode(GameModeController.Mode.Endless);
		loading.gameObject.SetActive(true);
		SceneManager.LoadScene("MainScene");
    }

    public void PressBackPlaySelect()
    {
        playSelectParent.SetActive(false);
		mainMenuParent.SetActive(true);
    }

	public void PressOptions()
	{
		optionsParent.SetActive(true);
		mainMenuParent.SetActive(false);
	}

	public void PressBackOptions()
	{
		optionsParent.SetActive(false);
		mainMenuParent.SetActive(true);
		PlayerInfo.controller.Save();
	}

    public void PressCredits()
    {
        creditsParent.SetActive(true);
		mainMenuParent.SetActive(false);
    }

    public void PressBackCredits()
    {
        creditsParent.SetActive(false);
        mainMenuParent.SetActive(true);
    }

	public void PressStoryMode()
	{
		levelSelectParent.SetActive(true);
		playSelectParent.SetActive(false);
	}

	public void PressBackLevelSelect()
	{
		levelSelectParent.SetActive(false);
		playSelectParent.SetActive(true);
	}
    #endregion

	public void MusicSliderUpdate(float f)
	{
		musicSlider.value = f;
	}

	public void FXSliderUpdate(float f)
	{
		fxSlider.value = f;
	}
}

