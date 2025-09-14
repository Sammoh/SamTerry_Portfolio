using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Holds a curated list of Goal ScriptableObjects for a given agent.
    /// </summary>
    [CreateAssetMenu(fileName = "GoalDatabase", menuName = "GOAP/Goal Database", order = 1)]
    public class GoalDatabase : ScriptableObject
    {
        [SerializeField] private List<NeedReductionGoalSO> goals = new();

        public IReadOnlyList<IGoal> Goals => goals;

        public IGoal FindByType(string goalType)
        {
            foreach (var g in goals)
                if (g != null && g.GoalType == goalType) return g;
            return null;
        }
    }
}