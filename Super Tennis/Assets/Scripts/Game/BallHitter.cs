using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;




public class BallHitter
{
    public enum Side
    {
        Left,
        Center,
        Right,
    }

    public enum Strength
    {
        Drop,
        Middle,
        Lob,
    }
    Transform racket;
    float sideWidth = 27.41f, sideLength = 38.95f, netHeight = 3.4f;
    float g = 9.8f;

    public BallHitter(Transform racket)
    {
        this.racket = racket;
    }

    public Vector3 hitBall(Side side, Strength strength, Vector3 ballPos, float accuracy)
    {
        if(strength == Strength.Lob)
        {
            return parabole(0.8f, 4f);
        } 
        else if(strength == Strength.Drop)
        {
            return parabole(0.2f, 0.5f);
        }
        return parabole(0.67f, 1f);
    }

    public Vector3 parabole(float targetRatio, float netDistance)
    {
        float pX = Mathf.Abs(racket.position.x);
        float targetX = sideLength * targetRatio;
        float dX = pX + targetX;
        float y0 = racket.position.y;
        float netTargetHeight = netHeight + netDistance;

        float vx = Mathf.Sqrt(0.5f*(g*(pX*dX - pX*pX))/(netTargetHeight + pX*y0/dX - y0));
        float vy = 0.5f*g*dX/vx - y0*vx/dX;

        //Debug.Log(vx);
        //Debug.Log(vy);

        return new Vector3(vx, vy, 0f);
    }
}