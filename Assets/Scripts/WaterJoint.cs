using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterJoint : MonoBehaviour {
	Mesh mesh;
	List<Vector3> vertices;
	List<Vector2> uv;
	int[] triangles;
	public WaterJoint prevJoint;
    public WaterJoint nextJoint;
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
		currentMesh = Instantiate (waterMeshPrefab,Vector3.zero,Quaternion.identity);
	}

	public void FixedUpdate() {
		generateMesh();
	}

	public void generateMesh() {
		if (prevJoint == null)
			return;
		vertices = new List<Vector3> ();

		Vector3 bottom = Camera.main.ViewportToWorldPoint (new Vector3(0,0));

		vertices.Add (transform.position);
		vertices.Add (new Vector3(transform.position.x,bottom.y));
		vertices.Add (prevJoint.transform.position);
		vertices.Add (new Vector3(prevJoint.transform.position.x,bottom.y));
		Destroy (mesh);
		mesh = new Mesh ();
		mesh.vertices = vertices.ToArray ();
		mesh.uv = uv.ToArray ();
		mesh.triangles = triangles;
		Mesh oldMesh = currentMesh.GetComponent<MeshFilter> ().mesh;
		currentMesh.GetComponent<MeshFilter> ().mesh = mesh;
		Destroy(oldMesh);
	}

	// Update is called once per frame
	void Update () {
		
	}

    public double getJointX()
    {
        return gameObject.transform.position.x;
    }
    public double getJointY()
    {
        return gameObject.transform.position.y;
    }

    public WaterJoint getLeftJoint()
    {
        if (prevJoint != null)
            return prevJoint;
        else
            return null;
    }
    public WaterJoint getRightJoint()
    {
        if (nextJoint != null)
            return nextJoint;
        else
            return null;
    }
}
