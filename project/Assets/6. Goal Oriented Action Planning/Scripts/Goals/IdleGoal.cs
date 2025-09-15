using System.Collections.Generic;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Enhanced idle goal with improved priority system.
    /// Base priority of 0.3 ensures idle activities have reasonable chance to execute.
    /// Additional priority boost when agent needs are satisfied (max 0.8 total).
    /// Prevents idle behaviors from being completely suppressed by high-priority needs.
    /// </summary>
    public class IdleGoal : IGoal
    {
        public string GoalType => "idle";
        public float Priority => 0.3f; // Base priority for reasonable execution chance

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
            // Base priority of 0.3 ensures idle activities have reasonable chance to execute
            float basePriority = 0.3f;
            
            // Calculate the maximum need level
            float maxNeed = 0f;
            if (agentState != null)
            {
                foreach (var need in agentState.GetAllNeeds().Values)
                {
                    if (need > maxNeed) maxNeed = need;
                }
            }
            
            // When needs are satisfied (low), boost priority up to 0.8 total
            // When needs are high, priority stays at base level (0.3)
            // This prevents idle behaviors from being completely suppressed by high-priority needs
            float needSatisfactionBonus = (1f - maxNeed) * 0.5f; // Max bonus of 0.5
            
            float finalPriority = basePriority + needSatisfactionBonus;
            
            // Ensure we don't exceed max priority of 0.8
            return Mathf.Min(finalPriority, 0.8f);
        }
    }
}