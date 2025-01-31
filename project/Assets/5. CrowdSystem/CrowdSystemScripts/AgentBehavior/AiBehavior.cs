using System;
using Sammoh.CrowdSystem;
using UnityEngine;

[Serializable]
public abstract class AIBehavior
{
    [SerializeField]
    private string behaviorName = "New Behavior";
    public float Speed = 3f;
    public float AngularSpeed = 120f;

    public abstract void Control(CrowdAgentAi crowdAgentAi);
    
    public void ChangeName(string newName)
    {
        behaviorName = newName;
    }
}