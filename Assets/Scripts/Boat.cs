using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour {

	public static Boat player;
	public float hSpeed = 5;
	[SerializeField]
	float uprightConstant = 1.0f;
	bool aiming;

	public bool moonOut = false;

	float power;

	public GameObject moonPrefab;

	bool thrown;
	bool dead;

	Rigidbody2D rb2d;
	int health;
	void Awake()
	{
		player = this;
		dead = false;
		rb2d = GetComponent<Rigidbody2D> ();
	}
	// Use this for initialization
	void Start () {
		health = 3;
	}
	
	// Update is called once per frame
	void Update () {
		float horizontal = Input.GetAxis ("Horizontal") * Time.deltaTime;
		if(Input.GetMouseButtonDown(0) && !moonOut)
        {
            CreateMoon();
        }
		Movement (horizontal);
	}

	void FixedUpdate() {
		SelfRight ();
	}

	void SelfRight() {
		transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler(new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,0)),Time.deltaTime * uprightConstant);
	}

	void Movement(float h)
	{
		if (dead)
			return;
		
		transform.position = Vector2.MoveTowards (transform.position, transform.position + Vector3.right * hSpeed * h, hSpeed); 
	}

	void CreateMoon()
	{
        moonOut = true;
		GameObject moon = Instantiate (moonPrefab, transform.position, Quaternion.identity) as GameObject;
        moon.GetComponent<Moon>().mouseClick = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}

	public void MoonReturned()
	{
		thrown = false;
	}

	public void TakeDamage()
	{
		health--;
		if (health < 1)
			Die ();
	}

	void Die()
	{
		dead = true;
		MainCanvas.controller.DeathScreen ();
	}

	public void AddHealth()
	{
		health++;
	}
}
