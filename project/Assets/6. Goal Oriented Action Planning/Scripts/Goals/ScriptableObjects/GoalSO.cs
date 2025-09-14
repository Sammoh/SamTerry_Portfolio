// Assets/GOAP/Goals/GoalSO.cs
using System.Collections.Generic;
using UnityEngine;
using Sammoh.GOAP;

public abstract class GoalSO : ScriptableObject, IGoal
{
    [Header("Identity")]
    [SerializeField] private string goalType = "unset"; // must be unique & stable (e.g., "hunger")
    [Tooltip("Used only as a floor; actual runtime priority should come from CalculatePriority.")]
    [SerializeField] private float basePriority = 0f;

    public string GoalType => goalType;
    public float Priority => basePriority;

    /// <summary>Return true if this goal is applicable given current agent/world state.</summary>
    public abstract bool CanSatisfy(IAgentState agentState, IWorldState worldState);

    /// <summary>Return true if this goal is satisfied in the current state.</summary>
    public abstract bool IsCompleted(IAgentState agentState, IWorldState worldState);

    /// <summary>Desired end-state expressed as world/agent facts.</summary>
    public abstract Dictionary<string, object> GetDesiredState();

    /// <summary>Dynamic priority based on needs/world (planner uses this each tick).</summary>
    public abstract float CalculatePriority(IAgentState agentState, IWorldState worldState);

    // Optional: Editor helper to ensure unique/valid strings
#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(goalType))
            goalType = name.Replace(" ", "_").ToLowerInvariant();
    }
#endif
}