using UnityEngine;
using System.Collections.Generic;

namespace Sammoh.TurnBasedStrategy
{
    /// <summary>
    /// Demonstration script showing how to use the Equipment system.
    /// This script would be attached to a GameObject in a Unity scene.
    /// </summary>
    public class EquipmentDemo : MonoBehaviour
    {
        [Header("Equipment Database")]
        [SerializeField] private EquipmentDatabase equipmentDatabase;
        
        [Header("Demo Controls")]
        [SerializeField] private bool initializeOnStart = true;
        [SerializeField] private string testEquipmentId = "Common_Sword_Sharp_Blade";
        
        [Header("Display Info")]
        [SerializeField] private int totalEquipmentCount;
        [SerializeField] private int weaponCount;
        [SerializeField] private int accessoryCount;
        [SerializeField] private int armorCount;
        
        private void Start()
        {
            if (initializeOnStart)
            {
                InitializeEquipmentSystem();
                DemonstrateEquipmentUsage();
                UpdateDisplayInfo();
            }
        }
        
        /// <summary>
        /// Initializes the equipment system and database.
        /// </summary>
        public void InitializeEquipmentSystem()
        {
            Debug.Log("=== Equipment System Initialization ===");
            
            // Get or load the equipment database
            if (equipmentDatabase == null)
            {
                equipmentDatabase = EquipmentGenerator.GetOrCreateEquipmentDatabase();
            }
            
            if (equipmentDatabase == null)
            {
                Debug.LogError("Failed to load Equipment Database! Make sure to generate equipment assets first.");
                return;
            }
            
            // Initialize lookup tables for fast access
            equipmentDatabase.InitializeLookupTables();
            
            Debug.Log($"Equipment Database loaded successfully!");
            Debug.Log($"Total Equipment: {equipmentDatabase.TotalEquipmentCount}");
            Debug.Log($"Last Updated: {equipmentDatabase.LastUpdated}");
        }
        
        /// <summary>
        /// Demonstrates various ways to use the equipment system.
        /// </summary>
        public void DemonstrateEquipmentUsage()
        {
            if (equipmentDatabase == null)
            {
                Debug.LogError("Equipment database not initialized!");
                return;
            }
            
            Debug.Log("=== Equipment System Usage Demo ===");
            
            // 1. Find equipment by ID (file name)
            DemonstrateEquipmentLookup();
            
            // 2. Get equipment by type
            DemonstrateTypeFiltering();
            
            // 3. Get equipment by rarity
            DemonstrateRarityFiltering();
            
            // 4. Show equipment statistics
            DemonstrateStatistics();
            
            // 5. Demonstrate equipment-specific functionality
            DemonstrateEquipmentFeatures();
        }
        
        private void DemonstrateEquipmentLookup()
        {
            Debug.Log("--- Equipment Lookup Demo ---");
            
            Equipment found = equipmentDatabase.FindEquipmentById(testEquipmentId);
            if (found != null)
            {
                Debug.Log($"Found equipment: {found.EquipmentName} (ID: {found.Id})");
                Debug.Log($"Type: {found.Type}, Rarity: {found.Rarity}, Level: {found.Level}");
            }
            else
            {
                Debug.LogWarning($"Equipment with ID '{testEquipmentId}' not found. Generate equipment assets first!");
            }
        }
        
        private void DemonstrateTypeFiltering()
        {
            Debug.Log("--- Type Filtering Demo ---");
            
            var weapons = equipmentDatabase.GetEquipmentByType(EquipmentType.Weapon);
            var accessories = equipmentDatabase.GetEquipmentByType(EquipmentType.Accessory);
            var armor = equipmentDatabase.GetEquipmentByType(EquipmentType.Armor);
            
            Debug.Log($"Weapons: {weapons.Count}");
            Debug.Log($"Accessories: {accessories.Count}");
            Debug.Log($"Armor: {armor.Count}");
            
            // Show first weapon if available
            if (weapons.Count > 0 && weapons[0] is Weapon weapon)
            {
                Debug.Log($"Sample Weapon: {weapon.EquipmentName} - Damage: {weapon.Damage}, DPS: {weapon.CalculateDPS():F2}");
            }
        }
        
        private void DemonstrateRarityFiltering()
        {
            Debug.Log("--- Rarity Filtering Demo ---");
            
            foreach (EquipmentRarity rarity in System.Enum.GetValues(typeof(EquipmentRarity)))
            {
                var equipment = equipmentDatabase.GetEquipmentByRarity(rarity);
                Debug.Log($"{rarity} Equipment: {equipment.Count} items");
            }
        }
        
        private void DemonstrateStatistics()
        {
            Debug.Log("--- Equipment Statistics ---");
            
            var stats = equipmentDatabase.GetStatistics();
            Debug.Log($"Total Equipment: {stats.TotalCount}");
            Debug.Log($"  Weapons: {stats.WeaponCount}");
            Debug.Log($"  Accessories: {stats.AccessoryCount}");
            Debug.Log($"  Armor: {stats.ArmorCount}");
            
            Debug.Log("Breakdown by Rarity:");
            foreach (var kvp in stats.CountByRarity)
            {
                Debug.Log($"  {kvp.Key}: {kvp.Value}");
            }
        }
        
        private void DemonstrateEquipmentFeatures()
        {
            Debug.Log("--- Equipment-Specific Features Demo ---");
            
            // Weapon features
            var weapons = equipmentDatabase.GetEquipmentByType(EquipmentType.Weapon);
            if (weapons.Count > 0 && weapons[0] is Weapon sampleWeapon)
            {
                Debug.Log($"Weapon '{sampleWeapon.EquipmentName}' DPS: {sampleWeapon.CalculateDPS():F2}");
            }
            
            // Armor features
            var armors = equipmentDatabase.GetEquipmentByType(EquipmentType.Armor);
            if (armors.Count > 0 && armors[0] is Armor sampleArmor)
            {
                int totalDefense = sampleArmor.CalculateTotalDefense();
                Debug.Log($"Armor '{sampleArmor.EquipmentName}' Total Defense: {totalDefense}");
                Debug.Log($"Fire Resistance: {sampleArmor.GetDamageResistance(DamageType.Fire)}%");
            }
            
            // Accessory features
            var accessories = equipmentDatabase.GetEquipmentByType(EquipmentType.Accessory);
            if (accessories.Count > 0 && accessories[0] is Accessory sampleAccessory)
            {
                Debug.Log($"Accessory '{sampleAccessory.EquipmentName}' has {sampleAccessory.StatModifiers.Length} stat modifiers");
                if (sampleAccessory.StatModifiers.Length > 0)
                {
                    var modifier = sampleAccessory.StatModifiers[0];
                    Debug.Log($"  First modifier: +{modifier.Value} {modifier.StatType} ({modifier.ModifierType})");
                }
            }
        }
        
        /// <summary>
        /// Updates the inspector display information.
        /// </summary>
        private void UpdateDisplayInfo()
        {
            if (equipmentDatabase != null)
            {
                var stats = equipmentDatabase.GetStatistics();
                totalEquipmentCount = stats.TotalCount;
                weaponCount = stats.WeaponCount;
                accessoryCount = stats.AccessoryCount;
                armorCount = stats.ArmorCount;
            }
        }
        
        /// <summary>
        /// Demonstrates loading equipment directly from Resources.
        /// This method shows how to bypass the database for direct loading.
        /// </summary>
        public void DemonstrateDirectResourceLoading()
        {
            Debug.Log("=== Direct Resource Loading Demo ===");
            
            // Load all weapons from Resources
            Weapon[] allWeapons = Resources.LoadAll<Weapon>("Equipment/Weapons");
            Debug.Log($"Loaded {allWeapons.Length} weapons directly from Resources");
            
            // Load all accessories from Resources
            Accessory[] allAccessories = Resources.LoadAll<Accessory>("Equipment/Accessories");
            Debug.Log($"Loaded {allAccessories.Length} accessories directly from Resources");
            
            // Load all armor from Resources
            Armor[] allArmor = Resources.LoadAll<Armor>("Equipment/Armor");
            Debug.Log($"Loaded {allArmor.Length} armor pieces directly from Resources");
            
            // Example of loading a specific item by path
            // Weapon specificWeapon = Resources.Load<Weapon>("Equipment/Weapons/Epic_Sword_Legendary_Blade");
            // if (specificWeapon != null)
            // {
            //     Debug.Log($"Loaded specific weapon: {specificWeapon.EquipmentName}");
            // }
        }
        
        /// <summary>
        /// Public method to trigger demo from inspector or other scripts.
        /// </summary>
        [ContextMenu("Run Equipment Demo")]
        public void RunDemo()
        {
            InitializeEquipmentSystem();
            DemonstrateEquipmentUsage();
            DemonstrateDirectResourceLoading();
            UpdateDisplayInfo();
        }
        
        /// <summary>
        /// Validate that the equipment system is working correctly.
        /// </summary>
        [ContextMenu("Validate Equipment System")]
        public void ValidateSystem()
        {
            Debug.Log("=== Equipment System Validation ===");
            
            bool isValid = true;
            
            // Check if database exists
            if (equipmentDatabase == null)
            {
                Debug.LogError("Equipment Database is null!");
                isValid = false;
            }
            
            // Check if equipment assets exist
            var allEquipment = EquipmentGenerator.GetAllExistingEquipment();
            if (allEquipment.Count == 0)
            {
                Debug.LogWarning("No equipment assets found! Generate default equipment first.");
                isValid = false;
            }
            
            // Validate equipment IDs
            foreach (var equipment in allEquipment)
            {
                if (string.IsNullOrEmpty(equipment.Id))
                {
                    Debug.LogError($"Equipment {equipment.name} has invalid ID!");
                    isValid = false;
                }
            }
            
            if (isValid)
            {
                Debug.Log("✓ Equipment System validation passed!");
            }
            else
            {
                Debug.LogError("✗ Equipment System validation failed!");
            }
        }
    }
}