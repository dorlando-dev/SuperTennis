using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadPlayer : MonoBehaviour
{
    public GameObject ball;
    public GameObject racket;
    public float hitThreshold = 5f;
    public Animator animator;
    private Rigidbody ballRb;
    BallHitter ballHitter;
    private float waitCounter = 0f;
    private float waitTime = 1f;
    private MatchManager.PlayerState state = MatchManager.PlayerState.Play;
    private MatchManager.PlayerState from;
    private BallHitter.Side serveSide;
    private BallHitter.Side hitSide;
    private BallHitter.Strength hitStrength;
    private float accuracy = 1.0f;
    public AudioSource audioClipHitBall;

    private Gamepad gp;

    public Vector3 hit;

    void Start()
    {
        gp = Gamepad.current;
        ballRb = ball.GetComponent<Rigidbody>();
        ballHitter = new BallHitter(racket.transform);
    }

    void Update()
    {
        switch (state)
        {
            case MatchManager.PlayerState.Serve:
                if (gp.rightTrigger.isPressed)
                {
                    animator.SetTrigger("Serve");
                    state = MatchManager.PlayerState.WaitAnimation;
                    from = MatchManager.PlayerState.Serve;
                    gameObject.GetComponent<GamepadPlayerMover>().serving = false;
                }
                else
                {
                    gameObject.GetComponent<GamepadPlayerMover>().serving = true;
                }
                break;
            case MatchManager.PlayerState.Play:
                if (gp.rightTrigger.isPressed)
                {
                    from = MatchManager.PlayerState.Play;
                    HitBall();
                }
                break;

            case MatchManager.PlayerState.WaitAnimation:
                if (waitCounter < waitTime)
                    waitCounter += Time.deltaTime;
                else
                {
                    if (from == MatchManager.PlayerState.Play)
                        HitBall();
                    else if (from == MatchManager.PlayerState.Serve)
                        Serve();
                    state = MatchManager.PlayerState.Play;
                    waitCounter = 0;
                }
                break;
        }

    }

    void HitBall()
    {
        if((MatchManager.Instance.GetCurrentPlayer() == 1 && gameObject.tag == "Player") || (MatchManager.Instance.GetCurrentPlayer() == 2 && gameObject.tag == "Player2"))
            ball.gameObject.GetComponent<Ball>().Freeze(false);

        float dist = Vector3.Distance(ball.transform.position, transform.position);
        Vector2 aim = gp.rightStick.ReadValue();
        if (dist <= hitThreshold)
        {
            ball.transform.position = racket.transform.position;
            List<Vector3> ret = ballHitter.hitBall(aim, accuracy);
            ballRb.velocity = ret[1];
            if (gameObject.tag == "Player")
                MatchManager.Instance.SetLastHit(1, ret[0]);
            else if (gameObject.tag == "Player2")
                MatchManager.Instance.SetLastHit(2, ret[0]);
            audioClipHitBall.Play();
        }
    }

    void Serve()
    {
        ball.gameObject.GetComponent<Ball>().Freeze(false);
        float dist = Vector3.Distance(ball.transform.position, transform.position);
        Vector2 aim = gp.rightStick.ReadValue();
        if (dist <= hitThreshold)
        {
            ball.transform.position = racket.transform.position;
            List<Vector3> ret = ballHitter.serve(aim, serveSide, accuracy);
            ballRb.velocity = ret[1];
            if (gameObject.tag == "Player")
                MatchManager.Instance.SetLastHit(1, ret[0]);
            else if (gameObject.tag == "Player2")
                MatchManager.Instance.SetLastHit(2, ret[0]);
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

    public void SetState(MatchManager.PlayerState newState)
    {
        state = newState;
    }
}
