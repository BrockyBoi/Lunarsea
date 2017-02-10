using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour {
	#region variables
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

	bool tutorialMode;
	#endregion

	void Awake()
	{
		player = this;
		dead = false;
		rb2d = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();


		colliders = new List<Collider2D>(GetComponents<CircleCollider2D> ());
		colliders.Add(GetComponent<BoxCollider2D> ());
	}
		
	void Start () {
		health = 3;
	}

	void Update () {

		float horizontal = Input.GetAxis ("Horizontal") * Time.deltaTime;

		if (Input.GetMouseButtonDown (0) && !moonOut) {
			if (CheckIfAllowed (TutorialController.TutorialStage.SPAWN_MOON))
				CreateMoon ();
		}
			
		Movement (horizontal);
		CheckBoundaries ();
	}

	void CheckBoundaries()
	{
		Vector3 screenCoor = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
		if(transform.position.x > screenCoor.x)
		{
			transform.position = new Vector3(screenCoor.x,transform.position.y);
		}
		if (transform.position.x < -screenCoor.x)
		{
			transform.position = new Vector3(-screenCoor.x, transform.position.y);
		}
	}

	void FixedUpdate() {
		SelfRight ();
	}

	void SelfRight() {
		transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler(new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,0)),Time.deltaTime * uprightConstant);
	}

	void Movement(float h)
	{
		if (h == 0)
			return;

		transform.position = Vector2.MoveTowards (transform.position, transform.position + Vector3.right * hSpeed * h, hSpeed); 


		if (tutorialMode && TutorialController.controller.CheckIfOnStage (TutorialController.TutorialStage.MOVEMENT))
			TutorialController.controller.SetStage (TutorialController.TutorialStage.SPAWN_MOON);
	}

	void CreateMoon()
	{
        moonOut = true;
		GameObject moon = Instantiate (moonPrefab, transform.position, Quaternion.identity) as GameObject;
        moon.GetComponent<Moon>().mouseClick = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		AudioController.controller.WaterRise ();

		if (tutorialMode && TutorialController.controller.CheckIfOnStage (TutorialController.TutorialStage.SPAWN_MOON))
			TutorialController.controller.SetStage (TutorialController.TutorialStage.RETRACT_MOON);
	}

	public void MoonReturned()
	{
		thrown = false;
	}
		
	#region Collisions/Triggers

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
	#endregion

	#region Health Scripts
	public void TakeMissileDamage()
	{
		rb2d.AddForce (Vector2.left * missileForce);
		TakeDamage ();
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

	void Die()
	{
		dead = true;
		MainCanvas.controller.DeathScreen ();
		AudioController.controller.BoatDeath ();
		enabled = false;
	}

	public void AddHealth()
	{
		health = Mathf.Min(health + 1, 3);
		MainCanvas.controller.HealthChange ();
		AudioController.controller.PlayRepairBoat ();
	}

	public int GetHealth()
	{
		return health;
	}
	#endregion

	#region Tutorial Functions

	public bool CheckTutorialMode()
	{
		return tutorialMode;
	}

	public void SetTutorialMode(bool b)
	{
		tutorialMode = b;
	}

	bool CheckIfAllowed(TutorialController.TutorialStage t)
	{
		if ((tutorialMode && TutorialController.controller.CheckIfOnStage (t)) || !tutorialMode)
			return true;

		return false;
	}
	#endregion
}
