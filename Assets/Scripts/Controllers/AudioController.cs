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
	public AudioClip boatDeath;
	public AudioClip boatHitsRock;
	public AudioClip repairBoat;
	public AudioClip gargling;

	void OnEnable()
	{
	}
	void Awake()
	{
		//DontDestroyOnLoad (this);
		if (controller == null)
			controller = this;
		else
			Destroy (gameObject);
	}
	// Use this for initialization
	void Start () {
		Boat.player.onBoatDeath += BoatDeath;
		
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
		music.loop = true;
		background.loop = true;

	}

	public void PlayFX(AudioClip clip)
	{
		fx.PlayOneShot (clip);
	}

	public void PlayRepairBoat() {
		PlayFX (repairBoat);
	}

	public void Gargle()
	{
		background.clip = gargling;
		background.Play ();
	}

	public void StopGargling()
	{
		background.clip = waveBackground;
		background.Play ();
	}
	public void PlayMissileSound()
	{
		fx.pitch = 1;
		int num = Random.Range (0, missileClips.Count);
		fx.PlayOneShot (missileClips [num]);
	}

	public void WaterRise()
	{
		RandomPitch ();
		StartCoroutine (FadeOut (fallFx,0.2f));
		fx.clip = waterRise;
		fx.Play ();
	}

	public void WaterFall()
	{
		RandomPitch ();
		StartCoroutine (FadeOut (fx,0.2f));
		fallFx.clip = waterFall;
		fallFx.Play ();
	}

	void RandomPitch()
	{
		fx.pitch = (float)Random.Range (.7f, 2);
	}

	public void BoatDeath()
	{
		RandomPitch ();
		PlayFX (boatDeath);
	}
		
	public void BoatHitsRock() {
		RandomPitch ();
		PlayFX (boatHitsRock);
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