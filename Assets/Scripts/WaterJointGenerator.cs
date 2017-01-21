using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterJointGenerator : MonoBehaviour {
	[SerializeField]
	GameObject waterJointPrefab;
	[SerializeField]
	int numJoints = Screen.width/10;
	float initialYPos = Screen.height/2;
	public List<GameObject> joints;
	public static WaterJointGenerator gen;
	void Awake() {
		if (gen == null)
			gen = this;
		else
			Destroy (this);
	}
	// Use this for initialization
	void Start () {
		joints = new List<GameObject> ();
		float jointDistance = Screen.width / (numJoints-1);
		float currentX = 0;

		for(int i = 0; i < numJoints; i++) {
			Vector3 cpos = Camera.main.ScreenToWorldPoint(new Vector3 (currentX,initialYPos));
			cpos.z = 0;
			currentX += jointDistance;
			GameObject joint = Instantiate (waterJointPrefab, cpos, Quaternion.identity);
			joint.transform.parent = transform;
			Component[] springJointComponents = joint.GetComponents (typeof(SpringJoint2D));
			if (i > 0) {
				((SpringJoint2D)springJointComponents [0]).connectedBody = joints [joints.Count - 1].GetComponent<Rigidbody2D> ();
				joint.GetComponent<WaterJoint> ().prevJoint = joints [joints.Count - 1].GetComponent<WaterJoint> ();
				joint.GetComponent<WaterJoint> ().generateMesh ();
			}
			else
				joint.GetComponent<Rigidbody2D> ().constraints = RigidbodyConstraints2D.FreezePosition;
			((SpringJoint2D)springJointComponents [1]).connectedAnchor = cpos;
			//add to list of joints
			joints.Add (joint);

		}
		joints[joints.Count-1].GetComponent<Rigidbody2D> ().constraints = RigidbodyConstraints2D.FreezePosition;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
