using System;
using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    /// <summary>
    /// Represents an equipment item that can be equipped by characters
    /// </summary>
    [CreateAssetMenu(fileName = "New Equipment", menuName = "Turn-Based Strategy/Equipment")]
    public class Equipment : ScriptableObject
    {
        [SerializeField] private string equipmentName;
        [SerializeField] private EquipmentSlot slot;
        [SerializeField] private StatModifier[] statModifiers;
        [SerializeField] private string description;

        public string EquipmentName => equipmentName;
        public EquipmentSlot Slot => slot;
        public StatModifier[] StatModifiers => statModifiers;
        public string Description => description;

        /// <summary>
        /// Initializes the equipment with the specified properties
        /// </summary>
        /// <param name="name">The equipment name</param>
        /// <param name="slot">The equipment slot</param>
        /// <param name="modifiers">Array of stat modifiers</param>
        /// <param name="description">Equipment description</param>
        public void Initialize(string name, EquipmentSlot slot, StatModifier[] modifiers, string description)
        {
            this.equipmentName = name ?? "Unnamed Equipment";
            this.slot = slot;
            this.statModifiers = modifiers ?? new StatModifier[0];
            this.description = description ?? "";

            // Validate modifiers
            ValidateModifiers();
        }

        /// <summary>
        /// Validates that all stat modifiers are properly configured
        /// </summary>
        private void ValidateModifiers()
        {
            if (statModifiers == null) return;

            for (int i = 0; i < statModifiers.Length; i++)
            {
                var modifier = statModifiers[i];
                if (modifier == null)
                {
                    Debug.LogWarning($"Equipment '{equipmentName}' has null modifier at index {i}");
                    continue;
                }

                // Warn about potentially problematic modifiers
                if (modifier.ModifierType == ModifierType.Multiplicative)
                {
                    if (modifier.Value < -100f)
                    {
                        Debug.LogWarning($"Equipment '{equipmentName}' has multiplicative modifier for {modifier.StatType} " +
                                       $"that reduces stat by more than 100% ({modifier.Value}%). This will result in negative values.");
                    }
                }
            }
        }

        /// <summary>
        /// Gets the total modifier for a specific stat type from this equipment
        /// Note: For complex calculations, use EquipmentManager.CalculateModifiedStat instead
        /// </summary>
        /// <param name="statType">The stat type to get modifiers for</param>
        /// <param name="baseValue">The base value of the stat</param>
        /// <returns>The modified value after applying this equipment's modifiers</returns>
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