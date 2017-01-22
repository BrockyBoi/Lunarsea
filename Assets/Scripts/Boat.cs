using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour {

	public static Boat player;
	public float hSpeed = 5;
	[SerializeField]
	float uprightConstant = 1.0f;
	bool aiming;
	[SerializeField]
	float missileForce = 500;
	BoxCollider2D boxCollider;
	CircleCollider2D[] circleColliders;
	public bool moonOut = false;

	float power;

	public GameObject moonPrefab;

	bool thrown;
	bool dead;

	Rigidbody2D rb2d;
	int health;

	Animator anim;

	 List<Collider2D> colliders;
	void Awake()
	{
		player = this;
		dead = false;
		rb2d = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();


		colliders = new List<Collider2D>(GetComponents<CircleCollider2D> ());
		colliders.Add(GetComponent<BoxCollider2D> ());


	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Water")) {
			AudioController.controller.Gargle ();
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer ("Water")) {
			AudioController.controller.StopGargling ();
		}
	}
	// Use this for initialization
	void Start () {
		health = 3;
	}
	
	// Update is called once per frame
	void Update () {
        if (dead)
        {
            return;
        }
        Vector3 screenCoor = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
        if(transform.position.x > screenCoor.x)
        {
            transform.position = new Vector3(screenCoor.x,transform.position.y);
        }
        if (transform.position.x < -screenCoor.x)
        {
            transform.position = new Vector3(-screenCoor.x, transform.position.y);
        }

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
		AudioController.controller.WaterRise ();
	}

	public void MoonReturned()
	{
		thrown = false;
	}

	public void TakeDamage()
	{
		health--;
		MainCanvas.controller.HealthChange ();
		anim.SetTrigger ("hit");
		if (health < 1) {
			for (int i = 0; i < colliders.Count; i++) {
				colliders [i].enabled = false;
			}
			Die ();
		}
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.CompareTag ("Missile")) {
			rb2d.AddForce (Vector2.left * missileForce);
		}
	}

	void Die()
	{
		dead = true;
		MainCanvas.controller.DeathScreen ();
		AudioController.controller.BoatDeath ();
	}

	public void AddHealth()
	{
		health++;
		MainCanvas.controller.HealthChange ();
		AudioController.controller.PlayRepairBoat ();
	}

	public int GetHealth()
	{
		return health;
	}
}
