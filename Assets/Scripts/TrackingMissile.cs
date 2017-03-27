﻿using System.Collections;
using UnityEngine;

public class TrackingMissile : Missile
{
    LineRenderer lineRend;

    protected virtual void Update()
    { }
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

        //http://answers.unity3d.com/questions/654222/make-sprite-look-at-vector2-in-unity-2d-1.html
        float angle = Mathf.Atan2(dirVector.y, dirVector.x) * Mathf.Rad2Deg - 180;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        while (true)
        {
            Vector3 vec = Vector3.MoveTowards(transform.position, transform.position + dirVector, speed * Time.deltaTime);
            vec.z = -10;
            transform.position = vec;
            if (lineRend.enabled == true && Boat.player.CheckIfAlive() && Mathf.Abs(Vector3.Distance(lineRend.GetPosition(0), lineRend.GetPosition(1))) > .05f)
            {
                lineRend.SetPosition(0, transform.position);
                lineRend.SetPosition(1, endSpot);
            }
            else lineRend.enabled = false;
            yield return null;
        }
    }
    protected virtual void OnEnable()
    {
        base.OnEnable();
        lineRend = gameObject.GetComponent<LineRenderer>();
        StartCoroutine(TakeShot());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    protected virtual void Explode()
    {
        base.Explode();
        lineRend.enabled = false;
    }
}
