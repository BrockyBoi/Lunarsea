using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainCanvas : MonoBehaviour {

	public static MainCanvas controller;
	public GameObject deathScreen;
	void Awake()
	{
		
		controller = this;
	}
		
	void Start () {
		deathScreen.SetActive (false);
	}

	public void DeathScreen()
	{
		deathScreen.SetActive (true);
	}

	public void PressRetry()
	{
		SceneManager.LoadScene ("Brock Scene");
	}

	public void PressMainMenu()
	{
		SceneManager.LoadScene("Main Menu");
	}
}
