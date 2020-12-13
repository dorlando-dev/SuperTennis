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
    public AudioSource audioClipHitBall;
    public float hitThreshold = 5f;
    private Rigidbody ballRb;
    BallHitter ballHitter;
    public Transform center;

    private float waitTime = 0f;
    private float waitingToServeTime = 3f;
    private float waitAnimationTime = 1f;
    private int counter = 0;

    private MatchManager.AIState state = MatchManager.AIState.MoveToCenter;
    private Vector3 ballDestination;
    private MatchManager.AIState from;

    private BallHitter.Side serveSide;
    private float difficulty;

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
            case MatchManager.AIState.WaitingToServe:
                Stop();
                if (waitTime < waitingToServeTime)
                    waitTime += Time.deltaTime;
                else
                {
                    state = MatchManager.AIState.WaitAnimation;
                    from = MatchManager.AIState.Serve;
                    waitTime = 0;
                    Animator.SetTrigger("Serve");
                }
                break;
            case MatchManager.AIState.Serve:
                Serve();
                state = MatchManager.AIState.MoveToCenter;
                break;
            case MatchManager.AIState.HitBall:
                HitBall();
                state = MatchManager.AIState.MoveToCenter;
                break;
            case MatchManager.AIState.WaitAnimation:
                if (from == MatchManager.AIState.Serve)
                {
                    if (waitTime < waitAnimationTime)
                        waitTime++;
                    else
                    {
                        state = MatchManager.AIState.Serve;
                        waitTime += Time.deltaTime;
                        Stop();
                    }
                }
                else
                {
                    state = MatchManager.AIState.HitBall;
                    Stop();
                }
                break;
            case MatchManager.AIState.MoveToCenter:
                MoveToCenter();
                break;
            case MatchManager.AIState.MovingToBall:
                MoveToBall();
                float dist = Vector3.Distance(ball.transform.position, transform.position);
                if (dist <= hitThreshold)
                {
                    state = MatchManager.AIState.WaitAnimation;
                    from = MatchManager.AIState.HitBall;
                }
                break;
            case MatchManager.AIState.Stop:
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

            Vector2 target = new Vector2((UnityEngine.Random.value * 2) - 1, (UnityEngine.Random.value * 2) - 1);

            Animator.SetTrigger("Drive");

            Vector3 hit = ballHitter.hitBall(target, difficulty, false)[1];
            ballRb.velocity = hit;
            MatchManager.Instance.SetLastHit(2, Vector3.zero);
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

            Vector2 target = new Vector2((UnityEngine.Random.value * 2) - 1, (UnityEngine.Random.value * 2) - 1);

            Animator.SetTrigger("Serve");

            Vector3 hit = ballHitter.serve(target, serveSide, difficulty, false)[1];
            ballRb.velocity = hit;
            MatchManager.Instance.SetLastHit(2, Vector3.zero);
            audioClipHitBall.Play();
        }
    }

    public void SetState(MatchManager.AIState newState)
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
