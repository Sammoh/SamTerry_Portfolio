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
                { "fact_idle", true }
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
    
    /// <summary>
    /// Action to move towards a POI
    /// </summary>
    public class MoveToAction : IAction
    {
        public string ActionType => "moveTo";
        public float Cost => 2f;
        public bool IsExecuting { get; private set; }
        
        private Transform agent;
        private string targetPOIType;
        private GameObject targetPOI;
        private float moveSpeed = 2f;
        private float arrivalDistance = 1.5f;
        
        public MoveToAction(Transform agentTransform, string poiType)
        {
            agent = agentTransform;
            targetPOIType = poiType;
        }
        
        public bool CheckPreconditions(IAgentState agentState, IWorldState worldState)
        {
            targetPOI = worldState.FindNearestPOI(agent.position, targetPOIType);
            return targetPOI != null;
        }
        
        public Dictionary<string, object> GetEffects()
        {
            return new Dictionary<string, object>
            {
                { $"at_{targetPOIType}", true }
            };
        }
        
        public void StartExecution(IAgentState agentState, IWorldState worldState)
        {
            IsExecuting = true;
            targetPOI = worldState.FindNearestPOI(agent.position, targetPOIType);
            Debug.Log($"Started moving towards {targetPOIType} POI: {targetPOI?.name}");
        }
        
        public ActionResult UpdateExecution(IAgentState agentState, IWorldState worldState, float deltaTime)
        {
            if (!IsExecuting || targetPOI == null)
                return ActionResult.Failed;
                
            // Simple movement towards target
            Vector3 direction = (targetPOI.transform.position - agent.position).normalized;
            agent.position += direction * moveSpeed * deltaTime;
            
            // Check if we've arrived
            float distance = Vector3.Distance(agent.position, targetPOI.transform.position);
            if (distance <= arrivalDistance)
            {
                IsExecuting = false;
                return ActionResult.Success;
            }
            
            return ActionResult.Running;
        }
        
        public void CancelExecution()
        {
            IsExecuting = false;
            Debug.Log($"Cancelled movement to {targetPOIType}");
        }
        
        public void ApplyEffects(IAgentState agentState, IWorldState worldState)
        {
            worldState.SetFact($"at_{targetPOIType}", true);
        }
        
        public string GetDescription()
        {
            return $"Move to nearest {targetPOIType}";
        }
    }
    
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
    
    /// <summary>
    /// Action to drink at a water POI
    /// </summary>
    public class DrinkAction : IAction
    {
        public string ActionType => "drink";
        public float Cost => 1f;
        public bool IsExecuting { get; private set; }
        
        private float executionTime;
        private float duration = 1.5f; // 1.5 seconds to drink
        
        public bool CheckPreconditions(IAgentState agentState, IWorldState worldState)
        {
            return worldState.GetFact("at_water"); // Must be at water location
        }
        
        public Dictionary<string, object> GetEffects()
        {
            return new Dictionary<string, object>
            {
                { "need_thirst", 0f }
            };
        }
        
        public void StartExecution(IAgentState agentState, IWorldState worldState)
        {
            IsExecuting = true;
            executionTime = 0f;
            Debug.Log("Started drinking");
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
            Debug.Log("Cancelled drinking");
        }
        
        public void ApplyEffects(IAgentState agentState, IWorldState worldState)
        {
            agentState.SetNeed("thirst", 0f);
            worldState.SetFact("at_water", false); // No longer at water after drinking
        }
        
        public string GetDescription()
        {
            return "Drink to satisfy thirst";
        }
    }
}