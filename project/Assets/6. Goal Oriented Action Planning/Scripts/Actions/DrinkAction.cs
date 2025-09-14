using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{

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
