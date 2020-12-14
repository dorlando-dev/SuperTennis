using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadPlayerMover : MonoBehaviour
{
    public int speed = 5;
    public CharacterController CharacterController;

    private int xMovement;
    private int zMovement;

    private Vector2 movement;

    private Gamepad gp;

    void Awake()
    {
        gp = Gamepad.current;
    }

    // Start is called before the first frame update
    void Update ()
    {
        var direction = gp.leftStick.ReadValue();

        MoveTo(new Vector2(-direction.y, direction.x).normalized);
    }

    private void MoveTo(Vector2 direction)
    {

        float x = Time.deltaTime * speed * direction.x;
        float z = Time.deltaTime * speed * direction.y;

        Vector3 newPosition = new Vector3(x, -3.067f, z);
        CharacterController.Move(newPosition);
    }
}
