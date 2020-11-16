﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public int speed = 5;
    public CharacterController CharacterController;
    public Animator Animator;
    public Rigidbody rb;
    public GameObject ball;
    public GameObject racket;
    public float hitThreshold = 5f;
    private Rigidbody ballRb;
    BallHitter ballHitter;

    private float waitTime = 0f;
    private float waitingToServeTime = 500f;
    private float waitAnimationTime = 0f;
    private int counter = 0;

    private State state = State.MoveToCenter;
    private Vector3 ballDestination;
    private State from;

    private BallHitter.Side serveSide;

    public enum State
    {
        WaitingToServe,
        Serve,
        HitBall,
        WaitingForPlayerServe,
        MoveToCenter,
        MovingToBall,
        WaitAnimation,
        Stop

    }

    // Start is called before the first frame update
    void Start()
    {
        ballRb = ball.GetComponent<Rigidbody>();
        ballHitter = new BallHitter(racket.transform);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.WaitingToServe:
                if (waitTime < waitingToServeTime)
                    waitTime++;
                else
                {
                    Animator.SetTrigger("Serve");
                    state = State.WaitAnimation;
                    from = State.Serve;
                    waitTime = 0;
                    Stop();
                }
                break;
            case State.Serve:
                Serve();
                state = State.MoveToCenter;
                break;
            case State.HitBall:
                HitBall();
                state = State.MoveToCenter;
                break;
            case State.WaitAnimation:
                if (waitTime < waitAnimationTime)
                    waitTime++;
                else
                {
                    state = from;
                    waitTime = 0;
                    Stop();
                }
                break;
            case State.MoveToCenter:
                break;
            case State.MovingToBall:
                MoveToBall();
                float dist = Vector3.Distance(ball.transform.position, transform.position);
                if (dist <= hitThreshold)
                {
                    state = State.WaitAnimation;
                    from = State.HitBall;
                    Animator.SetTrigger("Drive");
                }
                break;
            case State.Stop:
                Stop();
                break;
        }
    }

    void HitBall()
    {        
        ball.gameObject.GetComponent<Ball>().Freeze(false);        
        float dist = Vector3.Distance(ball.transform.position, transform.position);
        if (dist <= hitThreshold)
        {
            ball.transform.position = racket.transform.position;
            Vector3 hit = ballHitter.hitBall(BallHitter.Side.Center, BallHitter.Strength.Middle, 1f, false);
            ballRb.velocity = hit;
        }
    }

    void Serve()
    {
        ball.gameObject.GetComponent<Ball>().Freeze(false);
        MatchManager.Instance.SetLastHit(2, Vector3.zero);
        float dist = Vector3.Distance(ball.transform.position, transform.position);
        if (dist <= hitThreshold)
        {
            ball.transform.position = racket.transform.position;
            Vector3 hit = ballHitter.serve(BallHitter.Side.Center, serveSide, 1f, false);
            ballRb.velocity = hit;
        }
    }

    public void SetState(State newState)
    {
        state = newState;
    }

    public void SetBallDestination(Vector3 destination)
    {
        ballDestination = destination;
    }

    private int xMovement;
    private int zMovement;

    private void MoveToBall()
    {
        if (transform.position.x < ballDestination.x)
            xMovement = 1;
        else if (transform.position.x > ballDestination.x)
            xMovement = -1;
        if (transform.position.z < ballDestination.z)
            zMovement = 1;
        else if (transform.position.z < ballDestination.z)
            zMovement = -1;
        rb.velocity = new Vector3(xMovement * speed, 0, zMovement * speed);
    }

    private void Stop()
    {
        rb.velocity = Vector3.zero;
    }

    public void SetServeSide(int side)
    {
        if(side == 0)
        {
            serveSide = BallHitter.Side.Left;            
        }
        else
        {
            serveSide = BallHitter.Side.Right;
        }
    }
}
