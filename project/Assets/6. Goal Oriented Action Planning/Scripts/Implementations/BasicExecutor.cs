using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Basic implementation of plan executor
    /// </summary>
    public class BasicExecutor : IExecutor
    {
        public Plan CurrentPlan { get; private set; }
        public IAction CurrentAction { get; private set; }
        public IGoal CurrentGoal => CurrentPlan?.Goal;
        public bool IsExecuting => CurrentPlan != null && !IsComplete && !HasFailed;
        public bool IsComplete { get; private set; }
        public bool HasFailed { get; private set; }
        
        public System.Action<Plan, bool> OnPlanCompleted { get; set; }
        public System.Action<IAction> OnActionFailed { get; set; }
        public System.Action<IAction> OnActionStarted { get; set; }
        
        private bool actionStarted = false;
        
        public void SetPlan(Plan plan)
        {
            // Cancel current execution
            if (CurrentAction?.IsExecuting == true)
                CurrentAction.CancelExecution();
                
            CurrentPlan = plan;
            CurrentAction = null;
            IsComplete = false;
            HasFailed = false;
            actionStarted = false;
            
            if (plan != null)
            {
                Debug.Log($"New plan set for goal: {plan.Goal.GoalType} with {plan.Actions.Count} actions");
            }
        }
        
        public void Update(IAgentState agentState, IWorldState worldState, float deltaTime)
        {
            if (CurrentPlan == null || IsComplete || HasFailed)
                return;
                
            // If no current action, get the next one from the plan
            if (CurrentAction == null)
            {
                if (CurrentPlan.IsEmpty)
                {
                    // Plan completed successfully
                    Complete(true);
                    return;
                }
                
                CurrentAction = CurrentPlan.GetNextAction();
                actionStarted = false;
            }
            
            // Start the action if not started
            if (!actionStarted)
            {
                if (!CurrentAction.CheckPreconditions(agentState, worldState))
                {
                    Debug.LogWarning($"Action {CurrentAction.ActionType} preconditions failed during execution");
                    Fail();
                    return;
                }
                
                CurrentAction.StartExecution(agentState, worldState);
                actionStarted = true;
                OnActionStarted?.Invoke(CurrentAction);
                Debug.Log($"Started action: {CurrentAction.ActionType}");
            }
            
            // Update the current action
            var result = CurrentAction.UpdateExecution(agentState, worldState, deltaTime);
            
            switch (result)
            {
                case ActionResult.Success:
                    // Apply effects and move to next action
                    CurrentAction.ApplyEffects(agentState, worldState);
                    Debug.Log($"Action {CurrentAction.ActionType} completed successfully");
                    CurrentPlan.Advance();
                    CurrentAction = null;
                    actionStarted = false;
                    break;
                    
                case ActionResult.Failed:
                    Debug.LogError($"Action {CurrentAction.ActionType} failed");
                    OnActionFailed?.Invoke(CurrentAction);
                    Fail();
                    break;
                    
                case ActionResult.Running:
                    // Continue execution
                    break;
            }
        }
        
        public void Replan(IAgentState agentState, IWorldState worldState, List<IAction> availableActions, IPlanner planner)
        {
            if (CurrentGoal == null)
                return;
                
            Debug.Log($"Replanning for goal: {CurrentGoal.GoalType}");
            
            // Cancel current execution
            CancelExecution();
            
            // Create new plan
            var newPlan = planner.CreatePlan(CurrentGoal, agentState, worldState, availableActions);
            SetPlan(newPlan);
            
            if (newPlan == null)
            {
                Debug.LogWarning($"Failed to create new plan for goal: {CurrentGoal.GoalType}");
                Fail();
            }
        }
        
        public void CancelExecution()
        {
            if (CurrentAction?.IsExecuting == true)
            {
                CurrentAction.CancelExecution();
                Debug.Log($"Cancelled action: {CurrentAction.ActionType}");
            }
            
            CurrentAction = null;
            actionStarted = false;
        }
        
        private void Complete(bool success)
        {
            IsComplete = true;
            
            Debug.Log($"Plan completed. Success: {success}, Goal: {CurrentGoal?.GoalType}");
            OnPlanCompleted?.Invoke(CurrentPlan, success);
            
            if (!success)
                HasFailed = true;
        }
        
        private void Fail()
        {
            Complete(false);
        }
    }
}