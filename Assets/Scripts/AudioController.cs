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

	public List<AudioClip> missileClips;

	public AudioClip waterRise;
	public AudioClip waterFall;

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
		fx.loop = false;
		
	}

	public void PlayFX(AudioClip clip)
	{
		fx.PlayOneShot (clip);
	}

	public void PlayMissileSound()
	{
		int num = Random.Range (0, missileClips.Count);
		fx.PlayOneShot (missileClips [num]);
	}

	public void WaterRise()
	{
		fx.clip = waterRise;
		fx.Play ();
	}

	public void WaterFall()
	{
		fx.clip = waterFall;
		fx.Play ();
	}
}
