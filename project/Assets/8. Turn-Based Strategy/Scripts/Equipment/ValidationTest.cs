using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    /// <summary>
    /// Simple validation script to test the integrated equipment system.
    /// This can be added to a GameObject for runtime testing.
    /// </summary>
    public class ValidationTest : MonoBehaviour
    {
        [Header("Test Settings")]
        [SerializeField] private bool runTestOnStart = false;
        
        private void Start()
        {
            if (runTestOnStart)
            {
                RunValidationTest();
            }
        }
        
        [ContextMenu("Run Validation Test")]
        public void RunValidationTest()
        {
            Debug.Log("=== Equipment System Validation Test ===");
            
            TestLegacyEquipment();
            TestScriptableObjectEquipment();
            TestEquipmentManager();
            
            Debug.Log("=== Validation Test Complete ===");
        }
        
        private void TestLegacyEquipment()
        {
            Debug.Log("--- Testing Legacy Equipment ---");
            
            // Create a legacy weapon
            var legacyWeapon = new LegacyEquipment("Test Legacy Sword", EquipmentSlot.Weapon,
                new StatModifier[] {
                    new StatModifier(StatType.Attack, 10, ModifierType.Additive),
                    new StatModifier(StatType.Speed, 15, ModifierType.Multiplicative)
                },
                "A test weapon for validation");
            
            Debug.Log($"Created legacy weapon: {legacyWeapon.EquipmentName}");
            Debug.Log($"  Slot: {legacyWeapon.Slot}");
            Debug.Log($"  Modifiers: {legacyWeapon.StatModifiers.Length}");
            
            // Test stat modification
            float baseAttack = 10f;
            float modifiedAttack = legacyWeapon.GetModifiedValue(StatType.Attack, baseAttack);
            Debug.Log($"  Base Attack: {baseAttack}, Modified: {modifiedAttack}");
            
            float baseSpeed = 5f;
            float modifiedSpeed = legacyWeapon.GetModifiedValue(StatType.Speed, baseSpeed);
            Debug.Log($"  Base Speed: {baseSpeed}, Modified: {modifiedSpeed}");
        }
        
        private void TestScriptableObjectEquipment()
        {
            Debug.Log("--- Testing ScriptableObject Equipment Creation ---");
            
            // Create a weapon programmatically
            var weapon = ScriptableObject.CreateInstance<WeaponSO>();
            weapon.name = "Test_Validation_Sword";
            weapon.EquipmentName = "Validation Sword";
            weapon.WeaponType = WeaponType.Sword;
            weapon.Rarity = EquipmentRarity.Common;
            weapon.Damage = 12;
            weapon.AttackSpeed = 1.0f;
            weapon.Range = 1.2f;
            weapon.CriticalChance = 5;
            weapon.CriticalMultiplier = 1.5f;
            weapon.Slot = EquipmentSlot.Weapon;
            
            Debug.Log($"Created ScriptableObject weapon: {weapon.EquipmentName}");
            Debug.Log($"  Type: {weapon.Type}");
            Debug.Log($"  Damage: {weapon.Damage}");
            Debug.Log($"  DPS: {weapon.CalculateDPS():F2}");
            Debug.Log($"  ID: {weapon.Id}");
            
            // Create an armor programmatically
            var armor = ScriptableObject.CreateInstance<ArmorSO>();
            armor.name = "Test_Validation_Armor";
            armor.EquipmentName = "Validation Chainmail";
            armor.ArmorType = ArmorType.Chestplate;
            armor.ArmorMaterial = ArmorMaterial.Chainmail;
            armor.Rarity = EquipmentRarity.Common;
            armor.Defense = 8;
            armor.MagicResistance = 3;
            armor.Slot = EquipmentSlot.Armor;
            
            Debug.Log($"Created ScriptableObject armor: {armor.EquipmentName}");
            Debug.Log($"  Material: {armor.ArmorMaterial}");
            Debug.Log($"  Defense: {armor.Defense}");
            Debug.Log($"  Magic Resistance: {armor.MagicResistance}");
            
            // Test backward compatibility
            var legacyEquipment = weapon.ToLegacyEquipment();
            Debug.Log($"Converted to legacy: {legacyEquipment.EquipmentName}");
        }
        
        private void TestEquipmentManager()
        {
            Debug.Log("--- Testing Equipment Manager ---");
            
            var manager = new EquipmentManager();
            
            // Test legacy equipment
            var legacyWeapon = new LegacyEquipment("Manager Test Sword", EquipmentSlot.Weapon,
                new StatModifier[] {
                    new StatModifier(StatType.Attack, 8, ModifierType.Additive)
                },
                "Test weapon for manager");
            
            manager.EquipItem(legacyWeapon);
            Debug.Log($"Equipped legacy weapon: {manager.Weapon?.EquipmentName ?? "None"}");
            
            // Test ScriptableObject equipment
            var soWeapon = ScriptableObject.CreateInstance<WeaponSO>();
            soWeapon.name = "SO_Test_Weapon";
            soWeapon.EquipmentName = "SO Test Weapon";
            soWeapon.Slot = EquipmentSlot.Weapon;
            soWeapon.StatModifiers = new StatModifier[] {
                new StatModifier(StatType.Attack, 12, ModifierType.Additive)
            };
            
            manager.EquipWeapon(soWeapon);
            Debug.Log($"Equipped SO weapon: {manager.WeaponSO?.EquipmentName ?? "None"}");
            Debug.Log($"Legacy weapon cleared: {(manager.Weapon == null ? "Yes" : "No")}");
            
            // Test stat calculations
            float baseAttack = 10f;
            float modifiedAttack = manager.CalculateModifiedStat(StatType.Attack, baseAttack);
            Debug.Log($"Manager stat calculation - Base: {baseAttack}, Modified: {modifiedAttack}");
            
            var allEquipped = manager.GetAllEquippedItemsSO();
            Debug.Log($"Total SO equipment equipped: {allEquipped.Count}");
        }
    }
}