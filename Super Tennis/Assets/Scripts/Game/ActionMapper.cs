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

    public static bool GetMoveLeft()
    {
        return Input.GetKey(KeyCode.LeftArrow);
    }

    public static bool GetMoveRight()
    {
        return Input.GetKey(KeyCode.RightArrow);
    }
    public static bool GetMoveUp()
    {
        return Input.GetKey(KeyCode.UpArrow);
    }

    public static bool GetMoveDown()
    {
        return Input.GetKey(KeyCode.DownArrow);
    }
}
