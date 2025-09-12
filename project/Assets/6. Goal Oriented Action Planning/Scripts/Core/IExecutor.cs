namespace Sammoh.GOAP
{
    /// <summary>
    /// Manages the execution of plans and handles failures/replanning
    /// </summary>
    public interface IExecutor
    {
        /// <summary>
        /// Current plan being executed
        /// </summary>
        Plan CurrentPlan { get; }
        
        /// <summary>
        /// Current action being executed
        /// </summary>
        IAction CurrentAction { get; }
        
        /// <summary>
        /// Current goal being pursued
        /// </summary>
        IGoal CurrentGoal { get; }
        
        /// <summary>
        /// Set a new plan to execute
        /// </summary>
        void SetPlan(Plan plan);
        
        /// <summary>
        /// Update the executor (called each tick)
        /// </summary>
        void Update(IAgentState agentState, IWorldState worldState, float deltaTime);
        
        /// <summary>
        /// Force a replan for the current goal
        /// </summary>
        void Replan(IAgentState agentState, IWorldState worldState, System.Collections.Generic.List<IAction> availableActions, IPlanner planner);
        
        /// <summary>
        /// Cancel current execution and clear plan
        /// </summary>
        void CancelExecution();
        
        /// <summary>
        /// Check if the executor has a plan and is actively executing
        /// </summary>
        bool IsExecuting { get; }
        
        /// <summary>
        /// Check if the current plan has completed (successfully or failed)
        /// </summary>
        bool IsComplete { get; }
        
        /// <summary>
        /// Check if the current plan has failed
        /// </summary>
        bool HasFailed { get; }
        
        /// <summary>
        /// Event triggered when a plan completes
        /// </summary>
        System.Action<Plan, bool> OnPlanCompleted { get; set; } // Plan, success
        
        /// <summary>
        /// Event triggered when an action fails
        /// </summary>
        System.Action<IAction> OnActionFailed { get; set; }
        
        /// <summary>
        /// Event triggered when a new action starts
        /// </summary>
        System.Action<IAction> OnActionStarted { get; set; }
    }
}