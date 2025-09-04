using NUnit.Framework;
using UnityEngine;
using Sammoh.TurnBasedStrategy;

public class CharacterAbilityTests
{
    private CharacterStats casterStats;
    private CharacterStats targetStats;

    [SetUp]
    public void Setup()
    {
        casterStats = new CharacterStats(100, 15, 10, 12, 50);
        targetStats = new CharacterStats(80, 12, 8, 10, 40);
    }

    [Test]
    public void CharacterAbility_Constructor_SetsPropertiesCorrectly()
    {
        var ability = new CharacterAbility("Fireball", AbilityType.Attack, 25, 15, "A fiery attack");
        
        Assert.AreEqual("Fireball", ability.AbilityName);
        Assert.AreEqual(AbilityType.Attack, ability.AbilityType);
        Assert.AreEqual(25, ability.Power);
        Assert.AreEqual(15, ability.ManaCost);
        Assert.AreEqual("A fiery attack", ability.Description);
    }

    [Test]
    public void CharacterAbility_CanUse_ReturnsTrueWhenEnoughMana()
    {
        var ability = new CharacterAbility("Test", AbilityType.Attack, 10, 20, "Test");
        
        Assert.IsTrue(ability.CanUse(casterStats));
    }

    [Test]
    public void CharacterAbility_CanUse_ReturnsFalseWhenNotEnoughMana()
    {
        var ability = new CharacterAbility("Expensive", AbilityType.Attack, 10, 60, "Too expensive");
        
        Assert.IsFalse(ability.CanUse(casterStats));
    }

    [Test]
    public void CharacterAbility_CanUse_ReturnsFalseWhenDead()
    {
        var ability = new CharacterAbility("Test", AbilityType.Attack, 10, 20, "Test");
        casterStats.TakeDamage(200); // Kill character
        
        Assert.IsFalse(ability.CanUse(casterStats));
    }

    [Test]
    public void CharacterAbility_Execute_Attack_DealsDamage()
    {
        var ability = new CharacterAbility("Slash", AbilityType.Attack, 10, 5, "Basic attack");
        
        int result = ability.Execute(casterStats, targetStats);
        
        // Should deal: ability power (10) + caster attack (15) = 25 damage
        // Target takes: 25 - 8 defense = 17 damage
        // Target health: 80 - 17 = 63
        Assert.AreEqual(25, result); // Returns raw damage before defense
        Assert.AreEqual(63, targetStats.CurrentHealth);
        Assert.AreEqual(45, casterStats.CurrentMana); // 50 - 5 mana cost
    }

    [Test]
    public void CharacterAbility_Execute_Heal_RestoresHealth()
    {
        var ability = new CharacterAbility("Heal", AbilityType.Heal, 20, 10, "Healing spell");
        casterStats.TakeDamage(30); // Reduce health to 80
        
        int result = ability.Execute(casterStats);
        
        Assert.AreEqual(20, result);
        Assert.AreEqual(100, casterStats.CurrentHealth); // 80 + 20 = 100 (capped at max)
        Assert.AreEqual(40, casterStats.CurrentMana); // 50 - 10 mana cost
    }

    [Test]
    public void CharacterAbility_Execute_Defend_ReturnsDefenseValue()
    {
        var ability = new CharacterAbility("Shield", AbilityType.Defend, 15, 8, "Defensive stance");
        
        int result = ability.Execute(casterStats);
        
        Assert.AreEqual(15, result);
        Assert.AreEqual(42, casterStats.CurrentMana); // 50 - 8 mana cost
    }

    [Test]
    public void CharacterAbility_Execute_Special_WorksLikeAttack()
    {
        var ability = new CharacterAbility("Special Move", AbilityType.Special, 30, 20, "Unique ability");
        
        int result = ability.Execute(casterStats, targetStats);
        
        Assert.AreEqual(30, result);
        // Target should take 30 - 8 defense = 22 damage
        Assert.AreEqual(58, targetStats.CurrentHealth); // 80 - 22 = 58
        Assert.AreEqual(30, casterStats.CurrentMana); // 50 - 20 mana cost
    }

    [Test]
    public void CharacterAbility_Execute_FailsWithoutEnoughMana()
    {
        var ability = new CharacterAbility("Expensive", AbilityType.Attack, 10, 60, "Too expensive");
        
        int result = ability.Execute(casterStats, targetStats);
        
        Assert.AreEqual(0, result);
        Assert.AreEqual(80, targetStats.CurrentHealth); // No damage dealt
        Assert.AreEqual(50, casterStats.CurrentMana); // No mana consumed
    }
}