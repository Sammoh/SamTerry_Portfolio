// Assets/GOAP/Goals/IdleGoalSO.cs
using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    [CreateAssetMenu(fileName = "IdleGoal", menuName = "GOAP/Goals/Idle", order = 0)]

    public class IdleGoalSO : GoalSO
    {
        public override bool CanSatisfy(IAgentState agentState, IWorldState worldState) => true;

        public override bool IsCompleted(IAgentState agentState, IWorldState worldState) => true;

        public override Dictionary<string, object> GetDesiredState() =>
            new Dictionary<string, object> { { "idle", true } };

        public override float CalculatePriority(IAgentState agentState, IWorldState worldState)
            => 0.01f; // Always available, extremely low priority
    }
}