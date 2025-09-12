using System.Collections.Generic;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Action execution result
    /// </summary>
    public enum ActionResult
    {
        Running,    // Action is still executing
        Success,    // Action completed successfully
        Failed      // Action failed and should be aborted
    }
    
    /// <summary>
    /// Represents an action that can be executed by an agent
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Unique identifier for this action type
        /// </summary>
        string ActionType { get; }
        
        /// <summary>
        /// Cost of executing this action (used by planner for pathfinding)
        /// </summary>
        float Cost { get; }
        
        /// <summary>
        /// Check if this action can be executed given current conditions
        /// </summary>
        bool CheckPreconditions(IAgentState agentState, IWorldState worldState);
        
        /// <summary>
        /// Get the effects this action will have on the world/agent state
        /// Returns a dictionary of state changes this action will cause
        /// </summary>
        Dictionary<string, object> GetEffects();
        
        /// <summary>
        /// Start executing the action
        /// </summary>
        void StartExecution(IAgentState agentState, IWorldState worldState);
        
        /// <summary>
        /// Update the action execution (called each frame)
        /// </summary>
        ActionResult UpdateExecution(IAgentState agentState, IWorldState worldState, float deltaTime);
        
        /// <summary>
        /// Cancel the action execution
        /// </summary>
        void CancelExecution();
        
        /// <summary>
        /// Apply the action's effects to the world/agent state
        /// Called when the action completes successfully
        /// </summary>
        void ApplyEffects(IAgentState agentState, IWorldState worldState);
        
        /// <summary>
        /// Check if this action is currently executing
        /// </summary>
        bool IsExecuting { get; }
        
        /// <summary>
        /// Get a description of what this action does (for debugging)
        /// </summary>
        string GetDescription();
    }
}