// using System.Collections.Generic;
// using UnityEngine;
//
// namespace Sammoh.TurnBasedStrategy
// {
//     /// <summary>
//     /// ScriptableObject database for storing and managing equipment items
//     /// </summary>
//     [CreateAssetMenu(fileName = "EquipmentDatabase", menuName = "Turn-Based Strategy/Equipment Database")]
//     public class EquipmentDatabase : ScriptableObject
//     {
//         [SerializeField] private List<Equipment> weapons = new List<Equipment>();
//         [SerializeField] private List<Equipment> armor = new List<Equipment>();
//         [SerializeField] private List<Equipment> accessories = new List<Equipment>();
//
//         public List<Equipment> Weapons => weapons;
//         public List<Equipment> Armor => armor;
//         public List<Equipment> Accessories => accessories;
//
//         /// <summary>
//         /// Gets all equipment of a specific slot type
//         /// </summary>
//         /// <param name="slot">The equipment slot type</param>
//         /// <returns>List of equipment for the specified slot</returns>
//         public List<Equipment> GetEquipmentBySlot(EquipmentSlot slot)
//         {
//             switch (slot)
//             {
//                 case EquipmentSlot.Weapon:
//                     return new List<Equipment>(weapons);
//                 case EquipmentSlot.Armor:
//                     return new List<Equipment>(armor);
//                 case EquipmentSlot.Accessory:
//                     return new List<Equipment>(accessories);
//                 default:
//                     return new List<Equipment>();
//             }
//         }
//
//         /// <summary>
//         /// Gets all equipment in the database
//         /// </summary>
//         /// <returns>List of all equipment items</returns>
//         public List<Equipment> GetAllEquipment()
//         {
//             var allEquipment = new List<Equipment>();
//             allEquipment.AddRange(weapons);
//             allEquipment.AddRange(armor);
//             allEquipment.AddRange(accessories);
//             return allEquipment;
//         }
//
//         /// <summary>
//         /// Adds equipment to the database
//         /// </summary>
//         /// <param name="equipment">The equipment to add</param>
//         public void AddEquipment(Equipment equipment)
//         {
//             if (equipment == null) return;
//
//             switch (equipment.Slot)
//             {
//                 case EquipmentSlot.Weapon:
//                     if (!weapons.Contains(equipment))
//                         weapons.Add(equipment);
//                     break;
//                 case EquipmentSlot.Armor:
//                     if (!armor.Contains(equipment))
//                         armor.Add(equipment);
//                     break;
//                 case EquipmentSlot.Accessory:
//                     if (!accessories.Contains(equipment))
//                         accessories.Add(equipment);
//                     break;
//             }
//         }
//
//         /// <summary>
//         /// Removes equipment from the database
//         /// </summary>
//         /// <param name="equipment">The equipment to remove</param>
//         /// <returns>True if equipment was removed, false otherwise</returns>
//         public bool RemoveEquipment(Equipment equipment)
//         {
//             if (equipment == null) return false;
//
//             switch (equipment.Slot)
//             {
//                 case EquipmentSlot.Weapon:
//                     return weapons.Remove(equipment);
//                 case EquipmentSlot.Armor:
//                     return armor.Remove(equipment);
//                 case EquipmentSlot.Accessory:
//                     return accessories.Remove(equipment);
//                 default:
//                     return false;
//             }
//         }
//
//         /// <summary>
//         /// Finds equipment by name
//         /// </summary>
//         /// <param name="name">The name of the equipment</param>
//         /// <returns>The equipment if found, null otherwise</returns>
//         public Equipment FindEquipmentByName(string name)
//         {
//             if (string.IsNullOrEmpty(name)) return null;
//
//             var allEquipment = GetAllEquipment();
//             return allEquipment.Find(eq => eq.EquipmentName.Equals(name, System.StringComparison.OrdinalIgnoreCase));
//         }
//
//         /// <summary>
//         /// Creates default equipment for demonstration purposes
//         /// </summary>
//         [ContextMenu("Create Default Equipment")]
//         public void CreateDefaultEquipment()
//         {
//             // Clear existing equipment
//             weapons.Clear();
//             armor.Clear();
//             accessories.Clear();
//
//             // Create weapons
//             var ironSword = CreateInstance("Iron Sword") as Equipment;
//             var statModifiers = new [] {
//                 new StatModifier(StatType.Attack, 5, ModifierType.Flat)
//             };
//             ironSword.Slot = EquipmentSlot.Weapon;
//             ironSword.StatModifiers = statModifiers;
//             ironSword.Description = "A sturdy iron sword";
//             weapons.Add(ironSword);
//
//             var steelBlade = CreateInstance("Steel Blade") as Equipment;
//             var steelModifiers = new [] {
//                 new StatModifier(StatType.Attack, 8, ModifierType.Flat),
//                 new StatModifier(StatType.Speed, 2, ModifierType.Flat)
//             };
//             steelBlade.Slot = EquipmentSlot.Weapon;
//             steelBlade.StatModifiers = steelModifiers;
//             steelBlade.Description = "A sharp steel blade";
//             weapons.Add(steelBlade);
//             
//             var flameSword = CreateInstance("Flame Sword") as Equipment;
//             var flameModifiers = new [] {
//                 new StatModifier(StatType.Attack, 12, ModifierType.Flat),
//                 new StatModifier(StatType.Speed, 2, ModifierType.Flat)
//             };
//             flameSword.Slot = EquipmentSlot.Weapon;
//             flameSword.StatModifiers = flameModifiers;
//             flameSword.Description = "A magical sword wreathed in flames";
//             weapons.Add(flameSword);
//
//             // Create armor
//             var leatherArmor = CreateInstance("Leather Armor") as Equipment;
//             var leatherModifiers = new [] {
//                 new StatModifier(StatType.Defense, 3, ModifierType.Flat),
//                 new StatModifier(StatType.MaxHealth, 10, ModifierType.Flat)
//             };
//             leatherArmor.Slot = EquipmentSlot.Armor;
//             leatherArmor.StatModifiers = leatherModifiers;
//             leatherArmor.Description = "Light and flexible leather armor";
//             armor.Add(leatherArmor);
//             
//             var chainMail = CreateInstance("Chain Mail") as Equipment;
//             var chainModifiers = new [] {
//                 new StatModifier(StatType.Defense, 6, ModifierType.Flat),
//                 new StatModifier(StatType.MaxHealth, 15, ModifierType.Flat),
//                 new StatModifier(StatType.Speed, -2, ModifierType.Flat)
//             };
//             chainMail.Slot = EquipmentSlot.Armor;
//             chainMail.StatModifiers = chainModifiers;
//             chainMail.Description = "Heavy chain mail armor";
//             armor.Add(chainMail);
//             
//             var mageRobes = CreateInstance("Mage Robes") as Equipment;
//             var mageModifiers = new [] {
//                 new StatModifier(StatType.Mana, 20, ModifierType.Flat),
//                 new StatModifier(StatType.Defense, -2, ModifierType.Flat)
//             };
//             mageRobes.Slot = EquipmentSlot.Armor;
//             mageRobes.StatModifiers = mageModifiers;
//             mageRobes.Description = "Enchanted robes that enhance magical power";
//             armor.Add(mageRobes);
//
//             // Create accessories
//             var powerRing = CreateInstance("Power Ring") as Equipment;
//             var ringModifiers = new [] {
//                 new StatModifier(StatType.Attack, 20, ModifierType.Percentage)
//             };
//             powerRing.Slot = EquipmentSlot.Accessory;
//             powerRing.StatModifiers = ringModifiers;
//             powerRing.Description = "A ring that amplifies physical strength";
//             accessories.Add(powerRing);
//             
//             var healthAmulet = CreateInstance("Health Amulet") as Equipment;
//             var amuletModifiers = new [] {
//                 new StatModifier(StatType.MaxHealth, 30, ModifierType.Flat)
//             };
//             healthAmulet.Slot = EquipmentSlot.Accessory;
//             healthAmulet.StatModifiers = amuletModifiers;
//             healthAmulet.Description = "An amulet that grants additional vitality";
//             accessories.Add(healthAmulet);
//             
//             var luckCharm = CreateInstance("Luck Charm") as Equipment;
//             var charmModifiers = new [] {
//                 new StatModifier(StatType.Luck, 10, ModifierType.Percentage)
//             };
//             luckCharm.Slot = EquipmentSlot.Accessory;
//             luckCharm.StatModifiers = charmModifiers;
//             luckCharm.Description = "A charm that brings good fortune";
//             accessories.Add(luckCharm);
//
//             Debug.Log($"Created default equipment: {weapons.Count} weapons, {armor.Count} armor pieces, {accessories.Count} accessories");
//         }
//     }
// }