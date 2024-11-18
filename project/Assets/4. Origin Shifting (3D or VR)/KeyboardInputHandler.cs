using UnityEngine;

public class KeyboardInputHandler : IInputHandler
{
    public float GetHorizontalInput()
    {
        return Input.GetAxis("Horizontal");
    }

    public float GetVerticalInput()
    {
        return Input.GetAxis("Vertical");
    }

    public bool GetSwitchPlaneInput()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }
}
