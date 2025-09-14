using System.Collections.Generic;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Goal: Reduce the agent's sleep need back to zero by going to bed.
    /// </summary>
    public class SleepGoal : IGoal
    {
        // Type identifier used by the agent/planner
        public string GoalType { get; } = "sleep";

        // Optional fixed priority (not used in CalculatePriority directly)
        public float Priority { get; private set; }

        // Threshold at which this goal activates
        private const float SleepThreshold = 0.5f;

        // Multiplier to weight relative importance vs. other needs
        private const float PriorityScale = 10f;

        public bool CanSatisfy(IAgentState agentState, IWorldState worldState)
        {
            float sleep = agentState.GetNeed("sleep");
            return sleep > SleepThreshold;
        }

        public bool IsCompleted(IAgentState agentState, IWorldState worldState)
        {
            // Goal is complete when the agent is well rested
            return agentState.GetNeed("sleep") <= 0.05f;
        }

        public Dictionary<string, object> GetDesiredState()
        {
            // Desired outcome: sleep need reduced to zero
            return new Dictionary<string, object>
            {
                { "need_sleep", 0f }
            };
        }

        public float CalculatePriority(IAgentState agentState, IWorldState worldState)
        {
            float sleep = agentState.GetNeed("sleep");

            if (sleep > SleepThreshold)
            {
                Priority = sleep * PriorityScale;
            }
            else
            {
                Priority = 0f;
            }

            return Priority;
        }
    }
}