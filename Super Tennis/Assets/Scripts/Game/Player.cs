using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    PlayerControls controls;

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
        controls = new PlayerControls();
        controls.Gameplay.Shoot.performed += ctx => NormalServe(ctx.ReadValue<Vector2>());
    }

    private void NormalServe(Vector2 direction)
    {
        ball.gameObject.GetComponent<Ball>().Freeze(false);
        float dist = Vector3.Distance(ball.transform.position, transform.position);
        if (dist <= hitThreshold)
        {
            ball.transform.position = racket.transform.position;
            List<Vector3> ret = ballHitter.serve(direction, serveSide, accuracy, true);
            ballRb.velocity = ret[1];
            MatchManager.Instance.SetLastHit(1, ret[0]);
            audioClipHitBall.Play();

            controls.Gameplay.Shoot.performed += ctx => NormalShot(ctx.ReadValue<Vector2>());
            animator.SetTrigger("Serve");
        }
    }

    private void NormalShot(Vector2 direction)
    {
        if(MatchManager.Instance.GetCurrentPlayer() == 1)
            ball.gameObject.GetComponent<Ball>().Freeze(false);

        float dist = Vector3.Distance(ball.transform.position, transform.position);
        if (dist <= hitThreshold)
        {
            ball.transform.position = racket.transform.position;
            List<Vector3> ret = ballHitter.hitBall(direction, accuracy, true);
            ballRb.velocity = ret[1];
            MatchManager.Instance.SetLastHit(1, ret[0]);
            audioClipHitBall.Play();

            animator.SetTrigger("Drive");
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
        if(newState == State.Serve)
        {
            controls.Gameplay.Shoot.performed += ctx => NormalServe(ctx.ReadValue<Vector2>());
        }
        state = newState;
    }
}
