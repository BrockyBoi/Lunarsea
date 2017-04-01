using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundConroller : MonoBehaviour
{
    public static BackgroundConroller controller;
    public GameObject layer0;
    public GameObject layer1;
    public GameObject layer2;

    GameObject[][] layer;
    public float[] speed = { 2, 4, 3 };
    int[] front;                //keeps track of which tile is leftmost

    float speedMult;

    void Awake()
    {
        controller = this;
    }

    // Use this for initialization
    void Start()
    {
        layer = new GameObject[3][];
        front = new int[3];

        Setup(0, layer0);

        if (layer1 != null)
        {
            Setup(1, layer1);
        }
        if (layer2 != null)
        {
            Setup(2, layer2);
        }

        StopScrolling();
    }


    // Update is called once per frame
    void Update()
    {
        Move(0);
        Check(0);

        if (layer1 != null)
        {
            Move(1);
            Check(1);
        }
        if (layer2 != null)
        {
            Move(2);
            Check(2);
        }
    }


    #region Setup
    void Setup(int lay, GameObject theLayer)
    {
        layer[lay] = new GameObject[3];
        layer[lay][0] = Instantiate(theLayer, new Vector3(-6, 0, 0), Quaternion.identity);
        layer[lay][0].transform.SetParent(gameObject.transform);
        layer[lay][1] = Instantiate(theLayer, new Vector3(18.5f, 0, 0), Quaternion.identity);
        layer[lay][1].transform.SetParent(gameObject.transform);
        layer[lay][2] = Instantiate(theLayer, new Vector3(43, 0, 0), Quaternion.identity);
        layer[lay][2].transform.SetParent(gameObject.transform);

        front[lay] = 0;
    }
    #endregion

    #region Movement
    //lay is integer representing which layer is to be moved
    //speed is float determining how fast the background scrolls
    void Move(int lay)
    {
        if (speed[lay] == 0)
        {
            return;
        }
        else if (speed[lay] > 0)
        {
            for (int i = 0; i < 3; i++)
            {
                layer[lay][i].transform.position = Vector2.MoveTowards(layer[lay][i].transform.position, layer[lay][i].transform.position - Vector3.right * speed[lay] * Time.deltaTime, speed[lay]);
            }
        }
        else
        {
            //negative case must be handled slightly differently so that the direction is simply reversed
            for (int i = 0; i < 3; i++)
            {
                layer[lay][i].transform.position = Vector2.MoveTowards(layer[lay][i].transform.position, layer[lay][i].transform.position - Vector3.right * speed[lay] * Time.deltaTime, 0 - speed[lay]);
            }
        }
    }

    //lay is integer representing which layer is to be moved
    void Check(int lay)
    {
        if (speed[lay] == 0)
        {
            return;
        }
        else if (speed[lay] > 0)
        {
            if (layer[lay][front[lay]].transform.position.x <= -30.5)
            {
                layer[lay][front[lay]].transform.position = new Vector3(43, 0, 0);
                front[lay] = (front[lay] + 1) % 3;
            }
            return;
        }
        else
        {
            if (layer[lay][(front[lay] + 2) % 3].transform.position.x >= 43)
            {
                layer[lay][(front[lay] + 2) % 3].transform.position = new Vector3(-30.5f, 0, 0);
                front[lay] = (front[lay] + 2) % 3;
            }
        }
    }
    #endregion

    #region Setters and Getters
    //Takes in one float argument which is the new speed to be set to the base
    //This function automatically adjusts the other layers to keep the ratio if the base speed was not zero at first
    //if the base speed was zero, it simply adds the new speed to the other layers
    public void setSpeed(float newSpeed)
    {
        if (speed[0] != 0)
        {
            if (speed[1] != 0)
            {
                speed[1] = (speed[1] * newSpeed) / speed[0];
            }
            if (speed[2] != 0)
            {
                speed[2] = (speed[2] * newSpeed) / speed[0];
            }
        }
        else
        {
            speed[1] += newSpeed;
            speed[2] += newSpeed;
        }
        speed[0] = newSpeed;
    }

    //Takes in two arguments, an integer representing the layer whose speed will be changed and a float to change it to
    public void setSpeed(int lay, float newSpeed)
    {
        speed[lay] = newSpeed;
    }

    //Takes in one optional argument of which layer to get the speed of. Defaults to the base layer
    public float getSpeed(int lay = 0)
    {
        return speed[lay];
    }

    public void UpdateSpeedMult(float mult)
    {
        for (int i = 0; i < speed.Length; i++)
        {
            if (speed[i] > 0)
            {
                speed[i] += mult;
            }
            else if (speed[i] < 0)
            {
                speed[i] -= mult;
            }
        }
    }

    public void BeginLevel()
    {
        speed = new float[]{ 2, 4, 3 };
    }

    public void StopScrolling()
    {
        setSpeed(0, 0);
        setSpeed(1, .5f);
        setSpeed(2, .35f);

    }
    #endregion

}
