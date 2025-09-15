using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Base ScriptableObject action for need reduction activities (eat, drink, sleep, play).
    /// Provides common behavior for actions that reduce a specific need to zero at a specific location.
    /// </summary>
    public abstract class NeedReductionActionSO : ScriptableObject, IAction
    {
        [Header("Action Identity")]
        [SerializeField] protected string actionType = "need_reduction";
        [SerializeField] protected float cost = 1f;

        [Header("Location Requirement")]
        [Tooltip("World state key that must be true to execute this action (e.g., 'at_food', 'at_water')")]
        [SerializeField] protected string requiredLocationKey = "at_location";

        [Header("Need Configuration")]
        [Tooltip("Agent need key to reduce (e.g., 'hunger', 'thirst', 'sleep', 'play')")]
        [SerializeField] protected string needKey = "need";
        [Tooltip("World state key for planner effects (e.g., 'need_hunger', 'need_thirst')")]
        [SerializeField] protected string worldStateNeedKey = "need_generic";
        [Tooltip("Value to set the need to when action completes")]
        [SerializeField] protected float targetNeedValue = 0f;

        [Header("Execution")]
        [Tooltip("How long the action takes in simulation seconds")]
        [SerializeField] protected float duration = 2f;

        [Header("Debug")]
        [SerializeField] protected string debugMessage = "Performing action";

        // Runtime state
        private bool isExecuting;
        private float timer;

        // IAction implementation
        public string ActionType => actionType;
        public float Cost => cost;
        public bool IsExecuting => isExecuting;

        public bool CheckPreconditions(IAgentState agentState, IWorldState worldState)
        {
            // Must be at the required location
            return worldState != null && worldState.GetFact(requiredLocationKey);
        }

        public Dictionary<string, object> GetEffects()
        {
            // Advertise what this action will change for the planner
            return new Dictionary<string, object>
            {
                { worldStateNeedKey, targetNeedValue },
                { requiredLocationKey, false }
            };
        }

        public void StartExecution(IAgentState agentState, IWorldState worldState)
        {
            isExecuting = true;
            timer = 0f;
            
            if (!string.IsNullOrEmpty(debugMessage))
            {
                Debug.Log($"[{actionType}] {debugMessage}");
            }
        }

        public ActionResult UpdateExecution(IAgentState agentState, IWorldState worldState, float deltaTime)
        {
            if (!isExecuting) return ActionResult.Failed;
            
            timer += Mathf.Max(0f, deltaTime);
            return timer >= duration ? ActionResult.Success : ActionResult.Running;
        }

        public void CancelExecution()
        {
            isExecuting = false;
            timer = 0f;
        }

        public void ApplyEffects(IAgentState agentState, IWorldState worldState)
        {
            isExecuting = false;
            if (worldState == null || agentState == null) return;

            // Apply effects to agent state and world state
            agentState.SetNeed(needKey, targetNeedValue);
            worldState.SetFact(requiredLocationKey, false);
        }

        public string GetDescription()
        {
            return $"{actionType}: {debugMessage} for {duration:0.##}s to reduce '{needKey}' to {targetNeedValue} at '{requiredLocationKey}'";
        }
    }
}