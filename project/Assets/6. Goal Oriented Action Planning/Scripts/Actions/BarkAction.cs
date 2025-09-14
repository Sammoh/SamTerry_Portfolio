using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{    
    /// <summary>
    /// Action for the agent to bark/vocalize
    /// Can be used for communication, alerting, or expressing needs
    /// </summary>
    public class BarkAction : IAction
    {
        public string ActionType => "bark";
        public float Cost => 0.5f; // Low cost since barking is easy
        public bool IsExecuting { get; private set; }
        
        private float executionTime;
        private float duration = 1f; // 1 second bark duration
        
        public bool CheckPreconditions(IAgentState agentState, IWorldState worldState)
        {
            // Agent can bark anytime, but maybe not if sleeping
            return !agentState.HasEffect("sleeping");
        }
        
        public Dictionary<string, object> GetEffects()
        {
            return new Dictionary<string, object>
            {
                { "has_barked", true },
                { "communicated", true }
            };
        }
        
        public void StartExecution(IAgentState agentState, IWorldState worldState)
        {
            IsExecuting = true;
            executionTime = 0f;
            Debug.Log("Started barking - Woof! Woof!");
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
            Debug.Log("Stopped barking");
        }
        
        public void ApplyEffects(IAgentState agentState, IWorldState worldState)
        {
            // Mark that the agent has communicated/barked
            worldState.SetFact("has_barked", true);
            worldState.SetFact("communicated", true);
            
            // Barking might slightly reduce stress or express urgency
            // This could be expanded to affect other agents in the world
        }
        
        public string GetDescription()
        {
            return "Bark to communicate or express needs";
        }
    }
}