using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Sammoh.Two
{
    /// <summary>
    /// Custom editor window for managing equipment assets and database.
    /// Provides an intuitive UI for generation, selection, and asset management.
    /// </summary>
    public class EquipmentManagerWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private EquipmentDatabase database;
        private List<Equipment> allEquipment = new List<Equipment>();
        private Dictionary<Equipment, bool> selectedEquipment = new Dictionary<Equipment, bool>();
        
        // Filter options
        private EquipmentType filterType = EquipmentType.Weapon;
        private EquipmentRarity filterRarity = EquipmentRarity.Common;
        private bool showAllTypes = true;
        private bool showAllRarities = true;
        private string searchQuery = "";
        
        // Generation options
        private EquipmentType generateType = EquipmentType.Weapon;
        private EquipmentRarity generateRarity = EquipmentRarity.Common;
        private string customName = "";
        private bool regenerateSelected = false;
        
        // UI state
        private bool showGenerationSection = true;
        private bool showFilterSection = true;
        private bool showDatabaseSection = true;
        private bool showStatisticsSection = false;
        
        [MenuItem("Tools/Equipment/Equipment Manager", false, 1)]
        public static void ShowWindow()
        {
            var window = GetWindow<EquipmentManagerWindow>("Equipment Manager");
            window.minSize = new Vector2(400, 600);
            window.Show();
        }
        
        [MenuItem("Tools/Equipment/Generate Default Equipment", false, 2)]
        public static void GenerateDefaultEquipment()
        {
            EquipmentGenerator.GenerateDefaultEquipment();
        }
        
        [MenuItem("Tools/Equipment/Refresh Database", false, 3)]
        public static void RefreshDatabase()
        {
            var database = EquipmentGenerator.GetOrCreateEquipmentDatabase();
            database.RefreshDatabase();
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            Debug.Log("Equipment database refreshed!");
        }
        
        private void OnEnable()
        {
            RefreshEquipmentList();
        }
        
        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Equipment Asset Manager", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            DrawGenerationSection();
            EditorGUILayout.Space();
            
            DrawFilterSection();
            EditorGUILayout.Space();
            
            DrawDatabaseSection();
            EditorGUILayout.Space();
            
            DrawStatisticsSection();
            EditorGUILayout.Space();
            
            DrawEquipmentList();
        }
        
        private void DrawGenerationSection()
        {
            showGenerationSection = EditorGUILayout.Foldout(showGenerationSection, "Equipment Generation", true);
            if (!showGenerationSection) return;
            
            EditorGUI.indentLevel++;
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate All Defaults", GUILayout.Height(30)))
            {
                EquipmentGenerator.GenerateDefaultEquipment();
                RefreshEquipmentList();
            }
            if (GUILayout.Button("Refresh List", GUILayout.Height(30)))
            {
                RefreshEquipmentList();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Create Single Equipment:", EditorStyles.boldLabel);
            
            generateType = (EquipmentType)EditorGUILayout.EnumPopup("Type", generateType);
            generateRarity = (EquipmentRarity)EditorGUILayout.EnumPopup("Rarity", generateRarity);
            customName = EditorGUILayout.TextField("Custom Name (optional)", customName);
            
            if (GUILayout.Button("Generate Equipment", GUILayout.Height(25)))
            {
                var equipment = EquipmentGenerator.GenerateEquipment(generateType, generateRarity, customName);
                if (equipment != null)
                {
                    RefreshEquipmentList();
                    Selection.activeObject = equipment;
                }
            }
            
            EditorGUI.indentLevel--;
        }
        
        private void DrawFilterSection()
        {
            showFilterSection = EditorGUILayout.Foldout(showFilterSection, "Filters", true);
            if (!showFilterSection) return;
            
            EditorGUI.indentLevel++;
            
            searchQuery = EditorGUILayout.TextField("Search", searchQuery);
            
            EditorGUILayout.BeginHorizontal();
            showAllTypes = EditorGUILayout.Toggle("All Types", showAllTypes);
            if (!showAllTypes)
                filterType = (EquipmentType)EditorGUILayout.EnumPopup(filterType);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            showAllRarities = EditorGUILayout.Toggle("All Rarities", showAllRarities);
            if (!showAllRarities)
                filterRarity = (EquipmentRarity)EditorGUILayout.EnumPopup(filterRarity);
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.indentLevel--;
        }
        
        private void DrawDatabaseSection()
        {
            showDatabaseSection = EditorGUILayout.Foldout(showDatabaseSection, "Database Management", true);
            if (!showDatabaseSection) return;
            
            EditorGUI.indentLevel++;
            
            if (database == null)
                database = EquipmentGenerator.GetOrCreateEquipmentDatabase();
            
            EditorGUI.BeginDisabledGroup(true);
            database = (EquipmentDatabase)EditorGUILayout.ObjectField("Database", database, typeof(EquipmentDatabase), false);
            EditorGUI.EndDisabledGroup();
            
            if (database != null)
            {
                EditorGUILayout.LabelField($"Total Equipment: {database.TotalEquipmentCount}");
                EditorGUILayout.LabelField($"Last Updated: {database.LastUpdated}");
            }
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Update Database"))
            {
                if (database != null)
                {
                    database.RefreshDatabase();
                    EditorUtility.SetDirty(database);
                    AssetDatabase.SaveAssets();
                    RefreshEquipmentList();
                }
            }
            if (GUILayout.Button("Select Database"))
            {
                Selection.activeObject = database;
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.indentLevel--;
        }
        
        private void DrawStatisticsSection()
        {
            showStatisticsSection = EditorGUILayout.Foldout(showStatisticsSection, "Statistics", true);
            if (!showStatisticsSection) return;
            
            EditorGUI.indentLevel++;
            
            if (database != null)
            {
                var stats = database.GetStatistics();
                EditorGUILayout.LabelField($"Weapons: {stats.WeaponCount}");
                EditorGUILayout.LabelField($"Accessories: {stats.AccessoryCount}");
                EditorGUILayout.LabelField($"Armor: {stats.ArmorCount}");
                
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("By Rarity:", EditorStyles.boldLabel);
                foreach (var kvp in stats.CountByRarity)
                {
                    EditorGUILayout.LabelField($"{kvp.Key}: {kvp.Value}");
                }
            }
            
            EditorGUI.indentLevel--;
        }
        
        private void DrawEquipmentList()
        {
            EditorGUILayout.LabelField($"Equipment Assets ({GetFilteredEquipment().Count})", EditorStyles.boldLabel);
            
            // Selection controls
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select All Visible"))
            {
                foreach (var equipment in GetFilteredEquipment())
                {
                    selectedEquipment[equipment] = true;
                }
            }
            if (GUILayout.Button("Deselect All"))
            {
                selectedEquipment.Clear();
            }
            EditorGUILayout.EndHorizontal();
            
            // Batch operations
            int selectedCount = selectedEquipment.Count(kvp => kvp.Value);
            if (selectedCount > 0)
            {
                EditorGUILayout.LabelField($"Selected: {selectedCount} items", EditorStyles.helpBox);
                
                EditorGUILayout.BeginHorizontal();
                regenerateSelected = EditorGUILayout.Toggle("Regenerate Values", regenerateSelected);
                if (GUILayout.Button($"Update Selected ({selectedCount})"))
                {
                    UpdateSelectedEquipment();
                }
                EditorGUILayout.EndHorizontal();
            }
            
            // Equipment list
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            var filteredEquipment = GetFilteredEquipment();
            if (filteredEquipment.Count == 0)
            {
                EditorGUILayout.HelpBox("No equipment found matching the current filters.", MessageType.Info);
            }
            else
            {
                foreach (var equipment in filteredEquipment)
                {
                    DrawEquipmentItem(equipment);
                }
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        private void DrawEquipmentItem(Equipment equipment)
        {
            if (equipment == null) return;
            
            EditorGUILayout.BeginHorizontal("box");
            
            // Selection checkbox
            bool isSelected = selectedEquipment.GetValueOrDefault(equipment, false);
            bool newSelected = EditorGUILayout.Toggle(isSelected, GUILayout.Width(20));
            if (newSelected != isSelected)
            {
                selectedEquipment[equipment] = newSelected;
            }
            
            // Equipment info
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.BeginHorizontal();
            Color rarityColor = GetRarityColor(equipment.Rarity);
            var originalColor = GUI.color;
            GUI.color = rarityColor;
            EditorGUILayout.LabelField(equipment.EquipmentName, EditorStyles.boldLabel);
            GUI.color = originalColor;
            EditorGUILayout.LabelField($"({equipment.Type})", GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.LabelField($"ID: {equipment.Id} | Rarity: {equipment.Rarity} | Level: {equipment.Level}");
            
            EditorGUILayout.EndVertical();
            
            // Action buttons
            EditorGUILayout.BeginVertical(GUILayout.Width(100));
            if (GUILayout.Button("Select", GUILayout.Height(20)))
            {
                Selection.activeObject = equipment;
            }
            if (GUILayout.Button("Update", GUILayout.Height(20)))
            {
                EquipmentGenerator.UpdateEquipment(equipment, true);
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndHorizontal();
        }
        
        private List<Equipment> GetFilteredEquipment()
        {
            var filtered = allEquipment.Where(e => e != null);
            
            // Apply search filter
            if (!string.IsNullOrEmpty(searchQuery))
            {
                filtered = filtered.Where(e => e.EquipmentName.ToLower().Contains(searchQuery.ToLower()) ||
                                             e.Id.ToLower().Contains(searchQuery.ToLower()));
            }
            
            // Apply type filter
            if (!showAllTypes)
            {
                filtered = filtered.Where(e => e.Type == filterType);
            }
            
            // Apply rarity filter
            if (!showAllRarities)
            {
                filtered = filtered.Where(e => e.Rarity == filterRarity);
            }
            
            return filtered.OrderBy(e => e.Type).ThenBy(e => e.Rarity).ThenBy(e => e.EquipmentName).ToList();
        }
        
        private void UpdateSelectedEquipment()
        {
            foreach (var kvp in selectedEquipment)
            {
                if (kvp.Value && kvp.Key != null)
                {
                    EquipmentGenerator.UpdateEquipment(kvp.Key, regenerateSelected);
                }
            }
            
            AssetDatabase.SaveAssets();
            selectedEquipment.Clear();
            regenerateSelected = false;
        }
        
        private void RefreshEquipmentList()
        {
            allEquipment = EquipmentGenerator.GetAllExistingEquipment();
            database = EquipmentGenerator.GetOrCreateEquipmentDatabase();
            selectedEquipment.Clear();
        }
        
        private Color GetRarityColor(EquipmentRarity rarity)
        {
            return rarity switch
            {
                EquipmentRarity.Common => Color.white,
                EquipmentRarity.Uncommon => Color.green,
                EquipmentRarity.Rare => Color.blue,
                EquipmentRarity.Epic => new Color(0.6f, 0.2f, 0.8f), // Purple
                EquipmentRarity.Legendary => Color.yellow,
                _ => Color.white
            };
        }
    }
}
#endif