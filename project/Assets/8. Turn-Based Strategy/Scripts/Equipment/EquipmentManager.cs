using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    /// <summary>
    /// Manages equipped items for a character and calculates combined stat bonuses.
    /// Supports both legacy Equipment instances and new ScriptableObject-based equipment.
    /// </summary>
    [Serializable]
    public class EquipmentManager
    {
        [SerializeField] private LegacyEquipment weapon;
        [SerializeField] private LegacyEquipment armor;
        [SerializeField] private LegacyEquipment accessory;

        // Support for new ScriptableObject equipment
        [SerializeField] private EquipmentSO weaponSO;
        [SerializeField] private EquipmentSO armorSO;
        [SerializeField] private EquipmentSO accessorySO;

        /// <summary>
        /// Event fired when equipment changes
        /// </summary>
        public event Action OnEquipmentChanged;

        public LegacyEquipment Weapon => weapon;
        public LegacyEquipment Armor => armor;
        public LegacyEquipment Accessory => accessory;

        // New ScriptableObject accessors
        public EquipmentSO WeaponSO => weaponSO;
        public EquipmentSO ArmorSO => armorSO;
        public EquipmentSO AccessorySO => accessorySO;

        /// <summary>
        /// Gets the legacy equipment in the specified slot
        /// </summary>
        /// <param name="slot">The equipment slot</param>
        /// <returns>The equipped item or null if no item is equipped</returns>
        public LegacyEquipment GetEquippedItem(EquipmentSlot slot)
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
        /// Gets the ScriptableObject equipment in the specified slot
        /// </summary>
        /// <param name="slot">The equipment slot</param>
        /// <returns>The equipped item or null if no item is equipped</returns>
        public EquipmentSO GetEquippedItemSO(EquipmentSlot slot)
        {
            switch (slot)
            {
                case EquipmentSlot.Weapon:
                    return weaponSO;
                case EquipmentSlot.Armor:
                    return armorSO;
                case EquipmentSlot.Accessory:
                    return accessorySO;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Equips a legacy equipment item to the appropriate slot
        /// </summary>
        /// <param name="equipment">The equipment to equip</param>
        /// <returns>The previously equipped item in that slot, or null if no item was equipped</returns>
        public LegacyEquipment EquipItem(LegacyEquipment equipment)
        {
            if (equipment == null)
                return null;

            LegacyEquipment previousItem = GetEquippedItem(equipment.Slot);

            switch (equipment.Slot)
            {
                case EquipmentSlot.Weapon:
                    weapon = equipment;
                    weaponSO = null; // Clear ScriptableObject version
                    break;
                case EquipmentSlot.Armor:
                    armor = equipment;
                    armorSO = null;
                    break;
                case EquipmentSlot.Accessory:
                    accessory = equipment;
                    accessorySO = null;
                    break;
            }

            OnEquipmentChanged?.Invoke();
            return previousItem;
        }

        /// <summary>
        /// Equips a ScriptableObject equipment item to the appropriate slot
        /// </summary>
        /// <param name="equipment">The equipment to equip</param>
        /// <returns>The previously equipped item in that slot, or null if no item was equipped</returns>
        public EquipmentSO EquipItemSO(EquipmentSO equipment)
        {
            if (equipment == null)
                return null;

            EquipmentSO previousItem = GetEquippedItemSO(equipment.Slot);

            switch (equipment.Slot)
            {
                case EquipmentSlot.Weapon:
                    weaponSO = equipment;
                    weapon = null; // Clear legacy version
                    break;
                case EquipmentSlot.Armor:
                    armorSO = equipment;
                    armor = null;
                    break;
                case EquipmentSlot.Accessory:
                    accessorySO = equipment;
                    accessory = null;
                    break;
            }

            OnEquipmentChanged?.Invoke();
            return previousItem;
        }

        /// <summary>
        /// Unequips the legacy item in the specified slot
        /// </summary>
        /// <param name="slot">The slot to unequip</param>
        /// <returns>The unequipped item, or null if no item was equipped</returns>
        public LegacyEquipment UnequipItem(EquipmentSlot slot)
        {
            LegacyEquipment unequippedItem = GetEquippedItem(slot);

            switch (slot)
            {
                case EquipmentSlot.Weapon:
                    weapon = null;
                    weaponSO = null;
                    break;
                case EquipmentSlot.Armor:
                    armor = null;
                    armorSO = null;
                    break;
                case EquipmentSlot.Accessory:
                    accessory = null;
                    accessorySO = null;
                    break;
            }

            OnEquipmentChanged?.Invoke();
            return unequippedItem;
        }

        /// <summary>
        /// Unequips the ScriptableObject item in the specified slot
        /// </summary>
        /// <param name="slot">The slot to unequip</param>
        /// <returns>The unequipped item, or null if no item was equipped</returns>
        public EquipmentSO UnequipItemSO(EquipmentSlot slot)
        {
            EquipmentSO unequippedItem = GetEquippedItemSO(slot);

            switch (slot)
            {
                case EquipmentSlot.Weapon:
                    weapon = null;
                    weaponSO = null;
                    break;
                case EquipmentSlot.Armor:
                    armor = null;
                    armorSO = null;
                    break;
                case EquipmentSlot.Accessory:
                    accessory = null;
                    accessorySO = null;
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
            float result = baseValue;

            // Apply additive modifiers from all equipment first
            result += GetAdditiveModifier(statType);

            // Then apply multiplicative modifiers from all equipment
            float multiplicativeModifier = GetMultiplicativeModifier(statType);
            result *= (1 + multiplicativeModifier / 100f);

            return result;
        }

        /// <summary>
        /// Gets the total additive modifier for a stat type from all equipped items
        /// </summary>
        /// <param name="statType">The stat type</param>
        /// <returns>The total additive modifier</returns>
        public float GetAdditiveModifier(StatType statType)
        {
            float total = 0f;

            // Check legacy equipment
            if (weapon != null)
                total += GetAdditiveModifierFromLegacyEquipment(weapon, statType);
            if (armor != null)
                total += GetAdditiveModifierFromLegacyEquipment(armor, statType);
            if (accessory != null)
                total += GetAdditiveModifierFromLegacyEquipment(accessory, statType);

            // Check new ScriptableObject equipment
            if (weaponSO != null)
                total += GetAdditiveModifierFromEquipmentSO(weaponSO, statType);
            if (armorSO != null)
                total += GetAdditiveModifierFromEquipmentSO(armorSO, statType);
            if (accessorySO != null)
                total += GetAdditiveModifierFromEquipmentSO(accessorySO, statType);

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

            // Check legacy equipment
            if (weapon != null)
                total += GetMultiplicativeModifierFromLegacyEquipment(weapon, statType);
            if (armor != null)
                total += GetMultiplicativeModifierFromLegacyEquipment(armor, statType);
            if (accessory != null)
                total += GetMultiplicativeModifierFromLegacyEquipment(accessory, statType);

            // Check new ScriptableObject equipment
            if (weaponSO != null)
                total += GetMultiplicativeModifierFromEquipmentSO(weaponSO, statType);
            if (armorSO != null)
                total += GetMultiplicativeModifierFromEquipmentSO(armorSO, statType);
            if (accessorySO != null)
                total += GetMultiplicativeModifierFromEquipmentSO(accessorySO, statType);

            return total;
        }

        private float GetAdditiveModifierFromLegacyEquipment(LegacyEquipment equipment, StatType statType)
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

        private float GetMultiplicativeModifierFromLegacyEquipment(LegacyEquipment equipment, StatType statType)
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

        private float GetAdditiveModifierFromEquipmentSO(EquipmentSO equipment, StatType statType)
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

        private float GetMultiplicativeModifierFromEquipmentSO(EquipmentSO equipment, StatType statType)
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
        /// Gets all currently equipped legacy items
        /// </summary>
        /// <returns>List of equipped legacy items</returns>
        public List<LegacyEquipment> GetAllEquippedItems()
        {
            var equippedItems = new List<LegacyEquipment>();
            
            if (weapon != null) equippedItems.Add(weapon);
            if (armor != null) equippedItems.Add(armor);
            if (accessory != null) equippedItems.Add(accessory);
            
            return equippedItems;
        }

        /// <summary>
        /// Gets all currently equipped ScriptableObject items
        /// </summary>
        /// <returns>List of equipped ScriptableObject items</returns>
        public List<EquipmentSO> GetAllEquippedItemsSO()
        {
            var equippedItems = new List<EquipmentSO>();
            
            if (weaponSO != null) equippedItems.Add(weaponSO);
            if (armorSO != null) equippedItems.Add(armorSO);
            if (accessorySO != null) equippedItems.Add(accessorySO);
            
            return equippedItems;
        }

        /// <summary>
        /// Removes all equipped items (both legacy and ScriptableObject)
        /// </summary>
        /// <returns>List of legacy items that were unequipped</returns>
        public List<LegacyEquipment> UnequipAll()
        {
            var unequippedItems = GetAllEquippedItems();
            
            weapon = null;
            armor = null;
            accessory = null;
            weaponSO = null;
            armorSO = null;
            accessorySO = null;
            
            OnEquipmentChanged?.Invoke();
            return unequippedItems;
        }
    }
}