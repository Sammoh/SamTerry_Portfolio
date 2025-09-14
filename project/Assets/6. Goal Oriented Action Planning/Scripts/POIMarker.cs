using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Point of Interest marker. Now supports asset-driven goals (while keeping optional legacy tag).
    /// </summary>
    public class POIMarker : MonoBehaviour
    {
        [Header("Supported Goals (ScriptableObjects)")]
        [SerializeField] private List<NeedReductionGoalSO> supportedGoals = new();
        //
        // public string poiTag;
        //
        public IReadOnlyList<NeedReductionGoalSO> SupportedGoals => supportedGoals;
        //
        public bool SupportsGoal(IGoal goal)
        {
            if (goal == null) return false;
            foreach (var g in supportedGoals)
                if (g != null && g.GoalType == goal.GoalType) return true;
            return false;
        }
        
        // Add new supported goal
        public void AddSupportedGoal(NeedReductionGoalSO goal)
        {
            if (goal == null) return;
            if (!supportedGoals.Contains(goal))
                supportedGoals.Add(goal);
        }
    }
}