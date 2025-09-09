// using NUnit.Framework;
// using UnityEngine;
// using Sammoh.Two;
//
// namespace Sammoh.Two.Tests
// {
//     /// <summary>
//     /// Tests for the Equipment system functionality.
//     /// Following existing test patterns from the portfolio.
//     /// </summary>
//     public class EquipmentTests
//     {
//         private Weapon testWeapon;
//         private Accessory testAccessory;
//         private Armor testArmor;
//         private EquipmentDatabase testDatabase;
//
//         [SetUp]
//         public void SetUp()
//         {
//             // Create test equipment instances
//             testWeapon = ScriptableObject.CreateInstance<Weapon>();
//             testWeapon.name = "Test_Sword";
//             testWeapon.WeaponType = WeaponType.Sword;
//             testWeapon.Rarity = EquipmentRarity.Common;
//
//             testAccessory = ScriptableObject.CreateInstance<Accessory>();
//             testAccessory.name = "Test_Ring";
//             testAccessory.AccessoryType = AccessoryType.Ring;
//             testAccessory.Rarity = EquipmentRarity.Uncommon;
//
//             testArmor = ScriptableObject.CreateInstance<Armor>();
//             testArmor.name = "Test_Chestplate";
//             testArmor.ArmorType = ArmorType.Chestplate;
//             testArmor.Rarity = EquipmentRarity.Rare;
//
//             testDatabase = ScriptableObject.CreateInstance<EquipmentDatabase>();
//         }
//
//         [TearDown]
//         public void TearDown()
//         {
//             // Clean up test objects
//             if (testWeapon != null)
//                 Object.DestroyImmediate(testWeapon);
//             if (testAccessory != null)
//                 Object.DestroyImmediate(testAccessory);
//             if (testArmor != null)
//                 Object.DestroyImmediate(testArmor);
//             if (testDatabase != null)
//                 Object.DestroyImmediate(testDatabase);
//         }
//
//         [Test]
//         public void Equipment_Id_ShouldReturnAssetName()
//         {
//             Assert.AreEqual("Test_Sword", testWeapon.Id);
//             Assert.AreEqual("Test_Ring", testAccessory.Id);
//             Assert.AreEqual("Test_Chestplate", testArmor.Id);
//         }
//
//         [Test]
//         public void Equipment_Type_ShouldReturnCorrectType()
//         {
//             Assert.AreEqual(EquipmentType.Weapon, testWeapon.Type);
//             Assert.AreEqual(EquipmentType.Accessory, testAccessory.Type);
//             Assert.AreEqual(EquipmentType.Armor, testArmor.Type);
//         }
//
//         [Test]
//         public void Weapon_CalculateDPS_ShouldReturnCorrectValue()
//         {
//             // Set up weapon stats
//             testWeapon.Damage = 10;
//             testWeapon.AttackSpeed = 2.0f;
//             testWeapon.CriticalChance = 10; // 10%
//             testWeapon.CriticalMultiplier = 1.5f;
//
//             float expectedBaseDPS = 10 * 2.0f; // 20
//             float expectedCriticalBonus = (10f / 100f) * (1.5f - 1f); // 0.05
//             float expectedTotalDPS = expectedBaseDPS * (1f + expectedCriticalBonus); // 21
//
//             float actualDPS = testWeapon.CalculateDPS();
//             Assert.AreEqual(expectedTotalDPS, actualDPS, 0.001f);
//         }
//
//         [Test]
//         public void Armor_CalculateTotalDefense_ShouldIncludeMaterialBonus()
//         {
//             testArmor.Defense = 10;
//             testArmor.ArmorMaterial = ArmorMaterial.Plate; // Should have 1.3x multiplier
//
//             int expectedDefense = 13; // 10 * 1.3 = 13
//             int actualDefense = testArmor.CalculateTotalDefense();
//             Assert.AreEqual(expectedDefense, actualDefense);
//         }
//
//         [Test]
//         public void Accessory_AddStatModifier_ShouldIncreaseModifierCount()
//         {
//             int initialCount = testAccessory.StatModifiers.Length;
//             testAccessory.AddStatModifier(StatType.Health, 50f, ModifierType.Flat);
//             
//             Assert.AreEqual(initialCount + 1, testAccessory.StatModifiers.Length);
//             Assert.AreEqual(StatType.Health, testAccessory.StatModifiers[initialCount].StatType);
//             Assert.AreEqual(50f, testAccessory.StatModifiers[initialCount].Value);
//             Assert.AreEqual(ModifierType.Flat, testAccessory.StatModifiers[initialCount].ModifierType);
//         }
//
//         [Test]
//         public void EquipmentDatabase_AddWeapon_ShouldIncreaseWeaponCount()
//         {
//             int initialCount = testDatabase.Weapons.Count;
//             bool added = testDatabase.AddWeapon(testWeapon);
//             
//             Assert.IsTrue(added);
//             Assert.AreEqual(initialCount + 1, testDatabase.Weapons.Count);
//             Assert.Contains(testWeapon, testDatabase.Weapons);
//         }
//
//         [Test]
//         public void EquipmentDatabase_AddWeapon_ShouldRejectDuplicates()
//         {
//             testDatabase.AddWeapon(testWeapon);
//             int countAfterFirst = testDatabase.Weapons.Count;
//             
//             bool added = testDatabase.AddWeapon(testWeapon);
//             
//             Assert.IsFalse(added);
//             Assert.AreEqual(countAfterFirst, testDatabase.Weapons.Count);
//         }
//
//         [Test]
//         public void EquipmentDatabase_FindEquipmentById_ShouldReturnCorrectEquipment()
//         {
//             testDatabase.AddWeapon(testWeapon);
//             testDatabase.InitializeLookupTables();
//             
//             Equipment found = testDatabase.FindEquipmentById("Test_Sword");
//             
//             Assert.IsNotNull(found);
//             Assert.AreEqual(testWeapon, found);
//         }
//
//         [Test]
//         public void EquipmentDatabase_GetEquipmentByType_ShouldReturnCorrectType()
//         {
//             testDatabase.AddWeapon(testWeapon);
//             testDatabase.AddAccessory(testAccessory);
//             testDatabase.InitializeLookupTables();
//             
//             var weapons = testDatabase.GetEquipmentByType(EquipmentType.Weapon);
//             var accessories = testDatabase.GetEquipmentByType(EquipmentType.Accessory);
//             
//             Assert.AreEqual(1, weapons.Count);
//             Assert.AreEqual(1, accessories.Count);
//             Assert.Contains(testWeapon, weapons);
//             Assert.Contains(testAccessory, accessories);
//         }
//
//         [Test]
//         public void Equipment_GenerateDefaultValues_ShouldSetValidValues()
//         {
//             testWeapon.GenerateDefaultValues();
//             
//             Assert.IsTrue(testWeapon.Level >= 1);
//             Assert.IsTrue(testWeapon.Weight >= 0f);
//             Assert.IsTrue(testWeapon.Value >= 0);
//             Assert.IsTrue(testWeapon.Damage >= 1);
//             Assert.IsTrue(testWeapon.AttackSpeed >= 0.1f);
//             Assert.IsTrue(testWeapon.Range >= 0.1f);
//             Assert.IsTrue(testWeapon.CriticalChance >= 0 && testWeapon.CriticalChance <= 100);
//             Assert.IsTrue(testWeapon.CriticalMultiplier >= 1f);
//         }
//
//         [Test]
//         public void Armor_SetDamageResistance_ShouldAddOrUpdateResistance()
//         {
//             testArmor.SetDamageResistance(DamageType.Fire, 25f);
//             
//             float fireResistance = testArmor.GetDamageResistance(DamageType.Fire);
//             Assert.AreEqual(25f, fireResistance);
//             
//             // Update existing resistance
//             testArmor.SetDamageResistance(DamageType.Fire, 30f);
//             fireResistance = testArmor.GetDamageResistance(DamageType.Fire);
//             Assert.AreEqual(30f, fireResistance);
//         }
//
//         [Test]
//         public void Equipment_RarityMultiplier_ShouldBeCorrect()
//         {
//             testWeapon.Rarity = EquipmentRarity.Common;
//             testWeapon.Value = 100;
//             testWeapon.GenerateDefaultValues();
//             int commonValue = testWeapon.Value;
//
//             testWeapon.Rarity = EquipmentRarity.Legendary;
//             testWeapon.Value = 100;
//             testWeapon.GenerateDefaultValues();
//             int legendaryValue = testWeapon.Value;
//
//             Assert.Greater(legendaryValue, commonValue);
//         }
//     }
// }