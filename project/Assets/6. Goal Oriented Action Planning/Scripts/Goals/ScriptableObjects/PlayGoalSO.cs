// PlayGoalSO.cs
using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    [CreateAssetMenu(fileName = "PlayGoal", menuName = "GOAP/Goals/Play", order = 13)]
    public class PlayGoalSO : GoalSO
    {
        [Range(0f, 1f)] public float triggerThreshold = 0.6f;
        [Range(0.1f, 10f)] public float scale = 5f;

        public override bool CanSatisfy(IAgentState agentState, IWorldState worldState) => true;
        public override bool IsCompleted(IAgentState agentState, IWorldState worldState) => agentState.GetNeed("play") <= 0.01f;
        public override Dictionary<string, object> GetDesiredState() => new() { { "need_play", 0f } };

        public override float CalculatePriority(IAgentState agentState, IWorldState worldState)
        {
            float v = Mathf.Clamp01(agentState.GetNeed("play"));
            return v > triggerThreshold ? v * scale : 0f;
        }
    }
}