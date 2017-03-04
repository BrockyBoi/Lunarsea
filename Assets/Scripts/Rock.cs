using UnityEngine;

public class Rock : MonoBehaviour {
	public float speed;
	bool hitPlayer;
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		transform.position = Vector2.MoveTowards (transform.position, new Vector2 (transform.position.x - speed, transform.position.y + Mathf.Sin (0) * Mathf.Deg2Rad), speed);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag ("Player") && !hitPlayer) {
			other.gameObject.GetComponent<Boat> ().TakeDamage ();
			AudioController.controller.BoatHitsRock ();
			hitPlayer = true;
			Debug.Log("Rock hit boat");
		}
	}
}