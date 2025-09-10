using UnityEngine;
using System.Collections.Generic;

namespace Sammoh.TurnBasedStrategy
{
    /// <summary>
    /// Demonstrates the equipment system functionality
    /// </summary>
    public class EquipmentDemo : MonoBehaviour
    {
        [Header("Demo Settings")]
        [SerializeField] private bool autoRunOnStart = false;
        [SerializeField] private EquipmentDatabase equipmentDatabase;

        private Character demoCharacter;

        private void Start()
        {
            if (autoRunOnStart)
            {
                RunEquipmentDemo();
            }
        }

        /// <summary>
        /// Demonstrates the equipment system
        /// </summary>
        [ContextMenu("Run Equipment Demo")]
        public void RunEquipmentDemo()
        {
            Debug.Log("=== Equipment System Demo ===");

            // Create or find a demo character
            SetupDemoCharacter();

            if (demoCharacter == null)
            {
                Debug.LogError("No demo character available for equipment demo");
                return;
            }

            // Create equipment database if not assigned
            if (equipmentDatabase == null)
            {
                CreateTemporaryEquipmentDatabase();
            }

            // Demonstrate basic equipment functionality
            DemonstrateBasicEquipment();

            // Demonstrate stat calculations
            DemonstrateStatCalculations();

            // Demonstrate equipment swapping
            DemonstrateEquipmentSwapping();

            Debug.Log("=== Equipment Demo Complete ===");
        }

        /// <summary>
        /// Resets the demo character to default state
        /// </summary>
        [ContextMenu("Reset Character")]
        public void ResetCharacter()
        {
            if (demoCharacter != null)
            {
                // Unequip all items
                demoCharacter.EquipmentManager.UnequipAll();
                
                // Restore character to full health/mana
                demoCharacter.RestoreToFull();
                
                Debug.Log($"Reset {demoCharacter.CharacterName} to default state");
                LogCharacterStats("After Reset");
            }
        }

        private void SetupDemoCharacter()
        {
            // Try to find existing character component
            demoCharacter = GetComponent<Character>();

            if (demoCharacter == null)
            {
                // Create a new character for demo
                demoCharacter = gameObject.AddComponent<Character>();
                var stats = new CharacterStats(100, 15, 10, 12, 50);
                var abilities = new CharacterAbility[]
                {
                    new CharacterAbility("Demo Attack", AbilityType.Attack, 10, 5, "Basic attack for demo")
                };
                demoCharacter.Initialize("Demo Character", stats, abilities, true);
            }

            Debug.Log($"Demo character: {demoCharacter.CharacterName}");
        }

        private void CreateTemporaryEquipmentDatabase()
        {
            // Create temporary equipment for demo
            equipmentDatabase = ScriptableObject.CreateInstance<EquipmentDatabase>();
            
            // Instead of using CreateDefaultEquipment which creates asset instances,
            // create runtime equipment instances manually
            CreateRuntimeEquipmentForDemo();
            
            Debug.Log("Created temporary equipment database for demo");
        }

        /// <summary>
        /// Creates runtime equipment instances suitable for testing/demo purposes
        /// </summary>
        private void CreateRuntimeEquipmentForDemo()
        {
            var weaponsList = new List<Equipment>();
            var armorList = new List<Equipment>();
            var accessoriesList = new List<Equipment>();

            // Create weapons (runtime instances)
            var ironSword = CreateRuntimeEquipment("Iron Sword", EquipmentSlot.Weapon,
                new StatModifier[] {
                    new StatModifier(StatType.Attack, 5, ModifierType.Additive)
                },
                "A sturdy iron sword");
            weaponsList.Add(ironSword);

            var steelBlade = CreateRuntimeEquipment("Steel Blade", EquipmentSlot.Weapon,
                new StatModifier[] {
                    new StatModifier(StatType.Attack, 8, ModifierType.Additive),
                    new StatModifier(StatType.Speed, 2, ModifierType.Additive)
                },
                "A sharp steel blade");
            weaponsList.Add(steelBlade);

            // Create armor (runtime instances)
            var leatherArmor = CreateRuntimeEquipment("Leather Armor", EquipmentSlot.Armor,
                new StatModifier[] {
                    new StatModifier(StatType.Defense, 3, ModifierType.Additive),
                    new StatModifier(StatType.MaxHealth, 10, ModifierType.Additive)
                },
                "Light and flexible leather armor");
            armorList.Add(leatherArmor);

            // Create accessories (runtime instances)
            var powerRing = CreateRuntimeEquipment("Power Ring", EquipmentSlot.Accessory,
                new StatModifier[] {
                    new StatModifier(StatType.Attack, 20, ModifierType.Multiplicative)
                },
                "A ring that amplifies physical strength");
            accessoriesList.Add(powerRing);

            // Use reflection to set private arrays since we can't modify ScriptableObject at runtime normally
            var weaponsField = typeof(EquipmentDatabase).GetField("weapons", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var armorField = typeof(EquipmentDatabase).GetField("armor", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var accessoriesField = typeof(EquipmentDatabase).GetField("accessories", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            weaponsField?.SetValue(equipmentDatabase, weaponsList.ToArray());
            armorField?.SetValue(equipmentDatabase, armorList.ToArray());
            accessoriesField?.SetValue(equipmentDatabase, accessoriesList.ToArray());
        }

        /// <summary>
        /// Creates a runtime equipment instance for demonstration purposes
        /// </summary>
        private Equipment CreateRuntimeEquipment(string name, EquipmentSlot slot, StatModifier[] modifiers, string description)
        {
            var equipment = ScriptableObject.CreateInstance<Equipment>();
            equipment.Initialize(name, slot, modifiers, description);
            return equipment;
        }

        private void DemonstrateBasicEquipment()
        {
            Debug.Log("--- Basic Equipment Demo ---");
            
            LogCharacterStats("Before Equipment");

            // Equip a weapon
            var weapon = equipmentDatabase.Weapons[0]; // Iron Sword
            demoCharacter.EquipItem(weapon);
            Debug.Log($"Equipped: {weapon.EquipmentName}");
            LogCharacterStats("After Weapon");

            // Equip armor
            var armor = equipmentDatabase.Armor[0]; // Leather Armor
            demoCharacter.EquipItem(armor);
            Debug.Log($"Equipped: {armor.EquipmentName}");
            LogCharacterStats("After Armor");

            // Equip accessory
            var accessory = equipmentDatabase.Accessories[0]; // Power Ring
            demoCharacter.EquipItem(accessory);
            Debug.Log($"Equipped: {accessory.EquipmentName}");
            LogCharacterStats("After Accessory");
        }

        private void DemonstrateStatCalculations()
        {
            Debug.Log("--- Stat Calculation Demo ---");

            var stats = demoCharacter.Stats;
            
            Debug.Log($"Base Attack: {stats.BaseAttack}, Effective Attack: {stats.Attack}");
            Debug.Log($"Base Defense: {stats.BaseDefense}, Effective Defense: {stats.Defense}");
            Debug.Log($"Base Max Health: {stats.BaseMaxHealth}, Effective Max Health: {stats.MaxHealth}");
            Debug.Log($"Base Speed: {stats.BaseSpeed}, Effective Speed: {stats.Speed}");
            Debug.Log($"Base Mana: {stats.BaseMana}, Effective Mana: {stats.Mana}");

            // Show equipment bonuses
            var equipmentManager = demoCharacter.EquipmentManager;
            Debug.Log($"Attack Bonus: +{equipmentManager.GetAdditiveModifier(StatType.Attack)}, " +
                     $"x{1 + equipmentManager.GetMultiplicativeModifier(StatType.Attack)/100f:F2}");
        }

        private void DemonstrateEquipmentSwapping()
        {
            Debug.Log("--- Equipment Swapping Demo ---");

            // Swap to a better weapon
            var betterWeapon = equipmentDatabase.Weapons[1]; // Steel Blade
            var oldWeapon = demoCharacter.UnequipItem(EquipmentSlot.Weapon);
            demoCharacter.EquipItem(betterWeapon);
            
            Debug.Log($"Swapped {oldWeapon?.EquipmentName} for {betterWeapon.EquipmentName}");
            LogCharacterStats("After Weapon Swap");

            // Test unequipping
            var unequippedArmor = demoCharacter.UnequipItem(EquipmentSlot.Armor);
            Debug.Log($"Unequipped: {unequippedArmor?.EquipmentName}");
            LogCharacterStats("After Unequipping Armor");
        }

        private void LogCharacterStats(string context)
        {
            var stats = demoCharacter.Stats;
            Debug.Log($"{context} - HP:{stats.MaxHealth} ATK:{stats.Attack} DEF:{stats.Defense} SPD:{stats.Speed} MP:{stats.Mana}");
        }

        private void OnValidate()
        {
            // Ensure we have an equipment database reference
            if (equipmentDatabase == null)
            {
                // Try to find one in the project
                equipmentDatabase = Resources.FindObjectOfTypeAll<EquipmentDatabase>()[0];
            }
        }
    }
}