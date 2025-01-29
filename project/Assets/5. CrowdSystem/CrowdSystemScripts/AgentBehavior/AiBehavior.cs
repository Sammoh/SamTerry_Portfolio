using System;
using UnityEngine;

[Serializable]
public abstract class AIBehavior
{
    [SerializeField]
    public string behaviorName = "New Behavior";
    public abstract void Execute();
}