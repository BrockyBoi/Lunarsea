using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Coin : MonoBehaviour
{
	public GameObject particleEffect;
	bool followPlayer;

	[SerializeField]
	static float startingSpeed = 4;
	static float speed;

	AudioSource audio;
	public AudioClip coinPickUp1;
	public AudioClip coinPickUp2;

	static Queue<GameObject> particleQueue = new Queue<GameObject> ();

	SpriteRenderer spriteRend;
	CircleCollider2D collider;

	int coinNum;
	static int globalCoinNum;


	void Awake ()
	{
		collider = GetComponent<CircleCollider2D> ();
		audio = gameObject.AddComponent<AudioSource> ();
		spriteRend = GetComponent<SpriteRenderer> ();
		speed = startingSpeed;
	}

	void Start ()
	{
		AudioController.controller.FXChange += ChangeFX;


		coinNum = ++globalCoinNum;

		if (coinNum == 1) {
			for (int i = 0; i < 75; i++) {
				GameObject particle = Instantiate (particleEffect, transform.position, Quaternion.identity) as GameObject;
				particle.transform.SetParent (MainCanvas.controller.particles);
				particle.SetActive (false);
				particleQueue.Enqueue (particle);
			}
		}
	}

	public static void ResetStaticVariables ()
	{
		particleQueue.Clear ();
		globalCoinNum = 0;
	}

	void Update ()
	{
		Move (speed * 1.25f);
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.CompareTag ("Player")) {
			ChooseFX ();
			CoinController.controller.addCoin ();
			EnableParticle ();
			spriteRend.enabled = false;
			collider.enabled = false;

			Invoke ("TurnOffObject", 1);
		}

		if (other.gameObject.CompareTag ("Magnet")) {
			FollowPlayer ();
		}
	}

	void EnableParticle ()
	{
		GameObject particle = particleQueue.Dequeue ();
		particle.SetActive (true);
		particle.transform.position = transform.position;
		StartCoroutine (DisableParticle (particle, .65f));

	}

	void ChangeFX (float f)
	{
		audio.volume = f;
	}

	IEnumerator DisableParticle (GameObject particle, float timeToDisable)
	{
		float time = 0;
		while (time < timeToDisable) {
			time += Time.deltaTime;
			yield return null;
		}
		particle.SetActive (false);
		particleQueue.Enqueue (particle);
	}

	void TurnOffObject ()
	{
		gameObject.SetActive (false);
	}

	void ChooseFX ()
	{
		int randomNum = Random.Range (0, 2);
		if (randomNum == 0)
			audio.PlayOneShot (coinPickUp1);
		else
			audio.PlayOneShot (coinPickUp2);
	}

	void OnEnable ()
	{
		spriteRend.enabled = true;
		collider.enabled = true;
		audio.volume = AudioController.controller.GetFXVolume ();
	}

	void OnDisable ()
	{
		AudioController.controller.FXChange -= ChangeFX;
		followPlayer = false;
		CoinController.controller.EnqueueCoin (gameObject);
	}

	void Move (float speed)
	{
		if (!followPlayer) {
			if (speed == 0) {
				return;
			} else if (speed > 0) {
				Vector3 vec = Vector3.MoveTowards (transform.position, transform.position - Vector3.right * speed * Time.deltaTime, speed);
				vec.z = 10;
				transform.position = vec;
			} else {
				//negative case must be handled slightly differently so that the direction is simply reversed
				Vector3 vec = Vector3.MoveTowards (transform.position, transform.position - Vector3.right * speed * Time.deltaTime, 0 - speed);
				vec.z = 10;

				transform.position = vec;
			}
		} else if (followPlayer) {
			transform.position = Vector3.MoveTowards (transform.position, Boat.player.transform.position, 20 * Time.deltaTime);
		}
	}

	void FollowPlayer ()
	{
		followPlayer = true;
	}

	public static void GiveSpeedMultiplier (float speedMult)
	{
		speed = startingSpeed + speedMult;
	}
}