using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : MonoBehaviour {
	bool running = false;
    public Vector3 mouseClick;
    float slope;
    public float distance = .02f;
    public float magnification = 2;
    Vector3 destination;
    Vector3 size;
    float t = 0;
    bool sizeSet = false;

    void Awake()
	{
        
	}
	// Use this for initialization
	void Start () {
        mouseClick.z = 0;
        running = false;
        size = transform.localScale;
        if (mouseClick.y <= Boat.player.transform.position.y)
        {
            Boat.player.moonOut = false;
            Destroy(gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (!running)
        {
            if (!sizeSet)
            {
                destination = mouseClick;
                size = size * magnification;
                sizeSet = true;
            }
        }
        else
        {
            if (!sizeSet)
            {
                destination = Boat.player.transform.position;
                size = size / magnification;
                sizeSet = true;
            }
        }

        transform.position = Vector3.Lerp(transform.position, destination, t);
        transform.localScale = Vector3.Lerp(transform.localScale, size, t);
        t += Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            running = true;
            sizeSet = false;
        }

        if(Vector3.Distance(transform.position, destination) < .00001f)
        {
            t = 0;
            if (running)
            {
                Boat.player.moonOut = false;
                Destroy(gameObject);
            }
        }

    }
	
}
