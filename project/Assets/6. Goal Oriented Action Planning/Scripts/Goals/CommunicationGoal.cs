using System.Collections.Generic;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Goal for agent to communicate/vocalize when needs are urgent or to alert
    /// </summary>
    public class CommunicationGoal : IGoal
    {
        public string GoalType => "communication";
        public float Priority => 3f; // Medium priority
        
        public bool CanSatisfy(IAgentState agentState, IWorldState worldState)
        {
            // Can communicate if not sleeping and hasn't just barked
            return !agentState.HasEffect("sleeping") && !worldState.GetFact("has_barked");
        }
        
        public bool IsCompleted(IAgentState agentState, IWorldState worldState)
        {
            // Goal is completed when agent has communicated
            return worldState.GetFact("communicated") || worldState.GetFact("has_barked");
        }
        
        public Dictionary<string, object> GetDesiredState()
        {
            return new Dictionary<string, object>
            {
                { "communicated", true },
                { "has_barked", true }
            };
        }
        
        public float CalculatePriority(IAgentState agentState, IWorldState worldState)
        {
            // Higher priority when needs are urgent (above 0.7) or when multiple needs are high
            float totalUrgentNeeds = 0f;
            int urgentNeedCount = 0;
            
            foreach (var need in agentState.GetAllNeeds())
            {
                if (need.Value > 0.7f)
                {
                    totalUrgentNeeds += need.Value;
                    urgentNeedCount++;
                }
            }
            
            // If multiple needs are urgent, increase communication priority
            if (urgentNeedCount >= 2)
            {
                return 5f; // High priority when multiple urgent needs
            }
            else if (urgentNeedCount == 1)
            {
                return 3f; // Medium priority for single urgent need
            }
            
            // Low priority baseline - occasional communication is natural
            return 1f;
        }
    }
}