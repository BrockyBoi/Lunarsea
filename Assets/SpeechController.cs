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
		FirstPhrase ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			NextPhrase ();
		}
	}

	void FirstPhrase()
	{
		textField.text = phrases [0];
		textTime = true;
	}

	void NextPhrase()
	{
		if (currentPhrase < phrases.Count - 1) {
			currentPhrase++;
			textField.text = phrases [currentPhrase];
		} else {
			textTime = false;
			textField.transform.parent.gameObject.SetActive (false);
		}
	}

	public bool CheckTextTime()
	{
		return textTime;
	}
}
