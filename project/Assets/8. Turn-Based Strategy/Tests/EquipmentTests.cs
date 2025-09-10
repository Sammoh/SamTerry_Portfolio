using NUnit.Framework;
using UnityEngine;
using Sammoh.TurnBasedStrategy;

public class EquipmentTests
{
    private GameObject testGameObject;
    private Character testCharacter;
    private Equipment testWeapon;
    private Equipment testArmor;
    private Equipment testAccessory;

    [SetUp]
    public void SetUp()
    {
        testGameObject = new GameObject("TestCharacter");
        testCharacter = testGameObject.AddComponent<Character>();

        // Create test equipment
        testWeapon = new Equipment(
            "Test Sword", 
            "A test weapon", 
            EquipmentSlot.Weapon, 
            EquipmentRarity.Common,
            new EquipmentStats(0, 10, 0, 0, 0)
        );

        testArmor = new Equipment(
            "Test Armor", 
            "Test protection", 
            EquipmentSlot.Armor, 
            EquipmentRarity.Common,
            new EquipmentStats(20, 0, 15, -2, 0)
        );

        testAccessory = new Equipment(
            "Test Ring", 
            "A magical ring", 
            EquipmentSlot.Accessory, 
            EquipmentRarity.Rare,
            new EquipmentStats(5, 3, 3, 5, 10)
        );

        // Initialize character
        CharacterStats stats = new CharacterStats(100, 15, 10, 12, 50);
        CharacterAbility[] abilities = new CharacterAbility[] 
        {
            new CharacterAbility("Test Attack", AbilityType.Attack, 10, 5, "Test ability")
        };
        testCharacter.Initialize("TestCharacter", stats, abilities, true, CharacterClass.Warrior);
    }

    [TearDown]
    public void TearDown()
    {
        if (testGameObject != null)
            Object.DestroyImmediate(testGameObject);
    }

    [Test]
    public void Equipment_Creation_HasCorrectProperties()
    {
        Assert.AreEqual("Test Sword", testWeapon.EquipmentName);
        Assert.AreEqual(EquipmentSlot.Weapon, testWeapon.Slot);
        Assert.AreEqual(EquipmentRarity.Common, testWeapon.Rarity);
        Assert.AreEqual(10, testWeapon.Stats.AttackBonus);
    }

    [Test]
    public void EquipmentManager_EquipWeapon_UpdatesStats()
    {
        int initialAttack = testCharacter.Stats.Attack;
        
        bool equipped = testCharacter.EquipItem(testWeapon);
        
        Assert.IsTrue(equipped);
        Assert.AreEqual(initialAttack + 10, testCharacter.Stats.Attack);
        Assert.AreEqual(testWeapon, testCharacter.GetEquippedItem(EquipmentSlot.Weapon));
    }

    [Test]
    public void EquipmentManager_EquipArmor_UpdatesHealthAndDefense()
    {
        int initialHealth = testCharacter.Stats.MaxHealth;
        int initialDefense = testCharacter.Stats.Defense;
        int initialSpeed = testCharacter.Stats.Speed;
        
        bool equipped = testCharacter.EquipItem(testArmor);
        
        Assert.IsTrue(equipped);
        Assert.AreEqual(initialHealth + 20, testCharacter.Stats.MaxHealth);
        Assert.AreEqual(initialDefense + 15, testCharacter.Stats.Defense);
        Assert.AreEqual(initialSpeed - 2, testCharacter.Stats.Speed); // Speed penalty
    }

    [Test]
    public void EquipmentManager_EquipMultipleItems_CombinesStats()
    {
        int initialAttack = testCharacter.Stats.Attack;
        int initialDefense = testCharacter.Stats.Defense;
        int initialMana = testCharacter.Stats.Mana;
        
        testCharacter.EquipItem(testWeapon);  // +10 attack
        testCharacter.EquipItem(testAccessory); // +3 attack, +3 defense, +10 mana
        
        Assert.AreEqual(initialAttack + 13, testCharacter.Stats.Attack); // 10 + 3
        Assert.AreEqual(initialDefense + 3, testCharacter.Stats.Defense);
        Assert.AreEqual(initialMana + 10, testCharacter.Stats.Mana);
    }

    [Test]
    public void EquipmentManager_UnequipItem_RestoresOriginalStats()
    {
        int initialAttack = testCharacter.Stats.Attack;
        
        // Equip then unequip
        testCharacter.EquipItem(testWeapon);
        Equipment unequipped = testCharacter.UnequipItem(EquipmentSlot.Weapon);
        
        Assert.AreEqual(testWeapon, unequipped);
        Assert.AreEqual(initialAttack, testCharacter.Stats.Attack);
        Assert.IsNull(testCharacter.GetEquippedItem(EquipmentSlot.Weapon));
    }

    [Test]
    public void EquipmentManager_ReplaceEquipment_UpdatesStatsCorrectly()
    {
        Equipment betterWeapon = new Equipment(
            "Better Sword", 
            "A superior weapon", 
            EquipmentSlot.Weapon, 
            EquipmentRarity.Uncommon,
            new EquipmentStats(0, 20, 0, 0, 0)
        );

        int initialAttack = testCharacter.Stats.Attack;
        
        // Equip first weapon
        testCharacter.EquipItem(testWeapon); // +10 attack
        Assert.AreEqual(initialAttack + 10, testCharacter.Stats.Attack);
        
        // Replace with better weapon
        testCharacter.EquipItem(betterWeapon); // +20 attack
        Assert.AreEqual(initialAttack + 20, testCharacter.Stats.Attack);
        Assert.AreEqual(betterWeapon, testCharacter.GetEquippedItem(EquipmentSlot.Weapon));
    }

    [Test]
    public void EquipmentStats_GetTotalEquipmentStats_CalculatesCorrectly()
    {
        EquipmentManager manager = new EquipmentManager();
        
        manager.EquipItem(testWeapon, CharacterClass.Warrior);   // +10 attack
        manager.EquipItem(testArmor, CharacterClass.Warrior);    // +20 health, +15 defense, -2 speed
        manager.EquipItem(testAccessory, CharacterClass.Warrior); // +5 health, +3 attack, +3 defense, +5 speed, +10 mana
        
        EquipmentStats totalStats = manager.GetTotalEquipmentStats();
        
        Assert.AreEqual(25, totalStats.HealthBonus); // 20 + 5
        Assert.AreEqual(13, totalStats.AttackBonus); // 10 + 3
        Assert.AreEqual(18, totalStats.DefenseBonus); // 15 + 3
        Assert.AreEqual(3, totalStats.SpeedBonus); // -2 + 5
        Assert.AreEqual(10, totalStats.ManaBonus);
    }

    [Test]
    public void Equipment_CanEquip_AllowsAllClassesForBasicEquipment()
    {
        Assert.IsTrue(testWeapon.CanEquip(CharacterClass.Warrior));
        Assert.IsTrue(testWeapon.CanEquip(CharacterClass.Mage));
        Assert.IsTrue(testWeapon.CanEquip(CharacterClass.Rogue));
        Assert.IsTrue(testWeapon.CanEquip(CharacterClass.Healer));
    }

    [Test]
    public void EquipmentManager_HasEquipment_ReturnsCorrectStatus()
    {
        EquipmentManager manager = new EquipmentManager();
        
        Assert.IsFalse(manager.HasEquipment());
        
        manager.EquipItem(testWeapon, CharacterClass.Warrior);
        Assert.IsTrue(manager.HasEquipment());
        
        manager.UnequipItem(EquipmentSlot.Weapon);
        Assert.IsFalse(manager.HasEquipment());
    }

    [Test]
    public void EquipmentManager_GetAllEquippedItems_ReturnsCorrectList()
    {
        EquipmentManager manager = new EquipmentManager();
        
        var equippedItems = manager.GetAllEquippedItems();
        Assert.AreEqual(0, equippedItems.Count);
        
        manager.EquipItem(testWeapon, CharacterClass.Warrior);
        manager.EquipItem(testArmor, CharacterClass.Warrior);
        
        equippedItems = manager.GetAllEquippedItems();
        Assert.AreEqual(2, equippedItems.Count);
        Assert.Contains(testWeapon, equippedItems);
        Assert.Contains(testArmor, equippedItems);
    }
}