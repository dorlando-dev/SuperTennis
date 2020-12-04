using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    public int speed = 5;
    public CharacterController CharacterController;
    public Animator Animator;

    PlayerControls controls;

    private int xMovement;
    private int zMovement;

    private Vector2 movement;

    // Start is called before the first frame update
    void Start()
    {
        controls = new PlayerControls();
        controls.Gameplay.Move.performed += ctx => MoveTo(ctx.ReadValue<Vector2>());
        controls.Gameplay.Enable();
    }

    private void MoveTo(Vector2 direction)
    {
        Debug.Log(direction);
        float x = Time.deltaTime * speed * direction.y;
        float z = Time.deltaTime * speed * direction.x;

        Vector3 newPosition = new Vector3(x, -3.067f, z);
        CharacterController.Move(newPosition);
    }
}
