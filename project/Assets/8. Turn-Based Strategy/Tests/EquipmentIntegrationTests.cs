using NUnit.Framework;
using UnityEngine;
using Sammoh.TurnBasedStrategy;

public class EquipmentIntegrationTests
{
    private GameObject factoryObject;
    private CharacterFactory factory;

    [SetUp]
    public void SetUp()
    {
        factoryObject = new GameObject("TestFactory");
        factory = factoryObject.AddComponent<CharacterFactory>();
    }

    [TearDown]
    public void TearDown()
    {
        if (factoryObject != null)
            Object.DestroyImmediate(factoryObject);
    }

    [Test]
    public void Integration_CreatedCharacters_HaveWorkingEquipmentSystem()
    {
        // Create characters of different classes
        Character warrior = factory.CreateCharacter(CharacterClass.Warrior, "TestWarrior", true);
        Character mage = factory.CreateCharacter(CharacterClass.Mage, "TestMage", false);

        // Verify they have default equipment
        Assert.IsNotNull(warrior.GetEquippedItem(EquipmentSlot.Weapon));
        Assert.IsNotNull(warrior.GetEquippedItem(EquipmentSlot.Armor));
        Assert.IsNotNull(mage.GetEquippedItem(EquipmentSlot.Weapon));
        Assert.IsNotNull(mage.GetEquippedItem(EquipmentSlot.Armor));

        // Create custom equipment and test swapping
        Equipment testWeapon = new Equipment(
            "Test Blade", 
            "A test weapon", 
            EquipmentSlot.Weapon, 
            EquipmentRarity.Common,
            new EquipmentStats(0, 15, 0, 0, 0)
        );

        // Test equipment swapping affects stats
        int warriorInitialAttack = warrior.Stats.Attack;
        warrior.EquipItem(testWeapon);
        
        // The new weapon should change the attack value
        Assert.AreNotEqual(warriorInitialAttack, warrior.Stats.Attack);
        
        // Clean up
        Object.DestroyImmediate(warrior.gameObject);
        Object.DestroyImmediate(mage.gameObject);
    }

    [Test]
    public void Integration_EquipmentAffectsCombat_DamageCalculation()
    {
        Character attacker = factory.CreateCharacter(CharacterClass.Warrior, "Attacker", true);
        Character defender = factory.CreateCharacter(CharacterClass.Mage, "Defender", false);

        // Get initial stats
        int initialDefenderHealth = defender.Stats.CurrentHealth;
        int attackerAttackPower = attacker.Stats.Attack;

        // Create a test ability
        CharacterAbility testAttack = new CharacterAbility("Test Attack", AbilityType.Attack, 10, 0, "Test");

        // Execute ability
        int damage = testAttack.Execute(attacker.Stats, defender.Stats);

        // Verify damage was calculated using equipment-modified stats
        int expectedDamage = 10 + attackerAttackPower; // ability power + character attack (including equipment)
        int actualDamageDealt = initialDefenderHealth - defender.Stats.CurrentHealth;

        Assert.Greater(damage, 10); // Should be more than base ability power due to equipment
        Assert.Greater(actualDamageDealt, 0); // Some damage should have been dealt
        
        // Clean up
        Object.DestroyImmediate(attacker.gameObject);
        Object.DestroyImmediate(defender.gameObject);
    }

    [Test]
    public void Integration_EquipmentPersists_ThroughStatUpdates()
    {
        Character character = factory.CreateCharacter(CharacterClass.Healer, "TestHealer");
        
        // Record initial stats with equipment
        int initialMaxHealth = character.Stats.MaxHealth;
        int initialAttack = character.Stats.Attack;
        
        // Damage and heal the character
        character.Stats.TakeDamage(50);
        character.Stats.Heal(25);
        
        // Stats should still reflect equipment bonuses
        Assert.AreEqual(initialMaxHealth, character.Stats.MaxHealth);
        Assert.AreEqual(initialAttack, character.Stats.Attack);
        
        // Equipment should still be there
        Assert.IsNotNull(character.GetEquippedItem(EquipmentSlot.Weapon));
        Assert.IsNotNull(character.GetEquippedItem(EquipmentSlot.Armor));
        
        // Clean up
        Object.DestroyImmediate(character.gameObject);
    }

    [Test]
    public void Integration_ClassSpecificEquipment_ProperlyAssigned()
    {
        Character warrior = factory.CreateCharacter(CharacterClass.Warrior, "Warrior");
        Character mage = factory.CreateCharacter(CharacterClass.Mage, "Mage");
        Character rogue = factory.CreateCharacter(CharacterClass.Rogue, "Rogue");
        Character healer = factory.CreateCharacter(CharacterClass.Healer, "Healer");

        // Verify each class has appropriate default equipment names
        Assert.AreEqual("Iron Sword", warrior.GetEquippedItem(EquipmentSlot.Weapon).EquipmentName);
        Assert.AreEqual("Oak Staff", mage.GetEquippedItem(EquipmentSlot.Weapon).EquipmentName);
        Assert.AreEqual("Steel Dagger", rogue.GetEquippedItem(EquipmentSlot.Weapon).EquipmentName);
        Assert.AreEqual("Healing Rod", healer.GetEquippedItem(EquipmentSlot.Weapon).EquipmentName);

        Assert.AreEqual("Leather Armor", warrior.GetEquippedItem(EquipmentSlot.Armor).EquipmentName);
        Assert.AreEqual("Mage Robes", mage.GetEquippedItem(EquipmentSlot.Armor).EquipmentName);
        Assert.AreEqual("Studded Leather", rogue.GetEquippedItem(EquipmentSlot.Armor).EquipmentName);
        Assert.AreEqual("Blessed Garments", healer.GetEquippedItem(EquipmentSlot.Armor).EquipmentName);

        // Clean up
        Object.DestroyImmediate(warrior.gameObject);
        Object.DestroyImmediate(mage.gameObject);
        Object.DestroyImmediate(rogue.gameObject);
        Object.DestroyImmediate(healer.gameObject);
    }
}