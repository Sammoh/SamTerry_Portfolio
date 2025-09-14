// Assets/GOAP/Goals/ThirstGoalSO.cs
using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    [CreateAssetMenu(fileName = "ThirstGoal", menuName = "GOAP/Goals/Thirst", order = 11)]

    public class ThirstGoalSO : GoalSO
    {
        [Range(0f, 1f)] public float triggerThreshold = 0.4f;
        [Range(0.1f, 10f)] public float scale = 12f;

        public override bool CanSatisfy(IAgentState agentState, IWorldState worldState) => true;

        public override bool IsCompleted(IAgentState agentState, IWorldState worldState)
            => agentState.GetNeed("thirst") <= 0.01f;

        public override Dictionary<string, object> GetDesiredState() =>
            new Dictionary<string, object> { { "need_thirst", 0f } };

        public override float CalculatePriority(IAgentState agentState, IWorldState worldState)
        {
            float thirst = Mathf.Clamp01(agentState.GetNeed("thirst"));
            return thirst > triggerThreshold ? thirst * scale : 0f;
        }
    }
}