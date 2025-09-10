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

    public enum EquipmentRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    [Serializable]
    public class EquipmentStats
    {
        [SerializeField] private int healthBonus;
        [SerializeField] private int attackBonus;
        [SerializeField] private int defenseBonus;
        [SerializeField] private int speedBonus;
        [SerializeField] private int manaBonus;

        public int HealthBonus => healthBonus;
        public int AttackBonus => attackBonus;
        public int DefenseBonus => defenseBonus;
        public int SpeedBonus => speedBonus;
        public int ManaBonus => manaBonus;

        public EquipmentStats(int health = 0, int attack = 0, int defense = 0, int speed = 0, int mana = 0)
        {
            this.healthBonus = health;
            this.attackBonus = attack;
            this.defenseBonus = defense;
            this.speedBonus = speed;
            this.manaBonus = mana;
        }
    }

    [Serializable]
    public class Equipment
    {
        [SerializeField] private string equipmentName;
        [SerializeField] private string description;
        [SerializeField] private EquipmentSlot slot;
        [SerializeField] private EquipmentRarity rarity;
        [SerializeField] private EquipmentStats stats;

        public string EquipmentName => equipmentName;
        public string Description => description;
        public EquipmentSlot Slot => slot;
        public EquipmentRarity Rarity => rarity;
        public EquipmentStats Stats => stats;

        public Equipment(string name, string description, EquipmentSlot slot, EquipmentRarity rarity, EquipmentStats stats)
        {
            this.equipmentName = name;
            this.description = description;
            this.slot = slot;
            this.rarity = rarity;
            this.stats = stats;
        }

        public bool CanEquip(CharacterClass characterClass)
        {
            // Basic equipment restrictions based on character class
            switch (slot)
            {
                case EquipmentSlot.Weapon:
                    return true; // All classes can use weapons
                case EquipmentSlot.Armor:
                    return true; // All classes can wear armor
                case EquipmentSlot.Accessory:
                    return true; // All classes can use accessories
                default:
                    return true;
            }
        }
    }
}