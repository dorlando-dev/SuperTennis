using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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


public class BallHitter
{
    Transform racket;
    float sideWidth = 27.41f, sideLength = 38.95f, netHeight = 3.6f;
    float g = 9.8f;

    public BallHitter(Transform racket)
    {
        this.racket = racket;
    }

    public Vector3 hitBall(Side side, Strength strength, Vector3 ballPos, float accuracy)
    {
        if(strength == Strength.Lob)
        {
            return hitLob();
        }
        return new Vector3(0, 0, 0);
    }

    public Vector3 hitLob()
    {
        float dX = Math.Abs(racket.position.x) + sideLength*0.9f;
        Debug.Log(dX);
        float v0x = 15;

        float v0y = 0.5f*g*dX/v0x - racket.position.y*v0x/dX;

        return new Vector3(v0x, v0y, 0f);
    }
}