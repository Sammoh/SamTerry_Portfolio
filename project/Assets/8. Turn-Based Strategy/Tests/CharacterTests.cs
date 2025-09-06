using NUnit.Framework;
using UnityEngine;
using Sammoh.TurnBasedStrategy;

public class CharacterTests
{
    private Character character;
    private GameObject characterObject;

    [SetUp]
    public void Setup()
    {
        characterObject = new GameObject("TestCharacter");
        character = characterObject.AddComponent<Character>();
        
        var stats = new CharacterStats(100, 15, 10, 12, 50);
        var abilities = new CharacterAbility[]
        {
            new CharacterAbility("Test Attack", AbilityType.Attack, 10, 5, "Test attack ability"),
            new CharacterAbility("Test Heal", AbilityType.Heal, 20, 15, "Test heal ability")
        };
        
        character.Initialize("TestCharacter", stats, abilities, true);
    }

    [TearDown]
    public void TearDown()
    {
        if (characterObject != null)
            Object.DestroyImmediate(characterObject);
    }

    [Test]
    public void Character_Initialize_SetsPropertiesCorrectly()
    {
        Assert.AreEqual("TestCharacter", character.CharacterName);
        Assert.AreEqual(100, character.Stats.MaxHealth);
        Assert.AreEqual(15, character.Stats.BaseAttack); // Test base stats
        Assert.IsNotNull(character.Abilities);
        Assert.IsTrue(character.IsPlayerControlled);
        Assert.IsNotNull(character.EquipmentManager);
        Assert.IsNotNull(character.SkillTree);
    }

    [Test]
    public void Character_CanAct_ReturnsTrueWhenAlive()
    {
        Assert.IsTrue(character.CanAct());
    }

    [Test]
    public void Character_CanAct_ReturnsFalseWhenDead()
    {
        character.Stats.TakeDamage(200); // Kill character
        Assert.IsFalse(character.CanAct());
    }

    [Test]
    public void Character_ResetForNewTurn_RestoresMana()
    {
        character.Stats.UseMana(20); // Use some mana
        int manaBeforeReset = character.Stats.CurrentMana;
        
        character.ResetForNewTurn();
        
        Assert.AreEqual(manaBeforeReset + 5, character.Stats.CurrentMana);
    }

    [Test]
    public void Character_RestoreToFull_RestoresAllStats()
    {
        character.Stats.TakeDamage(50);
        character.Stats.UseMana(25);
        
        character.RestoreToFull();
        
        Assert.AreEqual(character.Stats.MaxHealth, character.Stats.CurrentHealth);
        Assert.AreEqual(character.Stats.Mana, character.Stats.CurrentMana);
    }

    [Test]
    public void Character_EquipItem_WorksCorrectly()
    {
        var weapon = new Equipment("Test Sword", EquipmentSlot.Weapon, 
            new StatModifier[] { new StatModifier(StatType.Attack, 5, ModifierType.Additive) }, 
            "Test weapon");

        bool result = character.EquipItem(weapon);
        
        Assert.IsTrue(result);
        Assert.AreEqual(weapon, character.EquipmentManager.GetEquippedItem(EquipmentSlot.Weapon));
        Assert.AreEqual(20, character.Stats.Attack); // 15 base + 5 from weapon
    }

    [Test]
    public void Character_UnequipItem_WorksCorrectly()
    {
        var weapon = new Equipment("Test Sword", EquipmentSlot.Weapon, 
            new StatModifier[] { new StatModifier(StatType.Attack, 5, ModifierType.Additive) }, 
            "Test weapon");

        character.EquipItem(weapon);
        Equipment unequipped = character.UnequipItem(EquipmentSlot.Weapon);
        
        Assert.AreEqual(weapon, unequipped);
        Assert.IsNull(character.EquipmentManager.GetEquippedItem(EquipmentSlot.Weapon));
        Assert.AreEqual(15, character.Stats.Attack); // Back to base attack
    }
}

public class CharacterStatsTests
{
    private CharacterStats stats;

    [SetUp]
    public void Setup()
    {
        stats = new CharacterStats(100, 15, 10, 12, 50);
    }

    [Test]
    public void CharacterStats_Constructor_SetsValuesCorrectly()
    {
        Assert.AreEqual(100, stats.MaxHealth);
        Assert.AreEqual(100, stats.CurrentHealth);
        Assert.AreEqual(15, stats.BaseAttack); // Test base stats
        Assert.AreEqual(10, stats.BaseDefense);
        Assert.AreEqual(12, stats.BaseSpeed);
        Assert.AreEqual(50, stats.BaseMana);
        Assert.AreEqual(50, stats.CurrentMana);
        Assert.IsTrue(stats.IsAlive);
    }

    [Test]
    public void CharacterStats_TakeDamage_ReducesHealthCorrectly()
    {
        stats.TakeDamage(20);
        
        // Damage reduced by defense: 20 - 10 = 10 damage taken
        Assert.AreEqual(90, stats.CurrentHealth);
    }

    [Test]
    public void CharacterStats_TakeDamage_MinimumOneDamage()
    {
        stats.TakeDamage(5); // Less than defense
        
        // Should still take 1 damage
        Assert.AreEqual(99, stats.CurrentHealth);
    }

    [Test]
    public void CharacterStats_Heal_IncreasesHealth()
    {
        stats.TakeDamage(30);
        stats.Heal(15);
        
        Assert.AreEqual(95, stats.CurrentHealth);
    }

    [Test]
    public void CharacterStats_Heal_CannotExceedMaxHealth()
    {
        stats.Heal(50);
        
        Assert.AreEqual(100, stats.CurrentHealth);
    }

    [Test]
    public void CharacterStats_UseMana_WorksWhenEnoughMana()
    {
        bool result = stats.UseMana(20);
        
        Assert.IsTrue(result);
        Assert.AreEqual(30, stats.CurrentMana);
    }

    [Test]
    public void CharacterStats_UseMana_FailsWhenNotEnoughMana()
    {
        bool result = stats.UseMana(60);
        
        Assert.IsFalse(result);
        Assert.AreEqual(50, stats.CurrentMana); // Should be unchanged
    }

    [Test]
    public void CharacterStats_IsAlive_FalseWhenHealthZero()
    {
        stats.TakeDamage(200);
        
        Assert.IsFalse(stats.IsAlive);
        Assert.AreEqual(0, stats.CurrentHealth);
    }
}