namespace Sammoh.GOAP
{
    /// <summary>
    /// Represents a goal that an agent can pursue
    /// </summary>
    public interface IGoal
    {
        /// <summary>
        /// Unique identifier for this goal type
        /// </summary>
        string GoalType { get; }
        
        /// <summary>
        /// Priority of this goal (higher values = higher priority)
        /// </summary>
        float Priority { get; }
        
        /// <summary>
        /// Check if this goal can be satisfied given the current world and agent state
        /// </summary>
        bool CanSatisfy(IAgentState agentState, IWorldState worldState);
        
        /// <summary>
        /// Check if this goal has been completed/satisfied
        /// </summary>
        bool IsCompleted(IAgentState agentState, IWorldState worldState);
        
        /// <summary>
        /// Get the desired end state that would satisfy this goal
        /// Returns a dictionary of world/agent state conditions that must be true
        /// </summary>
        System.Collections.Generic.Dictionary<string, object> GetDesiredState();
        
        /// <summary>
        /// Calculate the current priority based on agent state
        /// This allows dynamic priority adjustment based on need urgency
        /// </summary>
        float CalculatePriority(IAgentState agentState, IWorldState worldState);
    }
}