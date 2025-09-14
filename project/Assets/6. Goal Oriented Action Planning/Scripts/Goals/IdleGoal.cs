using System.Collections.Generic;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Simple idle goal - always available as a fallback
    /// </summary>
    public class IdleGoal : IGoal
    {
        public string GoalType => "idle";
        public float Priority => 1f; // Low priority

        public bool CanSatisfy(IAgentState agentState, IWorldState worldState)
        {
            return true; // Always available
        }

        public bool IsCompleted(IAgentState agentState, IWorldState worldState)
        {
            // Idle goal is completed when the idle fact is set
            return worldState.GetFact("idle");
        }

        public Dictionary<string, object> GetDesiredState()
        {
            return new Dictionary<string, object>
            {
                { "idle", true }
            };
        }

        public float CalculatePriority(IAgentState agentState, IWorldState worldState)
        {
            // Higher priority when no urgent needs
            float maxNeed = 0f;
            foreach (var need in agentState.GetAllNeeds().Values)
            {
                if (need > maxNeed) maxNeed = need;
            }

            // Lower priority when needs are high
            return 1f - maxNeed;
        }
    }
}