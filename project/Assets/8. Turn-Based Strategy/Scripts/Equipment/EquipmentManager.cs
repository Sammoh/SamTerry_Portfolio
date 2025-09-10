using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    /// <summary>
    /// Manages equipped items for a character and calculates combined stat bonuses
    /// </summary>
    [Serializable]
    public class EquipmentManager
    {
        [SerializeField] private Equipment weapon;
        [SerializeField] private Equipment armor;
        [SerializeField] private Equipment accessory;

        /// <summary>
        /// Event fired when equipment changes
        /// </summary>
        public event Action OnEquipmentChanged;

        public Equipment Weapon => weapon;
        public Equipment Armor => armor;
        public Equipment Accessory => accessory;

        /// <summary>
        /// Gets the equipment in the specified slot
        /// </summary>
        /// <param name="slot">The equipment slot</param>
        /// <returns>The equipped item or null if no item is equipped</returns>
        public Equipment GetEquippedItem(EquipmentSlot slot)
        {
            switch (slot)
            {
                case EquipmentSlot.Weapon:
                    return weapon;
                case EquipmentSlot.Armor:
                    return armor;
                case EquipmentSlot.Accessory:
                    return accessory;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Equips an item to the appropriate slot
        /// </summary>
        /// <param name="equipment">The equipment to equip</param>
        /// <returns>The previously equipped item in that slot, or null if no item was equipped</returns>
        public Equipment EquipItem(Equipment equipment)
        {
            if (equipment == null)
            {
                Debug.LogWarning("Attempted to equip null equipment");
                return null;
            }

            Equipment previousItem = GetEquippedItem(equipment.Slot);

            switch (equipment.Slot)
            {
                case EquipmentSlot.Weapon:
                    weapon = equipment;
                    break;
                case EquipmentSlot.Armor:
                    armor = equipment;
                    break;
                case EquipmentSlot.Accessory:
                    accessory = equipment;
                    break;
                default:
                    Debug.LogError($"Unknown equipment slot: {equipment.Slot}");
                    return null;
            }

            OnEquipmentChanged?.Invoke();
            return previousItem;
        }

        /// <summary>
        /// Unequips the item in the specified slot
        /// </summary>
        /// <param name="slot">The slot to unequip</param>
        /// <returns>The unequipped item, or null if no item was equipped</returns>
        public Equipment UnequipItem(EquipmentSlot slot)
        {
            Equipment unequippedItem = GetEquippedItem(slot);

            switch (slot)
            {
                case EquipmentSlot.Weapon:
                    weapon = null;
                    break;
                case EquipmentSlot.Armor:
                    armor = null;
                    break;
                case EquipmentSlot.Accessory:
                    accessory = null;
                    break;
            }

            OnEquipmentChanged?.Invoke();
            return unequippedItem;
        }

        /// <summary>
        /// Calculates the total modifier for a specific stat from all equipped items
        /// </summary>
        /// <param name="statType">The stat type to calculate modifiers for</param>
        /// <param name="baseValue">The base value of the stat</param>
        /// <returns>The modified value after applying all equipment bonuses</returns>
        public float CalculateModifiedStat(StatType statType, float baseValue)
        {
            if (baseValue < 0)
            {
                Debug.LogWarning($"Base value for {statType} is negative: {baseValue}");
            }

            float result = baseValue;

            // Apply additive modifiers from all equipment first
            result += GetAdditiveModifier(statType);

            // Then apply multiplicative modifiers from all equipment
            float multiplicativeModifier = GetMultiplicativeModifier(statType);
            result *= (1 + multiplicativeModifier / 100f);

            // Ensure certain stats don't go below minimum values
            result = ApplyStatLimits(statType, result);

            return result;
        }

        /// <summary>
        /// Applies minimum/maximum limits to stats to prevent invalid values
        /// </summary>
        /// <param name="statType">The stat type</param>
        /// <param name="value">The calculated stat value</param>
        /// <returns>The stat value clamped to valid limits</returns>
        private float ApplyStatLimits(StatType statType, float value)
        {
            switch (statType)
            {
                case StatType.MaxHealth:
                case StatType.Mana:
                    // Health and Mana cannot be negative
                    return Mathf.Max(1, value);
                
                case StatType.Attack:
                case StatType.Defense:
                    // Attack and Defense can be 0 but not negative
                    return Mathf.Max(0, value);
                
                case StatType.Speed:
                    // Speed cannot be negative
                    return Mathf.Max(1, value);
                
                default:
                    return value;
            }
        }

        /// <summary>
        /// Gets the total additive modifier for a stat type from all equipped items
        /// </summary>
        /// <param name="statType">The stat type</param>
        /// <returns>The total additive modifier</returns>
        public float GetAdditiveModifier(StatType statType)
        {
            float total = 0f;

            if (weapon != null)
                total += GetAdditiveModifierFromEquipment(weapon, statType);
            if (armor != null)
                total += GetAdditiveModifierFromEquipment(armor, statType);
            if (accessory != null)
                total += GetAdditiveModifierFromEquipment(accessory, statType);

            return total;
        }

        /// <summary>
        /// Gets the total multiplicative modifier for a stat type from all equipped items
        /// </summary>
        /// <param name="statType">The stat type</param>
        /// <returns>The total multiplicative modifier percentage</returns>
        public float GetMultiplicativeModifier(StatType statType)
        {
            float total = 0f;

            if (weapon != null)
                total += GetMultiplicativeModifierFromEquipment(weapon, statType);
            if (armor != null)
                total += GetMultiplicativeModifierFromEquipment(armor, statType);
            if (accessory != null)
                total += GetMultiplicativeModifierFromEquipment(accessory, statType);

            return total;
        }

        private float GetAdditiveModifierFromEquipment(Equipment equipment, StatType statType)
        {
            float total = 0f;
            foreach (var modifier in equipment.StatModifiers)
            {
                if (modifier.StatType == statType && modifier.ModifierType == ModifierType.Additive)
                {
                    total += modifier.Value;
                }
            }
            return total;
        }

        private float GetMultiplicativeModifierFromEquipment(Equipment equipment, StatType statType)
        {
            float total = 0f;
            foreach (var modifier in equipment.StatModifiers)
            {
                if (modifier.StatType == statType && modifier.ModifierType == ModifierType.Multiplicative)
                {
                    total += modifier.Value;
                }
            }
            return total;
        }

        /// <summary>
        /// Gets all currently equipped items
        /// </summary>
        /// <returns>List of equipped items</returns>
        public List<Equipment> GetAllEquippedItems()
        {
            var equippedItems = new List<Equipment>();
            
            if (weapon != null) equippedItems.Add(weapon);
            if (armor != null) equippedItems.Add(armor);
            if (accessory != null) equippedItems.Add(accessory);
            
            return equippedItems;
        }

        /// <summary>
        /// Removes all equipped items
        /// </summary>
        /// <returns>List of items that were unequipped</returns>
        public List<Equipment> UnequipAll()
        {
            var unequippedItems = GetAllEquippedItems();
            
            weapon = null;
            armor = null;
            accessory = null;
            
            OnEquipmentChanged?.Invoke();
            return unequippedItems;
        }
    }
}