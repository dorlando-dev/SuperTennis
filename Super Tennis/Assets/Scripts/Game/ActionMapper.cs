using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMapper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static bool GetMoveDown()
    {
        return Input.GetKey(KeyCode.S);
    }

    public static bool GetMoveUp()
    {
        return Input.GetKey(KeyCode.W);
    }
    public static bool GetMoveLeft()
    {
        return Input.GetKey(KeyCode.A);
    }

    public static bool GetMoveRight()
    {
        return Input.GetKey(KeyCode.D);
    }

    public static bool HitBall()
    {
        return Input.GetKey(KeyCode.Space);
    }
}
