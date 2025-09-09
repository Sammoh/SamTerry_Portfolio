using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Sammoh.TurnBasedStrategy.Editor
{
    /// <summary>
    /// Custom Unity Editor tool for creating and managing equipment
    /// </summary>
    public class EquipmentEditor : EditorWindow
    {
        private EquipmentDatabase database;
        private Character previewCharacter;
        
        // Equipment creation fields
        private string equipmentName = "";
        private EquipmentSlot selectedSlot = EquipmentSlot.Weapon;
        private string description = "";
        private List<StatModifier> modifiers = new List<StatModifier>();
        
        // UI state
        private Vector2 scrollPosition;
        private bool showCreatePanel = true;
        private bool showPreviewPanel = true;
        private bool showDatabasePanel = true;
        
        // Temporary modifier for adding new ones
        private StatType newModifierStat = StatType.Attack;
        private float newModifierValue = 0f;
        private ModifierType newModifierType = ModifierType.Additive;

        [MenuItem("Tools/Turn-Based Strategy/Equipment Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<EquipmentEditor>("Equipment Editor");
            window.minSize = new Vector2(400, 600);
        }

        private void OnEnable()
        {
            // Try to find existing equipment database
            FindEquipmentDatabase();
            
            // Initialize modifiers list
            if (modifiers == null)
                modifiers = new List<StatModifier>();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.LabelField("Equipment Editor", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            DrawDatabaseSection();
            EditorGUILayout.Space();

            DrawCreateEquipmentSection();
            EditorGUILayout.Space();

            DrawPreviewSection();

            EditorGUILayout.EndScrollView();
        }

        private void DrawDatabaseSection()
        {
            showDatabasePanel = EditorGUILayout.Foldout(showDatabasePanel, "Equipment Database", true);
            
            if (showDatabasePanel)
            {
                EditorGUILayout.BeginVertical("box");

                // Database field
                EquipmentDatabase newDatabase = (EquipmentDatabase)EditorGUILayout.ObjectField(
                    "Equipment Database", database, typeof(EquipmentDatabase), false);
                
                if (newDatabase != database)
                {
                    database = newDatabase;
                }

                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Create New Database"))
                {
                    CreateNewDatabase();
                }
                
                if (GUILayout.Button("Load Default Equipment") && database != null)
                {
                    database.CreateDefaultEquipment();
                    EditorUtility.SetDirty(database);
                    AssetDatabase.SaveAssets();
                }
                
                EditorGUILayout.EndHorizontal();

                // Show database contents
                if (database != null)
                {
                    EditorGUILayout.LabelField($"Weapons: {database.Weapons.Length}");
                    EditorGUILayout.LabelField($"Armor: {database.Armor.Length}");
                    EditorGUILayout.LabelField($"Accessories: {database.Accessories.Length}");
                }

                EditorGUILayout.EndVertical();
            }
        }

        private void DrawCreateEquipmentSection()
        {
            showCreatePanel = EditorGUILayout.Foldout(showCreatePanel, "Create New Equipment", true);
            
            if (showCreatePanel)
            {
                EditorGUILayout.BeginVertical("box");

                // Basic equipment info
                equipmentName = EditorGUILayout.TextField("Equipment Name", equipmentName);
                selectedSlot = (EquipmentSlot)EditorGUILayout.EnumPopup("Equipment Slot", selectedSlot);
                description = EditorGUILayout.TextArea(description, GUILayout.Height(60));

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Stat Modifiers", EditorStyles.boldLabel);

                // Add new modifier section
                EditorGUILayout.BeginHorizontal();
                newModifierStat = (StatType)EditorGUILayout.EnumPopup(newModifierStat, GUILayout.Width(100));
                newModifierValue = EditorGUILayout.FloatField(newModifierValue, GUILayout.Width(60));
                newModifierType = (ModifierType)EditorGUILayout.EnumPopup(newModifierType, GUILayout.Width(100));
                
                if (GUILayout.Button("Add", GUILayout.Width(50)))
                {
                    modifiers.Add(new StatModifier(newModifierStat, newModifierValue, newModifierType));
                    newModifierValue = 0f;
                }
                EditorGUILayout.EndHorizontal();

                // Display current modifiers
                for (int i = 0; i < modifiers.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(modifiers[i].ToString());
                    if (GUILayout.Button("Remove", GUILayout.Width(60)))
                    {
                        modifiers.RemoveAt(i);
                        i--;
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Space();

                // Create button
                GUI.enabled = !string.IsNullOrEmpty(equipmentName) && database != null;
                if (GUILayout.Button("Create Equipment"))
                {
                    CreateEquipment();
                }
                GUI.enabled = true;

                EditorGUILayout.EndVertical();
            }
        }

        private void DrawPreviewSection()
        {
            showPreviewPanel = EditorGUILayout.Foldout(showPreviewPanel, "Preview on Character", true);
            
            if (showPreviewPanel)
            {
                EditorGUILayout.BeginVertical("box");

                previewCharacter = (Character)EditorGUILayout.ObjectField(
                    "Preview Character", previewCharacter, typeof(Character), true);

                if (previewCharacter != null && modifiers.Count > 0)
                {
                    EditorGUILayout.LabelField("Stat Changes Preview:", EditorStyles.boldLabel);
                    
                    var stats = previewCharacter.Stats;
                    var tempEquipment = CreateInstance<Equipment>();
                    tempEquipment.Initialize(equipmentName, selectedSlot, modifiers.ToArray(), description);
                    
                    // Show stat changes
                    foreach (StatType statType in System.Enum.GetValues(typeof(StatType)))
                    {
                        float baseValue = GetBaseStat(stats, statType);
                        float modifiedValue = tempEquipment.GetModifiedValue(statType, baseValue);
                        
                        if (Mathf.Abs(modifiedValue - baseValue) > 0.01f)
                        {
                            float change = modifiedValue - baseValue;
                            string changeText = change > 0 ? $"+{change:F1}" : $"{change:F1}";
                            EditorGUILayout.LabelField($"{statType}: {baseValue:F0} → {modifiedValue:F0} ({changeText})");
                        }
                    }
                }
                else if (previewCharacter == null)
                {
                    EditorGUILayout.HelpBox("Select a character to preview stat changes", MessageType.Info);
                }

                EditorGUILayout.EndVertical();
            }
        }

        private float GetBaseStat(CharacterStats stats, StatType statType)
        {
            switch (statType)
            {
                case StatType.MaxHealth: return stats.BaseMaxHealth;
                case StatType.Attack: return stats.BaseAttack;
                case StatType.Defense: return stats.BaseDefense;
                case StatType.Speed: return stats.BaseSpeed;
                case StatType.Mana: return stats.BaseMana;
                default: return 0f;
            }
        }

        private void FindEquipmentDatabase()
        {
            if (database == null)
            {
                string[] guids = AssetDatabase.FindAssets("t:EquipmentDatabase");
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    database = AssetDatabase.LoadAssetAtPath<EquipmentDatabase>(path);
                }
            }
        }

        private void CreateNewDatabase()
        {
            database = CreateInstance<EquipmentDatabase>();
            
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Equipment Database",
                "EquipmentDatabase",
                "asset",
                "Choose where to save the equipment database");
            
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(database, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void CreateEquipment()
        {
            if (string.IsNullOrEmpty(equipmentName) || database == null)
                return;

            // Create the equipment ScriptableObject
            var equipment = CreateInstance<Equipment>();
            equipment.Initialize(equipmentName, selectedSlot, modifiers.ToArray(), description);

            // Save as asset file
            string fileName = equipmentName.Replace(" ", "_") + "_Equipment.asset";
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Equipment Asset",
                fileName,
                "asset",
                $"Choose where to save {equipmentName}");

            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(equipment, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                // Add to database
                database.AddEquipment(equipment);
                EditorUtility.SetDirty(database);
                AssetDatabase.SaveAssets();

                Debug.Log($"Created equipment asset: {equipment.EquipmentName} at {path}");

                // Clear form
                equipmentName = "";
                description = "";
                modifiers.Clear();
            }
        }
    }
}