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
    private float waitTime = 1f;
    private State state = State.Play;
    private State from;
    private BallHitter.Side serveSide;
    private BallHitter.Side hitSide;
    private BallHitter.Strength hitStrength;
    private float accuracy = 0.8f;
    public AudioSource audioClipHitBall;

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
                    GetShotType();
                }
                break;
            case State.Play:
                if (Input.GetKeyDown("space"))
                {
                    from = State.Play;
                    GetShotType();
                    HitBall();
                }
                break;

            case State.WaitAnimation:
                if (waitCounter < waitTime)
                    waitCounter += Time.deltaTime;
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

    private void GetShotType()
    {
        hitSide = BallHitter.Side.Center;
        if (Input.GetKey("left"))
        {
            hitSide = BallHitter.Side.Left;
        }
        else if (Input.GetKey("right"))
        {
            hitSide = BallHitter.Side.Right;
        }

        hitStrength = BallHitter.Strength.Middle;
        if (Input.GetKey("up"))
        {
            hitStrength = BallHitter.Strength.Lob;
        }
        else if (Input.GetKey("down"))
        {
            hitStrength = BallHitter.Strength.Drop;
        }
        if(from == State.Play)
        {
            if (hitStrength == BallHitter.Strength.Drop)
                animator.SetTrigger("Strafe");
            else if (hitStrength == BallHitter.Strength.Lob)
                animator.SetTrigger("PowerfullShot");
            else
                animator.SetTrigger("Drive");
        }
    }

    void HitBall()
    {
        if(MatchManager.Instance.GetCurrentPlayer() == 1)
            ball.gameObject.GetComponent<Ball>().Freeze(false);
                
        float dist = Vector3.Distance(ball.transform.position, transform.position);
        if (dist <= hitThreshold)
        {
            ball.transform.position = racket.transform.position;
            List<Vector3> ret = ballHitter.hitBall(hitSide, hitStrength, accuracy, true);
            ballRb.velocity = ret[1];
            MatchManager.Instance.SetLastHit(1, ret[0]);
            audioClipHitBall.Play();
        }
    }

    void Serve()
    {
        ball.gameObject.GetComponent<Ball>().Freeze(false);
        float dist = Vector3.Distance(ball.transform.position, transform.position);
        if (dist <= hitThreshold)
        {
            ball.transform.position = racket.transform.position;
            List<Vector3> ret = ballHitter.serve(hitSide, serveSide, accuracy, true);
            ballRb.velocity = ret[1];
            MatchManager.Instance.SetLastHit(1, ret[0]);
            audioClipHitBall.Play();
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