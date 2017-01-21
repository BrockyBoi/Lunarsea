using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour {

	public static Boat player;
	public float hSpeed;

	bool aiming;

	public float power;

	public GameObject moonPrefab;

	bool thrown;
	bool dead;

	Rigidbody2D rb2d;
	void Awake()
	{
		player = this;
		dead = false;
		rb2d = GetComponent<Rigidbody2D> ();
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float horizontal = Input.GetAxis ("Horizontal") * Time.deltaTime;

		if (Input.GetMouseButton (0)) {
			aiming = true;
		} else
			aiming = false;
		
		Movement (horizontal);
		AimMoon ();
	}

	void Movement(float h)
	{
		if (dead)
			return;
		
		transform.position = Vector2.MoveTowards (transform.position, transform.position + Vector3.right * h, hSpeed); 
	}

	void AimMoon()
	{
		if (thrown && !dead)
			return;

		if (aiming) {
			power += 60 * Time.deltaTime;
		}

		if (!aiming && power > 0) {
			GameObject moon = Instantiate (moonPrefab, transform.position, Quaternion.identity) as GameObject;
			moon.GetComponent<Rigidbody2D>().AddForce((Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position) * power);
			power = 0;
		}
	}

	public void MoonReturned()
	{
		thrown = false;
	}

	public void TakeDamage()
	{
		Die ();
	}

	void Die()
	{
		MainCanvas.controller.DeathScreen ();
	}
}
