using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterJoint : MonoBehaviour
{
	int[] triangles;
	public GameObject prevJoint;
	public GameObject nextJoint;
	int jointNum;
	static int globalJointNum;
	[SerializeField]
	GameObject waterSplashParticlePrefab;

	static Queue<GameObject> particleQueue = new Queue<GameObject> ();


	void Start ()
	{
		jointNum = ++globalJointNum;

		if (jointNum == 1) {
			for (int i = 0; i < 80; i++) {
				GameObject part = Instantiate (waterSplashParticlePrefab, transform.position, Quaternion.Euler (new Vector3 (-90, 0, 0))) as GameObject;
				part.name = "Water Particle " + i;
				part.transform.SetParent (MainCanvas.controller.particles);
				particleQueue.Enqueue (part);
			}	
		}
	}

	public static void ResetStaticVariables ()
	{
		particleQueue.Clear ();
		globalJointNum = 0;
	}

	void OnCollisionEnter2D (Collision2D collision)
	{
		GameObject particle = particleQueue.Dequeue ();
		particle.SetActive (true);
		particle.transform.position = new Vector3 (transform.position.x, transform.position.y, 60);
		StartCoroutine (DisableParticle (particle, .2f));
	}

	IEnumerator DisableParticle (GameObject particle, float timeToDisable)
	{
		float time = 0;
		while (time < timeToDisable) {
			time += Time.deltaTime;
			yield return null;
		}
		EnqueueParticle (particle);
	}

	void EnqueueParticle (GameObject particle)
	{
		particle.SetActive (false);
		particleQueue.Enqueue (particle);
	}

	public float getJointX ()
	{
		return gameObject.transform.position.x;
	}

	public float getJointY ()
	{
		return gameObject.transform.position.y;
	}

	public Vector3 getJointPos ()
	{
		return transform.position;
	}

	public GameObject getLeftJoint ()
	{
		if (prevJoint != null)
			return prevJoint;
		else
			return null;
	}

	public GameObject getRightJoint ()
	{
		if (nextJoint != null)
			return nextJoint;
		else
			return null;
	}
}
