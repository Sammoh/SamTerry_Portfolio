using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    [Serializable]
    public class EquipmentManager
    {
        [SerializeField] private Equipment weapon;
        [SerializeField] private Equipment armor;
        [SerializeField] private Equipment accessory;

        // Events for equipment changes
        public event Action<Equipment, Equipment> OnEquipmentChanged;

        public Equipment Weapon => weapon;
        public Equipment Armor => armor;
        public Equipment Accessory => accessory;

        public EquipmentManager()
        {
            // Initialize with no equipment
            weapon = null;
            armor = null;
            accessory = null;
        }

        public bool EquipItem(Equipment equipment, CharacterClass characterClass)
        {
            if (equipment == null)
                return false;

            if (!equipment.CanEquip(characterClass))
                return false;

            Equipment previousEquipment = GetEquippedItem(equipment.Slot);
            
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
                    return false;
            }

            OnEquipmentChanged?.Invoke(previousEquipment, equipment);
            return true;
        }

        public Equipment UnequipItem(EquipmentSlot slot)
        {
            Equipment previousEquipment = GetEquippedItem(slot);
            
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

            OnEquipmentChanged?.Invoke(previousEquipment, null);
            return previousEquipment;
        }

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

        public EquipmentStats GetTotalEquipmentStats()
        {
            int totalHealth = 0;
            int totalAttack = 0;
            int totalDefense = 0;
            int totalSpeed = 0;
            int totalMana = 0;

            Equipment[] allEquipment = { weapon, armor, accessory };
            
            foreach (var equipment in allEquipment)
            {
                if (equipment != null && equipment.Stats != null)
                {
                    totalHealth += equipment.Stats.HealthBonus;
                    totalAttack += equipment.Stats.AttackBonus;
                    totalDefense += equipment.Stats.DefenseBonus;
                    totalSpeed += equipment.Stats.SpeedBonus;
                    totalMana += equipment.Stats.ManaBonus;
                }
            }

            return new EquipmentStats(totalHealth, totalAttack, totalDefense, totalSpeed, totalMana);
        }

        public bool HasEquipment()
        {
            return weapon != null || armor != null || accessory != null;
        }

        public List<Equipment> GetAllEquippedItems()
        {
            List<Equipment> equippedItems = new List<Equipment>();
            
            if (weapon != null) equippedItems.Add(weapon);
            if (armor != null) equippedItems.Add(armor);
            if (accessory != null) equippedItems.Add(accessory);
            
            return equippedItems;
        }

        public void ClearAllEquipment()
        {
            UnequipItem(EquipmentSlot.Weapon);
            UnequipItem(EquipmentSlot.Armor);
            UnequipItem(EquipmentSlot.Accessory);
        }
    }
}