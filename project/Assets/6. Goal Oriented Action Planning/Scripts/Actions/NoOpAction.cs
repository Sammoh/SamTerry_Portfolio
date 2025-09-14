using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// No-op action that does nothing - for testing
    /// </summary>
    public class NoOpAction : IAction
    {
        public string ActionType => "noop";
        public float Cost => 1f;
        public bool IsExecuting { get; private set; }
        
        private float executionTime;
        private float duration = 1f; // 1 second duration
        
        public bool CheckPreconditions(IAgentState agentState, IWorldState worldState)
        {
            return true; // Always available
        }
        
        public Dictionary<string, object> GetEffects()
        {
            return new Dictionary<string, object>
            {
                { "idle", true }
            };
        }
        
        public void StartExecution(IAgentState agentState, IWorldState worldState)
        {
            IsExecuting = true;
            executionTime = 0f;
            Debug.Log("Started no-op action");
        }
        
        public ActionResult UpdateExecution(IAgentState agentState, IWorldState worldState, float deltaTime)
        {
            if (!IsExecuting)
                return ActionResult.Failed;
                
            executionTime += deltaTime;
            
            if (executionTime >= duration)
            {
                IsExecuting = false;
                return ActionResult.Success;
            }
            
            return ActionResult.Running;
        }
        
        public void CancelExecution()
        {
            IsExecuting = false;
            Debug.Log("Cancelled no-op action");
        }
        
        public void ApplyEffects(IAgentState agentState, IWorldState worldState)
        {
            worldState.SetFact("idle", true);
        }
        
        public string GetDescription()
        {
            return "Do nothing for a short time";
        }
    }
}
