using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{    
    /// <summary>
    /// Action to eat at a food POI
    /// </summary>
    public class EatAction : IAction
    {
        public string ActionType => "eat";
        public float Cost => 1f;
        public bool IsExecuting { get; private set; }
        
        private float executionTime;
        private float duration = 2f; // 2 seconds to eat
        
        public bool CheckPreconditions(IAgentState agentState, IWorldState worldState)
        {
            return worldState.GetFact("at_food"); // Must be at food location
        }
        
        public Dictionary<string, object> GetEffects()
        {
            return new Dictionary<string, object>
            {
                { "need_hunger", 0f }
            };
        }
        
        public void StartExecution(IAgentState agentState, IWorldState worldState)
        {
            IsExecuting = true;
            executionTime = 0f;
            Debug.Log("Started eating");
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
            Debug.Log("Cancelled eating");
        }
        
        public void ApplyEffects(IAgentState agentState, IWorldState worldState)
        {
            agentState.SetNeed("hunger", 0f);
            worldState.SetFact("at_food", false); // No longer at food after eating
        }
        
        public string GetDescription()
        {
            return "Eat to satisfy hunger";
        }
    }
}
