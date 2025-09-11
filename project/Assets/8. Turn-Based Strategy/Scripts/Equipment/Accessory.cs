using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    /// <summary>
    /// ScriptableObject representing an accessory equipment type.
    /// </summary>
    [CreateAssetMenu(fileName = "New_Accessory", menuName = "Equipment/Accessory", order = 2)]
    public class Accessory : Equipment
    {
        [Header("Accessory Properties")]
        [SerializeField] private AccessoryType accessoryType = AccessoryType.Ring;
        [SerializeField] private StatModifier[] statModifiers = new StatModifier[0];
        [SerializeField] private bool isStackable = false;
        [SerializeField] private int maxStackSize = 1;
        
        public override EquipmentType Type => EquipmentType.Accessory;
        
        /// <summary>
        /// Gets or sets the accessory type (ring, necklace, etc.).
        /// </summary>
        public AccessoryType AccessoryType 
        { 
            get => accessoryType; 
            set => accessoryType = value; 
        }
        
        /// <summary>
        /// Gets the stat modifiers provided by this accessory.
        /// </summary>
        public StatModifier[] StatModifiers 
        { 
            get => statModifiers; 
            set => statModifiers = value ?? new StatModifier[0]; 
        }
        
        /// <summary>
        /// Gets or sets whether this accessory can be stacked.
        /// </summary>
        public bool IsStackable 
        { 
            get => isStackable; 
            set => isStackable = value; 
        }
        
        /// <summary>
        /// Gets or sets the maximum stack size for this accessory.
        /// </summary>
        public int MaxStackSize 
        { 
            get => maxStackSize; 
            set => maxStackSize = Mathf.Max(1, value); 
        }
        
        /// <summary>
        /// Adds a stat modifier to this accessory.
        /// </summary>
        public void AddStatModifier(StatType statType, float value, ModifierType modifierType)
        {
            var modifiers = new StatModifier[statModifiers.Length + 1];
            System.Array.Copy(statModifiers, modifiers, statModifiers.Length);
            modifiers[statModifiers.Length] = new StatModifier(statType, value, modifierType);
            statModifiers = modifiers;
        }
        
        /// <summary>
        /// Removes all stat modifiers of the specified type.
        /// </summary>
        public void RemoveStatModifiers(StatType statType)
        {
            var filteredModifiers = System.Array.FindAll(statModifiers, 
                modifier => modifier.StatType != statType);
            statModifiers = filteredModifiers;
        }
        
        public override void GenerateDefaultValues()
        {
            base.GenerateDefaultValues();
            
            // Generate random stat modifiers based on rarity and type
            int modifierCount = Mathf.RoundToInt((float)rarity + 1);
            statModifiers = new StatModifier[modifierCount];
            
            for (int i = 0; i < modifierCount; i++)
            {
                StatType randomStat = GetRandomStatForAccessoryType();
                float baseValue = GetBaseStatValue(randomStat);
                float rarityMultiplier = GetRarityMultiplier(rarity);
                float finalValue = baseValue * rarityMultiplier;
                
                ModifierType modifierType = randomStat == StatType.Health || randomStat == StatType.Mana 
                    ? ModifierType.Flat : ModifierType.Percentage;
                
                statModifiers[i] = new StatModifier(randomStat, finalValue, modifierType);
            }
            
            // Set stacking properties based on accessory type
            switch (accessoryType)
            {
                case AccessoryType.Ring:
                    isStackable = true;
                    maxStackSize = 2; // Can wear 2 rings
                    break;
                case AccessoryType.Necklace:
                case AccessoryType.Amulet:
                    isStackable = false;
                    maxStackSize = 1;
                    break;
                case AccessoryType.Bracelet:
                    isStackable = true;
                    maxStackSize = 2; // Can wear 2 bracelets
                    break;
                case AccessoryType.Charm:
                    isStackable = true;
                    maxStackSize = 3;
                    break;
            }
        }
        
        private StatType GetRandomStatForAccessoryType()
        {
            var availableStats = accessoryType switch
            {
                AccessoryType.Ring => new[] { StatType.Damage, StatType.CriticalChance, StatType.AttackSpeed },
                AccessoryType.Necklace => new[] { StatType.Health, StatType.Mana, StatType.Defense },
                AccessoryType.Amulet => new[] { StatType.Mana, StatType.MagicPower, StatType.ManaRegeneration },
                AccessoryType.Bracelet => new[] { StatType.Agility, StatType.AttackSpeed, StatType.CriticalChance },
                AccessoryType.Charm => new[] { StatType.Luck, StatType.Experience, StatType.GoldFind },
                _ => new[] { StatType.Health, StatType.Damage }
            };
            
            return availableStats[Random.Range(0, availableStats.Length)];
        }
        
        private float GetBaseStatValue(StatType statType)
        {
            return statType switch
            {
                StatType.Health => 25f,
                StatType.Mana => 20f,
                StatType.Damage => 5f,
                StatType.Defense => 3f,
                StatType.AttackSpeed => 0.1f,
                StatType.CriticalChance => 2f,
                StatType.Agility => 1f,
                StatType.MagicPower => 8f,
                StatType.ManaRegeneration => 1f,
                StatType.Luck => 1f,
                StatType.Experience => 5f,
                StatType.GoldFind => 10f,
                _ => 1f
            };
        }
        
        private void OnValidate()
        {
            maxStackSize = Mathf.Max(1, maxStackSize);
            
            // Validate stat modifiers
            for (int i = 0; i < statModifiers.Length; i++)
            {
                if (statModifiers[i] != null)
                {
                    statModifiers[i].Validate();
                }
            }
        }
    }
}