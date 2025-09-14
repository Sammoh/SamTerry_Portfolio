using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Point of Interest marker. Declares which goals this POI can satisfy.
    /// </summary>
    public class POIMarker : MonoBehaviour
    {
        [Header("Goals supported by this POI (drag Goal assets)")]
        [SerializeField] private List<NeedReductionGoalSO> supportedGoals = new();

        public IReadOnlyList<NeedReductionGoalSO> SupportedGoals => supportedGoals;

        /// <summary>True if this POI supports the given runtime goal (by GoalType).</summary>
        public bool SupportsGoal(IGoal goal)
        {
            if (goal == null) return false;
            for (int i = 0; i < supportedGoals.Count; i++)
            {
                var g = supportedGoals[i];
                if (g != null && g.GoalType == goal.GoalType) return true;
            }
            return false;
        }

        public void AddSupportedGoal(NeedReductionGoalSO goalAsset)
        {
            if (goalAsset == null) return;
            if (!supportedGoals.Contains(goalAsset))
                supportedGoals.Add(goalAsset);
        }
    }
}