using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject ball;
    public float hitThreshold = 2f;
    private Rigidbody ballRb;

    void Start()
    {
        ballRb = ball.GetComponent<Rigidbody>();
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
        float dist = Vector3.Distance(ball.transform.position, transform.position);
        Debug.Log(dist);
        if (dist <= hitThreshold)
        {
            ballRb.AddForce(15, 15, 0, ForceMode.Impulse);
        }
    }
}