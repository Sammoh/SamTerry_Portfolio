using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sammoh.Two
{
    /// <summary>
    /// Handles the generation and management of equipment assets.
    /// Creates ScriptableObject assets with file-name-based IDs for fast lookup.
    /// </summary>
    public static class EquipmentGenerator
    {
        // Resource paths for equipment assets
        private const string EQUIPMENT_RESOURCES_PATH = "Assets/2. UI, Flow, & Files (2D Canvas)/Resources/Equipment";
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
            "Protective", "Defending", "Warding", "Shielding", "Guardian", "Stalwart", "Mighty", "Noble"
        };
        
        private static readonly string[] ArmorNouns = 
        {
            "Guard", "Wall", "Barrier", "Shield", "Plate", "Mail", "Hide", "Shell",
            "Cover", "Wrap", "Skin", "Carapace", "Armor", "Protection", "Defense"
        };
        
        /// <summary>
        /// Generates default equipment assets if they don't exist.
        /// </summary>
        public static void GenerateDefaultEquipment()
        {
#if UNITY_EDITOR
            EnsureDirectoriesExist();
            
            int weaponsCreated = GenerateDefaultWeapons();
            int accessoriesCreated = GenerateDefaultAccessories();
            int armorsCreated = GenerateDefaultArmor();
            
            // Create or update the equipment database
            var database = GetOrCreateEquipmentDatabase();
            database.RefreshDatabase();
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"Equipment generation complete! Created {weaponsCreated} weapons, " +
                     $"{accessoriesCreated} accessories, {armorsCreated} armor pieces.");
#endif
        }
        
        /// <summary>
        /// Generates a specific equipment type with the given parameters.
        /// </summary>
        public static Equipment GenerateEquipment(EquipmentType type, EquipmentRarity rarity, string customName = null)
        {
#if UNITY_EDITOR
            Equipment equipment = null;
            string fileName = customName ?? GenerateEquipmentName(type, rarity);
            string assetPath = GetAssetPath(type, fileName);
            
            // Check if asset already exists
            var existingAsset = AssetDatabase.LoadAssetAtPath<Equipment>(assetPath);
            if (existingAsset != null)
            {
                Debug.LogWarning($"Equipment asset already exists at {assetPath}. Use UpdateEquipment to modify existing assets.");
                return existingAsset;
            }
            
            // Create new equipment based on type
            switch (type)
            {
                case EquipmentType.Weapon:
                    var weapon = ScriptableObject.CreateInstance<Weapon>();
                    weapon.Rarity = rarity;
                    weapon.GenerateDefaultValues();
                    equipment = weapon;
                    break;
                    
                case EquipmentType.Accessory:
                    var accessory = ScriptableObject.CreateInstance<Accessory>();
                    accessory.Rarity = rarity;
                    accessory.GenerateDefaultValues();
                    equipment = accessory;
                    break;
                    
                case EquipmentType.Armor:
                    var armor = ScriptableObject.CreateInstance<Armor>();
                    armor.Rarity = rarity;
                    armor.GenerateDefaultValues();
                    equipment = armor;
                    break;
            }
            
            if (equipment != null)
            {
                AssetDatabase.CreateAsset(equipment, assetPath);
                AssetDatabase.SaveAssets();
                Debug.Log($"Created {type} asset: {assetPath}");
                return equipment;
            }
#endif
            return null;
        }
        
        /// <summary>
        /// Updates an existing equipment asset with new values.
        /// </summary>
        public static bool UpdateEquipment(Equipment equipment, bool regenerateValues = false)
        {
#if UNITY_EDITOR
            if (equipment == null) return false;
            
            if (regenerateValues)
            {
                equipment.GenerateDefaultValues();
            }
            
            EditorUtility.SetDirty(equipment);
            AssetDatabase.SaveAssets();
            Debug.Log($"Updated equipment asset: {equipment.name}");
            return true;
#else
            return false;
#endif
        }
        
        /// <summary>
        /// Gets all existing equipment assets from the Resources directory.
        /// </summary>
        public static List<Equipment> GetAllExistingEquipment()
        {
            var allEquipment = new List<Equipment>();
            
            var weapons = Resources.LoadAll<Weapon>("Equipment/Weapons");
            var accessories = Resources.LoadAll<Accessory>("Equipment/Accessories");
            var armors = Resources.LoadAll<Armor>("Equipment/Armor");
            
            allEquipment.AddRange(weapons);
            allEquipment.AddRange(accessories);
            allEquipment.AddRange(armors);
            
            return allEquipment;
        }

        #region Old content

//         /// <summary>
//         /// Gets or creates the main equipment database asset.
//         /// </summary>
//         public static EquipmentDatabase GetOrCreateEquipmentDatabase()
//         {
// #if UNITY_EDITOR
//             var database = AssetDatabase.LoadAssetAtPath<EquipmentDatabase>(DATABASE_PATH);
//             if (database == null)
//             {
//                 database = ScriptableObject.CreateInstance<EquipmentDatabase>();
//                 AssetDatabase.CreateAsset(database, DATABASE_PATH);
//                 AssetDatabase.SaveAssets();
//                 Debug.Log($"Created Equipment Database at {DATABASE_PATH}");
//             }
//             return database;
// #else
//             return Resources.Load<EquipmentDatabase>("Equipment/EquipmentDatabase");
// #endif
//      }

        #endregion
        
        public static EquipmentDatabase GetOrCreateEquipmentDatabase()
        {
            const string assetPath = "Assets/2. UI, Flow, & Files (2D Canvas)/Resources/Equipment/EquipmentDatabase.asset";

            var db = AssetDatabase.LoadAssetAtPath<EquipmentDatabase>(assetPath);
            if (db != null) return db;
            
            void EnsureAssetDirectoryExists(string assetPath)
            {
                var dir = Path.GetDirectoryName(assetPath)?.Replace('\\', '/');
                if (string.IsNullOrEmpty(dir) || AssetDatabase.IsValidFolder(dir)) return;

                var parts = dir.Split('/');
                string current = parts[0]; // "Assets"
                for (int i = 1; i < parts.Length; i++)
                {
                    var next = current + "/" + parts[i];
                    if (!AssetDatabase.IsValidFolder(next))
                        AssetDatabase.CreateFolder(current, parts[i]);
                    current = next;
                }
                AssetDatabase.Refresh();
            }

            EnsureAssetDirectoryExists(assetPath);

            db = ScriptableObject.CreateInstance<EquipmentDatabase>();
            AssetDatabase.CreateAsset(db, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return db;
        }
        
#if UNITY_EDITOR
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
        
        private static int GenerateDefaultWeapons()
        {
            int created = 0;
            
            // Generate one weapon of each type and rarity combination
            foreach (WeaponType weaponType in System.Enum.GetValues(typeof(WeaponType)))
            {
                foreach (EquipmentRarity rarity in System.Enum.GetValues(typeof(EquipmentRarity)))
                {
                    string fileName = $"{rarity}_{weaponType}_{GetRandomWeaponName()}";
                    string assetPath = GetAssetPath(EquipmentType.Weapon, fileName);
                    
                    if (AssetDatabase.LoadAssetAtPath<Weapon>(assetPath) == null)
                    {
                        var weapon = ScriptableObject.CreateInstance<Weapon>();
                        weapon.WeaponType = weaponType;
                        weapon.Rarity = rarity;
                        weapon.GenerateDefaultValues();
                        
                        AssetDatabase.CreateAsset(weapon, assetPath);
                        created++;
                    }
                }
            }
            
            return created;
        }
        
        private static int GenerateDefaultAccessories()
        {
            int created = 0;
            
            // Generate accessories of each type and rarity
            foreach (AccessoryType accessoryType in System.Enum.GetValues(typeof(AccessoryType)))
            {
                foreach (EquipmentRarity rarity in System.Enum.GetValues(typeof(EquipmentRarity)))
                {
                    string fileName = $"{rarity}_{accessoryType}_{GetRandomAccessoryName()}";
                    string assetPath = GetAssetPath(EquipmentType.Accessory, fileName);
                    
                    if (AssetDatabase.LoadAssetAtPath<Accessory>(assetPath) == null)
                    {
                        var accessory = ScriptableObject.CreateInstance<Accessory>();
                        accessory.AccessoryType = accessoryType;
                        accessory.Rarity = rarity;
                        accessory.GenerateDefaultValues();
                        
                        AssetDatabase.CreateAsset(accessory, assetPath);
                        created++;
                    }
                }
            }
            
            return created;
        }
        
        private static int GenerateDefaultArmor()
        {
            int created = 0;
            
            // Generate armor pieces of each type and rarity
            foreach (ArmorType armorType in System.Enum.GetValues(typeof(ArmorType)))
            {
                foreach (EquipmentRarity rarity in System.Enum.GetValues(typeof(EquipmentRarity)))
                {
                    string fileName = $"{rarity}_{armorType}_{GetRandomArmorName()}";
                    string assetPath = GetAssetPath(EquipmentType.Armor, fileName);
                    
                    if (AssetDatabase.LoadAssetAtPath<Armor>(assetPath) == null)
                    {
                        var armor = ScriptableObject.CreateInstance<Armor>();
                        armor.ArmorType = armorType;
                        armor.Rarity = rarity;
                        armor.GenerateDefaultValues();
                        
                        AssetDatabase.CreateAsset(armor, assetPath);
                        created++;
                    }
                }
            }
            
            return created;
        }
        
        private static string GetAssetPath(EquipmentType type, string fileName)
        {
            string directory = type switch
            {
                EquipmentType.Weapon => WEAPONS_PATH,
                EquipmentType.Accessory => ACCESSORIES_PATH,
                EquipmentType.Armor => ARMOR_PATH,
                _ => EQUIPMENT_RESOURCES_PATH
            };
            
            return Path.Combine(directory, fileName + ".asset").Replace('\\', '/');
        }
        
        private static string GenerateEquipmentName(EquipmentType type, EquipmentRarity rarity)
        {
            return type switch
            {
                EquipmentType.Weapon => $"{rarity}_Weapon_{GetRandomWeaponName()}",
                EquipmentType.Accessory => $"{rarity}_Accessory_{GetRandomAccessoryName()}",
                EquipmentType.Armor => $"{rarity}_Armor_{GetRandomArmorName()}",
                _ => $"{rarity}_Equipment_{Random.Range(1000, 9999)}"
            };
        }
        
        private static string GetRandomWeaponName()
        {
            string adjective = WeaponAdjectives[Random.Range(0, WeaponAdjectives.Length)];
            string noun = WeaponNouns[Random.Range(0, WeaponNouns.Length)];
            return $"{adjective}_{noun}";
        }
        
        private static string GetRandomAccessoryName()
        {
            string adjective = AccessoryAdjectives[Random.Range(0, AccessoryAdjectives.Length)];
            string noun = AccessoryNouns[Random.Range(0, AccessoryNouns.Length)];
            return $"{adjective}_{noun}";
        }
        
        private static string GetRandomArmorName()
        {
            string adjective = ArmorAdjectives[Random.Range(0, ArmorAdjectives.Length)];
            string noun = ArmorNouns[Random.Range(0, ArmorNouns.Length)];
            return $"{adjective}_{noun}";
        }
#endif
    }
}