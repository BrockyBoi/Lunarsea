using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : MonoBehaviour {
    public Vector3 destination;
    public float distance = .02f;
    public float magnification = 6;
    Vector3 size;
    bool returning;

	void Start () {
        size = transform.localScale * magnification;

		returning = false;
    }

	void Update () {
		if (Input.GetMouseButtonDown(0) && !returning)
        {
			returning = true;
			StartCoroutine (MoveMoon(Boat.player.transform.position));
        }
    }

	public void GiveVector(Vector3 pos)
	{
		StartCoroutine (MoveMoon(pos));
	}

	IEnumerator MoveMoon(Vector3 pos)
	{
		bool boat = false;
		if (pos == Boat.player.transform.position)
			boat = true;
		
		if (!returning && pos.y <= Boat.player.transform.position.y)
		{
			Boat.player.moonOut = false;
			Destroy(gameObject);
		}

		float t = 0;
		pos.z = 0;

		while (t < 1) {
			if (!boat)
				transform.position = Vector3.Lerp (transform.position, pos, t);
			else
				transform.position = Vector3.Lerp (transform.position, Boat.player.transform.position, t);
			if (!boat)
				transform.localScale = Vector3.Lerp (transform.localScale, size, t);
			else {
				size /= 1.2f;
				transform.localScale = size;
			}
			t += Time.deltaTime * 1.5f;

			yield return null;
		}

		if (boat) {
			moonDone ();
		}
		
	}

//    void moonFly()
//    {
//        transform.position = Vector3.Lerp(transform.position, destination, t);
//        transform.localScale = Vector3.Lerp(transform.localScale, size, t);
//        t += Time.deltaTime;
//    }

    void moonDone()
    {
        AudioController.controller.WaterFall();
        Boat.player.moonOut = false;

        if (Boat.player.CheckTutorialMode())
            TutorialController.controller.SetStage(TutorialController.TutorialStage.DONE);

        Destroy(gameObject);
    }

}
