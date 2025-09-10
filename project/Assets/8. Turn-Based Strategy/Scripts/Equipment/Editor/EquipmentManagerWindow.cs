#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Sammoh.TurnBasedStrategy.Editor
{
    /// <summary>
    /// Equipment Manager Window for managing equipment assets in the Turn-Based Strategy system.
    /// Provides UI for generation, filtering, and batch operations.
    /// </summary>
    public class EquipmentManagerWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private string searchFilter = "";
        private EquipmentType selectedTypeFilter = EquipmentType.Weapon;
        private EquipmentRarity selectedRarityFilter = EquipmentRarity.Common;
        private bool showAllTypes = true;
        private bool showAllRarities = true;
        
        // Generation settings
        private WeaponType selectedWeaponType = WeaponType.Sword;
        private ArmorType selectedArmorType = ArmorType.Chestplate;
        private AccessoryType selectedAccessoryType = AccessoryType.Ring;
        private ArmorMaterial selectedArmorMaterial = ArmorMaterial.Leather;
        private string customName = "";
        
        // Database reference
        private EquipmentDatabase database;
        
        [MenuItem("Tools/Equipment/Equipment Manager")]
        public static void ShowWindow()
        {
            var window = GetWindow<EquipmentManagerWindow>("Equipment Manager");
            window.minSize = new Vector2(600, 400);
            window.Show();
        }
        
        private void OnEnable()
        {
            RefreshDatabase();
        }
        
        private void RefreshDatabase()
        {
            database = EquipmentGenerator.GetOrCreateEquipmentDatabase();
            if (database != null)
            {
                database.InitializeLookupTables();
            }
        }
        
        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            
            DrawHeader();
            DrawGenerationSection();
            DrawFilterSection();
            DrawEquipmentList();
            DrawDatabaseActions();
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawHeader()
        {
            EditorGUILayout.LabelField("Turn-Based Strategy Equipment Manager", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            if (database == null)
            {
                EditorGUILayout.HelpBox("Equipment Database not found. Click 'Create Database' to create one.", MessageType.Warning);
                if (GUILayout.Button("Create Database"))
                {
                    RefreshDatabase();
                }
                return;
            }
            
            var stats = database.GetStatistics();
            EditorGUILayout.LabelField($"Total Equipment: {stats.TotalCount} " +
                                     $"(Weapons: {stats.WeaponCount}, Armor: {stats.ArmorCount}, Accessories: {stats.AccessoryCount})");
            EditorGUILayout.Space();
        }
        
        private void DrawGenerationSection()
        {
            EditorGUILayout.LabelField("Equipment Generation", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate All Defaults"))
            {
                EquipmentGenerator.GenerateDefaultEquipment();
                RefreshDatabase();
            }
            
            if (GUILayout.Button("Refresh Database"))
            {
                EquipmentGenerator.RefreshEquipmentDatabase();
                RefreshDatabase();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // Individual generation
            EditorGUILayout.LabelField("Generate Individual Equipment:", EditorStyles.miniBoldLabel);
            
            customName = EditorGUILayout.TextField("Custom Name (optional):", customName);
            selectedRarityFilter = (EquipmentRarity)EditorGUILayout.EnumPopup("Rarity:", selectedRarityFilter);
            
            EditorGUILayout.BeginHorizontal();
            
            // Weapon generation
            EditorGUILayout.BeginVertical(GUILayout.Width(180));
            EditorGUILayout.LabelField("Weapon", EditorStyles.centeredGreyMiniLabel);
            selectedWeaponType = (WeaponType)EditorGUILayout.EnumPopup(selectedWeaponType);
            if (GUILayout.Button("Generate Weapon"))
            {
                EquipmentGenerator.GenerateWeapon(selectedWeaponType, selectedRarityFilter, 
                    string.IsNullOrEmpty(customName) ? null : customName);
                RefreshDatabase();
            }
            EditorGUILayout.EndVertical();
            
            // Armor generation
            EditorGUILayout.BeginVertical(GUILayout.Width(180));
            EditorGUILayout.LabelField("Armor", EditorStyles.centeredGreyMiniLabel);
            selectedArmorType = (ArmorType)EditorGUILayout.EnumPopup(selectedArmorType);
            selectedArmorMaterial = (ArmorMaterial)EditorGUILayout.EnumPopup(selectedArmorMaterial);
            if (GUILayout.Button("Generate Armor"))
            {
                EquipmentGenerator.GenerateArmor(selectedArmorType, selectedRarityFilter, selectedArmorMaterial,
                    string.IsNullOrEmpty(customName) ? null : customName);
                RefreshDatabase();
            }
            EditorGUILayout.EndVertical();
            
            // Accessory generation
            EditorGUILayout.BeginVertical(GUILayout.Width(180));
            EditorGUILayout.LabelField("Accessory", EditorStyles.centeredGreyMiniLabel);
            selectedAccessoryType = (AccessoryType)EditorGUILayout.EnumPopup(selectedAccessoryType);
            if (GUILayout.Button("Generate Accessory"))
            {
                EquipmentGenerator.GenerateAccessory(selectedAccessoryType, selectedRarityFilter,
                    string.IsNullOrEmpty(customName) ? null : customName);
                RefreshDatabase();
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
        
        private void DrawFilterSection()
        {
            EditorGUILayout.LabelField("Filters", EditorStyles.boldLabel);
            
            searchFilter = EditorGUILayout.TextField("Search:", searchFilter);
            
            EditorGUILayout.BeginHorizontal();
            showAllTypes = EditorGUILayout.Toggle("All Types", showAllTypes);
            if (!showAllTypes)
            {
                selectedTypeFilter = (EquipmentType)EditorGUILayout.EnumPopup("Type:", selectedTypeFilter);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            showAllRarities = EditorGUILayout.Toggle("All Rarities", showAllRarities);
            if (!showAllRarities)
            {
                selectedRarityFilter = (EquipmentRarity)EditorGUILayout.EnumPopup("Rarity:", selectedRarityFilter);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
        }
        
        private void DrawEquipmentList()
        {
            if (database == null) return;
            
            EditorGUILayout.LabelField("Equipment Assets", EditorStyles.boldLabel);
            
            var allEquipment = database.GetAllEquipment();
            var filteredEquipment = FilterEquipment(allEquipment);
            
            EditorGUILayout.LabelField($"Showing {filteredEquipment.Count} of {allEquipment.Count} items");
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
            
            foreach (var equipment in filteredEquipment)
            {
                DrawEquipmentItem(equipment);
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        private void DrawEquipmentItem(EquipmentSO equipment)
        {
            EditorGUILayout.BeginHorizontal();
            
            // Equipment info
            EditorGUILayout.LabelField($"{equipment.EquipmentName} ({equipment.Type})", GUILayout.Width(200));
            EditorGUILayout.LabelField($"{equipment.Rarity}", GUILayout.Width(80));
            
            // Show type-specific info
            string typeInfo = equipment switch
            {
                WeaponSO weapon => $"Dmg: {weapon.Damage}, DPS: {weapon.CalculateDPS():F1}",
                ArmorSO armor => $"Def: {armor.Defense}, MRes: {armor.MagicResistance}",
                AccessorySO accessory => $"Mods: {accessory.StatModifiers.Length}, Stack: {accessory.MaxStackSize}",
                _ => "Unknown"
            };
            EditorGUILayout.LabelField(typeInfo, GUILayout.Width(120));
            
            // Actions
            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                Selection.activeObject = equipment;
                EditorGUIUtility.PingObject(equipment);
            }
            
            if (GUILayout.Button("Regenerate", GUILayout.Width(80)))
            {
                RegenerateEquipment(equipment);
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private List<EquipmentSO> FilterEquipment(List<EquipmentSO> equipment)
        {
            var filtered = equipment.AsEnumerable();
            
            // Search filter
            if (!string.IsNullOrEmpty(searchFilter))
            {
                filtered = filtered.Where(e => e.EquipmentName.ToLower().Contains(searchFilter.ToLower()));
            }
            
            // Type filter
            if (!showAllTypes)
            {
                filtered = filtered.Where(e => e.Type == selectedTypeFilter);
            }
            
            // Rarity filter
            if (!showAllRarities)
            {
                filtered = filtered.Where(e => e.Rarity == selectedRarityFilter);
            }
            
            return filtered.ToList();
        }
        
        private void RegenerateEquipment(EquipmentSO equipment)
        {
            switch (equipment)
            {
                case WeaponSO weapon:
                    EquipmentGenerator.GenerateWeapon(weapon.WeaponType, weapon.Rarity, weapon.EquipmentName);
                    break;
                case ArmorSO armor:
                    EquipmentGenerator.GenerateArmor(armor.ArmorType, armor.Rarity, armor.ArmorMaterial, armor.EquipmentName);
                    break;
                case AccessorySO accessory:
                    EquipmentGenerator.GenerateAccessory(accessory.AccessoryType, accessory.Rarity, accessory.EquipmentName);
                    break;
            }
            
            RefreshDatabase();
        }
        
        private void DrawDatabaseActions()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Database Actions", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Validate Database"))
            {
                ValidateDatabase();
            }
            
            if (GUILayout.Button("Clear All Equipment"))
            {
                if (EditorUtility.DisplayDialog("Clear Equipment", 
                    "Are you sure you want to delete all equipment assets? This cannot be undone!", 
                    "Yes", "Cancel"))
                {
                    ClearAllEquipment();
                }
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void ValidateDatabase()
        {
            if (database == null)
            {
                Debug.LogError("No database found!");
                return;
            }
            
            database.RefreshDatabase();
            var stats = database.GetStatistics();
            
            Debug.Log($"Database Validation Complete:\n" +
                     $"- Total Items: {stats.TotalCount}\n" +
                     $"- Weapons: {stats.WeaponCount}\n" +
                     $"- Armor: {stats.ArmorCount}\n" +
                     $"- Accessories: {stats.AccessoryCount}\n" +
                     $"- By Rarity: Common({stats.CommonCount}), Uncommon({stats.UncommonCount}), " +
                     $"Rare({stats.RareCount}), Epic({stats.EpicCount}), Legendary({stats.LegendaryCount})");
        }
        
        private void ClearAllEquipment()
        {
            // This is a destructive operation - implementation would delete all equipment assets
            Debug.LogWarning("Clear All Equipment not implemented for safety. Use AssetDatabase operations manually.");
        }
    }
}
#endif