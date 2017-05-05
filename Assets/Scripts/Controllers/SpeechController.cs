using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechController : MonoBehaviour
{
	public static SpeechController controller;

	int currentPhrase;
	string[] phrases = new string[3];

	public Text textField;

	public string introText;
	public Text introTextField;

	void OnEnable ()
	{
	}

	void Awake ()
	{
		controller = this;
	}
	// Use this for initialization
	void Start ()
	{
		#if UNITY_STANDALONE
		phrases [0] = "Use A and D to move the boat left and right";
		phrases [1] = "Left click above the water to summon the moon to create waves";
		phrases [2] = "Left click above the water again to retract the moon";
		#elif UNITY_ANDROID || UNITY_IOS
		phrases [0] = "Use left and right buttons to move the boat left and right";
		phrases [1] = "Tap above the water to summon the moon to create waves";
		phrases [2] = "Tap above the water again to retract the moon";
		#endif
		


		if (TutorialController.controller != null) {
			TutorialController.controller.onStartTutorial += FirstPhrase;

			TutorialController.controller.onFinishTutorial += CloseWindow;
		}

		if (textField != null)
			textField.transform.parent.gameObject.SetActive (false);
	}

	void OnDisable ()
	{
		if (TutorialController.controller != null) {
			TutorialController.controller.onStartTutorial -= FirstPhrase;

			TutorialController.controller.onFinishTutorial -= CloseWindow;
		}
	}

	public void DisplayStory ()
	{
		StartCoroutine (IntroText ());
	}

	IEnumerator IntroText ()
	{
		int count = 0;
		while (count < introText.Length) {
			introTextField.text += introText [count];
			count++;
			yield return new WaitForSeconds (.05f);
		}
	}

	void FirstPhrase ()
	{
		textField.transform.parent.gameObject.SetActive (true);
		textField.text = phrases [0];
	}

	public void NextPhrase ()
	{
		if (currentPhrase < phrases.Length - 1) {
			currentPhrase++;
			textField.text = phrases [currentPhrase];
		} else {
			CloseWindow ();
		}
	}

	public void CloseWindow ()
	{
		textField.transform.parent.gameObject.SetActive (false);
	}
}
