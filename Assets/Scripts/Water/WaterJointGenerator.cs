using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterJointGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject waterJointPrefab;
    [SerializeField]
    [Range(0, 200)]
    int numJoints = Screen.width / 10;
    [SerializeField]
    [Range(0.5f, 1)]
    float waterYPos = 0.4f;
    float initialYPos;
    public List<GameObject> joints;
    public static WaterJointGenerator gen;
    public Transform jointParent;
    public Transform meshParent;
    void Awake()
    {
        if (gen == null)
            gen = this;
        else
            Destroy(this);

        Vector3 leftSide = Camera.main.ViewportToWorldPoint(Vector2.zero);
        Vector3 rightSide = Camera.main.ViewportToWorldPoint(Vector2.right);
    }
    // Use this for initialization
    void Start()
    {
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, waterYPos));
        initialYPos = bottomLeft.y;
        joints = new List<GameObject>();
        float jointDistance = (Screen.width + 200) / (numJoints - 1);
        float currentX = Camera.main.ViewportToWorldPoint(Vector2.zero).x - 200;

        for (int i = 0; i < numJoints + (int)(0.1f * numJoints) + (int)(100 / jointDistance); i++)
        {
            Vector3 cpos = Camera.main.ScreenToWorldPoint(new Vector3(currentX, 0)) + new Vector3(0, initialYPos * 2);
            cpos.z = 0;
            currentX += jointDistance;
            GameObject joint = Instantiate(waterJointPrefab, cpos, Quaternion.identity);
            joint.transform.parent = transform;
            joint.transform.SetParent(jointParent);
            Component[] springJointComponents = joint.GetComponents(typeof(SpringJoint2D));
            if (i > 0)
            {
                ((SpringJoint2D)springJointComponents[0]).connectedBody = joints[joints.Count - 1].GetComponent<Rigidbody2D>();
                joint.GetComponent<WaterJoint>().prevJoint = joints[joints.Count - 1].GetComponent<WaterJoint>();
                joint.GetComponent<WaterJoint>().generateMesh();
            }
            else
                joint.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
            ((SpringJoint2D)springJointComponents[1]).connectedAnchor = cpos;
            //add to list of joints
            joints.Add(joint);

        }
        joints[joints.Count - 1].GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
    }

}
