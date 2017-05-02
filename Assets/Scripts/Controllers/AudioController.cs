﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioController : MonoBehaviour
{
	public static AudioController controller;

	#region Audio Sources

	AudioSource music;
	AudioSource background;
	AudioSource fx;
	AudioSource fallFx;

	#endregion

	public AudioClip mainMusic;
	public AudioClip waveBackground;

	#region Water Clips

	public AudioClip waterRise;
	public AudioClip waterFall;
	public AudioClip gargling;

	#endregion

	#region coinClips

	public AudioClip coinPickUp1;
	public AudioClip coinPickUp2;

	#endregion

	#region UI Clips

	public AudioClip click;
	public AudioClip coinPurchase;
	public AudioClip woodSignDrop;
	public AudioClip woodSignPullUp;

	#endregion

	#region Enemy Clips

	public List<AudioClip> missileClips;
	public AudioClip sonarPing;
	public AudioClip missileFire;
	public AudioClip cannonFire;
	public AudioClip homingMissile;

	#endregion

	#region Boat Clips

	public AudioClip boatDeath;
	public AudioClip boatHitsRock;
	public AudioClip repairBoat;
	public AudioClip mastUnfurl;

	#endregion

	public delegate void AudioChange (float f);

	public event AudioChange FXChange;
	public event AudioChange MusicChange;

	void Awake ()
	{
		//DontDestroyOnLoad (this);
		if (controller == null)
			controller = this;
		else
			this.enabled = false;

		music = gameObject.AddComponent<AudioSource> ();
		background = gameObject.AddComponent<AudioSource> ();
		fx = gameObject.AddComponent<AudioSource> ();
		fallFx = gameObject.AddComponent<AudioSource> ();

		music.volume = .5f;
		fx.volume = .5f;
		fallFx.volume = .5f;
		background.volume = .5f;

	}
	// Use this for initialization
	void Start ()
	{
		if (Boat.player != null)
			Boat.player.onBoatDeath += BoatDeath;


		music.clip = mainMusic;
		music.Play ();


		background.clip = waveBackground;
		background.Play ();


		fx.loop = false;


		fallFx.loop = false;
		music.loop = true;
		background.loop = true;

	}

	void OnEnable ()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable ()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	void OnLevelFinishedLoading (Scene scene, LoadSceneMode mode)
	{
		if (scene == SceneManager.GetSceneByName ("MainScene") && PlayerInfo.controller.CheckIfFirstTime ()) {
			ChangeFXVolume (GameModeController.controller.GetFXLevel (), true);
			ChangeMusicVolume (GameModeController.controller.GetMusicLevel (), true);
		}
	}

	public void ClickUI ()
	{
		PlayFX (click);
	}

	public void PlayFX (AudioClip clip)
	{
		fx.PlayOneShot (clip);
	}

	public void PlayRepairBoat ()
	{
		PlayFX (repairBoat);
	}

	public void Gargle ()
	{
		background.clip = gargling;
		background.Play ();
	}

	public void StopGargling ()
	{
		background.clip = waveBackground;
		background.Play ();
	}

	public void PlayMissileSound ()
	{
		fx.pitch = 1;
		int num = Random.Range (0, missileClips.Count);
		fx.PlayOneShot (missileClips [num]);
	}

	public AudioClip GetMissileSound ()
	{
		return missileClips [Random.Range (0, missileClips.Count)];
	}

	public void WaterRise ()
	{
		RandomPitch ();
		StartCoroutine (FadeOut (fallFx, 0.2f));
		fx.clip = waterRise;
		fx.Play ();
	}

	public void WaterFall ()
	{
		RandomPitch ();
		StartCoroutine (FadeOut (fx, 0.2f));
		fallFx.clip = waterFall;
		fallFx.Play ();
	}

	void RandomPitch ()
	{
		fx.pitch = (float)Random.Range (.7f, 2);
	}

	public void BoatDeath ()
	{
		RandomPitch ();
		PlayFX (boatDeath);
	}

	public void BoatHitsRock ()
	{
		RandomPitch ();
		PlayFX (boatHitsRock);
	}

	public void SliderSetMusicVolume (float f)
	{
		ChangeMusicVolume (f);
	}

	public void ChangeMusicVolume (float f, bool initialize = false)
	{
		music.volume = f;
		background.volume = f;

		if (!initialize)
			MusicChange (f);
	}

	public float GetMusicVolume ()
	{
		return music.volume;
	}

	public void SliderSetFXVolume (float f)
	{
		ChangeFXVolume (f);
	}

	public void ChangeFXVolume (float f, bool initialize = false)
	{
		fx.volume = f;
		fallFx.volume = f;
		if (!initialize)
			FXChange (f);
	}

	public float GetFXVolume ()
	{
		return fx.volume;
	}

	public static IEnumerator FadeOut (AudioSource audioSource, float FadeTime)
	{
		float startVolume = audioSource.volume;

		while (audioSource.volume > 0) {
			audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

			yield return null;
		}

		audioSource.Stop ();
		audioSource.volume = startVolume;
	}

	public static IEnumerator FadeIn (AudioSource audioSource, float FadeTime)
	{
		audioSource.Play ();
		float startVolume = audioSource.volume;

		while (audioSource.volume < 1) {
			audioSource.volume += startVolume * Time.deltaTime / FadeTime;

			yield return null;
		}
		audioSource.volume = startVolume;
	}
}