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

	// Use this for initialization
	void Start () {
		deathScreen.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void DeathScreen()
	{
		deathScreen.SetActive (true);
	}

	public void PressRetry()
	{
		SceneManager.LoadScene ("Main Scene");
	}

	public void PressMainMenu()
	{
		SceneManager.LoadScene("Main Menu");
	}
}
