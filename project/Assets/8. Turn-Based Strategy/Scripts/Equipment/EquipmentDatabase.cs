using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    /// <summary>
    /// ScriptableObject database for storing and managing equipment asset references
    /// </summary>
    [CreateAssetMenu(fileName = "EquipmentDatabase", menuName = "Turn-Based Strategy/Equipment Database")]
    public class EquipmentDatabase : ScriptableObject
    {
        [SerializeField] private Equipment[] weapons = new Equipment[0];
        [SerializeField] private Equipment[] armor = new Equipment[0];
        [SerializeField] private Equipment[] accessories = new Equipment[0];

        public Equipment[] Weapons => weapons;
        public Equipment[] Armor => armor;
        public Equipment[] Accessories => accessories;

        /// <summary>
        /// Gets all equipment of a specific slot type
        /// </summary>
        /// <param name="slot">The equipment slot type</param>
        /// <returns>List of equipment for the specified slot</returns>
        public List<Equipment> GetEquipmentBySlot(EquipmentSlot slot)
        {
            switch (slot)
            {
                case EquipmentSlot.Weapon:
                    return new List<Equipment>(weapons);
                case EquipmentSlot.Armor:
                    return new List<Equipment>(armor);
                case EquipmentSlot.Accessory:
                    return new List<Equipment>(accessories);
                default:
                    return new List<Equipment>();
            }
        }

        /// <summary>
        /// Gets all equipment in the database
        /// </summary>
        /// <returns>List of all equipment items</returns>
        public List<Equipment> GetAllEquipment()
        {
            var allEquipment = new List<Equipment>();
            allEquipment.AddRange(weapons);
            allEquipment.AddRange(armor);
            allEquipment.AddRange(accessories);
            return allEquipment;
        }

        /// <summary>
        /// Adds equipment to the database (Editor only - use SetEquipmentArrays for runtime)
        /// </summary>
        /// <param name="equipment">The equipment to add</param>
        public void AddEquipment(Equipment equipment)
        {
            if (equipment == null) return;

            var equipmentList = GetEquipmentBySlot(equipment.Slot);
            if (!equipmentList.Contains(equipment))
            {
                equipmentList.Add(equipment);
                SetEquipmentArrays(equipmentList, equipment.Slot);
            }
        }

        /// <summary>
        /// Removes equipment from the database (Editor only - use SetEquipmentArrays for runtime)
        /// </summary>
        /// <param name="equipment">The equipment to remove</param>
        /// <returns>True if equipment was removed, false otherwise</returns>
        public bool RemoveEquipment(Equipment equipment)
        {
            if (equipment == null) return false;

            var equipmentList = GetEquipmentBySlot(equipment.Slot);
            bool removed = equipmentList.Remove(equipment);
            
            if (removed)
            {
                SetEquipmentArrays(equipmentList, equipment.Slot);
            }
            
            return removed;
        }

        /// <summary>
        /// Sets the equipment array for a specific slot type
        /// </summary>
        /// <param name="equipmentList">List of equipment to set</param>
        /// <param name="slot">The slot type to update</param>
        private void SetEquipmentArrays(List<Equipment> equipmentList, EquipmentSlot slot)
        {
            switch (slot)
            {
                case EquipmentSlot.Weapon:
                    weapons = equipmentList.ToArray();
                    break;
                case EquipmentSlot.Armor:
                    armor = equipmentList.ToArray();
                    break;
                case EquipmentSlot.Accessory:
                    accessories = equipmentList.ToArray();
                    break;
            }
        }

        /// <summary>
        /// Finds equipment by name
        /// </summary>
        /// <param name="name">The name of the equipment</param>
        /// <returns>The equipment if found, null otherwise</returns>
        public Equipment FindEquipmentByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            var allEquipment = GetAllEquipment();
            return allEquipment.Find(eq => eq.EquipmentName.Equals(name, System.StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Creates default equipment for demonstration purposes (creates runtime instances, not asset files)
        /// </summary>
        [ContextMenu("Create Default Equipment")]
        public void CreateDefaultEquipment()
        {
            // Clear existing equipment
            weapons = new Equipment[0];
            armor = new Equipment[0];
            accessories = new Equipment[0];

            var weaponsList = new List<Equipment>();
            var armorList = new List<Equipment>();
            var accessoriesList = new List<Equipment>();

            // Create weapons
            var ironSword = ScriptableObject.CreateInstance<Equipment>();
            ironSword.Initialize("Iron Sword", EquipmentSlot.Weapon,
                new StatModifier[] {
                    new StatModifier(StatType.Attack, 5, ModifierType.Additive)
                },
                "A sturdy iron sword");
            weaponsList.Add(ironSword);

            var steelBlade = ScriptableObject.CreateInstance<Equipment>();
            steelBlade.Initialize("Steel Blade", EquipmentSlot.Weapon,
                new StatModifier[] {
                    new StatModifier(StatType.Attack, 8, ModifierType.Additive),
                    new StatModifier(StatType.Speed, 2, ModifierType.Additive)
                },
                "A sharp steel blade");
            weaponsList.Add(steelBlade);

            var flameSword = ScriptableObject.CreateInstance<Equipment>();
            flameSword.Initialize("Flame Sword", EquipmentSlot.Weapon,
                new StatModifier[] {
                    new StatModifier(StatType.Attack, 12, ModifierType.Additive),
                    new StatModifier(StatType.Mana, -5, ModifierType.Additive)
                },
                "A magical sword wreathed in flames");
            weaponsList.Add(flameSword);

            // Create armor
            var leatherArmor = ScriptableObject.CreateInstance<Equipment>();
            leatherArmor.Initialize("Leather Armor", EquipmentSlot.Armor,
                new StatModifier[] {
                    new StatModifier(StatType.Defense, 3, ModifierType.Additive),
                    new StatModifier(StatType.MaxHealth, 10, ModifierType.Additive)
                },
                "Light and flexible leather armor");
            armorList.Add(leatherArmor);

            var chainMail = ScriptableObject.CreateInstance<Equipment>();
            chainMail.Initialize("Chain Mail", EquipmentSlot.Armor,
                new StatModifier[] {
                    new StatModifier(StatType.Defense, 6, ModifierType.Additive),
                    new StatModifier(StatType.MaxHealth, 15, ModifierType.Additive),
                    new StatModifier(StatType.Speed, -2, ModifierType.Additive)
                },
                "Heavy chain mail armor");
            armorList.Add(chainMail);

            var mageRobes = ScriptableObject.CreateInstance<Equipment>();
            mageRobes.Initialize("Mage Robes", EquipmentSlot.Armor,
                new StatModifier[] {
                    new StatModifier(StatType.Mana, 20, ModifierType.Additive),
                    new StatModifier(StatType.Defense, -2, ModifierType.Additive)
                },
                "Enchanted robes that enhance magical power");
            armorList.Add(mageRobes);

            // Create accessories
            var powerRing = ScriptableObject.CreateInstance<Equipment>();
            powerRing.Initialize("Power Ring", EquipmentSlot.Accessory,
                new StatModifier[] {
                    new StatModifier(StatType.Attack, 20, ModifierType.Multiplicative)
                },
                "A ring that amplifies physical strength");
            accessoriesList.Add(powerRing);

            var healthAmulet = ScriptableObject.CreateInstance<Equipment>();
            healthAmulet.Initialize("Health Amulet", EquipmentSlot.Accessory,
                new StatModifier[] {
                    new StatModifier(StatType.MaxHealth, 30, ModifierType.Additive)
                },
                "An amulet that grants additional vitality");
            accessoriesList.Add(healthAmulet);

            var speedBoots = ScriptableObject.CreateInstance<Equipment>();
            speedBoots.Initialize("Speed Boots", EquipmentSlot.Accessory,
                new StatModifier[] {
                    new StatModifier(StatType.Speed, 5, ModifierType.Additive)
                },
                "Boots that enhance movement speed");
            accessoriesList.Add(speedBoots);

            // Set arrays
            weapons = weaponsList.ToArray();
            armor = armorList.ToArray();
            accessories = accessoriesList.ToArray();

            Debug.Log($"Created default equipment: {weapons.Length} weapons, {armor.Length} armor pieces, {accessories.Length} accessories");
        }
    }
}