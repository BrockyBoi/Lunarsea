using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoat : MonoBehaviour
{

	#region Variables

	public static EnemyBoat currentBoss;
	int health = 3;
	bool dead;
	public bool throwsProjectile;
	public GameObject explosiveProjectile;
	public GameObject normalProjectile;
	[SerializeField]
	List<Collider2D> colliders;

	public float force;
	public float spawnRate;
	float uprightConstant = 1.0f;

	int phase;

	public float speed;
	bool sailingIn;

	public AudioClip cannonFire;
	AudioSource audio;

	#endregion

	void Awake ()
	{
		currentBoss = this;
	}

	void Start ()
	{
		audio = gameObject.AddComponent<AudioSource> ();
		MainCanvas.controller.StartBossBattle (health);
		BackgroundConroller.controller.StopScrolling ();
		StartCoroutine (SailIn ());
		audio.volume = AudioController.controller.GetFXVolume ();
	}

	void FixedUpdate ()
	{
		CheckBoundaries ();

		float endPoint = Camera.main.ViewportToWorldPoint (new Vector2 (.9f, 0)).x;
		if (Vector2.Distance (transform.position, new Vector2 (endPoint, transform.position.y)) > .05f) {
			transform.position = Vector3.MoveTowards (transform.position, new Vector2 (endPoint, transform.position.y), speed / 2 * Time.deltaTime);
		}
		SelfRight ();
	}

	void SelfRight ()
	{
		transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (new Vector3 (transform.eulerAngles.x, transform.eulerAngles.y, 0)), Time.deltaTime * uprightConstant);
	}

	void CheckBoundaries ()
	{
		Vector3 leftSide = Camera.main.ViewportToWorldPoint (Vector2.zero);
		Vector3 rightSide = Camera.main.ViewportToWorldPoint (Vector2.right);

		if (!sailingIn && transform.position.x > rightSide.x - 1) {
			transform.position = new Vector3 (rightSide.x - 1, transform.position.y);
		}
		if (transform.position.x < leftSide.x + 1) {
			transform.position = new Vector3 (leftSide.x + 1, transform.position.y);
		}
	}

	IEnumerator SailIn ()
	{
		sailingIn = true;
		float endPoint = Camera.main.ViewportToWorldPoint (new Vector2 (.9f, 0)).x;

		while (Vector2.Distance (transform.position, new Vector2 (endPoint, transform.position.y)) > .05f) {
			transform.position = Vector3.MoveTowards (transform.position, new Vector2 (endPoint, transform.position.y), speed / 2 * Time.deltaTime);
			yield return null;
		}

		if (throwsProjectile)
			SpawnProjectiles ();

		sailingIn = false;
	}

	void SpawnProjectiles ()
	{
		if (dead)
			return;

		audio.PlayOneShot (cannonFire);
		GameObject proj = null;
		switch (phase) {
		case 0:
			proj = Instantiate (normalProjectile, transform.position, Quaternion.identity) as GameObject;
			break;
		case 1:
			proj = Instantiate (normalProjectile, transform.position, Quaternion.identity) as GameObject;
			break;
		default:
			proj = Instantiate (explosiveProjectile, transform.position, Quaternion.identity) as GameObject;
			break;

		}

		Vector2 dir = new Vector2 (-.25f, Mathf.Sin (Random.Range (0.05f, 0.35f)));
		proj.GetComponent<Rigidbody2D> ().AddForce (dir.normalized * (force * Random.Range (1.0f, 1.5f)));
		proj.transform.position = new Vector3 (proj.transform.position.x, proj.transform.position.y, 10);

		if (!dead)
			Invoke ("SpawnProjectiles", spawnRate * Random.Range (.5f, 1.5f));
	}

	public int GetPhase ()
	{
		return phase;
	}

	public void TakeDamage ()
	{
		phase++;
		health--;
		MainCanvas.controller.BossTakesDamage (health);
		if (!dead && health <= 0) {
			Die ();
		}
	}

	void Die ()
	{
		dead = true;
		AudioController.controller.BoatDeath ();
		gameObject.layer = LayerMask.NameToLayer ("IgnoreWater");
		MainCanvas.controller.BossBeaten ();
	}

	public bool CheckIfAlive ()
	{
		return !dead;
	}
}
