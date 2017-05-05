using System.Collections;
using UnityEngine;

public class TrackingMissile : Missile
{
	LineRenderer lineRend;

	public AudioClip trackingSound;

	protected override void Update ()
	{
	}

	IEnumerator TakeShot ()
	{
		audio.PlayOneShot (trackingSound);
		Vector3 dirVector;
		Vector3 endSpot = Vector3.zero;
		if (Boat.player.CheckIfAlive ()) {
			dirVector = Boat.player.transform.position - transform.position;
			endSpot = Boat.player.transform.position;
		} else {
			dirVector = new Vector3 (Random.Range (-5, 5), -10, 10) - transform.position;
		}
		endSpot += new Vector3 (endSpot.x - transform.position.x, endSpot.y - transform.position.y) * 10;

		lineRend.SetPosition (0, new Vector3 (transform.position.x, transform.position.y, 60));
		lineRend.SetPosition (1, new Vector3 (endSpot.x, endSpot.y, 60));

		//http://answers.unity3d.com/questions/654222/make-sprite-look-at-vector2-in-unity-2d-1.html
		float angle = Mathf.Atan2 (dirVector.y, dirVector.x) * Mathf.Rad2Deg + 180;
		transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
		dirVector.Normalize ();
		while (!dead) {
			Vector3 vec = Vector3.MoveTowards (transform.position, transform.position + dirVector, speed * Time.deltaTime);
			vec.z = -10;
			transform.position = vec;

			if (lineRend.enabled) {
				lineRend.SetPosition (0, new Vector3 (transform.position.x, transform.position.y, 60));
			}

			yield return null;
		}
	}

	protected override void OnCollisionEnter2D (Collision2D other)
	{
		base.OnCollisionEnter2D (other);

		if (other.gameObject.tag == "Missile") {
			other.gameObject.GetComponent<Missile> ().Explode ();
			Explode ();
		}
	}

	protected override void OnEnable ()
	{
		base.OnEnable ();
		lineRend = gameObject.GetComponent<LineRenderer> ();
		lineRend.enabled = true;
		StartCoroutine (TakeShot ());
	}

	protected override void OnDisable ()
	{
		AudioController.controller.FXChange -= FXChange;
		StopAllCoroutines ();
		MillileSpawner.controller.EnqueueTrackingMissile (gameObject);
	}

	IEnumerator selfDestruct ()
	{
		yield return new WaitForSecondsRealtime (3);
		Explode ();
	}

	public override void Explode ()
	{
		base.Explode ();
		lineRend.enabled = false;
	}
}
