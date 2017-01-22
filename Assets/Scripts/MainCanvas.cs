using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainCanvas : MonoBehaviour {

	public static MainCanvas controller;
	public GameObject deathScreen;

	public Image[] healthImages;
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
		SceneManager.LoadScene ("MainScene");
	}

	public void PressMainMenu()
	{
		SceneManager.LoadScene("Main Menu");
	}

	public void HealthChange()
	{
		int health = Boat.player.GetHealth ();
		for (int i = 0; i < healthImages.Length; i++) {
			if (i < health) {
				healthImages [i].gameObject.SetActive (true);
			} else
				healthImages [i].gameObject.SetActive (false);
		}
	}
}
