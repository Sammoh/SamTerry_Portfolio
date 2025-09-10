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
        
        // Cached effective stats to improve performance
        private int? cachedMaxHealth;
        private int? cachedAttack;
        private int? cachedDefense;
        private int? cachedSpeed;
        private int? cachedMana;

        // Base stats (equipment-free values)
        public int BaseMaxHealth => maxHealth;
        public int BaseAttack => attack;
        public int BaseDefense => defense;
        public int BaseSpeed => speed;
        public int BaseMana => mana;

        // Effective stats (base stats + equipment bonuses) with caching
        public int MaxHealth => cachedMaxHealth ?? (cachedMaxHealth = CalculateEffectiveStat(StatType.MaxHealth, maxHealth)).Value;
        public int CurrentHealth => currentHealth;
        public int Attack => cachedAttack ?? (cachedAttack = CalculateEffectiveStat(StatType.Attack, attack)).Value;
        public int Defense => cachedDefense ?? (cachedDefense = CalculateEffectiveStat(StatType.Defense, defense)).Value;
        public int Speed => cachedSpeed ?? (cachedSpeed = CalculateEffectiveStat(StatType.Speed, speed)).Value;
        public int Mana => cachedMana ?? (cachedMana = CalculateEffectiveStat(StatType.Mana, mana)).Value;
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
            // Unsubscribe from old equipment manager events
            if (this.equipmentManager != null)
            {
                this.equipmentManager.OnEquipmentChanged -= InvalidateStatCache;
            }

            this.equipmentManager = equipmentManager;

            // Subscribe to new equipment manager events
            if (this.equipmentManager != null)
            {
                this.equipmentManager.OnEquipmentChanged += InvalidateStatCache;
            }

            // Invalidate cache when equipment manager changes
            InvalidateStatCache();
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

        /// <summary>
        /// Calculates the effective stat value including equipment bonuses
        /// </summary>
        /// <param name="statType">The type of stat to calculate</param>
        /// <param name="baseValue">The base stat value</param>
        /// <returns>The effective stat value including equipment modifications</returns>
        private int CalculateEffectiveStat(StatType statType, int baseValue)
        {
            if (equipmentManager == null)
                return baseValue;

            return Mathf.RoundToInt(equipmentManager.CalculateModifiedStat(statType, baseValue));
        }

        /// <summary>
        /// Invalidates the cached stat values, forcing recalculation on next access
        /// </summary>
        private void InvalidateStatCache()
        {
            cachedMaxHealth = null;
            cachedAttack = null;
            cachedDefense = null;
            cachedSpeed = null;
            cachedMana = null;
        }
    }
}