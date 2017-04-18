using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechController : MonoBehaviour
{
    public static SpeechController controller;

    int currentPhrase;
    public List<string> phrases;

    public Text textField;

    bool textTime;

    void OnEnable()
    {
    }

    void Awake()
    {
        controller = this;
    }
    // Use this for initialization
    void Start()
    {
        TutorialController.controller.onStartTutorial += FirstPhrase;

        TutorialController.controller.onFinishTutorial += CloseWindow;
        textField.transform.parent.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FirstPhrase()
    {
        textField.transform.parent.gameObject.SetActive(true);
        textField.text = phrases[0];
        textTime = true;
    }

    public void NextPhrase()
    {
        if (currentPhrase < phrases.Count - 1)
        {
            currentPhrase++;
            textField.text = phrases[currentPhrase];
        }
        else
        {
            CloseWindow();
        }
    }

    public void CloseWindow()
    {
        textTime = false;
        textField.transform.parent.gameObject.SetActive(false);
    }

    public bool CheckTextTime()
    {
        return textTime;
    }
}
