using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterJoint : MonoBehaviour {
	Mesh mesh;
	List<Vector3> vertices;
	List<Vector2> uv;
	int[] triangles;
	public WaterJoint prevJoint;
	// Use this for initialization
	void Awake () {
		vertices = new List<Vector3> ();
		uv = new List<Vector2> ();
		uv.Add (new Vector2(0, 0));
		uv.Add (new Vector2(0, 1));
		uv.Add (new Vector2(1, 0));
		uv.Add (new Vector2(1, 1));
		triangles = new int[6] {0, 1, 3, 3, 2, 0};
	}

	public void generateMesh() {
		if (prevJoint == null)
			return;
		
		vertices.Add (prevJoint.transform.position);
		vertices.Add (transform.position);
		Vector3 bottom = Camera.main.ScreenToWorldPoint(new Vector3 (Screen.width,Screen.height));
		vertices.Add (new Vector3(prevJoint.transform.position.x,bottom.y));
		vertices.Add (new Vector3(transform.position.x,bottom.y));

		mesh = new Mesh ();
		GetComponent<MeshFilter> ().mesh = mesh;
		mesh.vertices = vertices.ToArray ();
		mesh.uv = uv.ToArray ();
		mesh.triangles = triangles;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
