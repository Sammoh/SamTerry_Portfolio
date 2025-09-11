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
        Assert.AreEqual(15, character.Stats.Attack);
        Assert.AreEqual(2, character.Abilities.Length);
        Assert.IsTrue(character.IsPlayerControlled);
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
        Assert.AreEqual(15, stats.Attack);
        Assert.AreEqual(10, stats.Defense);
        Assert.AreEqual(12, stats.Speed);
        Assert.AreEqual(50, stats.Mana);
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

public class CharacterEquipmentIntegrationTests
{
    private Character character;
    private GameObject characterObject;
    private Equipment testWeapon;
    private Equipment testArmor;

    [SetUp]
    public void Setup()
    {
        characterObject = new GameObject("TestCharacter");
        character = characterObject.AddComponent<Character>();
        
        var stats = new CharacterStats(100, 15, 10, 12, 50);
        var abilities = new CharacterAbility[]
        {
            new CharacterAbility("Test Attack", AbilityType.Attack, 10, 5, "Test attack ability")
        };
        
        character.Initialize("TestCharacter", stats, abilities, true);
        
        var testSword = ScriptableObject.CreateInstance<Equipment>();
        testSword.name = "Test Sword";
        testSword.EquipmentName = "Test Sword";
        testSword.Slot = EquipmentSlot.Weapon;
        testSword.StatModifiers = new StatModifier[] {
            new StatModifier(StatType.Attack, 5, ModifierType.Flat)
        };
        testSword.Description = "A test weapon";
        testWeapon = testSword;
        
        var testArmorSo = ScriptableObject.CreateInstance<Equipment>();
        testArmorSo.name = "Test Armor";
        testArmorSo.EquipmentName = "Test Armor";
        testArmorSo.Slot = EquipmentSlot.Armor;
        testArmorSo.StatModifiers = new StatModifier[] {
            new StatModifier(StatType.Defense, 3, ModifierType.Flat),
            new StatModifier(StatType.MaxHealth, 20, ModifierType.Flat)
        };
        testArmorSo.Description = "Test armor piece";
        testArmor = testArmorSo;
    }

    [TearDown]
    public void TearDown()
    {
        if (characterObject != null)
            Object.DestroyImmediate(characterObject);
    }

    [Test]
    public void Character_EquipItem_IncreasesEffectiveStats()
    {
        int baseAttack = character.Stats.BaseAttack;
        
        character.EquipItem(testWeapon);
        
        Assert.AreEqual(baseAttack + 5, character.Stats.Attack);
        Assert.AreEqual(baseAttack, character.Stats.BaseAttack); // Base should be unchanged
    }

    [Test]
    public void Character_EquipmentManager_NotNull()
    {
        Assert.IsNotNull(character.EquipmentManager);
    }

    [Test]
    public void Character_EquipItem_ReturnsTrue()
    {
        bool result = character.EquipItem(testWeapon);
        Assert.IsTrue(result);
    }

    [Test]
    public void Character_UnequipItem_ReturnsEquippedItem()
    {
        character.EquipItem(testWeapon);
        var unequippedItem = character.UnequipItem(EquipmentSlot.Weapon);
        
        Assert.AreEqual(testWeapon, unequippedItem);
    }

    [Test]
    public void Character_Stats_UsesEffectiveDefenseForDamageCalculation()
    {
        // Take damage without armor
        character.Stats.TakeDamage(20);
        int healthAfterFirstDamage = character.Stats.CurrentHealth;
        
        // Restore health and equip armor
        character.RestoreToFull();
        character.EquipItem(testArmor);
        
        // Take same damage with armor
        character.Stats.TakeDamage(20);
        int healthAfterSecondDamage = character.Stats.CurrentHealth;
        
        // Should take less damage with armor equipped
        Assert.Greater(healthAfterSecondDamage, healthAfterFirstDamage);
    }

    [Test]
    public void Character_Stats_UsesEffectiveMaxHealthForHealing()
    {
        character.EquipItem(testArmor); // +20 max health
        character.Stats.TakeDamage(50);
        character.Stats.Heal(100); // Try to heal more than base max health
        
        // Should heal to the new effective max health (120)
        Assert.AreEqual(120, character.Stats.CurrentHealth);
        Assert.AreEqual(120, character.Stats.MaxHealth);
    }

    [Test]
    public void Character_RestoreToFull_UsesEffectiveMaxValues()
    {
        character.EquipItem(testArmor); // +20 max health
        character.Stats.TakeDamage(50);
        
        character.RestoreToFull();
        
        Assert.AreEqual(120, character.Stats.CurrentHealth); // Should restore to effective max
        Assert.AreEqual(100, character.Stats.BaseMaxHealth); // Base should be unchanged
    }

    [Test]
    public void Character_BaseStats_UnchangedByEquipment()
    {
        int baseAttack = character.Stats.BaseAttack;
        int baseDefense = character.Stats.BaseDefense;
        int baseMaxHealth = character.Stats.BaseMaxHealth;
        
        character.EquipItem(testWeapon);
        character.EquipItem(testArmor);
        
        // Base stats should remain unchanged
        Assert.AreEqual(baseAttack, character.Stats.BaseAttack);
        Assert.AreEqual(baseDefense, character.Stats.BaseDefense);
        Assert.AreEqual(baseMaxHealth, character.Stats.BaseMaxHealth);
    }

    [Test]
    public void Character_EquipmentWithoutManager_HandledGracefully()
    {
        // This test ensures backward compatibility
        var manualStats = new CharacterStats(100, 15, 10, 12, 50);
        // Don't set equipment manager
        
        // Should work without equipment manager (base stats only)
        Assert.AreEqual(15, manualStats.Attack);
        Assert.AreEqual(10, manualStats.Defense);
        Assert.AreEqual(100, manualStats.MaxHealth);
    }
}