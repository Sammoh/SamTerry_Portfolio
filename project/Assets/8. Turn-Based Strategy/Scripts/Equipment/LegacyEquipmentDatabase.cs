using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    /// <summary>
    /// Legacy ScriptableObject database for storing and managing equipment items.
    /// This version uses the legacy Equipment class structure.
    /// </summary>
    [CreateAssetMenu(fileName = "LegacyEquipmentDatabase", menuName = "Turn-Based Strategy/Legacy Equipment Database")]
    public class LegacyEquipmentDatabase : ScriptableObject
    {
        [SerializeField] private List<LegacyEquipment> weapons = new List<LegacyEquipment>();
        [SerializeField] private List<LegacyEquipment> armor = new List<LegacyEquipment>();
        [SerializeField] private List<LegacyEquipment> accessories = new List<LegacyEquipment>();

        public List<LegacyEquipment> Weapons => weapons;
        public List<LegacyEquipment> Armor => armor;
        public List<LegacyEquipment> Accessories => accessories;

        /// <summary>
        /// Gets all equipment of a specific slot type
        /// </summary>
        /// <param name="slot">The equipment slot type</param>
        /// <returns>List of equipment for the specified slot</returns>
        public List<LegacyEquipment> GetEquipmentBySlot(EquipmentSlot slot)
        {
            switch (slot)
            {
                case EquipmentSlot.Weapon:
                    return new List<LegacyEquipment>(weapons);
                case EquipmentSlot.Armor:
                    return new List<LegacyEquipment>(armor);
                case EquipmentSlot.Accessory:
                    return new List<LegacyEquipment>(accessories);
                default:
                    return new List<LegacyEquipment>();
            }
        }

        /// <summary>
        /// Gets all equipment in the database
        /// </summary>
        /// <returns>List of all equipment items</returns>
        public List<LegacyEquipment> GetAllEquipment()
        {
            var allEquipment = new List<LegacyEquipment>();
            allEquipment.AddRange(weapons);
            allEquipment.AddRange(armor);
            allEquipment.AddRange(accessories);
            return allEquipment;
        }

        /// <summary>
        /// Adds equipment to the database
        /// </summary>
        /// <param name="equipment">The equipment to add</param>
        public void AddEquipment(LegacyEquipment equipment)
        {
            if (equipment == null) return;

            switch (equipment.Slot)
            {
                case EquipmentSlot.Weapon:
                    if (!weapons.Contains(equipment))
                        weapons.Add(equipment);
                    break;
                case EquipmentSlot.Armor:
                    if (!armor.Contains(equipment))
                        armor.Add(equipment);
                    break;
                case EquipmentSlot.Accessory:
                    if (!accessories.Contains(equipment))
                        accessories.Add(equipment);
                    break;
            }
        }

        /// <summary>
        /// Removes equipment from the database
        /// </summary>
        /// <param name="equipment">The equipment to remove</param>
        /// <returns>True if equipment was removed, false otherwise</returns>
        public bool RemoveEquipment(LegacyEquipment equipment)
        {
            if (equipment == null) return false;

            switch (equipment.Slot)
            {
                case EquipmentSlot.Weapon:
                    return weapons.Remove(equipment);
                case EquipmentSlot.Armor:
                    return armor.Remove(equipment);
                case EquipmentSlot.Accessory:
                    return accessories.Remove(equipment);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Finds equipment by name
        /// </summary>
        /// <param name="name">The name of the equipment</param>
        /// <returns>The equipment if found, null otherwise</returns>
        public LegacyEquipment FindEquipmentByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            var allEquipment = GetAllEquipment();
            return allEquipment.Find(eq => eq.EquipmentName.Equals(name, System.StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Creates default equipment for demonstration purposes
        /// </summary>
        [ContextMenu("Create Default Equipment")]
        public void CreateDefaultEquipment()
        {
            // Clear existing equipment
            weapons.Clear();
            armor.Clear();
            accessories.Clear();

            // Create weapons
            weapons.Add(new LegacyEquipment("Iron Sword", EquipmentSlot.Weapon,
                new StatModifier[] {
                    new StatModifier(StatType.Attack, 5, ModifierType.Additive)
                },
                "A sturdy iron sword"));

            weapons.Add(new LegacyEquipment("Steel Blade", EquipmentSlot.Weapon,
                new StatModifier[] {
                    new StatModifier(StatType.Attack, 8, ModifierType.Additive),
                    new StatModifier(StatType.Speed, 2, ModifierType.Additive)
                },
                "A sharp steel blade"));

            weapons.Add(new LegacyEquipment("Flame Sword", EquipmentSlot.Weapon,
                new StatModifier[] {
                    new StatModifier(StatType.Attack, 12, ModifierType.Additive),
                    new StatModifier(StatType.Mana, -5, ModifierType.Additive)
                },
                "A magical sword wreathed in flames"));

            // Create armor
            armor.Add(new LegacyEquipment("Leather Armor", EquipmentSlot.Armor,
                new StatModifier[] {
                    new StatModifier(StatType.Defense, 3, ModifierType.Additive),
                    new StatModifier(StatType.MaxHealth, 10, ModifierType.Additive)
                },
                "Light and flexible leather armor"));

            armor.Add(new LegacyEquipment("Chain Mail", EquipmentSlot.Armor,
                new StatModifier[] {
                    new StatModifier(StatType.Defense, 6, ModifierType.Additive),
                    new StatModifier(StatType.MaxHealth, 15, ModifierType.Additive),
                    new StatModifier(StatType.Speed, -2, ModifierType.Additive)
                },
                "Heavy chain mail armor"));

            armor.Add(new LegacyEquipment("Mage Robes", EquipmentSlot.Armor,
                new StatModifier[] {
                    new StatModifier(StatType.Mana, 20, ModifierType.Additive),
                    new StatModifier(StatType.Defense, -2, ModifierType.Additive)
                },
                "Enchanted robes that enhance magical power"));

            // Create accessories
            accessories.Add(new LegacyEquipment("Power Ring", EquipmentSlot.Accessory,
                new StatModifier[] {
                    new StatModifier(StatType.Attack, 20, ModifierType.Multiplicative)
                },
                "A ring that amplifies physical strength"));

            accessories.Add(new LegacyEquipment("Health Amulet", EquipmentSlot.Accessory,
                new StatModifier[] {
                    new StatModifier(StatType.MaxHealth, 30, ModifierType.Additive)
                },
                "An amulet that grants additional vitality"));

            accessories.Add(new LegacyEquipment("Speed Boots", EquipmentSlot.Accessory,
                new StatModifier[] {
                    new StatModifier(StatType.Speed, 5, ModifierType.Additive)
                },
                "Boots that enhance movement speed"));

            Debug.Log($"Created default equipment: {weapons.Count} weapons, {armor.Count} armor pieces, {accessories.Count} accessories");
        }
    }
}