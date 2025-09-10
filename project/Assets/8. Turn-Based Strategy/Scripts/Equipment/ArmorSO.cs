using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    /// <summary>
    /// ScriptableObject representing an armor equipment type.
    /// </summary>
    [CreateAssetMenu(fileName = "New_Armor", menuName = "Equipment/Armor", order = 3)]
    public class ArmorSO : EquipmentSO
    {
        [Header("Armor Properties")]
        [SerializeField] private ArmorType armorType = ArmorType.Chestplate;
        [SerializeField] private int defense = 5;
        [SerializeField] private int magicResistance = 2;
        [SerializeField] private float movementSpeedModifier = 0f; // Negative values slow down
        [SerializeField] private DamageResistance[] damageResistances = new DamageResistance[0];
        [SerializeField] private ArmorMaterial armorMaterial = ArmorMaterial.Leather;
        
        public override EquipmentType Type => EquipmentType.Armor;
        
        /// <summary>
        /// Gets or sets the armor type (helmet, chestplate, etc.).
        /// </summary>
        public ArmorType ArmorType 
        { 
            get => armorType; 
            set => armorType = value; 
        }
        
        /// <summary>
        /// Gets or sets the physical defense value.
        /// </summary>
        public int Defense 
        { 
            get => defense; 
            set => defense = Mathf.Max(0, value); 
        }
        
        /// <summary>
        /// Gets or sets the magic resistance value.
        /// </summary>
        public int MagicResistance 
        { 
            get => magicResistance; 
            set => magicResistance = Mathf.Max(0, value); 
        }
        
        /// <summary>
        /// Gets or sets the movement speed modifier (can be negative).
        /// </summary>
        public float MovementSpeedModifier 
        { 
            get => movementSpeedModifier; 
            set => movementSpeedModifier = value; 
        }
        
        /// <summary>
        /// Gets or sets the damage resistances.
        /// </summary>
        public DamageResistance[] DamageResistances 
        { 
            get => damageResistances; 
            set => damageResistances = value ?? new DamageResistance[0]; 
        }
        
        /// <summary>
        /// Gets or sets the armor material.
        /// </summary>
        public ArmorMaterial ArmorMaterial 
        { 
            get => armorMaterial; 
            set => armorMaterial = value; 
        }
        
        public override void GenerateDefaultValues()
        {
            base.GenerateDefaultValues();
            
            // Ensure armor goes in armor slot
            slot = EquipmentSlot.Armor;
            
            // Adjust armor stats based on rarity and material
            float rarityMultiplier = GetRarityMultiplier(rarity);
            defense = Mathf.RoundToInt(defense * rarityMultiplier);
            magicResistance = Mathf.RoundToInt(magicResistance * rarityMultiplier);
            
            // Material-based adjustments
            switch (armorMaterial)
            {
                case ArmorMaterial.Leather:
                    // Light armor - good mobility
                    movementSpeedModifier = 0.1f;
                    break;
                case ArmorMaterial.Chainmail:
                    // Medium armor - balanced
                    defense = Mathf.RoundToInt(defense * 1.2f);
                    movementSpeedModifier = -0.05f;
                    break;
                case ArmorMaterial.Plate:
                    // Heavy armor - high defense
                    defense = Mathf.RoundToInt(defense * 1.5f);
                    movementSpeedModifier = -0.15f;
                    break;
                case ArmorMaterial.Magical:
                    // Magical armor - high magic resistance
                    magicResistance = Mathf.RoundToInt(magicResistance * 2f);
                    break;
                case ArmorMaterial.Dragon:
                    // Dragon scale - best of both worlds
                    defense = Mathf.RoundToInt(defense * 1.4f);
                    magicResistance = Mathf.RoundToInt(magicResistance * 1.8f);
                    break;
            }
        }
        
        private void OnValidate()
        {
            defense = Mathf.Max(0, defense);
            magicResistance = Mathf.Max(0, magicResistance);
            if (damageResistances == null) damageResistances = new DamageResistance[0];
        }
    }
    
    /// <summary>
    /// Defines the different types of armor.
    /// </summary>
    public enum ArmorType
    {
        Helmet,
        Chestplate,
        Leggings,
        Boots,
        Gloves,
        Shield
    }
    
    /// <summary>
    /// Defines the different materials armor can be made from.
    /// </summary>
    public enum ArmorMaterial
    {
        Leather,
        Chainmail,
        Plate,
        Magical,
        Dragon
    }
    
    /// <summary>
    /// Defines resistance to specific damage types.
    /// </summary>
    [System.Serializable]
    public class DamageResistance
    {
        public DamageType damageType;
        [Range(0f, 1f)]
        public float resistancePercentage; // 0 = no resistance, 1 = immune
    }
    
    /// <summary>
    /// Types of damage that can be resisted.
    /// </summary>
    public enum DamageType
    {
        Physical,
        Fire,
        Ice,
        Lightning,
        Poison,
        Acid
    }
}