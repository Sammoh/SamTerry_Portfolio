using UnityEngine;

namespace Sammoh.Two
{
    /// <summary>
    /// Base class for all equipment types in the game.
    /// Uses file name as ID for fast lookup tables.
    /// </summary>
    public abstract class Equipment : ScriptableObject
    {
        [Header("Basic Information")]
        [SerializeField] protected string equipmentName;
        [SerializeField] protected string description;
        [SerializeField] protected Sprite icon;
        
        [Header("Properties")]
        [SerializeField] protected int level = 1;
        [SerializeField] protected float weight = 1.0f;
        [SerializeField] protected int value = 100;
        [SerializeField] protected EquipmentRarity rarity = EquipmentRarity.Common;
        
        /// <summary>
        /// Gets the equipment ID derived from the asset file name.
        /// This enables fast lookup tables based on file names.
        /// </summary>
        public string Id => name;
        
        /// <summary>
        /// Gets or sets the display name of the equipment.
        /// </summary>
        public string EquipmentName 
        { 
            get => string.IsNullOrEmpty(equipmentName) ? name : equipmentName;
            set => equipmentName = value;
        }
        
        /// <summary>
        /// Gets or sets the description of the equipment.
        /// </summary>
        public string Description 
        { 
            get => description; 
            set => description = value; 
        }
        
        /// <summary>
        /// Gets or sets the icon sprite for the equipment.
        /// </summary>
        public Sprite Icon 
        { 
            get => icon; 
            set => icon = value; 
        }
        
        /// <summary>
        /// Gets or sets the level requirement for this equipment.
        /// </summary>
        public int Level 
        { 
            get => level; 
            set => level = Mathf.Max(1, value); 
        }
        
        /// <summary>
        /// Gets or sets the weight of the equipment.
        /// </summary>
        public float Weight 
        { 
            get => weight; 
            set => weight = Mathf.Max(0f, value); 
        }
        
        /// <summary>
        /// Gets or sets the base value/cost of the equipment.
        /// </summary>
        public int Value 
        { 
            get => value; 
            set => this.value = Mathf.Max(0, value); 
        }
        
        /// <summary>
        /// Gets or sets the rarity of the equipment.
        /// </summary>
        public EquipmentRarity Rarity 
        { 
            get => rarity; 
            set => rarity = value; 
        }
        
        /// <summary>
        /// Gets the equipment type for categorization.
        /// </summary>
        public abstract EquipmentType Type { get; }
        
        /// <summary>
        /// Generates default values for this equipment based on its type and rarity.
        /// Override in derived classes for type-specific generation.
        /// </summary>
        public virtual void GenerateDefaultValues()
        {
            if (string.IsNullOrEmpty(equipmentName))
                equipmentName = name.Replace("_", " ");
            
            // Adjust values based on rarity
            float rarityMultiplier = GetRarityMultiplier();
            value = Mathf.RoundToInt(value * rarityMultiplier);
            weight = weight * (1f + (float)rarity * 0.1f);
        }
        
        protected float GetRarityMultiplier()
        {
            return rarity switch
            {
                EquipmentRarity.Common => 1f,
                EquipmentRarity.Uncommon => 1.5f,
                EquipmentRarity.Rare => 2.5f,
                EquipmentRarity.Epic => 4f,
                EquipmentRarity.Legendary => 6f,
                _ => 1f
            };
        }
        
        private void OnValidate()
        {
            level = Mathf.Max(1, level);
            weight = Mathf.Max(0f, weight);
            value = Mathf.Max(0, value);
        }
    }
    
    /// <summary>
    /// Defines the different types of equipment.
    /// </summary>
    public enum EquipmentType
    {
        Weapon,
        Accessory,
        Armor
    }
    
    /// <summary>
    /// Defines the rarity levels for equipment.
    /// </summary>
    public enum EquipmentRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
}