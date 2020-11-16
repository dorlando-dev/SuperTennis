using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    private float speed = 10f;
    public CharacterController CharacterController;
    public Animator Animator;
    public Rigidbody rb;
    public GameObject ball;
    public GameObject racket;
    public float hitThreshold = 5f;
    private Rigidbody ballRb;
    BallHitter ballHitter;
    public Transform center;

    private float waitTime = 0f;
    private float waitingToServeTime = 500f;
    private float waitAnimationTime = 300f;
    private int counter = 0;

    private State state = State.MoveToCenter;
    private Vector3 ballDestination;
    private State from;

    private BallHitter.Side serveSide;
    private float difficulty;

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
                    state = State.WaitAnimation;
                    from = State.Serve;
                    waitTime = 0;
                    Stop();
                    Animator.SetTrigger("Serve");
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
                if (from == State.Serve)
                {
                    if (waitTime < waitAnimationTime)
                        waitTime++;
                    else
                    {
                        state = State.Serve;
                        waitTime = 0;
                        Stop();
                    }
                }
                else
                {
                    state = State.HitBall;
                    Stop();
                }
                break;
            case State.MoveToCenter:
                MoveToCenter();
                break;
            case State.MovingToBall:
                MoveToBall();
                float dist = Vector3.Distance(ball.transform.position, transform.position);
                if (dist <= hitThreshold)
                {
                    state = State.WaitAnimation;
                    from = State.HitBall;
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
            BallHitter.Side side;
            float sideR = UnityEngine.Random.value;
            if(sideR < 0.33)
            {
                side = BallHitter.Side.Left;
            } else if (sideR < 0.66)
            {
                side = BallHitter.Side.Center;
            } else
            {
                side = BallHitter.Side.Right;
            }

            BallHitter.Strength depth;
            float depthR = UnityEngine.Random.value;
            if(depthR < 0.15)
            {
                depth = BallHitter.Strength.Drop;
                Animator.SetTrigger("Strafe");
            } else if (depthR < 0.8)
            {
                depth = BallHitter.Strength.Middle;
                Animator.SetTrigger("Drive");
            } else
            {
                depth = BallHitter.Strength.Lob;
                Animator.SetTrigger("PowerfullShot");
            }


            Vector3 hit = ballHitter.hitBall(side, depth, difficulty, false)[1];
            ballRb.velocity = hit;
            MatchManager.Instance.SetLastHit(2, Vector3.zero);
        }
    }

    void Serve()
    {
        ball.gameObject.GetComponent<Ball>().Freeze(false);        
        float dist = Vector3.Distance(ball.transform.position, transform.position);
        if (dist <= hitThreshold)
        {
            ball.transform.position = racket.transform.position;

            BallHitter.Side side;
            float sideR = UnityEngine.Random.value;
            if(sideR < 0.33)
            {
                side = BallHitter.Side.Left;
            } else if (sideR < 0.66)
            {
                side = BallHitter.Side.Center;
            } else
            {
                side = BallHitter.Side.Right;
            }

            Vector3 hit = ballHitter.serve(side, serveSide, difficulty, false)[1];
            ballRb.velocity = hit;
            MatchManager.Instance.SetLastHit(2, Vector3.zero);
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
        Vector3 direction = (ballDestination - transform.position).normalized;
        direction.y = 0f;
        rb.velocity = direction * speed;
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

    public void SetDifficulty(int newDifficulty)
    {
        if (newDifficulty == 1)
        {
            difficulty = 0.7f;
            speed = 6;
        }
        else if (newDifficulty == 2)
        {
            difficulty = 0.8f;
            speed = 8;
        }
        else if (newDifficulty == 3)
        {
            difficulty = 0.9f;
            speed = 10;
        }
    }

    private void MoveToCenter()
    {
        Vector3 direction = (center.position - transform.position).normalized;
        direction.y = 0f;
        rb.velocity = direction * speed;
    }
}
