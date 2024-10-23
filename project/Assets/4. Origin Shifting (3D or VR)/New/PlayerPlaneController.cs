using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlaneController : PlaneController
{
    public float speed = 10f;
    public float rotationSpeed = 100f;

    private IInputHandler inputHandler;

    private void Awake()
    {
        inputHandler = new KeyboardInputHandler(); // Could easily swap out in the future
    }

    public override void Move()
    {
        float moveAmount = speed * Time.deltaTime;
        transform.Translate(Vector3.forward * moveAmount);
    }

    public override void Rotate()
    {
        float horizontal = inputHandler.GetHorizontalInput() * rotationSpeed * Time.deltaTime;
        float vertical = inputHandler.GetVerticalInput() * rotationSpeed * Time.deltaTime;
        transform.Rotate(vertical, horizontal, 0);
    }

    private void Update()
    {
        Move();
        Rotate();
    }
}
