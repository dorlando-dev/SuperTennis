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
    public bool serving { get; set; }

    void Awake()
    {
        gp = Gamepad.current;
    }

    // Start is called before the first frame update
    void Update ()
    {
        var direction = gp.leftStick.ReadValue();

        if (CharacterController.transform.position.x < 0)
        {
            direction = new Vector2(direction.y, -direction.x);
        } else
        {
            direction = new Vector2(-direction.y, direction.x);
        }
        if(!serving)
            MoveTo(direction.normalized);
    }

    private void MoveTo(Vector2 direction)
    {

        float x = Time.deltaTime * speed * direction.x;
        float z = Time.deltaTime * speed * direction.y;

        Vector3 dir = new Vector3(x, 0, z);
        CharacterController.Move(dir);
    }
}
