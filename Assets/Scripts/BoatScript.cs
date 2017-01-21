using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatScript : MonoBehaviour{
    public double x;  //just values right now
    public double y;  //dont controll position
    public WaterJoint waterJointLeft;           //should these be GameObjects instead?
    public WaterJoint waterJointRight;
    double jointLeftX;
    double jointLeftY;
    double jointRightX;
    double jointRightY;

    // Use this for initialization
    void Start()
    {
        resetXY();  //setting the x and y for right and left water joints
    }
    void resetXY()
    {
        jointLeftX = waterJointLeft.getJointX();
        jointLeftY = waterJointLeft.getJointY();
        jointRightX = waterJointRight.getJointX();
        jointRightY = waterJointRight.getJointY();
    }

    // Update is called once per frame
    void Update()
    {
        // very basic test move
        Vector3 pos = new Vector3(0.05f,0,0);
        if (Input.GetKey("d"))
            gameObject.transform.position += pos;
        if (Input.GetKey("a"))
            gameObject.transform.position -= pos;



        x = gameObject.transform.position.x;
        if (jointLeftX < x)  //temp flipped for neg x
        {
            waterJointRight = waterJointLeft;
            waterJointLeft = waterJointLeft.getLeftJoint();
            resetXY();
        }
        if (jointRightX > x)  //temp flipped for neg x
        {
            waterJointLeft = waterJointRight;
            waterJointRight = waterJointLeft.getRightJoint();
            resetXY();
        }

        double slope = (jointLeftY - jointRightY) / (jointLeftX - jointRightX);

        y = ((x - jointLeftX) * slope) + jointLeftY;

    }
}
