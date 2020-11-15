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
            //Debug.Log("hitting ball");
            hitBall();
        }
    }

    void hitBall()
    {
        BallHitter.Side side = BallHitter.Side.Center;
        if(Input.GetKey("left"))
        {
            side = BallHitter.Side.Left;
        }
        else if(Input.GetKey("right"))
        {
            side = BallHitter.Side.Right;
        }

        BallHitter.Strength depth = BallHitter.Strength.Middle;
        if(Input.GetKey("up"))
        {
            depth = BallHitter.Strength.Lob;
        } 
        else if (Input.GetKey("down"))
        {
            depth = BallHitter.Strength.Drop;
        }
        ball.gameObject.GetComponent<Ball>().Freeze(false);
        MatchManager.Instance.SetLastHit(1);
        float dist = Vector3.Distance(ball.transform.position, transform.position);
        if (dist <= hitThreshold)
        {
            ball.transform.position = racket.transform.position;
            Vector3 hit = ballHitter.hitBall(side, depth, ball.transform.position, 1f);
            ballRb.velocity = hit;
        }
    }
}