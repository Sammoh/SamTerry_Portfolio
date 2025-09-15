using UnityEngine;
using UnityEditor;
using Sammoh.GOAP;

namespace Sammoh.GOAP.Editor
{
    /// <summary>
    /// Editor utility to create and configure ActionDatabase with default actions
    /// </summary>
    public static class ActionDatabaseSetup
    {
        [MenuItem("GOAP/Setup/Create Default Action Database")]
        public static void CreateDefaultActionDatabase()
        {
            // Create ActionDatabase
            var actionDatabase = ScriptableObject.CreateInstance<ActionDatabase>();
            
            // Create need reduction actions
            var eatAction = ScriptableObject.CreateInstance<EatActionSO>();
            var drinkAction = ScriptableObject.CreateInstance<DrinkActionSO>();
            var playAction = ScriptableObject.CreateInstance<PlayActionSO>();
            var sleepAction = ScriptableObject.CreateInstance<SleepActionSO>();
            
            // Save individual action assets
            AssetDatabase.CreateAsset(eatAction, "Assets/6. Goal Oriented Action Planning/Resources/EatAction.asset");
            AssetDatabase.CreateAsset(drinkAction, "Assets/6. Goal Oriented Action Planning/Resources/DrinkAction.asset");
            AssetDatabase.CreateAsset(playAction, "Assets/6. Goal Oriented Action Planning/Resources/PlayAction.asset");
            AssetDatabase.CreateAsset(sleepAction, "Assets/6. Goal Oriented Action Planning/Resources/SleepAction.asset");
            
            // Use reflection to set the private fields of ActionDatabase
            var needReductionActionsField = typeof(ActionDatabase).GetField("needReductionActions", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var moveToActionsField = typeof(ActionDatabase).GetField("moveToActions", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (needReductionActionsField != null)
            {
                var needReductionActions = new System.Collections.Generic.List<NeedReductionActionSO>
                {
                    eatAction, drinkAction, playAction, sleepAction
                };
                needReductionActionsField.SetValue(actionDatabase, needReductionActions);
            }
            
            if (moveToActionsField != null)
            {
                var existingMoveToActions = new System.Collections.Generic.List<MoveToActionSO>();
                
                // Load existing MoveToActionSO assets
                var moveToFood = AssetDatabase.LoadAssetAtPath<MoveToActionSO>("Assets/6. Goal Oriented Action Planning/Resources/MoveToFood.asset");
                var moveToWater = AssetDatabase.LoadAssetAtPath<MoveToActionSO>("Assets/6. Goal Oriented Action Planning/Resources/MoveToWater.asset");
                var moveToBed = AssetDatabase.LoadAssetAtPath<MoveToActionSO>("Assets/6. Goal Oriented Action Planning/Resources/MoveToBed.asset");
                var moveToToy = AssetDatabase.LoadAssetAtPath<MoveToActionSO>("Assets/6. Goal Oriented Action Planning/Resources/MoveToToy.asset");
                
                if (moveToFood != null) existingMoveToActions.Add(moveToFood);
                if (moveToWater != null) existingMoveToActions.Add(moveToWater);
                if (moveToBed != null) existingMoveToActions.Add(moveToBed);
                if (moveToToy != null) existingMoveToActions.Add(moveToToy);
                
                moveToActionsField.SetValue(actionDatabase, existingMoveToActions);
            }
            
            // Save ActionDatabase
            AssetDatabase.CreateAsset(actionDatabase, "Assets/6. Goal Oriented Action Planning/Resources/ActionDatabase.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("ActionDatabase created successfully with 4 need reduction actions and existing movement actions");
        }
        
        [MenuItem("GOAP/Setup/Validate Action Database")]
        public static void ValidateActionDatabase()
        {
            var actionDatabase = AssetDatabase.LoadAssetAtPath<ActionDatabase>("Assets/6. Goal Oriented Action Planning/Resources/ActionDatabase.asset");
            
            if (actionDatabase == null)
            {
                Debug.LogError("ActionDatabase not found. Create it first using 'GOAP/Setup/Create Default Action Database'");
                return;
            }
            
            var issues = GOAPValidation.ValidateActionDatabase(actionDatabase);
            GOAPValidation.LogValidationResults(issues, "ActionDatabase Validation");
            
            var allActions = actionDatabase.GetAllActions();
            Debug.Log($"ActionDatabase contains {allActions.Count} actions:");
            foreach (var action in allActions)
            {
                Debug.Log($"- {action.ActionType} (Cost: {action.Cost})");
            }
        }
    }
}