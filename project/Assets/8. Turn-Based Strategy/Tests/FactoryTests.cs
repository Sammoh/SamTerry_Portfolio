using NUnit.Framework;
using UnityEngine;
using Sammoh.TurnBasedStrategy;

public class FactoryTests
{
    private GameObject factoryObject;
    private CharacterFactory factory;

    [SetUp]
    public void Setup()
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
    public void CharacterFactory_CreateCharacter_Warrior_HasCorrectStats()
    {
        Character character = factory.CreateCharacter(CharacterClass.Warrior, "TestWarrior", true);
        
        Assert.IsNotNull(character);
        Assert.AreEqual("TestWarrior", character.CharacterName);
        Assert.AreEqual(CharacterClass.Warrior, character.CharacterClass);
        
        // Base stats + equipment bonuses (Iron Sword: +5 attack, Leather Armor: +15 health, +8 defense, -1 speed)
        Assert.AreEqual(135, character.Stats.MaxHealth); // 120 + 15
        Assert.AreEqual(25, character.Stats.Attack); // 20 + 5
        Assert.AreEqual(23, character.Stats.Defense); // 15 + 8
        Assert.AreEqual(7, character.Stats.Speed); // 8 - 1
        Assert.AreEqual(30, character.Stats.Mana);
        Assert.IsTrue(character.IsPlayerControlled);
        
        // Verify equipment is equipped
        Assert.IsNotNull(character.GetEquippedItem(EquipmentSlot.Weapon));
        Assert.IsNotNull(character.GetEquippedItem(EquipmentSlot.Armor));
        
        // Clean up
        Object.DestroyImmediate(character.gameObject);
    }

    [Test]
    public void CharacterFactory_CreateCharacter_Mage_HasCorrectStats()
    {
        Character character = factory.CreateCharacter(CharacterClass.Mage, "TestMage", false);
        
        Assert.IsNotNull(character);
        Assert.AreEqual("TestMage", character.CharacterName);
        Assert.AreEqual(CharacterClass.Mage, character.CharacterClass);
        
        // Base stats + equipment bonuses (Oak Staff: +2 attack, +10 mana; Mage Robes: +5 health, +2 defense, +1 speed, +15 mana)
        Assert.AreEqual(85, character.Stats.MaxHealth); // 80 + 5
        Assert.AreEqual(12, character.Stats.Attack); // 10 + 2
        Assert.AreEqual(7, character.Stats.Defense); // 5 + 2
        Assert.AreEqual(11, character.Stats.Speed); // 10 + 1
        Assert.AreEqual(105, character.Stats.Mana); // 80 + 10 + 15
        Assert.IsFalse(character.IsPlayerControlled);
        
        // Clean up
        Object.DestroyImmediate(character.gameObject);
    }

    [Test]
    public void CharacterFactory_CreateCharacter_Rogue_HasCorrectStats()
    {
        Character character = factory.CreateCharacter(CharacterClass.Rogue, "TestRogue");
        
        Assert.IsNotNull(character);
        Assert.AreEqual("TestRogue", character.CharacterName);
        Assert.AreEqual(CharacterClass.Rogue, character.CharacterClass);
        
        // Base stats + equipment bonuses (Steel Dagger: +4 attack, +2 speed; Studded Leather: +8 health, +4 defense, +3 speed)
        Assert.AreEqual(98, character.Stats.MaxHealth); // 90 + 8
        Assert.AreEqual(22, character.Stats.Attack); // 18 + 4
        Assert.AreEqual(12, character.Stats.Defense); // 8 + 4
        Assert.AreEqual(21, character.Stats.Speed); // 16 + 2 + 3
        Assert.AreEqual(40, character.Stats.Mana);
        
        // Clean up
        Object.DestroyImmediate(character.gameObject);
    }

    [Test]
    public void CharacterFactory_CreateCharacter_Healer_HasCorrectStats()
    {
        Character character = factory.CreateCharacter(CharacterClass.Healer, "TestHealer");
        
        Assert.IsNotNull(character);
        Assert.AreEqual("TestHealer", character.CharacterName);
        Assert.AreEqual(CharacterClass.Healer, character.CharacterClass);
        
        // Base stats + equipment bonuses (Healing Rod: +5 health, +1 attack, +2 defense, +8 mana; Blessed Garments: +10 health, +5 defense, +10 mana)
        Assert.AreEqual(115, character.Stats.MaxHealth); // 100 + 5 + 10
        Assert.AreEqual(9, character.Stats.Attack); // 8 + 1
        Assert.AreEqual(19, character.Stats.Defense); // 12 + 2 + 5
        Assert.AreEqual(12, character.Stats.Speed);
        Assert.AreEqual(88, character.Stats.Mana); // 70 + 8 + 10
        
        // Clean up
        Object.DestroyImmediate(character.gameObject);
    }

    [Test]
    public void CharacterFactory_CreateCharacter_Warrior_HasCorrectAbilities()
    {
        Character character = factory.CreateCharacter(CharacterClass.Warrior, "TestWarrior");
        
        Assert.AreEqual(3, character.Abilities.Length);
        Assert.AreEqual("Slash", character.Abilities[0].AbilityName);
        Assert.AreEqual("Shield Block", character.Abilities[1].AbilityName);
        Assert.AreEqual("Berserker Rage", character.Abilities[2].AbilityName);
        
        Assert.AreEqual(AbilityType.Attack, character.Abilities[0].AbilityType);
        Assert.AreEqual(AbilityType.Defend, character.Abilities[1].AbilityType);
        Assert.AreEqual(AbilityType.Special, character.Abilities[2].AbilityType);
        
        // Clean up
        Object.DestroyImmediate(character.gameObject);
    }

    [Test]
    public void CharacterFactory_CreateCharacter_Mage_HasCorrectAbilities()
    {
        Character character = factory.CreateCharacter(CharacterClass.Mage, "TestMage");
        
        Assert.AreEqual(3, character.Abilities.Length);
        Assert.AreEqual("Magic Missile", character.Abilities[0].AbilityName);
        Assert.AreEqual("Healing Light", character.Abilities[1].AbilityName);
        Assert.AreEqual("Fireball", character.Abilities[2].AbilityName);
        
        Assert.AreEqual(AbilityType.Attack, character.Abilities[0].AbilityType);
        Assert.AreEqual(AbilityType.Heal, character.Abilities[1].AbilityType);
        Assert.AreEqual(AbilityType.Special, character.Abilities[2].AbilityType);
        
        // Clean up
        Object.DestroyImmediate(character.gameObject);
    }

    [Test]
    public void CharacterFactory_CreateCharacter_WithParent_SetsParentCorrectly()
    {
        GameObject parent = new GameObject("Parent");
        Character character = factory.CreateCharacter(CharacterClass.Warrior, "TestWarrior", false, parent.transform);
        
        Assert.AreEqual(parent.transform, character.transform.parent);
        
        // Clean up
        Object.DestroyImmediate(parent);
        Object.DestroyImmediate(character.gameObject);
    }

    [Test]
    public void CharacterFactory_CreateCharacter_SetsGameObjectName()
    {
        Character character = factory.CreateCharacter(CharacterClass.Rogue, "NamedRogue");
        
        Assert.AreEqual("NamedRogue", character.gameObject.name);
        
        // Clean up
        Object.DestroyImmediate(character.gameObject);
    }

    [Test]
    public void CharacterFactory_CreateCharacter_EquipsDefaultEquipment()
    {
        Character character = factory.CreateCharacter(CharacterClass.Warrior, "TestWarrior");
        
        // Verify default equipment is equipped
        Equipment weapon = character.GetEquippedItem(EquipmentSlot.Weapon);
        Equipment armor = character.GetEquippedItem(EquipmentSlot.Armor);
        
        Assert.IsNotNull(weapon);
        Assert.IsNotNull(armor);
        Assert.AreEqual("Iron Sword", weapon.EquipmentName);
        Assert.AreEqual("Leather Armor", armor.EquipmentName);
        Assert.AreEqual(EquipmentSlot.Weapon, weapon.Slot);
        Assert.AreEqual(EquipmentSlot.Armor, armor.Slot);
        
        // Clean up
        Object.DestroyImmediate(character.gameObject);
    }
}