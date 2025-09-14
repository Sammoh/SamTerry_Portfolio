using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Sleep at the current bed/rest location.
    /// Preconditions: at_bed == true
    /// Effects: sets the sleep need to 0 and clears at_bed.
    /// </summary>
    public class SleepAction : IAction
    {
        public string ActionType { get; }
        public float Cost { get; }
        public bool IsExecuting { get; private set; }

        private readonly float duration;
        private float timer;

        /// <param name="durationSeconds">How long sleeping takes in simulation seconds.</param>
        /// <param name="cost">Planner cost for this action.</param>
        public SleepAction(float durationSeconds = 3f, float cost = 1f)
        {
            duration = Mathf.Max(0.1f, durationSeconds);
            Cost = Mathf.Max(0.001f, cost);
            ActionType = "Sleep";
        }

        public bool CheckPreconditions(IAgentState agentState, IWorldState worldState)
        {
            // Must already be at the bed/resting POI
            return worldState != null && worldState.GetFact("at_bed");
        }

        public Dictionary<string, object> GetEffects()
        {
            // Planner-visible effects
            return new Dictionary<string, object>
            {
                { "need_sleep", 0f },
                { "at_bed", false }
            };
        }

        public void StartExecution(IAgentState agentState, IWorldState worldState)
        {
            IsExecuting = true;
            timer = 0f;
        }

        public ActionResult UpdateExecution(IAgentState agentState, IWorldState worldState, float deltaTime)
        {
            if (!IsExecuting) return ActionResult.Failed;
            timer += Mathf.Max(0f, deltaTime);
            return timer >= duration ? ActionResult.Success : ActionResult.Running;
        }

        public void CancelExecution()
        {
            IsExecuting = false;
        }

        public void ApplyEffects(IAgentState agentState, IWorldState worldState)
        {
            IsExecuting = false;
            if (worldState == null || agentState == null) return;

            // Apply runtime effects
            agentState.SetNeed("sleep", 0f);
            worldState.SetFact("at_bed", false);
        }

        public string GetDescription()
        {
            return $"Sleep for {duration:0.##}s to reduce need_sleep to 0 and clear at_bed.";
        }
    }
}
