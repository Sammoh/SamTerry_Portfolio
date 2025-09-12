using System.Collections.Generic;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Represents a plan - a sequence of actions to achieve a goal
    /// </summary>
    public class Plan
    {
        public List<IAction> Actions { get; private set; }
        public IGoal Goal { get; private set; }
        public float TotalCost { get; private set; }
        
        public Plan(IGoal goal, List<IAction> actions, float totalCost)
        {
            Goal = goal;
            Actions = actions ?? new List<IAction>();
            TotalCost = totalCost;
        }
        
        public bool IsEmpty => Actions.Count == 0;
        
        public IAction GetNextAction()
        {
            return Actions.Count > 0 ? Actions[0] : null;
        }
        
        public void RemoveFirstAction()
        {
            if (Actions.Count > 0)
                Actions.RemoveAt(0);
        }
    }
    
    /// <summary>
    /// Plans sequences of actions to achieve goals
    /// </summary>
    public interface IPlanner
    {
        /// <summary>
        /// Create a plan to achieve the given goal
        /// Returns null if no plan can be found
        /// </summary>
        Plan CreatePlan(IGoal goal, IAgentState agentState, IWorldState worldState, List<IAction> availableActions);
        
        /// <summary>
        /// Check if a plan is still valid given current state
        /// </summary>
        bool IsPlanValid(Plan plan, IAgentState agentState, IWorldState worldState);
        
        /// <summary>
        /// Maximum number of planning iterations to prevent infinite loops
        /// </summary>
        int MaxPlanningIterations { get; set; }
        
        /// <summary>
        /// Maximum depth for action sequences
        /// </summary>
        int MaxPlanDepth { get; set; }
    }
}