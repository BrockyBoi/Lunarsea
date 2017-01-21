using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterJoint : MonoBehaviour {
	Mesh mesh;
	List<Vector3> vertices;
	List<Vector2> uv;
	int[] triangles;
	public WaterJoint prevJoint;
	Material waterMaterial;
	[SerializeField]
	GameObject waterMeshPrefab;
	GameObject currentMesh;

	void Awake () {
		vertices = new List<Vector3> ();
		uv = new List<Vector2> ();
		uv.Add (new Vector2(0, 0));
		uv.Add (new Vector2(0, 1));
		uv.Add (new Vector2(1, 0));
		uv.Add (new Vector2(1, 1));
		triangles = new int[6] {0, 1, 3, 3, 2, 0};
		waterMaterial = Resources.Load ("WaterMaterial") as Material;
	}

	public void FixedUpdate() {
		generateMesh();
	}

	public void generateMesh() {
		if (prevJoint == null)
			return;
		if (mesh)
			Destroy (currentMesh);
		vertices = new List<Vector3> ();

		Vector3 bottom = Camera.main.ScreenToWorldPoint(new Vector3 (Screen.width,Screen.height));
		vertices.Add (new Vector3(prevJoint.transform.position.x,0));
		vertices.Add (prevJoint.transform.position);
		vertices.Add (new Vector3(transform.position.x,0));
		vertices.Add (transform.position);

		mesh = new Mesh ();
		//GetComponent<MeshFilter> ().mesh = mesh;
		mesh.vertices = vertices.ToArray ();
		mesh.uv = uv.ToArray ();
		mesh.triangles = triangles;
		currentMesh = Instantiate (waterMeshPrefab);
		currentMesh.GetComponent<MeshFilter> ().mesh = mesh;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
