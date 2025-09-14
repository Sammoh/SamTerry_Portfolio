using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// ScriptableObject-driven goal that reduces a named "need" toward zero.
    /// Create instances for Sleep, Eat, Drink, Play with different parameters.
    /// </summary>
    [CreateAssetMenu(fileName = "NewNeedGoal", menuName = "GOAP/Goal/Need Reduction", order = 0)]
    public class NeedReductionGoalSO : ScriptableObject, IGoal
    {
        [Header("Identity")]
        [SerializeField] private string goalType = "sleep";       // e.g., "sleep", "eat", "drink", "play"

        [Header("Need Keys")]
        [Tooltip("AgentState need key to read, e.g. 'sleep', 'hunger', 'thirst', 'play'.")]
        [SerializeField] private string needKey = "sleep";

        [Tooltip("World/desired-state key (planner uses this to satisfy), e.g. 'need_sleep'.")]
        [SerializeField] private string desiredWorldStateKey = "need_sleep";

        [Header("Tuning")]
        [Tooltip("Above this, the goal activates.")]
        [SerializeField, Range(0f, 1f)] private float activationThreshold = 0.5f;

        [Tooltip("At or below this, the goal is considered complete.")]
        [SerializeField, Range(0f, 1f)] private float completionThreshold = 0.05f;

        [Tooltip("Priority multiplier once above activation threshold.")]
        [SerializeField] private float priorityScale = 10f;

        // IGoal
        public string GoalType => goalType;

        public float Priority { get; private set; }

        public bool CanSatisfy(IAgentState agentState, IWorldState worldState)
        {
            float v = agentState.GetNeed(needKey);
            return v > activationThreshold;
        }

        public bool IsCompleted(IAgentState agentState, IWorldState worldState)
        {
            float v = agentState.GetNeed(needKey);
            return v <= completionThreshold;
        }

        public Dictionary<string, object> GetDesiredState()
        {
            return new Dictionary<string, object>
            {
                { desiredWorldStateKey, 0f }
            };
        }

        public float CalculatePriority(IAgentState agentState, IWorldState worldState)
        {
            float v = agentState.GetNeed(needKey);
            Priority = (v > activationThreshold) ? (v * priorityScale) : 0f;
            return Priority;
        }
    }
}
