using UnityEngine;

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
            equipmentDatabase.CreateDefaultEquipment();
            Debug.Log("Created temporary equipment database for demo");
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
                equipmentDatabase = Resources.FindObjectsOfTypeAll<EquipmentDatabase>()[0];
            }
        }
    }
}