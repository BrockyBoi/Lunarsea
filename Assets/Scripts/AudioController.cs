using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {
	public static AudioController controller;
	AudioSource music;
	AudioSource background;
	AudioSource fx;
	AudioSource fallFx;

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

		fallFx = gameObject.AddComponent<AudioSource> ();
		fallFx.loop = false;

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
		StartCoroutine (FadeOut (fallFx));
		fx.clip = waterRise;
		fx.Play ();
	}

	public void WaterFall()
	{
		StartCoroutine (FadeOut (fx));
		fallFx.clip = waterFall;
		fx.Play ();
	}

	public static IEnumerator FadeOut (AudioSource audioSource, float FadeTime) {
		float startVolume = audioSource.volume;

		while (audioSource.volume > 0) {
			audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

			yield return null;
		}

		audioSource.Stop ();
		audioSource.volume = startVolume;
	}
	public static IEnumerator FadeIn (AudioSource audioSource, float FadeTime) {
		audioSource.Play ();
		float startVolume = audioSource.volume;

		while (audioSource.volume < 1) {
			audioSource.volume += startVolume * Time.deltaTime / FadeTime;

			yield return null;
		}
		audioSource.volume = startVolume;
	}
}