using NUnit.Framework;
using UnityEngine;
using Sammoh.TurnBasedStrategy;

public class EquipmentSystemValidationTests
{
    private CharacterFactory characterFactory;
    private GameObject factoryGameObject;

    [SetUp]
    public void Setup()
    {
        factoryGameObject = new GameObject("CharacterFactory");
        characterFactory = factoryGameObject.AddComponent<CharacterFactory>();
    }

    [TearDown] 
    public void TearDown()
    {
        if (factoryGameObject != null) Object.DestroyImmediate(factoryGameObject);
    }

    [Test]
    public void CharacterFactory_CreatedCharacter_HasEquipmentManager()
    {
        var character = characterFactory.CreateCharacter(CharacterClass.Warrior, "Test Warrior", true);
        
        Assert.IsNotNull(character);
        Assert.IsNotNull(character.EquipmentManager);
        Assert.IsNotNull(character.Stats);
        
        // Verify equipment manager is connected to stats
        int baseAttack = character.Stats.Attack;
        
        // Create and equip a weapon
        var weapon = ScriptableObject.CreateInstance<Equipment>();
        weapon.Initialize("Test Weapon", EquipmentSlot.Weapon,
            new StatModifier[] { new StatModifier(StatType.Attack, 10, ModifierType.Additive) },
            "Test weapon");
            
        character.EquipItem(weapon);
        
        Assert.AreEqual(baseAttack + 10, character.Stats.Attack);
        
        // Clean up
        Object.DestroyImmediate(weapon);
        Object.DestroyImmediate(character.gameObject);
    }

    [Test]
    public void EquipmentSystem_CompleteWorkflow_IntegratesCorrectly()
    {
        // Create a character
        var character = characterFactory.CreateCharacter(CharacterClass.Mage, "Test Mage", true);
        
        // Record initial stats
        int initialAttack = character.Stats.Attack;
        int initialDefense = character.Stats.Defense;
        int initialMana = character.Stats.Mana;
        
        // Create equipment with various modifiers
        var staff = ScriptableObject.CreateInstance<Equipment>();
        staff.Initialize("Magic Staff", EquipmentSlot.Weapon,
            new StatModifier[] {
                new StatModifier(StatType.Attack, 5, ModifierType.Additive),
                new StatModifier(StatType.Mana, 25, ModifierType.Multiplicative)
            },
            "A staff that enhances magical power");

        var robes = ScriptableObject.CreateInstance<Equipment>();
        robes.Initialize("Enchanted Robes", EquipmentSlot.Armor,
            new StatModifier[] {
                new StatModifier(StatType.Defense, 3, ModifierType.Additive),
                new StatModifier(StatType.Mana, 15, ModifierType.Additive)
            },
            "Robes that provide protection and mana");

        // Equip items
        character.EquipItem(staff);
        character.EquipItem(robes);

        // Verify combined effects
        int expectedAttack = initialAttack + 5;
        int expectedDefense = initialDefense + 3;
        int expectedMana = Mathf.RoundToInt((initialMana + 15) * 1.25f); // Additive first, then multiplicative

        Assert.AreEqual(expectedAttack, character.Stats.Attack, "Attack calculation incorrect");
        Assert.AreEqual(expectedDefense, character.Stats.Defense, "Defense calculation incorrect");
        Assert.AreEqual(expectedMana, character.Stats.Mana, "Mana calculation incorrect");

        // Test unequipping
        character.UnequipItem(EquipmentSlot.Weapon);
        
        // Attack should return to initial + robes bonus only
        Assert.AreEqual(initialAttack, character.Stats.Attack, "Attack should return to base after unequipping weapon");
        
        // Mana should be initial + robes bonus only
        int expectedManaWithoutStaff = initialMana + 15;
        Assert.AreEqual(expectedManaWithoutStaff, character.Stats.Mana, "Mana should only have robes bonus");

        // Clean up
        Object.DestroyImmediate(staff);
        Object.DestroyImmediate(robes);
        Object.DestroyImmediate(character.gameObject);
    }

    [Test]
    public void EquipmentSystem_StatModifierOrder_CalculatesCorrectly()
    {
        // Test that additive modifiers are applied before multiplicative ones
        var character = characterFactory.CreateCharacter(CharacterClass.Warrior, "Test Warrior", true);
        
        int baseAttack = character.Stats.BaseAttack; // Should be 20 for Warrior
        
        var complexWeapon = ScriptableObject.CreateInstance<Equipment>();
        complexWeapon.Initialize("Complex Weapon", EquipmentSlot.Weapon,
            new StatModifier[] {
                new StatModifier(StatType.Attack, 10, ModifierType.Additive),    // +10
                new StatModifier(StatType.Attack, 50, ModifierType.Multiplicative) // +50%
            },
            "Weapon with both additive and multiplicative bonuses");

        character.EquipItem(complexWeapon);

        // Expected: (base + additive) * (1 + multiplicative/100)
        // (20 + 10) * (1 + 50/100) = 30 * 1.5 = 45
        int expectedAttack = Mathf.RoundToInt((baseAttack + 10) * 1.5f);
        
        Assert.AreEqual(expectedAttack, character.Stats.Attack, 
            $"Expected {expectedAttack} but got {character.Stats.Attack}. " +
            $"Base: {baseAttack}, should be (base+10)*1.5");

        // Clean up
        Object.DestroyImmediate(complexWeapon);
        Object.DestroyImmediate(character.gameObject);
    }

    [Test]
    public void EquipmentSystem_ErrorHandling_DoesNotBreak()
    {
        var character = characterFactory.CreateCharacter(CharacterClass.Rogue, "Test Rogue", true);

        // Try equipping null - should not crash
        Assert.DoesNotThrow(() => character.EquipItem(null));

        // Try unequipping from empty slot - should not crash
        Assert.DoesNotThrow(() => character.UnequipItem(EquipmentSlot.Weapon));

        // Create equipment with extreme values - should be clamped
        var extremeEquipment = ScriptableObject.CreateInstance<Equipment>();
        extremeEquipment.Initialize("Extreme Item", EquipmentSlot.Accessory,
            new StatModifier[] {
                new StatModifier(StatType.MaxHealth, -200, ModifierType.Multiplicative)
            },
            "Equipment that should result in minimum health");

        character.EquipItem(extremeEquipment);

        // Health should be clamped to minimum (1)
        Assert.GreaterOrEqual(character.Stats.MaxHealth, 1, "MaxHealth should be clamped to minimum value");

        // Clean up
        Object.DestroyImmediate(extremeEquipment);
        Object.DestroyImmediate(character.gameObject);
    }
}