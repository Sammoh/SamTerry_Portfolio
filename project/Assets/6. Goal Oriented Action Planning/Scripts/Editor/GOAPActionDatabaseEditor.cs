using UnityEngine;
using UnityEditor;
using System.IO;

namespace Sammoh.GOAP.Editor
{
    /// <summary>
    /// Unity Editor tools for creating and managing GOAP ActionDatabase
    /// </summary>
    public static class GOAPActionDatabaseEditor
    {
        private const string RESOURCES_PATH = "Assets/6. Goal Oriented Action Planning/Resources";
        private const string ACTION_DATABASE_PATH = RESOURCES_PATH + "/ActionDatabase.asset";
        private const string ACTIONS_PATH = RESOURCES_PATH + "/Actions";
        
        [MenuItem("GOAP/Setup/Create Action Database", false, 1)]
        public static void CreateActionDatabase()
        {
            // Ensure Resources directory exists
            if (!AssetDatabase.IsValidFolder(RESOURCES_PATH))
            {
                var basePath = "Assets/6. Goal Oriented Action Planning";
                if (!AssetDatabase.IsValidFolder(basePath))
                {
                    Debug.LogError("GOAP folder not found! Expected at: " + basePath);
                    return;
                }
                AssetDatabase.CreateFolder(basePath, "Resources");
            }
            
            // Ensure Actions subdirectory exists
            if (!AssetDatabase.IsValidFolder(ACTIONS_PATH))
            {
                AssetDatabase.CreateFolder(RESOURCES_PATH, "Actions");
            }
            
            // Create or load existing ActionDatabase
            var actionDatabase = AssetDatabase.LoadAssetAtPath<ActionDatabase>(ACTION_DATABASE_PATH);
            if (actionDatabase == null)
            {
                actionDatabase = ScriptableObject.CreateInstance<ActionDatabase>();
                AssetDatabase.CreateAsset(actionDatabase, ACTION_DATABASE_PATH);
                Debug.Log($"Created new ActionDatabase at {ACTION_DATABASE_PATH}");
            }
            else
            {
                Debug.Log($"ActionDatabase already exists at {ACTION_DATABASE_PATH}");
            }
            
            // Create default action assets
            CreateDefaultActionAssets(actionDatabase);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            // Select the ActionDatabase in the Project window
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = actionDatabase;
            
            Debug.Log("ActionDatabase setup complete! Check the Inspector to configure actions.");
        }
        
        [MenuItem("GOAP/Setup/Validate Action Database", false, 2)]
        public static void ValidateActionDatabase()
        {
            var actionDatabase = Resources.Load<ActionDatabase>("ActionDatabase");
            if (actionDatabase == null)
            {
                Debug.LogWarning("ActionDatabase not found in Resources folder. Use 'GOAP > Setup > Create Action Database' first.");
                return;
            }
            
            var issues = GOAPValidation.ValidateActionDatabase(actionDatabase);
            GOAPValidation.LogValidationResults(issues, "Action Database Validation");
            
            if (issues.Count == 0)
            {
                Debug.Log("✅ ActionDatabase validation passed!");
            }
            else
            {
                Debug.LogWarning($"⚠️ ActionDatabase has {issues.Count} validation issues. See console for details.");
            }
        }
        
        [MenuItem("GOAP/Setup/Create Default Actions", false, 3)]
        public static void CreateDefaultActions()
        {
            var actionDatabase = Resources.Load<ActionDatabase>("ActionDatabase");
            if (actionDatabase == null)
            {
                Debug.LogError("ActionDatabase not found! Use 'GOAP > Setup > Create Action Database' first.");
                return;
            }
            
            CreateDefaultActionAssets(actionDatabase);
            EditorUtility.SetDirty(actionDatabase);
            AssetDatabase.SaveAssets();
            
            Debug.Log("Default actions created and added to ActionDatabase");
        }
        
        private static void CreateDefaultActionAssets(ActionDatabase actionDatabase)
        {
            // Create default action ScriptableObjects
            var actionConfigs = new[]
            {
                new { Type = typeof(EatActionSO), Name = "EatAction" },
                new { Type = typeof(DrinkActionSO), Name = "DrinkAction" },
                new { Type = typeof(SleepActionSO), Name = "SleepAction" },
                new { Type = typeof(PlayActionSO), Name = "PlayAction" }
            };
            
            foreach (var config in actionConfigs)
            {
                var assetPath = $"{ACTIONS_PATH}/{config.Name}.asset";
                var existingAsset = AssetDatabase.LoadAssetAtPath(assetPath, config.Type);
                
                if (existingAsset == null)
                {
                    var newAction = ScriptableObject.CreateInstance(config.Type);
                    AssetDatabase.CreateAsset(newAction, assetPath);
                    
                    // Add to database if not already present
                    var actionSO = newAction as ScriptableObject;
                    if (actionSO != null && !actionDatabase.Actions.Contains(actionSO as IAction))
                    {
                        actionDatabase.AddAction(actionSO);
                    }
                    
                    Debug.Log($"Created {config.Name} at {assetPath}");
                }
                else
                {
                    // Make sure existing action is in the database
                    var actionSO = existingAsset as ScriptableObject;
                    if (actionSO != null && !actionDatabase.Actions.Contains(actionSO as IAction))
                    {
                        actionDatabase.AddAction(actionSO);
                        Debug.Log($"Added existing {config.Name} to ActionDatabase");
                    }
                }
            }
        }
        
        [MenuItem("GOAP/Setup/Clear Action Database", false, 11)]
        public static void ClearActionDatabase()
        {
            var actionDatabase = Resources.Load<ActionDatabase>("ActionDatabase");
            if (actionDatabase == null)
            {
                Debug.LogWarning("ActionDatabase not found in Resources folder.");
                return;
            }
            
            if (EditorUtility.DisplayDialog("Clear Action Database", 
                "This will remove all actions from the ActionDatabase. Are you sure?", 
                "Yes", "Cancel"))
            {
                actionDatabase.ClearAll();
                EditorUtility.SetDirty(actionDatabase);
                AssetDatabase.SaveAssets();
                Debug.Log("ActionDatabase cleared");
            }
        }
    }
}