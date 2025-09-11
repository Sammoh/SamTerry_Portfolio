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
        var testSword = ScriptableObject.CreateInstance<Weapon>();
        testSword.name = "Test Sword";
        testSword.EquipmentName = "Test Sword";
        testSword.Slot = EquipmentSlot.Weapon;
        testSword.StatModifiers = new StatModifier[] {
            new StatModifier(StatType.Attack, 10, ModifierType.Flat),
            new StatModifier(StatType.Speed, 5, ModifierType.Flat)
        };
        testSword.Description = "A test weapon";
        testWeapon = testSword;

        var testArmorSo = ScriptableObject.CreateInstance<Armor>();
        testArmorSo.name = "Test Armor";
        testArmorSo.EquipmentName = "Test Armor";
        testArmorSo.Slot = EquipmentSlot.Armor;
        testArmorSo.StatModifiers = new StatModifier[] {
            new StatModifier(StatType.Defense, 5, ModifierType.Flat),
            new StatModifier(StatType.MaxHealth, 20, ModifierType.Percentage)
        };
        testArmorSo.Description = "Test armor piece";
        testArmor = testArmorSo;
        
        var testRing = ScriptableObject.CreateInstance<Accessory>();
        testRing.name = "Test Ring";
        testRing.EquipmentName = "Test Ring";
        testRing.Slot = EquipmentSlot.Accessory;
        testRing.StatModifiers = new StatModifier[] {
            new StatModifier(StatType.Attack, 15, ModifierType.Percentage)
        };
        testRing.Description = "A magical test ring";
        testAccessory = testRing;

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
        var combinedEquipment = ScriptableObject.CreateInstance<Weapon>();
        combinedEquipment.name = "Combined";
        combinedEquipment.EquipmentName = "Combined";
        combinedEquipment.Slot = EquipmentSlot.Weapon;
        combinedEquipment.StatModifiers = new StatModifier[] {
            new StatModifier(StatType.Attack, 5, ModifierType.Flat),
            new StatModifier(StatType.Attack, 10, ModifierType.Percentage)
        };
        combinedEquipment.Description = "Equipment with both modifier types";

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
        var secondWeapon = ScriptableObject.CreateInstance<Weapon>();
        secondWeapon.name = "Second Sword";
        secondWeapon.EquipmentName = "Second Sword";
        secondWeapon.Slot = EquipmentSlot.Weapon;
        secondWeapon.StatModifiers = new StatModifier[0];
        secondWeapon.Description = "Another weapon";

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
        var modifier = new StatModifier(StatType.Attack, 5, ModifierType.Flat);
        float result = modifier.ApplyModifier(10f);
        Assert.AreEqual(15f, result);
    }

    [Test]
    public void StatModifier_ApplyModifier_MultiplicativeWorks()
    {
        var modifier = new StatModifier(StatType.Attack, 20, ModifierType.Percentage);
        float result = modifier.ApplyModifier(10f);
        Assert.AreEqual(12f, result); // 10 * (1 + 20/100)
    }

    [Test]
    public void StatModifier_ToString_AdditiveFormat()
    {
        var modifier = new StatModifier(StatType.Attack, 5, ModifierType.Flat);
        Assert.AreEqual("+5 Attack", modifier.ToString());
    }

    [Test]
    public void StatModifier_ToString_MultiplicativeFormat()
    {
        var modifier = new StatModifier(StatType.Attack, 20, ModifierType.Percentage);
        Assert.AreEqual("+20% Attack", modifier.ToString());
    }

    [Test]
    public void StatModifier_ToString_NegativeValues()
    {
        var additiveModifier = new StatModifier(StatType.Speed, -5, ModifierType.Flat);
        var multiplicativeModifier = new StatModifier(StatType.Attack, -10, ModifierType.Percentage);
        
        Assert.AreEqual("-5 Speed", additiveModifier.ToString());
        Assert.AreEqual("-10% Attack", multiplicativeModifier.ToString());
    }

    [Test]
    public void Equipment_WithAbilities_StoresAbilitiesCorrectly()
    {
        var ability1 = new CharacterAbility("Fire Strike", AbilityType.Attack, 15, 10, "A fiery weapon attack");
        var ability2 = new CharacterAbility("Shield Bash", AbilityType.Special, 10, 5, "A defensive strike");
        
        testWeapon.Abilities = new CharacterAbility[] { ability1, ability2 };
        
        Assert.AreEqual(2, testWeapon.Abilities.Length);
        Assert.AreEqual("Fire Strike", testWeapon.Abilities[0].AbilityName);
        Assert.AreEqual("Shield Bash", testWeapon.Abilities[1].AbilityName);
    }

    [Test]
    public void Equipment_Abilities_DefaultsToEmpty()
    {
        var newEquipment = ScriptableObject.CreateInstance<Weapon>();
        Assert.IsNotNull(newEquipment.Abilities);
        Assert.AreEqual(0, newEquipment.Abilities.Length);
    }

    [Test]
    public void EquipmentManager_GetEquipmentAbilities_ReturnsAllAbilities()
    {
        var weaponAbility = new CharacterAbility("Sword Slash", AbilityType.Attack, 20, 8, "Basic sword attack");
        var armorAbility = new CharacterAbility("Iron Will", AbilityType.Defend, 15, 12, "Defensive boost from armor");
        
        testWeapon.Abilities = new CharacterAbility[] { weaponAbility };
        testArmor.Abilities = new CharacterAbility[] { armorAbility };
        
        equipmentManager.EquipItem(testWeapon);
        equipmentManager.EquipItem(testArmor);
        
        var abilities = equipmentManager.GetEquipmentAbilities();
        
        Assert.AreEqual(2, abilities.Length);
        Assert.Contains(weaponAbility, abilities);
        Assert.Contains(armorAbility, abilities);
    }

    [Test]
    public void EquipmentManager_GetEquipmentAbilities_ReturnsEmptyWhenNoAbilities()
    {
        equipmentManager.EquipItem(testWeapon); // testWeapon has no abilities by default
        
        var abilities = equipmentManager.GetEquipmentAbilities();
        
        Assert.AreEqual(0, abilities.Length);
    }

    [Test]
    public void EquipmentManager_GetAbilitiesFromSlot_ReturnsCorrectAbilities()
    {
        var weaponAbility = new CharacterAbility("Weapon Power", AbilityType.Attack, 25, 15, "Special weapon ability");
        testWeapon.Abilities = new CharacterAbility[] { weaponAbility };
        
        equipmentManager.EquipItem(testWeapon);
        
        var weaponAbilities = equipmentManager.GetAbilitiesFromSlot(EquipmentSlot.Weapon);
        var armorAbilities = equipmentManager.GetAbilitiesFromSlot(EquipmentSlot.Armor);
        
        Assert.AreEqual(1, weaponAbilities.Length);
        Assert.AreEqual(weaponAbility, weaponAbilities[0]);
        Assert.AreEqual(0, armorAbilities.Length);
    }

    [Test]
    public void EquipmentManager_GetAbilitiesFromSlot_ReturnsEmptyWhenNothingEquipped()
    {
        var abilities = equipmentManager.GetAbilitiesFromSlot(EquipmentSlot.Weapon);
        Assert.AreEqual(0, abilities.Length);
    }
}