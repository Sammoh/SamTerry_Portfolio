// Assets/GOAP/Goals/HungerGoalSO.cs
using System.Collections.Generic;
using UnityEngine;
using Sammoh.GOAP;

#region Sammoh.GOAP

[CreateAssetMenu(fileName = "HungerGoal", menuName = "GOAP/Goals/Hunger", order = 10)]
public class HungerGoalSO : GoalSO
{
    [Range(0f, 1f)] public float triggerThreshold = 0.4f;
    [Range(0.1f, 10f)] public float scale = 10f;

    public override bool CanSatisfy(IAgentState agentState, IWorldState worldState) => true;

    public override bool IsCompleted(IAgentState agentState, IWorldState worldState)
        => agentState.GetNeed("hunger") <= 0.01f;

    public override Dictionary<string, object> GetDesiredState() =>
        new Dictionary<string, object> { { "need_hunger", 0f } };

    public override float CalculatePriority(IAgentState agentState, IWorldState worldState)
    {
        float hunger = Mathf.Clamp01(agentState.GetNeed("hunger"));
        return hunger > triggerThreshold ? hunger * scale : 0f;
    }
}

#endregion