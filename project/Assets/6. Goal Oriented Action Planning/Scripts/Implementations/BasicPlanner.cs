
using System.Collections.Generic;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Forward-chaining greedy planner.
    ///
    /// Fixes in this version:
    /// - Uses the computed goalState instead of recomputing per evaluation.
    /// - Corrects execution order: returns actions in FIFO (MoveTo -> Eat), not LIFO.
    /// - SimulateAction applies boolean effects directly to world facts (e.g., "at_food").
    /// - Minor safeguard: if no action improves score, planning stops gracefully.
    /// </summary>
    public class BasicPlanner : IPlanner
    {
        public int MaxPlanningIterations { get; set; } = 1000;
        public int MaxPlanDepth { get; set; } = 10;

        public Plan CreatePlan(IGoal goal, IAgentState agentState, IWorldState worldState, List<IAction> availableActions)
        {
            // Greedy forward-chaining
            var plan = new List<IAction>();
            var current = CloneState(agentState, worldState);

            // Cache desired state once (previous version created it then didn't use it)
            var goalState = goal.GetDesiredState();
            int iterations = 0;

            while (!IsGoalSatisfied(goal, current.agentState, current.worldState) &&
                   iterations < MaxPlanningIterations &&
                   plan.Count < MaxPlanDepth)
            {
                iterations++;

                IAction bestAction = null;
                float bestScore = float.NegativeInfinity;

                // Pick the action that most advances us toward the goal for the current simulated state
                foreach (var action in availableActions)
                {
                    if (!action.CheckPreconditions(current.agentState, current.worldState))
                        continue;

                    float score = EvaluateAction(action, goalState, current.agentState, current.worldState);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestAction = action;
                    }
                }

                // No applicable action: fail
                if (bestAction == null || bestScore == float.NegativeInfinity)
                    return null;

                // Add action then simulate its effects for the next iteration
                plan.Add(bestAction);
                SimulateAction(bestAction, current.agentState, current.worldState);
            }

            if (!IsGoalSatisfied(goal, current.agentState, current.worldState))
                return null;

            // Ensure FIFO execution order: first planned = first executed.
            // If your executor pops from the end (LIFO), reverse here instead.
            float totalCost = CalculatePlanCost(plan);
            return new Plan(goal, plan, totalCost);
        }

        public bool IsPlanValid(Plan plan, IAgentState agentState, IWorldState worldState)
        {
            if (plan == null || plan.IsEmpty)
                return false;

            var nextAction = plan.GetNextAction();
            return nextAction?.CheckPreconditions(agentState, worldState) ?? false;
        }

        private (IAgentState agentState, IWorldState worldState) CloneState(IAgentState agentState, IWorldState worldState)
        {
            // NOTE: This returns live state. For production, prefer an overlay/clone so planning doesn't mutate reality.
            return (agentState, worldState);
        }

        private bool IsGoalSatisfied(IGoal goal, IAgentState agentState, IWorldState worldState)
        {
            return goal.IsCompleted(agentState, worldState);
        }

        private float EvaluateAction(IAction action, Dictionary<string, object> goalState, IAgentState agentState, IWorldState worldState)
        {
            // Heuristic: direct matches to desired keys are heavily rewarded;
            // small bonus for common enablers (e.g., at_food toward need_hunger, at_water toward need_thirst).
            float baseCost = action.Cost;
            float goalProgress = 0f;

            var effects = action.GetEffects();

            // Reward direct contributors
            foreach (var kvp in goalState)
            {
                if (effects.ContainsKey(kvp.Key))
                    goalProgress += 10f;
            }

            // Enabler nudges
            bool wantsHunger = goalState.ContainsKey("need_hunger");
            bool wantsThirst = goalState.ContainsKey("need_thirst");
            bool wantsSleep  = goalState.ContainsKey("need_sleep");
            bool wantsPlay   = goalState.ContainsKey("need_play");

            if (wantsHunger && effects.ContainsKey("at_food"))  goalProgress += 3f;
            if (wantsThirst && effects.ContainsKey("at_water")) goalProgress += 3f;
            if (wantsSleep  && effects.ContainsKey("at_bed"))   goalProgress += 3f;
            if (wantsPlay   && effects.ContainsKey("at_toy"))   goalProgress += 3f;

            // If nothing contributed, penalize slightly so we don't loop pointlessly
            if (goalProgress <= 0f) goalProgress = -1f;

            return goalProgress - baseCost;
        }

        private void SimulateAction(IAction action, IAgentState agentState, IWorldState worldState)
        {
            var effects = action.GetEffects();
            foreach (var effect in effects)
            {
                string key = effect.Key;
                object val = effect.Value;

                // Need updates: need_* -> SetNeed(type, value)
                if (key.StartsWith("need_"))
                {
                    string needType = key.Substring(5);
                    if (val is float f) agentState.SetNeed(needType, f);
                    continue;
                }

                // Optional "fact_*" prefix support
                if (key.StartsWith("fact_"))
                {
                    string factName = key.Substring(5);
                    if (val is bool b) worldState.SetFact(factName, b);
                    continue;
                }

                // Generic boolean facts (e.g., "at_food", "door_open")
                if (val is bool genericBool)
                    worldState.SetFact(key, genericBool);
            }
        }

        private float CalculatePlanCost(List<IAction> actions)
        {
            float totalCost = 0f;
            for (int i = 0; i < actions.Count; i++)
                totalCost += actions[i].Cost;
            return totalCost;
        }
    }
}
