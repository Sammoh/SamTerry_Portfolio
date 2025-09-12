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
            return false; // Never truly complete - it's a fallback
        }
        
        public Dictionary<string, object> GetDesiredState()
        {
            return new Dictionary<string, object>
            {
                { "fact_idle", true }
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
    
    /// <summary>
    /// Goal to satisfy play/entertainment need
    /// </summary>
    public class PlayGoal : IGoal
    {
        public string GoalType => "play";
        public float Priority => 3f; // Lower priority
        
        public bool CanSatisfy(IAgentState agentState, IWorldState worldState)
        {
            return agentState.GetNeed("play") > 0.6f; // Only when bored
        }
        
        public bool IsCompleted(IAgentState agentState, IWorldState worldState)
        {
            return agentState.GetNeed("play") < 0.3f; // Satisfied when entertainment need is low
        }
        
        public Dictionary<string, object> GetDesiredState()
        {
            return new Dictionary<string, object>
            {
                { "need_play", 0f }
            };
        }
        
        public float CalculatePriority(IAgentState agentState, IWorldState worldState)
        {
            float play = agentState.GetNeed("play");
            return play > 0.6f ? play * 5f : 0f; // Lower urgency than basic needs
        }
    }
}