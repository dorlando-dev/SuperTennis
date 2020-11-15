using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject ball;
    public GameObject racket;
    public float hitThreshold = 2f;
    private Rigidbody ballRb;
    BallHitter ballHitter;

    public Vector3 hit;

    void Start()
    {
        ballRb = ball.GetComponent<Rigidbody>();
        ballHitter = new BallHitter(racket.transform);
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
            hit.y = 17;
            hit.x = 17;
        } 
        else if (Input.GetKey("down"))
        {
            hit.y = 10;
            hit.x = 15;
        }
        ball.gameObject.GetComponent<Ball>().Freeze(false);
        MatchManager.Instance.SetLastHit(1);
        float dist = Vector3.Distance(ball.transform.position, transform.position);
        if (dist <= hitThreshold)
        {
            ball.transform.position = racket.transform.position;
            Vector3 hit = ballHitter.hitLob();
            //ballRb.AddForce(hit.x, hit.y, hit.z, ForceMode.Impulse);
            ballRb.velocity = hit;
        }
    }
}