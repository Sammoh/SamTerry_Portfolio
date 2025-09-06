using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    [Serializable]
    public class EquipmentManager
    {
        [SerializeField] private Equipment[] equippedItems = new Equipment[3]; // Weapon, Armor, Accessory

        public Equipment GetEquippedItem(EquipmentSlot slot)
        {
            return equippedItems[(int)slot];
        }

        public bool EquipItem(Equipment equipment)
        {
            if (equipment == null) return false;

            equippedItems[(int)equipment.Slot] = equipment;
            return true;
        }

        public Equipment UnequipItem(EquipmentSlot slot)
        {
            Equipment oldItem = equippedItems[(int)slot];
            equippedItems[(int)slot] = null;
            return oldItem;
        }

        public bool IsSlotEquipped(EquipmentSlot slot)
        {
            return equippedItems[(int)slot] != null;
        }

        public Equipment[] GetAllEquippedItems()
        {
            return equippedItems.Where(item => item != null).ToArray();
        }

        public StatModifier[] GetAllStatModifiers()
        {
            List<StatModifier> allModifiers = new List<StatModifier>();
            
            foreach (var equipment in GetAllEquippedItems())
            {
                if (equipment.StatModifiers != null)
                {
                    allModifiers.AddRange(equipment.StatModifiers);
                }
            }

            return allModifiers.ToArray();
        }

        public int CalculateStatWithEquipment(int baseStat, StatType statType)
        {
            int finalStat = baseStat;
            StatModifier[] modifiers = GetAllStatModifiers();

            // Apply additive modifiers first
            foreach (var modifier in modifiers.Where(m => m.StatType == statType && m.ModifierType == ModifierType.Additive))
            {
                finalStat = modifier.ApplyModifier(finalStat);
            }

            // Then apply multiplicative modifiers
            foreach (var modifier in modifiers.Where(m => m.StatType == statType && m.ModifierType == ModifierType.Multiplicative))
            {
                finalStat = modifier.ApplyModifier(finalStat);
            }

            return Mathf.Max(1, finalStat); // Ensure stats never go below 1
        }
    }
}