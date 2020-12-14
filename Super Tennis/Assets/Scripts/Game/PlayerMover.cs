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
        controls.Gameplay.Enable();
    }


    void Update()
    {
        HandleMovement(controls.Gameplay.Move.ReadValue<Vector2>());
    }

    private void HandleMovement(Vector2 direction)
    {
        float x = Time.deltaTime * speed * direction.y;
        float z = Time.deltaTime * speed * direction.x;

        Vector3 newPosition = new Vector3(x, -3.067f, z);
        CharacterController.Move(newPosition);
    }
}
