using System;
using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    /// <summary>
    /// Represents an equipment item that can be equipped by characters
    /// </summary>
    [Serializable]
    public class Equipment
    {
        [SerializeField] private string equipmentName;
        [SerializeField] private EquipmentSlot slot;
        [SerializeField] private StatModifier[] statModifiers;
        [SerializeField] private string description;

        public string EquipmentName => equipmentName;
        public EquipmentSlot Slot => slot;
        public StatModifier[] StatModifiers => statModifiers;
        public string Description => description;

        public Equipment(string name, EquipmentSlot slot, StatModifier[] modifiers, string description)
        {
            this.equipmentName = name;
            this.slot = slot;
            this.statModifiers = modifiers ?? new StatModifier[0];
            this.description = description ?? "";
        }

        /// <summary>
        /// Gets the total modifier for a specific stat type
        /// </summary>
        /// <param name="statType">The stat type to get modifiers for</param>
        /// <param name="baseValue">The base value of the stat</param>
        /// <returns>The modified value after applying all relevant modifiers</returns>
        public float GetModifiedValue(StatType statType, float baseValue)
        {
            float result = baseValue;
            
            // Apply additive modifiers first
            foreach (var modifier in statModifiers)
            {
                if (modifier.StatType == statType && modifier.ModifierType == ModifierType.Additive)
                {
                    result += modifier.Value;
                }
            }
            
            // Then apply multiplicative modifiers
            foreach (var modifier in statModifiers)
            {
                if (modifier.StatType == statType && modifier.ModifierType == ModifierType.Multiplicative)
                {
                    result *= (1 + modifier.Value / 100f);
                }
            }
            
            return result;
        }

        /// <summary>
        /// Gets all modifiers for a specific stat type
        /// </summary>
        /// <param name="statType">The stat type to get modifiers for</param>
        /// <returns>Array of modifiers for the specified stat type</returns>
        public StatModifier[] GetModifiersForStat(StatType statType)
        {
            return System.Array.FindAll(statModifiers, modifier => modifier.StatType == statType);
        }

        public override string ToString()
        {
            return $"{equipmentName} ({slot})";
        }
    }
}