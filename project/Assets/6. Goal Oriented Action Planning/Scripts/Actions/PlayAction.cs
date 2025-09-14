using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Play with a toy at the current location.
    /// Preconditions: at_toy == true
    /// Effects: lowers the configured play-need (default "need_play") and clears at_toy.
    /// </summary>
    public class PlayAction : IAction
    {
        public string ActionType { get; }
        public float Cost { get; }
        public bool IsExecuting { get; private set; }

        private readonly float duration;
        private readonly string playNeedKey;
        private readonly float targetNeedValue;
        private float timer;

        /// <param name="durationSeconds">How long the play takes (simulation seconds).</param>
        /// <param name="cost">Planner cost for this action.</param>
        /// <param name="playNeedKey">World/agent need key to modify (default "need_play").</param>
        /// <param name="targetNeedValue">Value to set when done (default 0).</param>
        public PlayAction(
            float durationSeconds = 2f,
            float cost = 1f,
            string playNeedKey = "need_play",
            float targetNeedValue = 0f)
        {
            this.duration = Mathf.Max(0.1f, durationSeconds);
            this.Cost = Mathf.Max(0.001f, cost);
            this.playNeedKey = string.IsNullOrEmpty(playNeedKey) ? "need_play" : playNeedKey;
            this.targetNeedValue = targetNeedValue;
            this.ActionType = "Play";
        }

        public bool CheckPreconditions(IAgentState agentState, IWorldState worldState)
        {
            // Must already be at the toy location
            // return worldState != null
            //     && worldState.TryGetBool("at_toy", out bool atToy)
            //     && atToy;
            return worldState.GetFact("at_toy"); // Must be at food location

        }

        public Dictionary<string, object> GetEffects()
        {
            // Advertise what this action will change so the planner can chain it.
            return new Dictionary<string, object>
            {
                { playNeedKey, targetNeedValue },
                { "at_toy", false }
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

            // Write final effects into the live state
            agentState.SetNeed(playNeedKey.Replace("need_", ""), targetNeedValue); // allow agent SetNeed("play", 0)
            worldState.SetFact("at_toy", false);

            // Also ensure the world-side need key (if directly stored in world) is updated for consistency with planner simulation.
            // This is harmless if your system ignores it.
            // If your needs are only on agentState, the planner will still work because it reads effects from GetEffects().
            if (playNeedKey.StartsWith("need_"))
            {
                // No-op here; some implementations keep needs solely on agentState.
                // Keeping this comment as a reminder of the split.
            }
        }

        public string GetDescription()
        {
            return $"Play with a toy for {duration:0.##}s to reduce '{playNeedKey}' to {targetNeedValue} and clear at_toy.";
        }
    }
}
