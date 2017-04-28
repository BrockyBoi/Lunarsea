using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    public static PauseScript controller;
    public GameObject pauseScreen;
    public GameObject audioOptions;
    public GameObject pauseOptions;
    bool gameStarted;

    public Slider musicSlider;
    public Slider fxSlider;

    void Awake()
    {
        controller = this;
    }

    void Start()
    {
        UpgradeController.controller.notUpgrading += StartGame;
    }

    void OnDisable()
    {
        UpgradeController.controller.notUpgrading -= StartGame;
    }
    public void PressPause()
    {
        if (!Boat.player.CheckIfAlive())
            return;

        if (gameStarted)
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
                pauseScreen.SetActive(true);
            }
            else
            {
                Time.timeScale = 1;
                pauseScreen.SetActive(false);
            }

            Time.fixedDeltaTime = 0.02F * Time.timeScale;
        }
        else
        {
            pauseScreen.SetActive(!pauseScreen.activeInHierarchy);
        }
    }

    public void PressOptions()
    {
        audioOptions.SetActive(true);
        pauseOptions.SetActive(false);
    }

    public void PressBackSound()
    {
        audioOptions.SetActive(false);
        pauseOptions.SetActive(true);

        PlayerInfo.controller.Save();
    }

    public void PressMainMenu()
    {
		Time.timeScale = 1;
		Time.fixedDeltaTime = 0.02F * Time.timeScale;
        SceneManager.LoadScene("Main Menu");
    }

    void StartGame()
    {
        gameStarted = true;
    }

    public void MusicSliderUpdate(float f)
    {
        musicSlider.value = f;
    }

    public void FXSliderUpdate(float f)
    {
        fxSlider.value = f;
    }
}
