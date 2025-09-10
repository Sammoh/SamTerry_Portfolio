using NUnit.Framework;
using UnityEngine;
using Sammoh.TurnBasedStrategy;

public class EquipmentIntegrationTests
{
    private Character testCharacter;
    private GameObject testGameObject;
    private Equipment testWeapon;
    private Equipment testArmor;

    [SetUp]
    public void Setup()
    {
        // Create test game object and character
        testGameObject = new GameObject("TestCharacter");
        testCharacter = testGameObject.AddComponent<Character>();

        // Initialize character with base stats
        var stats = new CharacterStats(100, 15, 10, 12, 50);
        var abilities = new CharacterAbility[]
        {
            new CharacterAbility("Test Attack", AbilityType.Attack, 10, 5, "Test ability")
        };
        testCharacter.Initialize("Test Character", stats, abilities, true);

        // Create test equipment
        testWeapon = ScriptableObject.CreateInstance<Equipment>();
        testWeapon.Initialize("Test Weapon", EquipmentSlot.Weapon,
            new StatModifier[] {
                new StatModifier(StatType.Attack, 5, ModifierType.Additive)
            },
            "A test weapon");

        testArmor = ScriptableObject.CreateInstance<Equipment>();
        testArmor.Initialize("Test Armor", EquipmentSlot.Armor,
            new StatModifier[] {
                new StatModifier(StatType.Defense, 3, ModifierType.Additive),
                new StatModifier(StatType.MaxHealth, 20, ModifierType.Multiplicative)
            },
            "Test armor");
    }

    [TearDown]
    public void TearDown()
    {
        if (testWeapon != null) Object.DestroyImmediate(testWeapon);
        if (testArmor != null) Object.DestroyImmediate(testArmor);
        if (testGameObject != null) Object.DestroyImmediate(testGameObject);
    }

    [Test]
    public void Character_EquipmentManager_InitializesCorrectly()
    {
        Assert.IsNotNull(testCharacter.EquipmentManager);
        Assert.IsNotNull(testCharacter.Stats);
    }

    [Test]
    public void Character_EquipWeapon_UpdatesStatsCorrectly()
    {
        // Base attack should be 15
        Assert.AreEqual(15, testCharacter.Stats.Attack);

        // Equip weapon (+5 Attack)
        bool equipped = testCharacter.EquipItem(testWeapon);
        Assert.IsTrue(equipped);

        // Attack should now be 20 (15 + 5)
        Assert.AreEqual(20, testCharacter.Stats.Attack);
        Assert.AreEqual(testWeapon, testCharacter.EquipmentManager.Weapon);
    }

    [Test]
    public void Character_EquipArmor_UpdatesMultipleStats()
    {
        // Base stats
        Assert.AreEqual(10, testCharacter.Stats.Defense);
        Assert.AreEqual(100, testCharacter.Stats.MaxHealth);

        // Equip armor (+3 Defense, +20% MaxHealth)
        testCharacter.EquipItem(testArmor);

        // Defense should be 13 (10 + 3)
        Assert.AreEqual(13, testCharacter.Stats.Defense);
        
        // MaxHealth should be 120 (100 * 1.2)
        Assert.AreEqual(120, testCharacter.Stats.MaxHealth);
    }

    [Test]
    public void Character_UnequipItem_RestoresBaseStats()
    {
        // Equip weapon
        testCharacter.EquipItem(testWeapon);
        Assert.AreEqual(20, testCharacter.Stats.Attack); // 15 + 5

        // Unequip weapon
        var unequipped = testCharacter.UnequipItem(EquipmentSlot.Weapon);
        Assert.AreEqual(testWeapon, unequipped);
        Assert.IsNull(testCharacter.EquipmentManager.Weapon);

        // Attack should return to base value
        Assert.AreEqual(15, testCharacter.Stats.Attack);
    }

    [Test]
    public void Character_MultipleEquipment_CombinesCorrectly()
    {
        // Equip both weapon and armor
        testCharacter.EquipItem(testWeapon);
        testCharacter.EquipItem(testArmor);

        // Verify both bonuses are applied
        Assert.AreEqual(20, testCharacter.Stats.Attack);     // 15 + 5 from weapon
        Assert.AreEqual(13, testCharacter.Stats.Defense);    // 10 + 3 from armor
        Assert.AreEqual(120, testCharacter.Stats.MaxHealth); // 100 * 1.2 from armor
    }

    [Test]
    public void Character_EquipmentEvents_UpdateStatsOnEquip()
    {
        // Equip weapon
        testCharacter.EquipItem(testWeapon);
        int attackWithWeapon = testCharacter.Stats.Attack;

        // Equip armor (this should trigger stat recalculation)
        testCharacter.EquipItem(testArmor);

        // Weapon bonus should still be applied
        Assert.AreEqual(attackWithWeapon, testCharacter.Stats.Attack);
        
        // Armor bonuses should also be applied
        Assert.AreEqual(13, testCharacter.Stats.Defense);
        Assert.AreEqual(120, testCharacter.Stats.MaxHealth);
    }

    [Test]
    public void Character_StatCaching_WorksCorrectly()
    {
        // Access attack stat to cache it
        int initialAttack = testCharacter.Stats.Attack;
        Assert.AreEqual(15, initialAttack);

        // Equip weapon - this should invalidate cache
        testCharacter.EquipItem(testWeapon);

        // Next access should return updated value
        int updatedAttack = testCharacter.Stats.Attack;
        Assert.AreEqual(20, updatedAttack);
    }

    [Test]
    public void Character_BaseVsEffectiveStats_DifferentiatCorrectly()
    {
        // Equip weapon
        testCharacter.EquipItem(testWeapon);

        // Base stats should remain unchanged
        Assert.AreEqual(15, testCharacter.Stats.BaseAttack);
        
        // Effective stats should include equipment
        Assert.AreEqual(20, testCharacter.Stats.Attack);
    }

    [Test]
    public void Character_TakeDamage_UsesEffectiveDefense()
    {
        // Equip armor for defense bonus
        testCharacter.EquipItem(testArmor);
        
        int initialHealth = testCharacter.Stats.CurrentHealth;
        int effectiveDefense = testCharacter.Stats.Defense; // Should be 13

        // Take 20 damage
        testCharacter.Stats.TakeDamage(20);
        
        // Damage should be reduced by effective defense (20 - 13 = 7 damage)
        int expectedHealth = initialHealth - 7;
        Assert.AreEqual(expectedHealth, testCharacter.Stats.CurrentHealth);
    }

    [Test]
    public void Character_Heal_UsesEffectiveMaxHealth()
    {
        // Equip armor for max health bonus
        testCharacter.EquipItem(testArmor);
        
        // Take some damage first
        testCharacter.Stats.TakeDamage(50);
        
        // Heal beyond base max health
        testCharacter.Stats.Heal(200);
        
        // Should be capped at effective max health (120, not 100)
        Assert.AreEqual(120, testCharacter.Stats.CurrentHealth);
    }
}