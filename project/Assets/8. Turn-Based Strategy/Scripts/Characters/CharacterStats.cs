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

        public int MaxHealth => maxHealth;
        public int CurrentHealth => currentHealth;
        public int Attack => attack;
        public int Defense => defense;
        public int Speed => speed;
        public int Mana => mana;
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

        public void TakeDamage(int damage)
        {
            int finalDamage = Mathf.Max(1, damage - defense);
            currentHealth = Mathf.Max(0, currentHealth - finalDamage);
        }

        public void Heal(int amount)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
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
            currentMana = Mathf.Min(mana, currentMana + amount);
        }

        public void FullRestore()
        {
            currentHealth = maxHealth;
            currentMana = mana;
        }
    }
}