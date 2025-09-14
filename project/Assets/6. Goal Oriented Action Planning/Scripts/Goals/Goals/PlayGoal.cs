using System.Collections.Generic;

namespace Sammoh.GOAP
{
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