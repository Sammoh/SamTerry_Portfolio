// Assets/GOAP/Goals/SleepGoalSO.cs
using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    [CreateAssetMenu(fileName = "SleepGoal", menuName = "GOAP/Goals/Sleep", order = 12)]

    public class SleepGoalSO : GoalSO
    {
        [Range(0f, 1f)] public float triggerThreshold = 0.5f;
        [Range(0.1f, 10f)] public float scale = 8f;

        public override bool CanSatisfy(IAgentState agentState, IWorldState worldState) => true;

        public override bool IsCompleted(IAgentState agentState, IWorldState worldState)
            => agentState.GetNeed("sleep") <= 0.01f;

        public override Dictionary<string, object> GetDesiredState() =>
            new Dictionary<string, object> { { "need_sleep", 0f } };

        public override float CalculatePriority(IAgentState agentState, IWorldState worldState)
        {
            float v = Mathf.Clamp01(agentState.GetNeed("sleep"));
            return v > triggerThreshold ? v * scale : 0f;
        }
    }
}