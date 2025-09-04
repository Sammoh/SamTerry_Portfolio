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
        Assert.AreEqual(120, character.Stats.MaxHealth);
        Assert.AreEqual(20, character.Stats.Attack);
        Assert.AreEqual(15, character.Stats.Defense);
        Assert.AreEqual(8, character.Stats.Speed);
        Assert.AreEqual(30, character.Stats.Mana);
        Assert.IsTrue(character.IsPlayerControlled);
        
        // Clean up
        Object.DestroyImmediate(character.gameObject);
    }

    [Test]
    public void CharacterFactory_CreateCharacter_Mage_HasCorrectStats()
    {
        Character character = factory.CreateCharacter(CharacterClass.Mage, "TestMage", false);
        
        Assert.IsNotNull(character);
        Assert.AreEqual("TestMage", character.CharacterName);
        Assert.AreEqual(80, character.Stats.MaxHealth);
        Assert.AreEqual(10, character.Stats.Attack);
        Assert.AreEqual(5, character.Stats.Defense);
        Assert.AreEqual(10, character.Stats.Speed);
        Assert.AreEqual(80, character.Stats.Mana);
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
        Assert.AreEqual(90, character.Stats.MaxHealth);
        Assert.AreEqual(18, character.Stats.Attack);
        Assert.AreEqual(8, character.Stats.Defense);
        Assert.AreEqual(16, character.Stats.Speed);
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
        Assert.AreEqual(100, character.Stats.MaxHealth);
        Assert.AreEqual(8, character.Stats.Attack);
        Assert.AreEqual(12, character.Stats.Defense);
        Assert.AreEqual(12, character.Stats.Speed);
        Assert.AreEqual(70, character.Stats.Mana);
        
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
}