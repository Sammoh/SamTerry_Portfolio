using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputHandler
{
    float GetHorizontalInput();
    float GetVerticalInput();
    bool GetSwitchPlaneInput();
}
