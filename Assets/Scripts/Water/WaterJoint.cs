using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterJoint : MonoBehaviour {
	int[] triangles;
	public GameObject prevJoint;
    public GameObject nextJoint;
	[SerializeField]
	GameObject waterSplashParticlePrefab;

    

	void OnCollisionEnter2D(Collision2D collision) {
		GameObject part = Instantiate (waterSplashParticlePrefab, transform.position, Quaternion.Euler(new Vector3(-90,0,0))) as GameObject;
		part.transform.SetParent(MainCanvas.controller.particles);
		Destroy(part,0.2f);
	}

	// Update is called once per frame
	void Update () {
		
	}

    public float getJointX()
    {
        return gameObject.transform.position.x;
    }
    public float getJointY()
    {
        return gameObject.transform.position.y;
    }

    public Vector3 getJointPos()
    {
        return transform.position;
    }

    public GameObject getLeftJoint()
    {
        if (prevJoint != null)
            return prevJoint;
        else
            return null;
    }
    public GameObject getRightJoint()
    {
        if (nextJoint != null)
            return nextJoint;
        else
            return null;
    }
}
