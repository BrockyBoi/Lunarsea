using System.Collections;
using UnityEngine;

public class TrackingMissile : Missile
{
    LineRenderer lineRend;

    public AudioClip trackingSound;
    private AudioSource audio;

    protected override void Update()
    {}

    void Awake()
    {
        audio = gameObject.AddComponent<AudioSource>();
        audio.clip = trackingSound;
        audio.volume = AudioController.controller.GetFXVolume();
    }
    IEnumerator TakeShot()
    {
        Vector3 dirVector;
        Vector3 endSpot = Vector3.zero;
        if (Boat.player.CheckIfAlive())
        {
            dirVector = Boat.player.transform.position - transform.position;
            endSpot = Boat.player.transform.position;
        }
        else
        {
            dirVector = new Vector3(Random.Range(-5, 5), -10, 10) - transform.position;
        }

        lineRend.SetPosition(0, transform.position);
        lineRend.SetPosition(1, endSpot);

        //http://answers.unity3d.com/questions/654222/make-sprite-look-at-vector2-in-unity-2d-1.html
        float angle = Mathf.Atan2(dirVector.y, dirVector.x) * Mathf.Rad2Deg + 180;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        dirVector.Normalize();
        while (!dead)
        {
            Vector3 vec = Vector3.MoveTowards(transform.position, transform.position + dirVector, speed * Time.deltaTime);
            vec.z = -10;
            transform.position = vec;

            if (lineRend.enabled)
            {
                if (Mathf.Abs(Vector3.Distance(lineRend.GetPosition(0), lineRend.GetPosition(1))) > .5f)
                {
                    lineRend.SetPosition(0, transform.position);
                }
                else lineRend.enabled = false;
            }
            selfDestruct();
            yield return null;
        }
    }

	protected override void OnCollisionEnter2D(Collision2D other)
	{
		base.OnCollisionEnter2D (other);

		if (other.gameObject.tag == "Missile") {
			other.gameObject.GetComponent<Missile> ().Explode ();
			Explode ();
		}
	}
    protected override void OnEnable()
    {
        base.OnEnable();
        lineRend = gameObject.GetComponent<LineRenderer>();
        lineRend.enabled = true;
        StartCoroutine(TakeShot());
    }

    protected override void OnDisable()
    {
        StopAllCoroutines();
		MillileSpawner.controller.EnqueueTrackingMissile (gameObject);
    }

    IEnumerator selfDestruct()
    {
        yield return new WaitForSecondsRealtime(3);
        Explode();
    }

	public override void Explode()
    {
        base.Explode();
        lineRend.enabled = false;
    }
}
