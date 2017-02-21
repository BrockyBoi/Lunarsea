using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechController : MonoBehaviour {
	public static SpeechController controller;

	int currentPhrase;
	public List<string> phrases;

	public Text textField;

	bool textTime;

	void Awake()
	{
		controller = this;
	}
	// Use this for initialization
	void Start () {
		if(TutorialController.controller.tutorialMode)
		{
			FirstPhrase ();
		}
	}
	
	// Update is called once per frame
	void Update () {
	}

	void FirstPhrase()
	{
		textField.text = phrases [0];
		textTime = true;
	}

	public void NextPhrase()
	{
		if (currentPhrase < phrases.Count - 1) {
			currentPhrase++;
			textField.text = phrases [currentPhrase];
		} else {
			CloseWindow ();
		}
	}

	public void CloseWindow()
	{
		textTime = false;
		textField.transform.parent.gameObject.SetActive (false);
	}

	public bool CheckTextTime()
	{
		return textTime;
	}
}
