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
    float serveWidth = 13.94f, serveLength = 21.06f;
    float g = -Physics.gravity.y;

    public BallHitter(Transform racket)
    {
        this.racket = racket;
    }

    public List<Vector3> hitBall(Vector2 aim, float accuracy, bool isPlayer)
    {
        Vector2 pPos = new Vector2(racket.position.x, racket.position.y);

        float netTargetHeight = 2f;

        Vector2 tPos = getTargetPosition(aim, sideLength, sideWidth, isPlayer);

        Vector3 vel = hitTowardsPoint(tPos, netTargetHeight);

        List<Vector3> ret = new List<Vector3>();
        ret.Add(new Vector3(tPos.x, 0f, tPos.y));
        ret.Add(applyError(vel, accuracy));
        return ret;
    }

    public List<Vector3> serve(Vector2 aim, Side serve, float accuracy, bool isPlayer)
    {
        Vector2 pPos = new Vector2(racket.position.x, racket.position.y);

        float netTargetHeight = 1.5f;

        Vector2 tPos = getServeTargetPosition(aim, serve, serveLength, serveWidth, isPlayer);

        Vector3 vel = hitTowardsPoint(tPos, netTargetHeight);

        List<Vector3> ret = new List<Vector3>();
        ret.Add(new Vector3(tPos.x, 0f, tPos.y));
        ret.Add(applyError(vel, accuracy));
        return ret;
    }

    public Vector2 getTargetPosition(Vector2 aim, float depth, float width, bool isPlayer)
    {
        float tDepth = (aim.x + 1) / 2;
        float tSide = (aim.y + 1) / 2;

        float tX = tDepth * depth;
        float tZ = - width * tSide;

        if (!isPlayer)
        {
            tX = -tX;
            tZ = -tZ;
        }

        return new Vector2(tX, tZ);
    }

    public Vector2 getServeTargetPosition(Vector2 aim, Side serve, float depth, float width, bool isPlayer)
    {
        float tDepth = (aim.x + 1) / 2;
        float tSide = (aim.y + 1) / 2;

        float tX = tDepth * depth;
        float tZ = 0;
        if (serve == Side.Left)
        {
            tZ = width * (1f - tSide);
        }
        else
        {
            tZ = - width * tSide;
        }

        if (!isPlayer)
        {
            tX = -tX;
            tZ = -tZ;
        }

        return new Vector2(tX, tZ);
    }

    public Vector3 applyError(Vector3 v, float accuracy)
    {
        float dX = (UnityEngine.Random.value - 0.5f)*(1 - accuracy);
        float vx = v.x*(1f + dX);
        float dY = (UnityEngine.Random.value - 0.5f)*(1 - accuracy);
        float vy = v.y*(1f + dY);
        float dZ = (UnityEngine.Random.value - 0.5f)*(1 - accuracy);
        float vz = v.z*(1f + dZ);
        return new Vector3(vx, vy, vz);
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
