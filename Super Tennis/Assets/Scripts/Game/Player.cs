using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject ball;
    public GameObject racket;
    public float hitThreshold = 5f;
    public Animator animator;
    private Rigidbody ballRb;
    BallHitter ballHitter;
    private float waitCounter = 0f;
    private float waitTime = 350f;
    private State state = State.Play;
    private State from;
    private BallHitter.Side serveSide;

    public enum State
    {
        Serve,
        Play,
        WaitAnimation
    }

    public Vector3 hit;

    void Start()
    {
        ballRb = ball.GetComponent<Rigidbody>();
        ballHitter = new BallHitter(racket.transform);
    }

    void Update()
    {
        switch (state)
        {
            case State.Serve:
                if (Input.GetKeyDown("space"))
                {
                    animator.SetTrigger("Serve");
                    state = State.WaitAnimation;
                    from = State.Serve;
                }
                break;
            case State.Play:
                if (Input.GetKeyDown("space"))
                {
                    animator.SetTrigger("Drive");
                    state = State.WaitAnimation;
                    from = State.Play;
                }
                break;

            case State.WaitAnimation:
                if (waitCounter < waitTime)
                    waitCounter++;
                else
                {
                    if (from == State.Play)
                        HitBall();
                    else if (from == State.Serve)
                        Serve();
                    state = State.Play;
                    waitCounter = 0;
                }
                break;
        }
        
    }

    void HitBall()
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
                
        float dist = Vector3.Distance(ball.transform.position, transform.position);
        if (dist <= hitThreshold)
        {
            ball.transform.position = racket.transform.position;
            Vector3 hit = ballHitter.hitBall(side, depth, 0.8f, true);
            ballRb.velocity = hit;
            MatchManager.Instance.SetLastHit(1, new Vector3(0, 0, 0));
        }
    }

    void Serve()
    {
        BallHitter.Side side = BallHitter.Side.Center;
        if (Input.GetKey("left"))
        {
            side = BallHitter.Side.Left;
        }
        else if (Input.GetKey("right"))
        {
            side = BallHitter.Side.Right;
        }
        ball.gameObject.GetComponent<Ball>().Freeze(false);
        MatchManager.Instance.SetLastHit(1, Vector3.zero);
        float dist = Vector3.Distance(ball.transform.position, transform.position);
        if (dist <= hitThreshold)
        {
            ball.transform.position = racket.transform.position;
            Vector3 hit = ballHitter.serve(side, serveSide, 1f, true);
            ballRb.velocity = hit;
        }
    }

    public void SetServeSide(int side)
    {
        if (side == 0)
            serveSide = BallHitter.Side.Left;
        else
            serveSide = BallHitter.Side.Right;
    }

    public void SetState(State newState)
    {
        state = newState;
    }
}