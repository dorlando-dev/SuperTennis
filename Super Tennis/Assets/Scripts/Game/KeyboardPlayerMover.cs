using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardPlayerMover : MonoBehaviour
{
    public int speed = 10;
    public CharacterController CharacterController;

    private int xMovement;
    private int zMovement;

    private Vector2 movement;

    private Keyboard kb;
    public bool serving { get; set; }

    void Awake()
    {
        kb = Keyboard.current;
    }

    // Start is called before the first frame update
    void Update ()
    {
        xMovement = 0;
        zMovement = 0;
        if (kb.wKey.isPressed)
        {
            xMovement += 1;
        }

        if (kb.sKey.isPressed)
        {
            xMovement -= 1;
        }

        if (kb.aKey.isPressed)
        {
            zMovement += 1;
        }

        if (kb.dKey.isPressed)
        {
            zMovement -= 1;
        }

        if (!serving)
            MoveTo(new Vector2(xMovement, zMovement).normalized);
    }

    private void MoveTo(Vector2 direction)
    {

        float x = Time.deltaTime * speed * direction.x;
        float z = Time.deltaTime * speed * direction.y;

        Vector3 dir = new Vector3(x, 0, z);
        CharacterController.Move(dir);
    }
}
