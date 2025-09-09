using System;
using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    [Serializable]
    public class CharacterStats
    {
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int currentHealth = 100;
        [SerializeField] private int attack = 15;
        [SerializeField] private int defense = 10;
        [SerializeField] private int speed = 12;
        [SerializeField] private int mana = 50;
        [SerializeField] private int currentMana = 50;

        // Equipment manager reference for calculating effective stats
        private EquipmentManager equipmentManager;

        // Base stats (equipment-free values)
        public int BaseMaxHealth => maxHealth;
        public int BaseAttack => attack;
        public int BaseDefense => defense;
        public int BaseSpeed => speed;
        public int BaseMana => mana;

        // Effective stats (base stats + equipment bonuses)
        public int MaxHealth => equipmentManager != null ? 
            Mathf.RoundToInt(equipmentManager.CalculateModifiedStat(StatType.MaxHealth, maxHealth)) : maxHealth;
        public int CurrentHealth => currentHealth;
        public int Attack => equipmentManager != null ? 
            Mathf.RoundToInt(equipmentManager.CalculateModifiedStat(StatType.Attack, attack)) : attack;
        public int Defense => equipmentManager != null ? 
            Mathf.RoundToInt(equipmentManager.CalculateModifiedStat(StatType.Defense, defense)) : defense;
        public int Speed => equipmentManager != null ? 
            Mathf.RoundToInt(equipmentManager.CalculateModifiedStat(StatType.Speed, speed)) : speed;
        public int Mana => equipmentManager != null ? 
            Mathf.RoundToInt(equipmentManager.CalculateModifiedStat(StatType.Mana, mana)) : mana;
        public int CurrentMana => currentMana;

        public bool IsAlive => currentHealth > 0;

        public CharacterStats(int health, int attack, int defense, int speed, int mana)
        {
            this.maxHealth = health;
            this.currentHealth = health;
            this.attack = attack;
            this.defense = defense;
            this.speed = speed;
            this.mana = mana;
            this.currentMana = mana;
        }

        /// <summary>
        /// Sets the equipment manager for calculating effective stats
        /// </summary>
        /// <param name="equipmentManager">The equipment manager to use</param>
        public void SetEquipmentManager(EquipmentManager equipmentManager)
        {
            this.equipmentManager = equipmentManager;
        }

        public void TakeDamage(int damage)
        {
            // Use effective defense for damage calculation
            int finalDamage = Mathf.Max(1, damage - Defense);
            currentHealth = Mathf.Max(0, currentHealth - finalDamage);
        }

        public void Heal(int amount)
        {
            // Use effective max health for healing cap
            currentHealth = Mathf.Min(MaxHealth, currentHealth + amount);
        }

        public bool UseMana(int amount)
        {
            if (currentMana >= amount)
            {
                currentMana -= amount;
                return true;
            }
            return false;
        }

        public void RestoreMana(int amount)
        {
            // Use effective max mana for restoration cap
            currentMana = Mathf.Min(Mana, currentMana + amount);
        }

        public void FullRestore()
        {
            // Use effective max values for full restore
            currentHealth = MaxHealth;
            currentMana = Mana;
        }
    }
}