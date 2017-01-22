using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {
	public static AudioController controller;
	AudioSource music;
	AudioSource background;
	AudioSource fx;

	public AudioClip mainMusic;
	public AudioClip waveBackground;

	void Awake()
	{
		DontDestroyOnLoad (this);
		if (controller == null)
			controller = this;
		else
			Destroy (gameObject);
	}
	// Use this for initialization
	void Start () {

		music = gameObject.AddComponent<AudioSource> ();
		music.clip = mainMusic;
		music.Play ();
		background = gameObject.AddComponent<AudioSource> ();
		background.clip = waveBackground;
		background.Play ();
		fx = gameObject.AddComponent<AudioSource> ();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PlayFX(AudioClip clip)
	{
		fx.PlayOneShot (clip);
	}
}
