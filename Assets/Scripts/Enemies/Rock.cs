using UnityEngine;

public class Rock : MonoBehaviour
{
	[SerializeField]
	static float startingSpeed = 7;
	static float speed;

	// Update is called once per frame

	void Awake ()
	{
		speed = startingSpeed;
	}


	void Update ()
	{
		transform.position = Vector2.MoveTowards (transform.position, new Vector2 (transform.position.x - speed * Time.deltaTime, transform.position.y + Mathf.Sin (0) * Mathf.Deg2Rad), speed);
	}

	void OnDisable ()
	{
		MillileSpawner.controller.EnqueueRock (gameObject);
		MillileSpawner.controller.GetOutofCurrentRockList (this);
	}


	void OnCollisionEnter2D (Collision2D other)
	{
		if (other.gameObject.CompareTag ("Player")) {
			other.gameObject.GetComponent<Boat> ().TakeDamage ();
			AudioController.controller.BoatHitsRock ();
		}

		if (other.gameObject.CompareTag ("Enemy Boat") && EnemyBoat.currentBoss.GetPhase () == 0) {
			Debug.Log ("Rock hit boss boat");
			other.gameObject.GetComponent<EnemyBoat> ().TakeDamage ();
			AudioController.controller.BoatHitsRock ();
		}
	}

	public void StopObject ()
	{
		speed = 0;
	}

	public static void GiveSpeedMultiplier (float mult)
	{
		speed = startingSpeed + mult;
	}
		
}