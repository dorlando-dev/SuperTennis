using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject ball;
    public GameObject racket;
    public float hitThreshold = 2f;
    private Rigidbody ballRb;

    public Vector3 hit;

    void Start()
    {
        ballRb = ball.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("hitting ball");
            hitBall();
        }
    }

    void hitBall()
    {
        float z = 0;
        if(Input.GetKey("left"))
        {
            z = 8;
        }
        else if(Input.GetKey("right"))
        {
            z = -8;
        }
        hit.z = z;

        if(Input.GetKey("up"))
        {
            hit.y = 15;
            hit.x = 10;
        } 
        else if (Input.GetKey("down"))
        {
            hit.y = 10;
            hit.x = 10;
        }


        float dist = Vector3.Distance(ball.transform.position, transform.position);
        Debug.Log(dist);
        if (dist <= hitThreshold)
        {
            ball.transform.position = racket.transform.position;
            //ballRb.AddForce(hit.x, hit.y, hit.z, ForceMode.Impulse);
            ballRb.velocity = hit;
        }
    }
}