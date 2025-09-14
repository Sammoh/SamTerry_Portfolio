using System.Collections.Generic;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Goal to satisfy thirst need
    /// </summary>
    public class DrinkGoal : IGoal
    {
        public string GoalType => "drink";
        public float Priority => 6f; // High priority
        
        public bool CanSatisfy(IAgentState agentState, IWorldState worldState)
        {
            return agentState.GetNeed("thirst") > 0.4f; // Only when thirsty
        }
        
        public bool IsCompleted(IAgentState agentState, IWorldState worldState)
        {
            return agentState.GetNeed("thirst") < 0.1f; // Satisfied when thirst is very low
        }
        
        public Dictionary<string, object> GetDesiredState()
        {
            return new Dictionary<string, object>
            {
                { "need_thirst", 0f }
            };
        }
        
        public float CalculatePriority(IAgentState agentState, IWorldState worldState)
        {
            float thirst = agentState.GetNeed("thirst");
            return thirst > 0.4f ? thirst * 12f : 0f; // Higher urgency than hunger
        }
    }
}