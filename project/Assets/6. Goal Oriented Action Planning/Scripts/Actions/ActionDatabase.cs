using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Holds a curated list of Action ScriptableObjects for a given agent.
    /// Supports both need reduction actions and movement actions.
    /// </summary>
    [CreateAssetMenu(fileName = "ActionDatabase", menuName = "GOAP/Action Database", order = 2)]
    public class ActionDatabase : ScriptableObject
    {
        [Header("Need Reduction Actions")]
        [SerializeField] private List<NeedReductionActionSO> needReductionActions = new();
        
        [Header("Movement Actions")]
        [SerializeField] private List<MoveToActionSO> moveToActions = new();
        
        [Header("Other Actions")]
        [SerializeField] private List<ScriptableObject> otherActions = new();

        /// <summary>
        /// Get all actions as IAction interface
        /// </summary>
        public IReadOnlyList<IAction> GetAllActions()
        {
            var allActions = new List<IAction>();
            
            // Add need reduction actions
            foreach (var action in needReductionActions)
            {
                if (action != null)
                    allActions.Add(action);
            }
            
            // Add movement actions
            foreach (var action in moveToActions)
            {
                if (action != null)
                    allActions.Add(action);
            }
            
            // Add other actions (that implement IAction)
            foreach (var action in otherActions)
            {
                if (action is IAction iAction)
                    allActions.Add(iAction);
            }
            
            return allActions;
        }

        /// <summary>
        /// Find action by type
        /// </summary>
        public IAction FindByType(string actionType)
        {
            var allActions = GetAllActions();
            foreach (var action in allActions)
            {
                if (action.ActionType == actionType)
                    return action;
            }
            return null;
        }

        /// <summary>
        /// Get only need reduction actions
        /// </summary>
        public IReadOnlyList<NeedReductionActionSO> NeedReductionActions => needReductionActions;

        /// <summary>
        /// Get only movement actions
        /// </summary>
        public IReadOnlyList<MoveToActionSO> MoveToActions => moveToActions;
    }
}