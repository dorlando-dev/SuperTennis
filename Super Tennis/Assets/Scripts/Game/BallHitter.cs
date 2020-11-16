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
        Vector2 pPos = new Vector2(racket.position.x, racket.position.y);

        float targetDepthR = 0.6f;
        float netTargetHeight = 0.5f;
        if(strength == Strength.Lob)
        {
            targetDepthR = 0.85f;
            netTargetHeight = 2f;
        } 
        else if(strength == Strength.Drop)
        {
            targetDepthR = 0.2f;
            netTargetHeight = 0.3f;
        }

        float targetSideR = 0f;
        if(side == Side.Right)
        {
            targetSideR = -0.4f;
        }
        else if(side == Side.Left)
        {
            targetSideR = 0.4f;
        }

        Vector2 tPos = new Vector2(sideLength*targetDepthR, sideWidth*targetSideR);

        return hitTowardsPoint(tPos, netTargetHeight);
    }

    public Vector3 hitTowardsPoint(Vector2 target, float netHeight) {
        Vector2 pPos = new Vector2(racket.position.x, racket.position.z);

        float dX = (target - pPos).magnitude;
        float alfa = Mathf.Atan2(target.y - pPos.y, target.x - pPos.x);

        float pX = racket.position.x*Mathf.Cos(alfa);

        Vector2 v = parabole(Mathf.Abs(pX), dX, netHeight);
        float vX = v.x*Mathf.Cos(alfa);
        float vZ = v.x*Mathf.Sin(alfa);

        return new Vector3(vX, v.y, vZ);
    }

    public Vector2 parabole(float distanceToNet, float distanceToTarget, float netDistance)
    {
        float pX = distanceToNet;
        float dX = distanceToTarget;
        float y0 = racket.position.y;
        float netTargetHeight = netHeight + netDistance;

        float vx = Mathf.Sqrt(0.5f*(g*(pX*dX - pX*pX))/(netTargetHeight + pX*y0/dX - y0));
        float vy = 0.5f*g*dX/vx - y0*vx/dX;

        return new Vector3(vx, vy);
    }
}