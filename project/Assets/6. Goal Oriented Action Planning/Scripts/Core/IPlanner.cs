using System.Collections.Generic;
using System.Text;

namespace Sammoh.GOAP
{
     /// <summary>
    /// Represents a plan - a sequence of actions to achieve a goal (FIFO execution).
    /// Uses a cursor so advancing is O(1) with no list shifting.
    /// </summary>
    public class Plan
    {
        private readonly List<IAction> _actions;
        private int _cursor; // index of the next action to execute (FIFO)

        public IGoal Goal { get; private set; }
        public float TotalCost { get; private set; }

        public Plan(IGoal goal, List<IAction> actions, float totalCost)
        {
            Goal = goal;
            _actions = actions ?? new List<IAction>();
            TotalCost = totalCost;
            _cursor = 0;
        }

        /// <summary>
        /// Read-only access to planned actions in original planning order.
        /// </summary>
        public List<IAction> Actions => _actions;

        /// <summary>
        /// True if no actions remain to execute.
        /// </summary>
        public bool IsEmpty => _cursor >= _actions.Count;

        /// <summary>
        /// Number of actions remaining (including the current one).
        /// </summary>
        public int ActionsRemaining => _actions.Count - _cursor;

        /// <summary>
        /// Returns the next action to execute (FIFO) without advancing.
        /// </summary>
        public IAction GetNextAction()
        {
            return _cursor < _actions.Count ? _actions[_cursor] : null;
        }

        /// <summary>
        /// Advance to the next action (O(1)).
        /// </summary>
        public void Advance()
        {
            if (_cursor < _actions.Count) _cursor++;
        }

        /// <summary>
        /// Resets the plan to the beginning.
        /// </summary>
        public void Reset()
        {
            _cursor = 0;
        }

        /// <summary>
        /// Legacy helper: remove the first action. Prefer Advance() for O(1).
        /// </summary>
        public void RemoveFirstAction()
        {
            Advance();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Plan[");
            for (int i = 0; i < _actions.Count; i++)
            {
                if (i > 0) sb.Append(" -> ");
                if (i == _cursor) sb.Append("▶"); // marks current action
                sb.Append(_actions[i]?.ActionType ?? "null");
            }
            sb.Append($"] cost={TotalCost:0.##} remaining={ActionsRemaining}");
            return sb.ToString();
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