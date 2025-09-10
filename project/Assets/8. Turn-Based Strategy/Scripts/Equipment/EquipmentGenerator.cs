using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sammoh.TurnBasedStrategy
{
    /// <summary>
    /// Handles the generation and management of equipment assets for the Turn-Based Strategy system.
    /// Creates ScriptableObject assets with file-name-based IDs for fast lookup.
    /// </summary>
    public static class EquipmentGenerator
    {
        // Resource paths for equipment assets (adjusted for Turn-Based Strategy structure)
        private const string EQUIPMENT_RESOURCES_PATH = "Assets/8. Turn-Based Strategy/Resources/Equipment";
        private const string WEAPONS_PATH = EQUIPMENT_RESOURCES_PATH + "/Weapons";
        private const string ACCESSORIES_PATH = EQUIPMENT_RESOURCES_PATH + "/Accessories";
        private const string ARMOR_PATH = EQUIPMENT_RESOURCES_PATH + "/Armor";
        private const string DATABASE_PATH = EQUIPMENT_RESOURCES_PATH + "/EquipmentDatabase.asset";
        
        // Name generators for procedural equipment
        private static readonly string[] WeaponAdjectives = 
        {
            "Sharp", "Deadly", "Ancient", "Cursed", "Blessed", "Flaming", "Frozen", "Lightning",
            "Masterwork", "Crude", "Elegant", "Brutal", "Swift", "Heavy", "Light", "Balanced"
        };
        
        private static readonly string[] WeaponNouns = 
        {
            "Blade", "Edge", "Fang", "Claw", "Strike", "Bite", "Cleaver", "Slicer",
            "Piercer", "Crusher", "Breaker", "Render", "Splitter", "Cutter", "Ripper"
        };
        
        private static readonly string[] AccessoryAdjectives = 
        {
            "Shimmering", "Mystic", "Arcane", "Divine", "Infernal", "Celestial", "Earthen", "Ethereal",
            "Radiant", "Shadow", "Crystal", "Golden", "Silver", "Platinum", "Enchanted", "Runic"
        };
        
        private static readonly string[] AccessoryNouns = 
        {
            "Band", "Loop", "Circle", "Chain", "Pendant", "Gem", "Stone", "Orb",
            "Charm", "Token", "Sigil", "Mark", "Symbol", "Emblem", "Trinket"
        };
        
        private static readonly string[] ArmorAdjectives = 
        {
            "Sturdy", "Reinforced", "Tempered", "Hardened", "Fortified", "Resilient", "Durable", "Tough",
            "Impenetrable", "Solid", "Robust", "Protective", "Defensive", "Shielding", "Guardian"
        };
        
        private static readonly string[] ArmorNouns = 
        {
            "Guard", "Shield", "Barrier", "Wall", "Bulwark", "Bastion", "Defender", "Protector",
            "Aegis", "Ward", "Cover", "Shell", "Carapace", "Hide", "Skin"
        };

#if UNITY_EDITOR
        /// <summary>
        /// Generates a weapon with procedural stats and name based on type and rarity.
        /// </summary>
        /// <param name="weaponType">The type of weapon to create</param>
        /// <param name="rarity">The rarity level</param>
        /// <param name="customName">Optional custom name (if null, generates procedural name)</param>
        /// <returns>The created weapon asset</returns>
        public static WeaponSO GenerateWeapon(WeaponType weaponType, EquipmentRarity rarity, string customName = null)
        {
            EnsureDirectoriesExist();
            
            // Generate name
            string assetName = customName ?? GenerateWeaponName(weaponType, rarity);
            string fileName = SanitizeFileName(assetName);
            string fullPath = Path.Combine(WEAPONS_PATH, $"{fileName}.asset");
            
            // Check if asset already exists
            if (AssetDatabase.LoadAssetAtPath<WeaponSO>(fullPath) != null)
            {
                Debug.LogWarning($"Weapon asset already exists at {fullPath}. Skipping creation.");
                return AssetDatabase.LoadAssetAtPath<WeaponSO>(fullPath);
            }
            
            // Create weapon instance
            var weapon = ScriptableObject.CreateInstance<WeaponSO>();
            weapon.name = fileName;
            weapon.EquipmentName = assetName;
            weapon.WeaponType = weaponType;
            weapon.Rarity = rarity;
            weapon.Slot = EquipmentSlot.Weapon;
            
            // Generate procedural stats
            GenerateWeaponStats(weapon);
            weapon.GenerateDefaultValues();
            
            // Save asset
            AssetDatabase.CreateAsset(weapon, fullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"Generated weapon: {assetName} at {fullPath}");
            return weapon;
        }

        /// <summary>
        /// Generates an armor piece with procedural stats and name.
        /// </summary>
        public static ArmorSO GenerateArmor(ArmorType armorType, EquipmentRarity rarity, ArmorMaterial material, string customName = null)
        {
            EnsureDirectoriesExist();
            
            // Generate name
            string assetName = customName ?? GenerateArmorName(armorType, rarity, material);
            string fileName = SanitizeFileName(assetName);
            string fullPath = Path.Combine(ARMOR_PATH, $"{fileName}.asset");
            
            // Check if asset already exists
            if (AssetDatabase.LoadAssetAtPath<ArmorSO>(fullPath) != null)
            {
                Debug.LogWarning($"Armor asset already exists at {fullPath}. Skipping creation.");
                return AssetDatabase.LoadAssetAtPath<ArmorSO>(fullPath);
            }
            
            // Create armor instance
            var armor = ScriptableObject.CreateInstance<ArmorSO>();
            armor.name = fileName;
            armor.EquipmentName = assetName;
            armor.ArmorType = armorType;
            armor.Rarity = rarity;
            armor.ArmorMaterial = material;
            armor.Slot = EquipmentSlot.Armor;
            
            // Generate procedural stats
            GenerateArmorStats(armor);
            armor.GenerateDefaultValues();
            
            // Save asset
            AssetDatabase.CreateAsset(armor, fullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"Generated armor: {assetName} at {fullPath}");
            return armor;
        }

        /// <summary>
        /// Generates an accessory with procedural stats and name.
        /// </summary>
        public static AccessorySO GenerateAccessory(AccessoryType accessoryType, EquipmentRarity rarity, string customName = null)
        {
            EnsureDirectoriesExist();
            
            // Generate name
            string assetName = customName ?? GenerateAccessoryName(accessoryType, rarity);
            string fileName = SanitizeFileName(assetName);
            string fullPath = Path.Combine(ACCESSORIES_PATH, $"{fileName}.asset");
            
            // Check if asset already exists
            if (AssetDatabase.LoadAssetAtPath<AccessorySO>(fullPath) != null)
            {
                Debug.LogWarning($"Accessory asset already exists at {fullPath}. Skipping creation.");
                return AssetDatabase.LoadAssetAtPath<AccessorySO>(fullPath);
            }
            
            // Create accessory instance
            var accessory = ScriptableObject.CreateInstance<AccessorySO>();
            accessory.name = fileName;
            accessory.EquipmentName = assetName;
            accessory.AccessoryType = accessoryType;
            accessory.Rarity = rarity;
            accessory.Slot = EquipmentSlot.Accessory;
            
            // Generate procedural stats
            GenerateAccessoryStats(accessory);
            accessory.GenerateDefaultValues();
            
            // Save asset
            AssetDatabase.CreateAsset(accessory, fullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"Generated accessory: {assetName} at {fullPath}");
            return accessory;
        }

        /// <summary>
        /// Generates a complete set of default equipment for all types and rarities.
        /// </summary>
        [MenuItem("Tools/Equipment/Generate Default Equipment")]
        public static void GenerateDefaultEquipment()
        {
            EnsureDirectoriesExist();
            
            int totalGenerated = 0;
            
            // Generate weapons
            foreach (WeaponType weaponType in System.Enum.GetValues(typeof(WeaponType)))
            {
                foreach (EquipmentRarity rarity in System.Enum.GetValues(typeof(EquipmentRarity)))
                {
                    var weapon = GenerateWeapon(weaponType, rarity);
                    if (weapon != null) totalGenerated++;
                }
            }
            
            // Generate armor
            foreach (ArmorType armorType in System.Enum.GetValues(typeof(ArmorType)))
            {
                foreach (ArmorMaterial material in System.Enum.GetValues(typeof(ArmorMaterial)))
                {
                    var armor = GenerateArmor(armorType, EquipmentRarity.Common, material);
                    if (armor != null) totalGenerated++;
                }
            }
            
            // Generate accessories
            foreach (AccessoryType accessoryType in System.Enum.GetValues(typeof(AccessoryType)))
            {
                foreach (EquipmentRarity rarity in System.Enum.GetValues(typeof(EquipmentRarity)))
                {
                    var accessory = GenerateAccessory(accessoryType, rarity);
                    if (accessory != null) totalGenerated++;
                }
            }
            
            // Update database
            RefreshEquipmentDatabase();
            
            Debug.Log($"Generated {totalGenerated} equipment items!");
        }

        /// <summary>
        /// Gets or creates the main equipment database.
        /// </summary>
        public static EquipmentDatabase GetOrCreateEquipmentDatabase()
        {
            EnsureDirectoriesExist();
            
            var database = AssetDatabase.LoadAssetAtPath<EquipmentDatabase>(DATABASE_PATH);
            if (database == null)
            {
                database = ScriptableObject.CreateInstance<EquipmentDatabase>();
                AssetDatabase.CreateAsset(database, DATABASE_PATH);
                AssetDatabase.SaveAssets();
            }
            
            return database;
        }

        /// <summary>
        /// Refreshes the equipment database by scanning for all equipment assets.
        /// </summary>
        [MenuItem("Tools/Equipment/Refresh Database")]
        public static void RefreshEquipmentDatabase()
        {
            var database = GetOrCreateEquipmentDatabase();
            database.RefreshDatabase();
            
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
        }
        
        private static void EnsureDirectoriesExist()
        {
            if (!Directory.Exists(EQUIPMENT_RESOURCES_PATH))
                Directory.CreateDirectory(EQUIPMENT_RESOURCES_PATH);
            if (!Directory.Exists(WEAPONS_PATH))
                Directory.CreateDirectory(WEAPONS_PATH);
            if (!Directory.Exists(ACCESSORIES_PATH))
                Directory.CreateDirectory(ACCESSORIES_PATH);
            if (!Directory.Exists(ARMOR_PATH))
                Directory.CreateDirectory(ARMOR_PATH);
        }

        private static string GenerateWeaponName(WeaponType weaponType, EquipmentRarity rarity)
        {
            string adjective = WeaponAdjectives[Random.Range(0, WeaponAdjectives.Length)];
            string noun = WeaponNouns[Random.Range(0, WeaponNouns.Length)];
            return $"{rarity}_{weaponType}_{adjective}_{noun}";
        }

        private static string GenerateArmorName(ArmorType armorType, EquipmentRarity rarity, ArmorMaterial material)
        {
            string adjective = ArmorAdjectives[Random.Range(0, ArmorAdjectives.Length)];
            string noun = ArmorNouns[Random.Range(0, ArmorNouns.Length)];
            return $"{rarity}_{material}_{armorType}_{adjective}_{noun}";
        }

        private static string GenerateAccessoryName(AccessoryType accessoryType, EquipmentRarity rarity)
        {
            string adjective = AccessoryAdjectives[Random.Range(0, AccessoryAdjectives.Length)];
            string noun = AccessoryNouns[Random.Range(0, AccessoryNouns.Length)];
            return $"{rarity}_{accessoryType}_{adjective}_{noun}";
        }

        private static void GenerateWeaponStats(WeaponSO weapon)
        {
            // Base stats based on weapon type
            switch (weapon.WeaponType)
            {
                case WeaponType.Sword:
                    weapon.Damage = Random.Range(8, 12);
                    weapon.AttackSpeed = Random.Range(0.9f, 1.1f);
                    weapon.Range = Random.Range(1.0f, 1.2f);
                    weapon.CriticalChance = Random.Range(3, 8);
                    break;
                case WeaponType.Dagger:
                    weapon.Damage = Random.Range(4, 8);
                    weapon.AttackSpeed = Random.Range(1.3f, 1.7f);
                    weapon.Range = Random.Range(0.8f, 1.0f);
                    weapon.CriticalChance = Random.Range(8, 15);
                    break;
                case WeaponType.Axe:
                    weapon.Damage = Random.Range(12, 18);
                    weapon.AttackSpeed = Random.Range(0.6f, 0.9f);
                    weapon.Range = Random.Range(1.1f, 1.3f);
                    weapon.CriticalChance = Random.Range(2, 6);
                    break;
                case WeaponType.Bow:
                    weapon.Damage = Random.Range(6, 10);
                    weapon.AttackSpeed = Random.Range(1.0f, 1.3f);
                    weapon.Range = Random.Range(3.0f, 5.0f);
                    weapon.CriticalChance = Random.Range(5, 12);
                    break;
                case WeaponType.Staff:
                    weapon.Damage = Random.Range(5, 9);
                    weapon.AttackSpeed = Random.Range(0.8f, 1.2f);
                    weapon.Range = Random.Range(2.0f, 3.0f);
                    weapon.CriticalChance = Random.Range(4, 9);
                    break;
                case WeaponType.Mace:
                    weapon.Damage = Random.Range(10, 16);
                    weapon.AttackSpeed = Random.Range(0.7f, 1.0f);
                    weapon.Range = Random.Range(1.0f, 1.2f);
                    weapon.CriticalChance = Random.Range(2, 7);
                    break;
            }
            
            weapon.CriticalMultiplier = Random.Range(1.3f, 2.0f);
        }

        private static void GenerateArmorStats(ArmorSO armor)
        {
            // Base stats based on armor type and material
            int baseDefense = armor.ArmorType switch
            {
                ArmorType.Helmet => Random.Range(2, 5),
                ArmorType.Chestplate => Random.Range(5, 12),
                ArmorType.Leggings => Random.Range(3, 8),
                ArmorType.Boots => Random.Range(1, 4),
                ArmorType.Gloves => Random.Range(1, 3),
                ArmorType.Shield => Random.Range(4, 10),
                _ => 3
            };
            
            armor.Defense = baseDefense;
            armor.MagicResistance = Random.Range(1, 4);
            armor.MovementSpeedModifier = Random.Range(-0.1f, 0.1f);
        }

        private static void GenerateAccessoryStats(AccessorySO accessory)
        {
            // Accessories primarily provide stat modifiers
            var modifiers = new List<StatModifier>();
            
            // Generate 1-3 random stat modifiers
            int modifierCount = Random.Range(1, 4);
            var possibleStats = new[] { StatType.Attack, StatType.Defense, StatType.MaxHealth, StatType.Mana, StatType.Speed };
            
            for (int i = 0; i < modifierCount; i++)
            {
                var statType = possibleStats[Random.Range(0, possibleStats.Length)];
                var modifierType = Random.Range(0, 2) == 0 ? ModifierType.Additive : ModifierType.Multiplicative;
                
                float value = modifierType == ModifierType.Additive 
                    ? Random.Range(1f, 10f) 
                    : Random.Range(5f, 25f);
                    
                modifiers.Add(new StatModifier(statType, value, modifierType));
            }
            
            accessory.StatModifiers = modifiers.ToArray();
        }

        private static string SanitizeFileName(string fileName)
        {
            // Replace invalid characters with underscores
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName;
        }
#endif
    }
}