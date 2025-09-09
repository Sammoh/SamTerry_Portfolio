using UnityEngine;
using UnityEditor;

namespace Sammoh.TurnBasedStrategy.Editor
{
    /// <summary>
    /// Utility class for generating default equipment assets
    /// </summary>
    public static class EquipmentAssetGenerator
    {
        private const string EQUIPMENT_ASSETS_PATH = "Assets/8. Turn-Based Strategy/Equipment Assets";
        private const string WEAPONS_PATH = EQUIPMENT_ASSETS_PATH + "/Weapons";
        private const string ARMOR_PATH = EQUIPMENT_ASSETS_PATH + "/Armor";
        private const string ACCESSORIES_PATH = EQUIPMENT_ASSETS_PATH + "/Accessories";

        [MenuItem("Tools/Turn-Based Strategy/Generate Default Equipment Assets")]
        public static void GenerateDefaultEquipmentAssets()
        {
            // Ensure directories exist
            EnsureDirectoryExists(WEAPONS_PATH);
            EnsureDirectoryExists(ARMOR_PATH);
            EnsureDirectoryExists(ACCESSORIES_PATH);

            // Create weapon assets
            CreateWeaponAssets();
            
            // Create armor assets
            CreateArmorAssets();
            
            // Create accessory assets
            CreateAccessoryAssets();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("Generated default equipment assets in: " + EQUIPMENT_ASSETS_PATH);
        }

        private static void CreateWeaponAssets()
        {
            // Iron Sword
            var ironSword = ScriptableObject.CreateInstance<Equipment>();
            ironSword.Initialize("Iron Sword", EquipmentSlot.Weapon,
                new StatModifier[] {
                    new StatModifier(StatType.Attack, 5, ModifierType.Additive)
                },
                "A sturdy iron sword");
            CreateEquipmentAsset(ironSword, WEAPONS_PATH + "/Iron_Sword_Equipment.asset");

            // Steel Blade
            var steelBlade = ScriptableObject.CreateInstance<Equipment>();
            steelBlade.Initialize("Steel Blade", EquipmentSlot.Weapon,
                new StatModifier[] {
                    new StatModifier(StatType.Attack, 8, ModifierType.Additive),
                    new StatModifier(StatType.Speed, 2, ModifierType.Additive)
                },
                "A sharp steel blade");
            CreateEquipmentAsset(steelBlade, WEAPONS_PATH + "/Steel_Blade_Equipment.asset");

            // Flame Sword
            var flameSword = ScriptableObject.CreateInstance<Equipment>();
            flameSword.Initialize("Flame Sword", EquipmentSlot.Weapon,
                new StatModifier[] {
                    new StatModifier(StatType.Attack, 12, ModifierType.Additive),
                    new StatModifier(StatType.Mana, -5, ModifierType.Additive)
                },
                "A magical sword wreathed in flames");
            CreateEquipmentAsset(flameSword, WEAPONS_PATH + "/Flame_Sword_Equipment.asset");
        }

        private static void CreateArmorAssets()
        {
            // Leather Armor
            var leatherArmor = ScriptableObject.CreateInstance<Equipment>();
            leatherArmor.Initialize("Leather Armor", EquipmentSlot.Armor,
                new StatModifier[] {
                    new StatModifier(StatType.Defense, 3, ModifierType.Additive),
                    new StatModifier(StatType.MaxHealth, 10, ModifierType.Additive)
                },
                "Light and flexible leather armor");
            CreateEquipmentAsset(leatherArmor, ARMOR_PATH + "/Leather_Armor_Equipment.asset");

            // Chain Mail
            var chainMail = ScriptableObject.CreateInstance<Equipment>();
            chainMail.Initialize("Chain Mail", EquipmentSlot.Armor,
                new StatModifier[] {
                    new StatModifier(StatType.Defense, 6, ModifierType.Additive),
                    new StatModifier(StatType.MaxHealth, 15, ModifierType.Additive),
                    new StatModifier(StatType.Speed, -2, ModifierType.Additive)
                },
                "Heavy chain mail armor");
            CreateEquipmentAsset(chainMail, ARMOR_PATH + "/Chain_Mail_Equipment.asset");

            // Mage Robes
            var mageRobes = ScriptableObject.CreateInstance<Equipment>();
            mageRobes.Initialize("Mage Robes", EquipmentSlot.Armor,
                new StatModifier[] {
                    new StatModifier(StatType.Mana, 20, ModifierType.Additive),
                    new StatModifier(StatType.Defense, -2, ModifierType.Additive)
                },
                "Enchanted robes that enhance magical power");
            CreateEquipmentAsset(mageRobes, ARMOR_PATH + "/Mage_Robes_Equipment.asset");
        }

        private static void CreateAccessoryAssets()
        {
            // Power Ring
            var powerRing = ScriptableObject.CreateInstance<Equipment>();
            powerRing.Initialize("Power Ring", EquipmentSlot.Accessory,
                new StatModifier[] {
                    new StatModifier(StatType.Attack, 20, ModifierType.Multiplicative)
                },
                "A ring that amplifies physical strength");
            CreateEquipmentAsset(powerRing, ACCESSORIES_PATH + "/Power_Ring_Equipment.asset");

            // Health Amulet
            var healthAmulet = ScriptableObject.CreateInstance<Equipment>();
            healthAmulet.Initialize("Health Amulet", EquipmentSlot.Accessory,
                new StatModifier[] {
                    new StatModifier(StatType.MaxHealth, 30, ModifierType.Additive)
                },
                "An amulet that grants additional vitality");
            CreateEquipmentAsset(healthAmulet, ACCESSORIES_PATH + "/Health_Amulet_Equipment.asset");

            // Speed Boots
            var speedBoots = ScriptableObject.CreateInstance<Equipment>();
            speedBoots.Initialize("Speed Boots", EquipmentSlot.Accessory,
                new StatModifier[] {
                    new StatModifier(StatType.Speed, 5, ModifierType.Additive)
                },
                "Boots that enhance movement speed");
            CreateEquipmentAsset(speedBoots, ACCESSORIES_PATH + "/Speed_Boots_Equipment.asset");
        }

        private static void CreateEquipmentAsset(Equipment equipment, string assetPath)
        {
            AssetDatabase.CreateAsset(equipment, assetPath);
            Debug.Log($"Created equipment asset: {equipment.EquipmentName} at {assetPath}");
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parentPath = System.IO.Path.GetDirectoryName(path).Replace('\\', '/');
                string folderName = System.IO.Path.GetFileName(path);
                
                if (!AssetDatabase.IsValidFolder(parentPath))
                {
                    // Recursively create parent directories
                    EnsureDirectoryExists(parentPath);
                }
                
                AssetDatabase.CreateFolder(parentPath, folderName);
            }
        }
    }
}