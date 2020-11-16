using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    public int speed = 5;
    public CharacterController CharacterController;
    public Animator Animator;

    private int xMovement;
    private int zMovement;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ReadInput();
        UpdatePosition();
    }

    private void ReadInput()
    {
        if (ActionMapper.GetMoveLeft())
            zMovement = 1;
        else if (ActionMapper.GetMoveRight())
            zMovement = -1;
        else
            zMovement = 0;

        if (ActionMapper.GetMoveDown())
            xMovement = -1;
        else if (ActionMapper.GetMoveUp())
            xMovement = 1;
        else
            xMovement = 0;
    }

    private void UpdatePosition()
    {
        float x = Time.deltaTime * speed * xMovement;
        float y = -3.067f;
        float z = Time.deltaTime * speed * zMovement;
        Vector3 newPosition = new Vector3(x, y, z);
        CharacterController.Move(newPosition);
    }
}
