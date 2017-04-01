using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWaveMover : MonoBehaviour {

	#region Variables
	public float speed;
	#endregion

	void Start () {
		
	}
	

	void Update () {
		Vector3 forward = new Vector3 (transform.position.x + -speed, transform.position.y);
		transform.position = Vector2.MoveTowards(transform.position, forward, speed * Time.deltaTime);
	}
}
