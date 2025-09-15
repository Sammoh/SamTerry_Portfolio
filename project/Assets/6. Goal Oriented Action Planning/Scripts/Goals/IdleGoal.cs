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

            // Increase idle priority when needs are low, but add some base priority
            // This ensures idle behaviors have a reasonable chance to execute
            float basePriority = 0.3f; // Base priority for idle activities
            float needBasedPriority = 1f - maxNeed;
            
            return basePriority + (needBasedPriority * 0.5f); // Max priority of 0.8 when all needs are satisfied
        }
    }
}