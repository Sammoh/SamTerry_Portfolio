using System;
using Sammoh.CrowdSystem;
using UnityEngine;

[Serializable]
public class AiBehavior_ForwardBack : AIBehavior
{
    public float travelDistance = 5f; // Editable in Inspector

    private enum MovementState { MovingForward, MovingBackward }
    private MovementState _currentState = MovementState.MovingForward;

    public override void Control(CrowdAgentAi crowdAgentAi)
    {
        Vector3 destination;

        if (_currentState == MovementState.MovingForward)
        {
            destination = crowdAgentAi.transform.position + crowdAgentAi.transform.forward * travelDistance;
            _currentState = MovementState.MovingBackward;
        }
        else
        {
            destination = crowdAgentAi.transform.position - crowdAgentAi.transform.forward * travelDistance;
            _currentState = MovementState.MovingForward;
        }

        crowdAgentAi.MoveTo(destination);
    }
}
