using System;
using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    public enum AbilityType
    {
        Attack,
        Heal,
        Defend,
        Special
    }

    [Serializable]
    public class CharacterAbility
    {
        [SerializeField] private string abilityName;
        [SerializeField] private AbilityType abilityType;
        [SerializeField] private int power;
        [SerializeField] private int manaCost;
        [SerializeField] private string description;

        public string AbilityName => abilityName;
        public AbilityType AbilityType => abilityType;
        public int Power => power;
        public int ManaCost => manaCost;
        public string Description => description;

        public CharacterAbility(string name, AbilityType type, int power, int manaCost, string description)
        {
            this.abilityName = name;
            this.abilityType = type;
            this.power = power;
            this.manaCost = manaCost;
            this.description = description;
        }

        public bool CanUse(CharacterStats stats)
        {
            return stats.CurrentMana >= manaCost && stats.IsAlive;
        }

        public int Execute(CharacterStats casterStats, CharacterStats targetStats = null)
        {
            if (!casterStats.UseMana(manaCost))
                return 0;

            switch (abilityType)
            {
                case AbilityType.Attack:
                    if (targetStats != null)
                    {
                        int damage = power + casterStats.Attack;
                        targetStats.TakeDamage(damage);
                        return damage;
                    }
                    break;

                case AbilityType.Heal:
                    if (targetStats != null)
                    {
                        targetStats.Heal(power);
                        return power;
                    }
                    else
                    {
                        // If no target specified, heal self
                        casterStats.Heal(power);
                        return power;
                    }

                case AbilityType.Defend:
                    // Defensive abilities could reduce incoming damage for next turn
                    // For simplicity, we'll just return the defense boost value
                    return power;

                case AbilityType.Special:
                    // Special abilities have custom logic
                    if (targetStats != null)
                    {
                        int damage = power + casterStats.Attack;
                        targetStats.TakeDamage(damage);
                        return damage;
                    }
                    break;
            }

            return 0;
        }
    }
}