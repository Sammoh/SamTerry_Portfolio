using System;
using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    public enum EquipmentSlot
    {
        Weapon,
        Armor,
        Accessory
    }

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
            this.description = description;
        }
    }

    [Serializable]
    public class StatModifier
    {
        [SerializeField] private StatType statType;
        [SerializeField] private int value;
        [SerializeField] private ModifierType modifierType;

        public StatType StatType => statType;
        public int Value => value;
        public ModifierType ModifierType => modifierType;

        public StatModifier(StatType statType, int value, ModifierType modifierType = ModifierType.Additive)
        {
            this.statType = statType;
            this.value = value;
            this.modifierType = modifierType;
        }

        public int ApplyModifier(int baseValue)
        {
            switch (modifierType)
            {
                case ModifierType.Additive:
                    return baseValue + value;
                case ModifierType.Multiplicative:
                    return Mathf.RoundToInt(baseValue * (1 + value / 100f));
                default:
                    return baseValue;
            }
        }
    }

    public enum StatType
    {
        MaxHealth,
        Attack,
        Defense,
        Speed,
        Mana
    }

    public enum ModifierType
    {
        Additive,      // +5 Attack
        Multiplicative // +20% Attack
    }
}