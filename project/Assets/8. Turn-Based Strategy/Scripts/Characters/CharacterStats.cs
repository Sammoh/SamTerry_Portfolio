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

        private EquipmentManager equipmentManager;

        // Base stats (without equipment)
        public int BaseMaxHealth => baseMaxHealth;
        public int BaseAttack => baseAttack;
        public int BaseDefense => baseDefense;
        public int BaseSpeed => baseSpeed;
        public int BaseMana => baseMana;

        // Effective stats (with equipment bonuses)
        public int MaxHealth => equipmentManager?.CalculateStatWithEquipment(baseMaxHealth, StatType.MaxHealth) ?? baseMaxHealth;
        public int Attack => equipmentManager?.CalculateStatWithEquipment(baseAttack, StatType.Attack) ?? baseAttack;
        public int Defense => equipmentManager?.CalculateStatWithEquipment(baseDefense, StatType.Defense) ?? baseDefense;
        public int Speed => equipmentManager?.CalculateStatWithEquipment(baseSpeed, StatType.Speed) ?? baseSpeed;
        public int Mana => equipmentManager?.CalculateStatWithEquipment(baseMana, StatType.Mana) ?? baseMana;
        
        public int CurrentHealth => currentHealth;
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
        }

        public void SetEquipmentManager(EquipmentManager manager)
        {
            equipmentManager = manager;
            // Adjust current health if max health changed due to equipment
            if (currentHealth > MaxHealth)
                currentHealth = MaxHealth;
            // Adjust current mana if max mana changed due to equipment
            if (currentMana > Mana)
                currentMana = Mana;
        }

        public void TakeDamage(int damage)
        {
            int finalDamage = Mathf.Max(1, damage - defense);
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
    }
}