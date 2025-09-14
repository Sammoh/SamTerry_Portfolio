using System.Collections.Generic;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Goal to satisfy hunger need
    /// </summary>
    public class EatGoal : IGoal
    {
        public string GoalType => "eat";
        public float Priority => 5f; // Medium priority
        
        public bool CanSatisfy(IAgentState agentState, IWorldState worldState)
        {
            return agentState.GetNeed("hunger") > 0.5f; // Only when hungry
        }
        
        public bool IsCompleted(IAgentState agentState, IWorldState worldState)
        {
            return agentState.GetNeed("hunger") < 0.2f; // Satisfied when hunger is low
        }
        
        public Dictionary<string, object> GetDesiredState()
        {
            return new Dictionary<string, object>
            {
                { "need_hunger", 0f }
            };
        }
        
        public float CalculatePriority(IAgentState agentState, IWorldState worldState)
        {
            float hunger = agentState.GetNeed("hunger");
            return hunger > 0.5f ? hunger * 10f : 0f; // Urgency increases with hunger
        }
    }
}