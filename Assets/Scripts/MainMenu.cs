using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
	public Text credits;
	public List<Button> buttons;
	public Button back;
	// Use this for initialization
	void Start () {
		credits.gameObject.SetActive (false);
		back.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PressPlay()
	{
		SceneManager.LoadScene ("MainScene");
	}

	public void PressCredits()
	{
		for (int i = 0; i < buttons.Count; i++) {
			buttons [i].gameObject.SetActive (false);
		}
		credits.gameObject.SetActive (true);
		back.gameObject.SetActive (true);
	}

	public void PressBack()
	{
		for (int i = 0; i < buttons.Count; i++) {
			buttons [i].gameObject.SetActive (true);
		}
		credits.gameObject.SetActive (false);
		back.gameObject.SetActive (false);
	}
}

