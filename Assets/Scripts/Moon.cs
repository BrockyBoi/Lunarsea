using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : MonoBehaviour {
    public Vector3 destination;
    public float distance = .02f;
    public float magnification = 6;
    Vector3 size;
    float t = 0;
    bool returning;

	// Use this for initialization
	void Start () {
        destination.z = 0;
        size = transform.localScale * magnification;

        if (destination.y <= Boat.player.transform.position.y)
        {
            Boat.player.moonOut = false;
            Destroy(gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () {

        moonFly();

        if (Input.GetMouseButtonDown(0))
        {
            destination = Boat.player.transform.position;
            size = size / magnification;
            returning = true;
        }

        if((Vector3.Distance(transform.position, destination) < distance) && returning == true)
        {
            moonDone();
        }

    }

    void moonFly()
    {
        transform.position = Vector3.Lerp(transform.position, destination, t);
        transform.localScale = Vector3.Lerp(transform.localScale, size, t);
        t += Time.deltaTime;
    }

    void moonDone()
    {
        AudioController.controller.WaterFall();
        Boat.player.moonOut = false;

        if (Boat.player.CheckTutorialMode())
            TutorialController.controller.SetStage(TutorialController.TutorialStage.DONE);

        Destroy(gameObject);
    }

}
