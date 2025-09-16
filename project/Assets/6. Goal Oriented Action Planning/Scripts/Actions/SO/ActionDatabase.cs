using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// ScriptableObject container for managing collections of ScriptableObject actions.
    /// Provides centralized action management and validation.
    /// </summary>
    [CreateAssetMenu(fileName = "ActionDatabase", menuName = "GOAP/Action Database", order = 0)]
    public class ActionDatabase : ScriptableObject
    {
        [Header("ScriptableObject Actions")]
        [SerializeField] private List<ScriptableObject> scriptableActions = new List<ScriptableObject>();
        
        [Header("Database Info")]
        [SerializeField] private int totalActions = 0;
        [SerializeField] private string lastUpdated = "";
        
        /// <summary>
        /// Get all actions as IAction interfaces
        /// </summary>
        public List<IAction> Actions
        {
            get
            {
                var actions = new List<IAction>();
                foreach (var so in scriptableActions)
                {
                    if (so is IAction action)
                        actions.Add(action);
                }
                return actions;
            }
        }
        
        /// <summary>
        /// Get all ScriptableObject actions of a specific type
        /// </summary>
        public List<T> GetActionsOfType<T>() where T : ScriptableObject, IAction
        {
            var actions = new List<T>();
            foreach (var so in scriptableActions)
            {
                if (so is T action)
                    actions.Add(action);
            }
            return actions;
        }
        
        /// <summary>
        /// Add an action to the database
        /// </summary>
        public bool AddAction(ScriptableObject action)
        {
            if (action == null || !(action is IAction))
                return false;
                
            if (!scriptableActions.Contains(action))
            {
                scriptableActions.Add(action);
                UpdateDatabaseInfo();
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Remove an action from the database
        /// </summary>
        public bool RemoveAction(ScriptableObject action)
        {
            if (scriptableActions.Remove(action))
            {
                UpdateDatabaseInfo();
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Clear all actions from the database
        /// </summary>
        public void ClearAll()
        {
            scriptableActions.Clear();
            UpdateDatabaseInfo();
        }
        
        /// <summary>
        /// Validate all actions in the database
        /// </summary>
        public List<string> ValidateActions()
        {
            var issues = new List<string>();
            
            if (scriptableActions.Count == 0)
            {
                issues.Add("ActionDatabase is empty - no actions configured");
                return issues;
            }
            
            var actionTypes = new HashSet<string>();
            
            for (int i = 0; i < scriptableActions.Count; i++)
            {
                var so = scriptableActions[i];
                
                if (so == null)
                {
                    issues.Add($"Action at index {i} is null");
                    continue;
                }
                
                if (!(so is IAction action))
                {
                    issues.Add($"Action '{so.name}' does not implement IAction interface");
                    continue;
                }
                
                // Check for duplicate action types
                if (actionTypes.Contains(action.ActionType))
                {
                    issues.Add($"Duplicate action type '{action.ActionType}' found in action '{so.name}'");
                }
                else
                {
                    actionTypes.Add(action.ActionType);
                }
                
                // Validate action configuration
                if (string.IsNullOrEmpty(action.ActionType))
                {
                    issues.Add($"Action '{so.name}' has empty or null ActionType");
                }
                
                if (action.Cost <= 0)
                {
                    issues.Add($"Action '{so.name}' has invalid cost ({action.Cost})");
                }
                
                var effects = action.GetEffects();
                if (effects == null || effects.Count == 0)
                {
                    issues.Add($"Action '{so.name}' has no effects defined");
                }
            }
            
            return issues;
        }
        
        /// <summary>
        /// Create default actions for demonstration
        /// </summary>
        [ContextMenu("Create Default Actions")]
        public void CreateDefaultActions()
        {
            scriptableActions.Clear();
            
            // Note: In a real implementation, these would be created as asset files
            // This is just for demonstration of the structure
            Debug.Log("ActionDatabase: Use GOAP > Setup > Create Action Database to create default actions as asset files");
            UpdateDatabaseInfo();
        }
        
        /// <summary>
        /// Update database metadata
        /// </summary>
        private void UpdateDatabaseInfo()
        {
            totalActions = scriptableActions.Count;
            lastUpdated = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        
        /// <summary>
        /// Called when the asset is validated in the editor
        /// </summary>
        private void OnValidate()
        {
            UpdateDatabaseInfo();
        }
    }
}