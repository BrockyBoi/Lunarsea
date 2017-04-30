using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterJointGenerator : MonoBehaviour
{
	[SerializeField]
	GameObject waterJointPrefab;
	[SerializeField]
	[Range (0, 200)]
	int numJoints = Screen.width / 10;
	[SerializeField]
	[Range (0.5f, 1)]
	float waterYPos;
	float initialYPos;
	public List<GameObject> joints;
	public static WaterJointGenerator gen;
	public Transform jointParent;
	public Transform meshParent;
    


	Mesh mesh;
	[SerializeField]
	GameObject waterMeshPrefab;
	GameObject currentMesh;

	Material waterMaterial;

	List<int> triangles = new List<int> ();
	List<Vector3> vertices;
	List<Vector2> uv;

	public static WaterJointGenerator controller;

	void Awake ()
	{
		if (gen == null)
			gen = this;
		else
			Destroy (this);

		controller = this;
		Vector3 leftSide = Camera.main.ViewportToWorldPoint (Vector2.zero);
		Vector3 rightSide = Camera.main.ViewportToWorldPoint (Vector2.right);
	}
	// Use this for initialization
	void Start ()
	{
		Vector3 bottomLeft = Camera.main.ViewportToWorldPoint (new Vector3 (0, waterYPos));
		Debug.Log (GetWaterHeight ());
		initialYPos = bottomLeft.y;
		joints = new List<GameObject> ();
		float jointDistance = (Screen.width + (Screen.width / 4)) / (numJoints - 1);
		float currentX = Camera.main.ViewportToWorldPoint (Vector2.zero).x - (Screen.width / 8);

		for (int i = 0; i < numJoints + (int)(0.1f * numJoints) + (int)((Screen.width / 8) / jointDistance); i++) {
			Vector3 cpos = Camera.main.ScreenToWorldPoint (new Vector3 (currentX, 0)) + new Vector3 (0, initialYPos * 2);
			cpos.z = 0;
			currentX += jointDistance;
			GameObject joint = Instantiate (waterJointPrefab, cpos, Quaternion.identity);
			joint.transform.parent = transform;
			joint.transform.SetParent (jointParent);
			Component[] springJointComponents = joint.GetComponents (typeof(SpringJoint2D));
			if (i > 0) {
				((SpringJoint2D)springJointComponents [0]).connectedBody = joints [joints.Count - 1].GetComponent<Rigidbody2D> ();
				joint.GetComponent<WaterJoint> ().prevJoint = joints [joints.Count - 1];
				//joint.GetComponent<WaterJoint>().generateMesh();
			} else
				joint.GetComponent<Rigidbody2D> ().constraints = RigidbodyConstraints2D.FreezePosition;
			((SpringJoint2D)springJointComponents [1]).connectedAnchor = cpos;
			//add to list of joints
			joints.Add (joint);
            
		}
		joints [joints.Count - 1].GetComponent<Rigidbody2D> ().constraints = RigidbodyConstraints2D.FreezePosition;


		InitilizeMesh ();
		UpdateMesh ();
	}

	public float GetWaterHeight ()
	{
		return Camera.main.ViewportToWorldPoint (new Vector3 (0, waterYPos)).y; 
	}

	public void FixedUpdate ()
	{
		//ChangeMesh();
		UpdateMesh ();
	}

	void InitilizeMesh ()
	{
		vertices = new List<Vector3> ();
		uv = new List<Vector2> ();


		for (int i = 0; i < joints.Count; i++) {
			if ((i % 2) == 1) {
				uv.Add (new Vector2 (0, 0));
				uv.Add (new Vector2 (0, 1));
			} else {
				uv.Add (new Vector2 (1, 0));
				uv.Add (new Vector2 (1, 1));
			}
		}

		for (int i = 0; i < joints.Count * 1.85; i++) {
			if ((i % 2) != 1) {
				triangles.Add (i);
				triangles.Add (i + 2);
				triangles.Add (i + 1);
			} else {
				triangles.Add (i + 1);
				triangles.Add (i + 2);
				triangles.Add (i);
			}
            
            
		}
        

		currentMesh = Instantiate (waterMeshPrefab, Vector3.zero, Quaternion.identity);
		currentMesh.transform.SetParent (WaterJointGenerator.gen.meshParent);
	}

	void UpdateMesh ()
	{
		vertices = new List<Vector3> ();

		Vector3 bottom = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0));

		int j = 0;
		for (int i = 0; i < joints.Count; i++) {
			vertices.Add (joints [i].GetComponent<WaterJoint> ().getJointPos ());
			vertices.Add (new Vector3 (joints [i].GetComponent<WaterJoint> ().getJointX (), bottom.y));
			j = j + 1;
		}

		Destroy (mesh);
		mesh = new Mesh ();
		mesh.vertices = vertices.ToArray ();
		mesh.uv = uv.ToArray ();
		mesh.triangles = triangles.ToArray ();
		Mesh oldMesh = currentMesh.GetComponent<MeshFilter> ().mesh;
		currentMesh.GetComponent<MeshFilter> ().mesh = mesh;
		Destroy (oldMesh);
	}

	void ChangeMesh ()
	{
		Mesh meshTest = currentMesh.GetComponent<MeshFilter> ().mesh;
		Vector3[] vertices = meshTest.vertices;
		Vector3[] normals = meshTest.normals;
		int i = 0;
		while (i < vertices.Length) {
			if ((i % 2) != 1) {
				vertices [i] = joints [i].GetComponent<WaterJoint> ().getJointPos ();
			}
			i++;
		}
		meshTest.vertices = vertices;
	}

}
