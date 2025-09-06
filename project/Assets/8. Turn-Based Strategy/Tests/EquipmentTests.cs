using NUnit.Framework;
using UnityEngine;
using Sammoh.TurnBasedStrategy;

public class EquipmentTests
{
    private Equipment testWeapon;
    private Equipment testArmor;
    private EquipmentManager equipmentManager;
    private CharacterStats characterStats;

    [SetUp]
    public void Setup()
    {
        // Create test equipment
        StatModifier[] weaponModifiers = {
            new StatModifier(StatType.Attack, 5, ModifierType.Additive),
            new StatModifier(StatType.Speed, 2, ModifierType.Additive)
        };
        testWeapon = new Equipment("Iron Sword", EquipmentSlot.Weapon, weaponModifiers, "A basic iron sword");

        StatModifier[] armorModifiers = {
            new StatModifier(StatType.Defense, 3, ModifierType.Additive),
            new StatModifier(StatType.MaxHealth, 10, ModifierType.Additive)
        };
        testArmor = new Equipment("Leather Armor", EquipmentSlot.Armor, armorModifiers, "Basic leather protection");

        equipmentManager = new EquipmentManager();
        characterStats = new CharacterStats(100, 15, 10, 12, 50);
        characterStats.SetEquipmentManager(equipmentManager);
    }

    [Test]
    public void Equipment_Constructor_SetsPropertiesCorrectly()
    {
        Assert.AreEqual("Iron Sword", testWeapon.EquipmentName);
        Assert.AreEqual(EquipmentSlot.Weapon, testWeapon.Slot);
        Assert.AreEqual(2, testWeapon.StatModifiers.Length);
        Assert.AreEqual("A basic iron sword", testWeapon.Description);
    }

    [Test]
    public void StatModifier_ApplyModifier_AdditiveWorksCorrectly()
    {
        var modifier = new StatModifier(StatType.Attack, 5, ModifierType.Additive);
        int result = modifier.ApplyModifier(15);
        Assert.AreEqual(20, result);
    }

    [Test]
    public void StatModifier_ApplyModifier_MultiplicativeWorksCorrectly()
    {
        var modifier = new StatModifier(StatType.Attack, 20, ModifierType.Multiplicative); // +20%
        int result = modifier.ApplyModifier(15);
        Assert.AreEqual(18, result); // 15 * 1.2 = 18
    }

    [Test]
    public void EquipmentManager_EquipItem_WorksCorrectly()
    {
        bool result = equipmentManager.EquipItem(testWeapon);
        
        Assert.IsTrue(result);
        Assert.AreEqual(testWeapon, equipmentManager.GetEquippedItem(EquipmentSlot.Weapon));
        Assert.IsTrue(equipmentManager.IsSlotEquipped(EquipmentSlot.Weapon));
    }

    [Test]
    public void EquipmentManager_UnequipItem_WorksCorrectly()
    {
        equipmentManager.EquipItem(testWeapon);
        Equipment unequipped = equipmentManager.UnequipItem(EquipmentSlot.Weapon);
        
        Assert.AreEqual(testWeapon, unequipped);
        Assert.IsNull(equipmentManager.GetEquippedItem(EquipmentSlot.Weapon));
        Assert.IsFalse(equipmentManager.IsSlotEquipped(EquipmentSlot.Weapon));
    }

    [Test]
    public void EquipmentManager_CalculateStatWithEquipment_AppliesBonus()
    {
        equipmentManager.EquipItem(testWeapon); // +5 Attack, +2 Speed
        
        int modifiedAttack = equipmentManager.CalculateStatWithEquipment(15, StatType.Attack);
        int modifiedSpeed = equipmentManager.CalculateStatWithEquipment(12, StatType.Speed);
        int unmodifiedDefense = equipmentManager.CalculateStatWithEquipment(10, StatType.Defense);
        
        Assert.AreEqual(20, modifiedAttack); // 15 + 5
        Assert.AreEqual(14, modifiedSpeed);  // 12 + 2
        Assert.AreEqual(10, unmodifiedDefense); // No modifier
    }

    [Test]
    public void CharacterStats_WithEquipment_ReturnsModifiedStats()
    {
        equipmentManager.EquipItem(testWeapon); // +5 Attack, +2 Speed
        equipmentManager.EquipItem(testArmor);  // +3 Defense, +10 MaxHealth
        
        Assert.AreEqual(20, characterStats.Attack);    // 15 + 5
        Assert.AreEqual(13, characterStats.Defense);   // 10 + 3
        Assert.AreEqual(14, characterStats.Speed);     // 12 + 2
        Assert.AreEqual(110, characterStats.MaxHealth); // 100 + 10
        Assert.AreEqual(50, characterStats.Mana);      // No modifier
    }

    [Test]
    public void CharacterStats_WithoutEquipment_ReturnsBaseStats()
    {
        // No equipment equipped
        Assert.AreEqual(15, characterStats.Attack);
        Assert.AreEqual(10, characterStats.Defense);
        Assert.AreEqual(12, characterStats.Speed);
        Assert.AreEqual(100, characterStats.MaxHealth);
        Assert.AreEqual(50, characterStats.Mana);
    }

    [Test]
    public void EquipmentManager_GetAllStatModifiers_ReturnsAllModifiers()
    {
        equipmentManager.EquipItem(testWeapon); // 2 modifiers
        equipmentManager.EquipItem(testArmor);  // 2 modifiers
        
        StatModifier[] allModifiers = equipmentManager.GetAllStatModifiers();
        Assert.AreEqual(4, allModifiers.Length);
    }
}