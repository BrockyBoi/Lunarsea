using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormGenerator : MonoBehaviour
{

    #region Variables
    Vector3[] boundaries = new Vector3[2];

    public float speed;


    #endregion

    void Start()
    {
        boundaries[0] = Camera.main.ViewportToWorldPoint(new Vector2(0, .5f));
        boundaries[1] = Camera.main.ViewportToWorldPoint(new Vector2(1, .5f));

        StartCoroutine(MoveStorm());
    }


    void Update()
    {
    }

    IEnumerator MoveStorm()
    {
        int currentSpot = 0;
        while (true)
        {
            while (Vector2.Distance(transform.position, boundaries[currentSpot]) > .5f)
            {
                transform.position = Vector2.MoveTowards(transform.position, boundaries[currentSpot], speed * Time.deltaTime);
                yield return null;
            }
            if (currentSpot == 0)
                currentSpot++;
            else currentSpot = 0;
            yield return null;
        }
    }
}
