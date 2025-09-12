using System.Collections.Generic;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Simple A* based planner implementation
    /// </summary>
    public class BasicPlanner : IPlanner
    {
        public int MaxPlanningIterations { get; set; } = 1000;
        public int MaxPlanDepth { get; set; } = 10;
        
        public Plan CreatePlan(IGoal goal, IAgentState agentState, IWorldState worldState, List<IAction> availableActions)
        {
            // Simple forward-chaining planner
            var plan = new List<IAction>();
            var currentState = CloneState(agentState, worldState);
            var goalState = goal.GetDesiredState();
            var iterations = 0;
            
            while (!IsGoalSatisfied(goal, currentState.agentState, currentState.worldState) && 
                   iterations < MaxPlanningIterations && 
                   plan.Count < MaxPlanDepth)
            {
                iterations++;
                
                IAction bestAction = null;
                float bestScore = float.MinValue;
                
                // Find the best action that gets us closer to the goal
                foreach (var action in availableActions)
                {
                    if (action.CheckPreconditions(currentState.agentState, currentState.worldState))
                    {
                        float score = EvaluateAction(action, goal, currentState.agentState, currentState.worldState);
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestAction = action;
                        }
                    }
                }
                
                if (bestAction == null)
                {
                    // No valid action found, planning failed
                    return null;
                }
                
                // Add action to plan and simulate its effects
                plan.Add(bestAction);
                SimulateAction(bestAction, currentState.agentState, currentState.worldState);
            }
            
            if (IsGoalSatisfied(goal, currentState.agentState, currentState.worldState))
            {
                float totalCost = CalculatePlanCost(plan);
                return new Plan(goal, plan, totalCost);
            }
            
            return null; // Could not find a valid plan
        }
        
        public bool IsPlanValid(Plan plan, IAgentState agentState, IWorldState worldState)
        {
            if (plan == null || plan.IsEmpty)
                return false;
                
            // Check if the first action in the plan can still be executed
            var nextAction = plan.GetNextAction();
            return nextAction?.CheckPreconditions(agentState, worldState) ?? false;
        }
        
        private (IAgentState agentState, IWorldState worldState) CloneState(IAgentState agentState, IWorldState worldState)
        {
            // For simplicity, we'll work with the actual states
            // In a full implementation, you'd want to create copies for simulation
            return (agentState, worldState);
        }
        
        private bool IsGoalSatisfied(IGoal goal, IAgentState agentState, IWorldState worldState)
        {
            return goal.IsCompleted(agentState, worldState);
        }
        
        private float EvaluateAction(IAction action, IGoal goal, IAgentState agentState, IWorldState worldState)
        {
            // Simple heuristic: prefer actions with lower cost that move toward goal
            float baseCost = action.Cost;
            float goalProgress = 0f;
            
            // Check how much this action contributes to the goal
            var effects = action.GetEffects();
            var desiredState = goal.GetDesiredState();
            
            foreach (var desired in desiredState)
            {
                if (effects.ContainsKey(desired.Key))
                {
                    goalProgress += 10f; // Bonus for contributing to goal
                }
            }
            
            return goalProgress - baseCost;
        }
        
        private void SimulateAction(IAction action, IAgentState agentState, IWorldState worldState)
        {
            // Apply the action's effects to simulate the state change
            // This is a simplified simulation
            var effects = action.GetEffects();
            
            foreach (var effect in effects)
            {
                if (effect.Key.StartsWith("need_"))
                {
                    string needType = effect.Key.Substring(5);
                    if (effect.Value is float needValue)
                    {
                        agentState.SetNeed(needType, needValue);
                    }
                }
                else if (effect.Key.StartsWith("fact_"))
                {
                    string factName = effect.Key.Substring(5);
                    if (effect.Value is bool factValue)
                    {
                        worldState.SetFact(factName, factValue);
                    }
                }
            }
        }
        
        private float CalculatePlanCost(List<IAction> actions)
        {
            float totalCost = 0f;
            foreach (var action in actions)
            {
                totalCost += action.Cost;
            }
            return totalCost;
        }
    }
}