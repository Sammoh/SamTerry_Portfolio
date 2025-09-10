using System;
using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    [Serializable]
    public class CharacterStats
    {
        [SerializeField] private int baseMaxHealth = 100;
        [SerializeField] private int currentHealth = 100;
        [SerializeField] private int baseAttack = 15;
        [SerializeField] private int baseDefense = 10;
        [SerializeField] private int baseSpeed = 12;
        [SerializeField] private int baseMana = 50;
        [SerializeField] private int currentMana = 50;

        private EquipmentStats equipmentBonus;

        public int MaxHealth => baseMaxHealth + (equipmentBonus?.HealthBonus ?? 0);
        public int CurrentHealth => currentHealth;
        public int Attack => baseAttack + (equipmentBonus?.AttackBonus ?? 0);
        public int Defense => baseDefense + (equipmentBonus?.DefenseBonus ?? 0);
        public int Speed => baseSpeed + (equipmentBonus?.SpeedBonus ?? 0);
        public int Mana => baseMana + (equipmentBonus?.ManaBonus ?? 0);
        public int CurrentMana => currentMana;

        public bool IsAlive => currentHealth > 0;

        public CharacterStats(int health, int attack, int defense, int speed, int mana)
        {
            this.baseMaxHealth = health;
            this.currentHealth = health;
            this.baseAttack = attack;
            this.baseDefense = defense;
            this.baseSpeed = speed;
            this.baseMana = mana;
            this.currentMana = mana;
            this.equipmentBonus = null;
        }

        public void TakeDamage(int damage)
        {
            int finalDamage = Mathf.Max(1, damage - Defense);
            currentHealth = Mathf.Max(0, currentHealth - finalDamage);
        }

        public void Heal(int amount)
        {
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
            currentMana = Mathf.Min(Mana, currentMana + amount);
        }

        public void FullRestore()
        {
            currentHealth = MaxHealth;
            currentMana = Mana;
        }

        public void UpdateEquipmentBonus(EquipmentStats newBonus)
        {
            int healthDifference = 0;
            int manaDifference = 0;

            // Calculate the difference in max stats
            if (equipmentBonus != null)
            {
                healthDifference = (newBonus?.HealthBonus ?? 0) - equipmentBonus.HealthBonus;
                manaDifference = (newBonus?.ManaBonus ?? 0) - equipmentBonus.ManaBonus;
            }
            else
            {
                healthDifference = newBonus?.HealthBonus ?? 0;
                manaDifference = newBonus?.ManaBonus ?? 0;
            }

            // Update equipment bonus
            equipmentBonus = newBonus;

            // Adjust current health and mana to maintain proportional values
            if (healthDifference != 0)
            {
                currentHealth = Mathf.Min(MaxHealth, currentHealth + healthDifference);
            }

            if (manaDifference != 0)
            {
                currentMana = Mathf.Min(Mana, currentMana + manaDifference);
            }
        }
    }
}