using NUnit.Framework;
using UnityEngine;
using Sammoh.TurnBasedStrategy;

public class EquipmentTests
{
    private Equipment testWeapon;
    private Equipment testArmor;
    private Equipment testAccessory;
    private EquipmentManager equipmentManager;

    [SetUp]
    public void Setup()
    {
        // Create test equipment
        testWeapon = new Equipment("Test Sword", EquipmentSlot.Weapon,
            new StatModifier[] {
                new StatModifier(StatType.Attack, 10, ModifierType.Additive),
                new StatModifier(StatType.Speed, 5, ModifierType.Additive)
            },
            "A test weapon");

        testArmor = new Equipment("Test Armor", EquipmentSlot.Armor,
            new StatModifier[] {
                new StatModifier(StatType.Defense, 5, ModifierType.Additive),
                new StatModifier(StatType.MaxHealth, 20, ModifierType.Multiplicative)
            },
            "Test armor piece");

        testAccessory = new Equipment("Test Ring", EquipmentSlot.Accessory,
            new StatModifier[] {
                new StatModifier(StatType.Attack, 15, ModifierType.Multiplicative)
            },
            "A magical test ring");

        equipmentManager = new EquipmentManager();
    }

    [Test]
    public void Equipment_Creation_SetsPropertiesCorrectly()
    {
        Assert.AreEqual("Test Sword", testWeapon.EquipmentName);
        Assert.AreEqual(EquipmentSlot.Weapon, testWeapon.Slot);
        Assert.AreEqual(2, testWeapon.StatModifiers.Length);
        Assert.AreEqual("A test weapon", testWeapon.Description);
    }

    [Test]
    public void Equipment_GetModifiedValue_AppliesAdditiveModifiers()
    {
        float result = testWeapon.GetModifiedValue(StatType.Attack, 10f);
        Assert.AreEqual(20f, result); // 10 base + 10 from weapon
    }

    [Test]
    public void Equipment_GetModifiedValue_AppliesMultiplicativeModifiers()
    {
        float result = testArmor.GetModifiedValue(StatType.MaxHealth, 100f);
        Assert.AreEqual(120f, result); // 100 * (1 + 20/100)
    }

    [Test]
    public void Equipment_GetModifiedValue_CombinesAdditiveAndMultiplicative()
    {
        var combinedEquipment = new Equipment("Combined", EquipmentSlot.Weapon,
            new StatModifier[] {
                new StatModifier(StatType.Attack, 5, ModifierType.Additive),
                new StatModifier(StatType.Attack, 10, ModifierType.Multiplicative)
            },
            "Equipment with both modifier types");

        float result = combinedEquipment.GetModifiedValue(StatType.Attack, 10f);
        // Should be (10 + 5) * (1 + 10/100) = 15 * 1.1 = 16.5
        Assert.AreEqual(16.5f, result, 0.01f);
    }

    [Test]
    public void Equipment_GetModifiersForStat_ReturnsCorrectModifiers()
    {
        var modifiers = testWeapon.GetModifiersForStat(StatType.Attack);
        Assert.AreEqual(1, modifiers.Length);
        Assert.AreEqual(StatType.Attack, modifiers[0].StatType);
        Assert.AreEqual(10f, modifiers[0].Value);
    }

    [Test]
    public void EquipmentManager_EquipItem_StoresItemCorrectly()
    {
        var previousItem = equipmentManager.EquipItem(testWeapon);
        
        Assert.IsNull(previousItem);
        Assert.AreEqual(testWeapon, equipmentManager.GetEquippedItem(EquipmentSlot.Weapon));
        Assert.AreEqual(testWeapon, equipmentManager.Weapon);
    }

    [Test]
    public void EquipmentManager_EquipItem_ReturnsReplacedItem()
    {
        var firstWeapon = testWeapon;
        var secondWeapon = new Equipment("Second Sword", EquipmentSlot.Weapon,
            new StatModifier[0], "Another weapon");

        equipmentManager.EquipItem(firstWeapon);
        var replacedItem = equipmentManager.EquipItem(secondWeapon);

        Assert.AreEqual(firstWeapon, replacedItem);
        Assert.AreEqual(secondWeapon, equipmentManager.GetEquippedItem(EquipmentSlot.Weapon));
    }

    [Test]
    public void EquipmentManager_UnequipItem_RemovesAndReturnsItem()
    {
        equipmentManager.EquipItem(testWeapon);
        var unequippedItem = equipmentManager.UnequipItem(EquipmentSlot.Weapon);

        Assert.AreEqual(testWeapon, unequippedItem);
        Assert.IsNull(equipmentManager.GetEquippedItem(EquipmentSlot.Weapon));
    }

    [Test]
    public void EquipmentManager_UnequipItem_ReturnsNullWhenNothingEquipped()
    {
        var unequippedItem = equipmentManager.UnequipItem(EquipmentSlot.Weapon);
        Assert.IsNull(unequippedItem);
    }

    [Test]
    public void EquipmentManager_CalculateModifiedStat_CombinesAllEquipment()
    {
        equipmentManager.EquipItem(testWeapon); // +10 Attack
        equipmentManager.EquipItem(testAccessory); // +15% Attack

        float result = equipmentManager.CalculateModifiedStat(StatType.Attack, 10f);
        // Should be (10 + 10) * (1 + 15/100) = 20 * 1.15 = 23
        Assert.AreEqual(23f, result, 0.01f);
    }

    [Test]
    public void EquipmentManager_GetAdditiveModifier_SumsAllAdditiveModifiers()
    {
        equipmentManager.EquipItem(testWeapon); // +10 Attack
        equipmentManager.EquipItem(testArmor);  // +5 Defense

        float attackModifier = equipmentManager.GetAdditiveModifier(StatType.Attack);
        float defenseModifier = equipmentManager.GetAdditiveModifier(StatType.Defense);

        Assert.AreEqual(10f, attackModifier);
        Assert.AreEqual(5f, defenseModifier);
    }

    [Test]
    public void EquipmentManager_GetMultiplicativeModifier_SumsAllMultiplicativeModifiers()
    {
        equipmentManager.EquipItem(testArmor); // +20% MaxHealth
        equipmentManager.EquipItem(testAccessory); // +15% Attack

        float healthModifier = equipmentManager.GetMultiplicativeModifier(StatType.MaxHealth);
        float attackModifier = equipmentManager.GetMultiplicativeModifier(StatType.Attack);

        Assert.AreEqual(20f, healthModifier);
        Assert.AreEqual(15f, attackModifier);
    }

    [Test]
    public void EquipmentManager_GetAllEquippedItems_ReturnsAllItems()
    {
        equipmentManager.EquipItem(testWeapon);
        equipmentManager.EquipItem(testArmor);

        var equippedItems = equipmentManager.GetAllEquippedItems();

        Assert.AreEqual(2, equippedItems.Count);
        Assert.Contains(testWeapon, equippedItems);
        Assert.Contains(testArmor, equippedItems);
    }

    [Test]
    public void EquipmentManager_UnequipAll_RemovesAllItems()
    {
        equipmentManager.EquipItem(testWeapon);
        equipmentManager.EquipItem(testArmor);
        equipmentManager.EquipItem(testAccessory);

        var unequippedItems = equipmentManager.UnequipAll();

        Assert.AreEqual(3, unequippedItems.Count);
        Assert.IsNull(equipmentManager.Weapon);
        Assert.IsNull(equipmentManager.Armor);
        Assert.IsNull(equipmentManager.Accessory);
    }

    [Test]
    public void EquipmentManager_OnEquipmentChanged_FiresWhenEquipping()
    {
        bool eventFired = false;
        equipmentManager.OnEquipmentChanged += () => eventFired = true;

        equipmentManager.EquipItem(testWeapon);

        Assert.IsTrue(eventFired);
    }

    [Test]
    public void EquipmentManager_OnEquipmentChanged_FiresWhenUnequipping()
    {
        equipmentManager.EquipItem(testWeapon);
        
        bool eventFired = false;
        equipmentManager.OnEquipmentChanged += () => eventFired = true;

        equipmentManager.UnequipItem(EquipmentSlot.Weapon);

        Assert.IsTrue(eventFired);
    }

    [Test]
    public void StatModifier_ApplyModifier_AdditiveWorks()
    {
        var modifier = new StatModifier(StatType.Attack, 5, ModifierType.Additive);
        float result = modifier.ApplyModifier(10f);
        Assert.AreEqual(15f, result);
    }

    [Test]
    public void StatModifier_ApplyModifier_MultiplicativeWorks()
    {
        var modifier = new StatModifier(StatType.Attack, 20, ModifierType.Multiplicative);
        float result = modifier.ApplyModifier(10f);
        Assert.AreEqual(12f, result); // 10 * (1 + 20/100)
    }

    [Test]
    public void StatModifier_ToString_AdditiveFormat()
    {
        var modifier = new StatModifier(StatType.Attack, 5, ModifierType.Additive);
        Assert.AreEqual("+5 Attack", modifier.ToString());
    }

    [Test]
    public void StatModifier_ToString_MultiplicativeFormat()
    {
        var modifier = new StatModifier(StatType.Attack, 20, ModifierType.Multiplicative);
        Assert.AreEqual("+20% Attack", modifier.ToString());
    }

    [Test]
    public void StatModifier_ToString_NegativeValues()
    {
        var additiveModifier = new StatModifier(StatType.Speed, -5, ModifierType.Additive);
        var multiplicativeModifier = new StatModifier(StatType.Attack, -10, ModifierType.Multiplicative);
        
        Assert.AreEqual("-5 Speed", additiveModifier.ToString());
        Assert.AreEqual("-10% Attack", multiplicativeModifier.ToString());
    }
}