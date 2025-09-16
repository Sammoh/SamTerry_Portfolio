using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Validation helper for GOAP system setup - can be used to check configuration
    /// </summary>
    public static class GOAPValidation
    {
        /// <summary>
        /// Validate that goals and actions are properly configured
        /// </summary>
        public static List<string> ValidateSetup(List<IGoal> goals, List<IAction> actions)
        {
            var issues = new List<string>();
            
            if (goals == null || goals.Count == 0)
            {
                issues.Add("No goals configured - agent will have nothing to pursue");
            }
            
            if (actions == null || actions.Count == 0)
            {
                issues.Add("No actions configured - agent cannot do anything");
                return issues;
            }
            
            // Check that each goal has at least one action that can help achieve it
            foreach (var goal in goals)
            {
                if (goal == null) continue;
                
                var desiredState = goal.GetDesiredState();
                bool hasRelevantAction = false;
                
                foreach (var action in actions)
                {
                    if (action == null) continue;
                    
                    var effects = action.GetEffects();
                    foreach (var desired in desiredState)
                    {
                        if (effects.ContainsKey(desired.Key))
                        {
                            hasRelevantAction = true;
                            break;
                        }
                    }
                    
                    if (hasRelevantAction) break;
                }
                
                if (!hasRelevantAction)
                {
                    issues.Add($"Goal '{goal.GoalType}' has no actions that can achieve its desired state: {string.Join(", ", desiredState.Keys)}");
                }
            }
            
            // Check for orphaned movement actions (actions that require movement but have no POIs)
            var movementGoalTypes = new HashSet<string>();
            foreach (var action in actions)
            {
                if (action is MoveToAction moveAction)
                {
                    // This is a bit hacky since we can't easily get the current goal from the action
                    // but we can check if it's a movement action
                    var effects = action.GetEffects();
                    foreach (var effect in effects.Keys)
                    {
                        if (effect.StartsWith("at_"))
                        {
                            var goalType = effect.Substring(3);
                            // Map location facts back to goal types
                            switch (goalType)
                            {
                                case "food": movementGoalTypes.Add("hunger"); break;
                                case "water": movementGoalTypes.Add("thirst"); break;
                                case "bed": movementGoalTypes.Add("sleep"); break;
                                case "toy": movementGoalTypes.Add("play"); break;
                                default: movementGoalTypes.Add(goalType); break;
                            }
                        }
                    }
                }
            }
            
            // Validate action chains - movement actions should have corresponding consumption actions
            var locationFacts = new HashSet<string>();
            var consumptionActions = new HashSet<string>();
            
            foreach (var action in actions)
            {
                var effects = action.GetEffects();
                foreach (var effect in effects.Keys)
                {
                    if (effect.StartsWith("at_"))
                    {
                        locationFacts.Add(effect);
                    }
                    if (effect.StartsWith("need_"))
                    {
                        consumptionActions.Add(effect);
                    }
                }
            }
            
            // Check that location facts have corresponding consumption actions
            var locationToNeed = new Dictionary<string, string>
            {
                {"at_food", "need_hunger"},
                {"at_water", "need_thirst"}, 
                {"at_bed", "need_sleep"},
                {"at_toy", "need_play"}
            };
            
            foreach (var location in locationFacts)
            {
                if (locationToNeed.TryGetValue(location, out string needKey))
                {
                    if (!consumptionActions.Contains(needKey))
                    {
                        issues.Add($"Location fact '{location}' has no corresponding consumption action for '{needKey}'");
                    }
                }
            }
            
            return issues;
        }
        
        /// <summary>
        /// Validate goal database configuration
        /// </summary>
        public static List<string> ValidateGoalDatabase(GoalDatabase goalDatabase)
        {
            var issues = new List<string>();
            
            if (goalDatabase == null)
            {
                issues.Add("GoalDatabase is null - agent will have no goals");
                return issues;
            }
            
            if (goalDatabase.Goals == null || goalDatabase.Goals.Count == 0)
            {
                issues.Add("GoalDatabase has no goals configured");
                return issues;
            }
            
            var goalTypes = new HashSet<string>();
            foreach (var goal in goalDatabase.Goals)
            {
                if (goal == null)
                {
                    issues.Add("GoalDatabase contains null goal reference");
                    continue;
                }
                
                if (string.IsNullOrEmpty(goal.GoalType))
                {
                    issues.Add("Goal has empty or null GoalType");
                    continue;
                }
                
                if (goalTypes.Contains(goal.GoalType))
                {
                    issues.Add($"Duplicate goal type '{goal.GoalType}' found in database");
                }
                else
                {
                    goalTypes.Add(goal.GoalType);
                }
                
                // Validate goal configuration
                if (goal is NeedReductionGoalSO needGoal)
                {
                    var desiredState = needGoal.GetDesiredState();
                    if (desiredState == null || desiredState.Count == 0)
                    {
                        issues.Add($"Goal '{goal.GoalType}' has no desired state defined");
                    }
                }
            }
            
            return issues;
        }
        
        /// <summary>
        /// Log validation results
        /// </summary>
        public static void LogValidationResults(List<string> issues, string context = "GOAP Setup")
        {
            if (issues.Count == 0)
            {
                Debug.Log($"{context} validation passed - no issues found");
            }
            else
            {
                Debug.LogWarning($"{context} validation found {issues.Count} issues:");
                foreach (var issue in issues)
                {
                    Debug.LogWarning($"- {issue}");
                }
            }
        }
        
        /// <summary>
        /// Validate ActionDatabase configuration
        /// </summary>
        public static List<string> ValidateActionDatabase(ActionDatabase actionDatabase)
        {
            var issues = new List<string>();
            
            if (actionDatabase == null)
            {
                issues.Add("ActionDatabase is null");
                return issues;
            }
            
            // Use the built-in validation from ActionDatabase
            var dbIssues = actionDatabase.ValidateActions();
            issues.AddRange(dbIssues);
            
            // Additional validation specific to GOAP system
            var actions = actionDatabase.Actions;
            if (actions != null && actions.Count > 0)
            {
                // Check for essential action types
                bool hasNoOp = false;
                var actionTypes = new HashSet<string>();
                
                foreach (var action in actions)
                {
                    if (action == null) continue;
                    
                    if (action.ActionType == "noop")
                        hasNoOp = true;
                        
                    actionTypes.Add(action.ActionType);
                }
                
                if (!hasNoOp)
                {
                    issues.Add("ActionDatabase missing NoOp action - recommended for fallback behavior");
                }
                
                // Check for common need reduction actions
                var expectedActions = new[] { "eat", "drink", "sleep", "play" };
                foreach (var expected in expectedActions)
                {
                    if (!actionTypes.Contains(expected))
                    {
                        issues.Add($"ActionDatabase missing common action type '{expected}' - may cause planning issues");
                    }
                }
            }
            
            return issues;
        }
    }
}