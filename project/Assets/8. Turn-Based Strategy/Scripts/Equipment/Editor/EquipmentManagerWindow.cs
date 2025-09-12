#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Sammoh.Two
{
    /// <summary>
    /// Equipment Manager with StatModifiers authoring & batch tools (aligned to Sammoh.Two model).
    /// </summary>
    public class EquipmentManagerWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private EquipmentDatabase database;
        private List<Equipment> allEquipment = new List<Equipment>();
        private readonly Dictionary<Equipment, bool> selectedEquipment = new Dictionary<Equipment, bool>();
        private readonly Dictionary<Equipment, bool> expandedInlineEditor = new Dictionary<Equipment, bool>();
        private readonly Dictionary<Equipment, ReorderableList> modifierLists = new Dictionary<Equipment, ReorderableList>();
        private readonly Dictionary<Equipment, ReorderableList> abilityLists = new Dictionary<Equipment, ReorderableList>();

        // Filters
        private EquipmentType filterType = EquipmentType.Weapon;
        private EquipmentRarity filterRarity = EquipmentRarity.Common;
        private bool showAllTypes = true;
        private bool showAllRarities = true;
        private string searchQuery = "";

        // Generation
        private EquipmentType generateType = EquipmentType.Weapon;
        private EquipmentRarity generateRarity = EquipmentRarity.Common;
        private string customName = "";
        private bool regenerateSelected = false;

        // UI state
        private bool showGenerationSection = true;
        private bool showFilterSection = true;
        private bool showDatabaseSection = true;
        private bool showStatisticsSection = false;
        private bool showBatchModifiers = true;

        // Batch modifier payload (Sammoh.Two enums)
        private StatType batchStatType = StatType.Health;
        private ModifierType batchModifierType = ModifierType.Flat; // Flat or Percentage
        private float batchValue = 0f;

        [MenuItem("Tools/Equipment/Equipment Manager", false, 1)]
        public static void ShowWindow()
        {
            var window = GetWindow<EquipmentManagerWindow>("Equipment Manager");
            window.minSize = new Vector2(520, 680);
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

        private void OnEnable() => RefreshEquipmentList();

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

            DrawBatchModifierSection(); // fixed path uses Sammoh.Two constructor ordering
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

        private void DrawBatchModifierSection()
        {
            showBatchModifiers = EditorGUILayout.Foldout(showBatchModifiers, "Batch Modifiers (Selected Items)", true);
            if (!showBatchModifiers) return;

            EditorGUI.indentLevel++;

            int selectedCount = selectedEquipment.Count(kvp => kvp.Value);
            EditorGUILayout.HelpBox(selectedCount > 0
                ? $"You have {selectedCount} equipment selected."
                : "Select equipment below to enable batch operations.", MessageType.Info);

            using (new EditorGUI.DisabledScope(selectedCount == 0))
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Add Modifier To Selected", EditorStyles.boldLabel);
                batchStatType = (StatType)EditorGUILayout.EnumPopup("Stat", batchStatType);
                batchModifierType = (ModifierType)EditorGUILayout.EnumPopup("Type", batchModifierType);
                batchValue = EditorGUILayout.FloatField("Value", batchValue);

                if (GUILayout.Button("Add Modifier"))
                {
                    AddModifierToSelected(batchStatType, batchModifierType, batchValue);
                }

                EditorGUILayout.Space();
                if (GUILayout.Button("Clear All Modifiers on Selected"))
                {
                    if (EditorUtility.DisplayDialog("Clear Modifiers",
                        "Remove ALL stat modifiers from the selected equipment assets?", "Yes", "No"))
                    {
                        ClearModifiersOnSelected();
                    }
                }
                EditorGUILayout.EndVertical();
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

            // Batch update
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

            // List
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

        private void DrawEquipmentItem(Equipment equipmentSo)
        {
            if (equipmentSo == null) return;

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();

            // Selection checkbox
            bool isSelected = selectedEquipment.GetValueOrDefault(equipmentSo, false);
            bool newSelected = GUILayout.Toggle(isSelected, GUIContent.none, GUILayout.Width(18));
            if (newSelected != isSelected)
                selectedEquipment[equipmentSo] = newSelected;

            // Name + type + rarity
            Color rarityColor = GetRarityColor(equipmentSo.Rarity);
            var originalColor = GUI.color;
            GUI.color = rarityColor;
            EditorGUILayout.LabelField(equipmentSo.EquipmentName, EditorStyles.boldLabel);
            GUI.color = originalColor;
            GUILayout.Space(6);
            EditorGUILayout.LabelField($"({equipmentSo.Type})", GUILayout.Width(80));

            GUILayout.FlexibleSpace();

            // Quick summary
            EditorGUILayout.LabelField(
                $"Lvl {equipmentSo.Level} • {equipmentSo.Rarity} • Slot {equipmentSo.Slot} • Mods {equipmentSo.StatModifiers?.Length ?? 0} • Abilities {equipmentSo.Abilities?.Length ?? 0}",
                GUILayout.MaxWidth(400));

            // Actions
            if (GUILayout.Button("Select", GUILayout.Height(20), GUILayout.Width(64)))
            {
                Selection.activeObject = equipmentSo;
            }
            if (GUILayout.Button("Update", GUILayout.Height(20), GUILayout.Width(64)))
            {
                EquipmentGenerator.UpdateEquipment(equipmentSo, true);
            }

            EditorGUILayout.EndHorizontal();

            // Inline editor
            bool expanded = expandedInlineEditor.GetValueOrDefault(equipmentSo, false);
            expanded = EditorGUILayout.Foldout(expanded, "Inline Edit: Slot, Stat Modifiers & Abilities", true);
            if (expanded) DrawInlineModifierEditor(equipmentSo);
            expandedInlineEditor[equipmentSo] = expanded;

            EditorGUILayout.EndVertical();
        }

        private void DrawInlineModifierEditor(Equipment equipmentSo)
        {
            var so = new SerializedObject(equipmentSo);
            var slotProp = so.FindProperty("slot");                  // protected in Equipment
            var modsProp = so.FindProperty("statModifiers");         // protected in Equipment (Two)
            var abilitiesProp = so.FindProperty("abilities");        // protected in Equipment

            EditorGUI.indentLevel++;

            // Slot
            EditorGUILayout.PropertyField(slotProp, new GUIContent("Equipment Slot"));

            EditorGUILayout.Space();

            // Reorderable list for StatModifiers (Two: fields are lowerCamel private: statType, value, modifierType)
            EnsureReorderableList(equipmentSo, so, modsProp);
            modifierLists[equipmentSo].DoLayoutList();

            EditorGUILayout.Space();

            // Reorderable list for Abilities 
            EnsureAbilityReorderableList(equipmentSo, so, abilitiesProp);
            abilityLists[equipmentSo].DoLayoutList();

            // Commit
            if (so.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(equipmentSo);
                AssetDatabase.SaveAssets();
            }

            EditorGUI.indentLevel--;
        }

        private void EnsureReorderableList(Equipment equipmentSo, SerializedObject so, SerializedProperty modsProp)
        {
            if (modifierLists.TryGetValue(equipmentSo, out var list) && list.serializedProperty == modsProp)
                return;

            var l = new ReorderableList(so, modsProp, true, true, true, true);

            l.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Stat Modifiers (Flat first, then Percentage)");
            };

            l.elementHeight = EditorGUIUtility.singleLineHeight * 3f + 10f;

            l.drawElementCallback = (rect, index, active, focused) =>
            {
                var element = modsProp.GetArrayElementAtIndex(index);
                rect.y += 2;

                // NOTE: Two model uses *private* serialized fields with lowerCamel names
                var statTypeProp   = element.FindPropertyRelative("statType");
                var modifierTypeProp = element.FindPropertyRelative("modifierType");
                var valueProp      = element.FindPropertyRelative("value");

                // Row 1: Stat
                var r1 = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(r1, statTypeProp, new GUIContent("Stat"));

                // Row 2: Type + Value
                var r2 = new Rect(rect.x, r1.yMax + 2, rect.width * 0.5f - 4, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(r2, modifierTypeProp, new GUIContent("Type"));
                var r2b = new Rect(rect.x + rect.width * 0.5f + 4, r1.yMax + 2, rect.width * 0.5f - 4, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(r2b, valueProp, new GUIContent("Value"));
            };

            modifierLists[equipmentSo] = l;
        }

        private void EnsureAbilityReorderableList(Equipment equipmentSo, SerializedObject so, SerializedProperty abilitiesProp)
        {
            if (abilityLists.TryGetValue(equipmentSo, out var list) && list.serializedProperty == abilitiesProp)
                return;

            var l = new ReorderableList(so, abilitiesProp, true, true, true, true);

            l.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Equipment Abilities");
            };

            l.elementHeight = EditorGUIUtility.singleLineHeight * 5f + 12f;

            l.drawElementCallback = (rect, index, active, focused) =>
            {
                var element = abilitiesProp.GetArrayElementAtIndex(index);
                rect.y += 2;

                // CharacterAbility fields: abilityName, abilityType, power, manaCost, description
                var abilityNameProp = element.FindPropertyRelative("abilityName");
                var abilityTypeProp = element.FindPropertyRelative("abilityType");
                var powerProp = element.FindPropertyRelative("power");
                var manaCostProp = element.FindPropertyRelative("manaCost");
                var descriptionProp = element.FindPropertyRelative("description");

                float lineHeight = EditorGUIUtility.singleLineHeight;
                float spacing = 2f;

                // Row 1: Ability Name
                var r1 = new Rect(rect.x, rect.y, rect.width, lineHeight);
                EditorGUI.PropertyField(r1, abilityNameProp, new GUIContent("Name"));

                // Row 2: Type and Power
                var r2a = new Rect(rect.x, r1.yMax + spacing, rect.width * 0.5f - 2, lineHeight);
                EditorGUI.PropertyField(r2a, abilityTypeProp, new GUIContent("Type"));
                var r2b = new Rect(rect.x + rect.width * 0.5f + 2, r1.yMax + spacing, rect.width * 0.5f - 2, lineHeight);
                EditorGUI.PropertyField(r2b, powerProp, new GUIContent("Power"));

                // Row 3: Mana Cost
                var r3 = new Rect(rect.x, r2a.yMax + spacing, rect.width, lineHeight);
                EditorGUI.PropertyField(r3, manaCostProp, new GUIContent("Mana Cost"));

                // Row 4: Description
                var r4 = new Rect(rect.x, r3.yMax + spacing, rect.width, lineHeight);
                EditorGUI.PropertyField(r4, descriptionProp, new GUIContent("Description"));
            };

            abilityLists[equipmentSo] = l;
        }

        private List<Equipment> GetFilteredEquipment()
        {
            var filtered = allEquipment.Where(e => e != null);

            if (!string.IsNullOrEmpty(searchQuery))
            {
                string q = searchQuery.ToLower();
                filtered = filtered.Where(e =>
                    e.EquipmentName.ToLower().Contains(q) ||
                    e.Id.ToLower().Contains(q));
            }

            if (!showAllTypes)
                filtered = filtered.Where(e => e.Type == filterType);

            if (!showAllRarities)
                filtered = filtered.Where(e => e.Rarity == filterRarity);

            return filtered
                .OrderBy(e => e.Type)
                .ThenBy(e => e.Rarity)
                .ThenBy(e => e.EquipmentName)
                .ToList();
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
            expandedInlineEditor.Clear();
            modifierLists.Clear();
            abilityLists.Clear();
        }

        private void AddModifierToSelected(StatType stat, ModifierType type, float value)
        {
            var list = selectedEquipment.Where(kvp => kvp.Value).Select(kvp => kvp.Key).Where(e => e != null).ToList();
            if (list.Count == 0) return;

            Undo.RegisterCompleteObjectUndo(list.ToArray(), "Add Modifiers To Selected");

            foreach (var eq in list)
            {
                var mods = eq.StatModifiers ?? System.Array.Empty<StatModifier>();
                var newMods = new StatModifier[mods.Length + 1];
                for (int i = 0; i < mods.Length; i++) newMods[i] = mods[i];

                // IMPORTANT: Sammoh.Two constructor order: (StatType, float value, ModifierType)
                newMods[newMods.Length - 1] = new StatModifier(stat, value, type);

                eq.StatModifiers = newMods;
                EditorUtility.SetDirty(eq);
            }
            AssetDatabase.SaveAssets();
        }

        private void ClearModifiersOnSelected()
        {
            var list = selectedEquipment.Where(kvp => kvp.Value).Select(kvp => kvp.Key).Where(e => e != null).ToList();
            if (list.Count == 0) return;

            Undo.RegisterCompleteObjectUndo(list.ToArray(), "Clear Modifiers On Selected");

            foreach (var eq in list)
            {
                eq.StatModifiers = System.Array.Empty<StatModifier>();
                EditorUtility.SetDirty(eq);
            }
            AssetDatabase.SaveAssets();
        }

        private Color GetRarityColor(EquipmentRarity rarity)
        {
            return rarity switch
            {
                EquipmentRarity.Common => Color.white,
                EquipmentRarity.Uncommon => Color.green,
                EquipmentRarity.Rare => Color.blue,
                EquipmentRarity.Epic => new Color(0.6f, 0.2f, 0.8f),
                EquipmentRarity.Legendary => Color.yellow,
                _ => Color.white
            };
        }
    }
}
#endif
