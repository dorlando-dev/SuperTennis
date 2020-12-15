using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardPlayer : MonoBehaviour
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
    private float accuracy = 0.8f;
    public AudioSource audioClipHitBall;

    private int hitCooldown = 200;
    private int hitCounter;

    private Keyboard kb;

    public Vector3 hit;

    void Start()
    {
        kb = Keyboard.current;
        ballRb = ball.GetComponent<Rigidbody>();
        ballHitter = new BallHitter(racket.transform);
    }

    void Update()
    {
        hitCounter = hitCounter - 1 > 0 ? hitCounter - 1 : 0;
        switch (state)
        {
            case MatchManager.PlayerState.Serve:
                if (kb.spaceKey.isPressed)
                {
                    animator.SetTrigger("Serve");
                    state = MatchManager.PlayerState.WaitAnimation;
                    from = MatchManager.PlayerState.Serve;
                    GetShotType();
                    gameObject.GetComponent<KeyboardPlayerMover>().serving = false;
                }
                else
                {
                    gameObject.GetComponent<KeyboardPlayerMover>().serving = true;
                }
                break;
            case MatchManager.PlayerState.Play:
                if (kb.spaceKey.isPressed)
                {
                    from = MatchManager.PlayerState.Play;
                    GetShotType();
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
        if(from == MatchManager.PlayerState.Play)
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
        if((MatchManager.Instance.GetCurrentPlayer() == 1 && gameObject.tag == "Player") || (MatchManager.Instance.GetCurrentPlayer() == 2 && gameObject.tag == "Player2"))
            ball.gameObject.GetComponent<Ball>().Freeze(false);

        float dist = Vector3.Distance(ball.transform.position, transform.position);
        if (dist <= hitThreshold && hitCounter <= 0)
        {
            ball.transform.position = racket.transform.position;
            List<Vector3> ret = ballHitter.hitBall(hitSide, hitStrength, accuracy);
            ballRb.velocity = ret[1];
            if(gameObject.tag == "Player")
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
        if (dist <= hitThreshold && hitCounter <= 0)
        {
            ball.transform.position = racket.transform.position;
            List<Vector3> ret = ballHitter.serve(hitSide, serveSide, accuracy);
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
