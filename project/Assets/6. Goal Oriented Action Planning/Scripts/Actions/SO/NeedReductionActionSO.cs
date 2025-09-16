using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Abstract base class for configurable need reduction actions implemented as ScriptableObjects.
    /// Provides a unified framework for actions like eating, drinking, sleeping, and playing.
    /// </summary>
    public abstract class NeedReductionActionSO : ScriptableObject, IAction
    {
        [Header("Action Configuration")]
        [SerializeField] protected string actionType = "eat";
        [SerializeField] protected float cost = 1f;
        [SerializeField] protected float duration = 2f;
        
        [Header("Need Configuration")]
        [Tooltip("The need type this action reduces (e.g., 'hunger', 'thirst', 'sleep', 'play')")]
        [SerializeField] protected string needType = "hunger";
        [SerializeField] protected float targetNeedValue = 0f;
        
        [Header("World State Configuration")]
        [Tooltip("World state fact required to execute this action (e.g., 'at_food', 'at_water')")]
        [SerializeField] protected string requiredWorldFact = "at_food";
        [Tooltip("World state fact key for need satisfaction (e.g., 'need_hunger')")]
        [SerializeField] protected string needWorldStateKey = "need_hunger";
        
        // Runtime state
        private bool isExecuting;
        private float executionTime;
        
        // IAction implementation
        public virtual string ActionType => actionType;
        public virtual float Cost => cost;
        public virtual bool IsExecuting => isExecuting;
        
        public virtual bool CheckPreconditions(IAgentState agentState, IWorldState worldState)
        {
            return worldState != null && worldState.GetFact(requiredWorldFact);
        }
        
        public virtual Dictionary<string, object> GetEffects()
        {
            return new Dictionary<string, object>
            {
                { needWorldStateKey, targetNeedValue },
                { requiredWorldFact, false }
            };
        }
        
        public virtual void StartExecution(IAgentState agentState, IWorldState worldState)
        {
            isExecuting = true;
            executionTime = 0f;
            OnStartExecution(agentState, worldState);
        }
        
        public virtual ActionResult UpdateExecution(IAgentState agentState, IWorldState worldState, float deltaTime)
        {
            if (!isExecuting)
                return ActionResult.Failed;
                
            executionTime += deltaTime;
            
            if (executionTime >= duration)
            {
                isExecuting = false;
                return ActionResult.Success;
            }
            
            return ActionResult.Running;
        }
        
        public virtual void CancelExecution()
        {
            isExecuting = false;
            OnCancelExecution();
        }
        
        public virtual void ApplyEffects(IAgentState agentState, IWorldState worldState)
        {
            if (agentState != null)
                agentState.SetNeed(needType, targetNeedValue);
                
            if (worldState != null)
                worldState.SetFact(requiredWorldFact, false);
                
            OnApplyEffects(agentState, worldState);
        }
        
        public virtual string GetDescription()
        {
            return $"{actionType} to reduce {needType} to {targetNeedValue} (duration: {duration}s, cost: {cost})";
        }
        
        // Protected virtual methods for derived classes to override
        protected virtual void OnStartExecution(IAgentState agentState, IWorldState worldState)
        {
            Debug.Log($"Started {actionType}");
        }
        
        protected virtual void OnCancelExecution()
        {
            Debug.Log($"Cancelled {actionType}");
        }
        
        protected virtual void OnApplyEffects(IAgentState agentState, IWorldState worldState)
        {
            // Override in derived classes for additional effects
        }
        
        // Editor validation
        protected virtual void OnValidate()
        {
            cost = Mathf.Max(0.001f, cost);
            duration = Mathf.Max(0.1f, duration);
            targetNeedValue = Mathf.Clamp01(targetNeedValue);
        }
    }
}