using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainCanvas : MonoBehaviour {

	public static MainCanvas controller;
	public GameObject deathScreen;
	public Text scoreText;
	public Image[] healthImages;
	float score = 0.0f;
	void Awake()
	{
		controller = this;
	}
		
	void Start () {
		deathScreen.SetActive (false);
	}


	void FixedUpdate() {
		UpdateScore ();
	}

	void UpdateScore() {
		if (Boat.player.GetHealth () > 0 && !SpeechController.controller.CheckTextTime()) {
			score += Time.fixedDeltaTime;
		}
		scoreText.text = "Score: "+string.Format("{0:0.0}",score) + " m";
	}

	public void DeathScreen()
	{
		deathScreen.SetActive (true);
	}

	#region Press Button
	public void PressRetry()
	{
		SceneManager.LoadScene ("MainScene");
	}

	public void PressMainMenu()
	{
		SceneManager.LoadScene("Main Menu");
	}
	#endregion

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
