using UnityEngine;

namespace Sammoh.Two
{
    /// <summary>
    /// ScriptableObject representing an armor equipment type.
    /// </summary>
    [CreateAssetMenu(fileName = "New_Armor", menuName = "Equipment/Armor", order = 3)]
    public class Armor : Equipment
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
        /// Gets the damage resistances provided by this armor.
        /// </summary>
        public DamageResistance[] DamageResistances => damageResistances;
        
        /// <summary>
        /// Gets or sets the material this armor is made from.
        /// </summary>
        public ArmorMaterial ArmorMaterial 
        { 
            get => armorMaterial; 
            set => armorMaterial = value; 
        }
        
        /// <summary>
        /// Calculates the total defense value including material bonuses.
        /// </summary>
        public int CalculateTotalDefense()
        {
            float materialMultiplier = GetMaterialDefenseMultiplier();
            return Mathf.RoundToInt(defense * materialMultiplier);
        }
        
        /// <summary>
        /// Calculates the total magic resistance including material bonuses.
        /// </summary>
        public int CalculateTotalMagicResistance()
        {
            float materialMultiplier = GetMaterialMagicResistanceMultiplier();
            return Mathf.RoundToInt(magicResistance * materialMultiplier);
        }
        
        /// <summary>
        /// Gets the resistance percentage for a specific damage type.
        /// </summary>
        public float GetDamageResistance(DamageType damageType)
        {
            foreach (var resistance in damageResistances)
            {
                if (resistance.DamageType == damageType)
                {
                    return resistance.ResistancePercentage;
                }
            }
            return 0f;
        }
        
        /// <summary>
        /// Adds or updates a damage resistance.
        /// </summary>
        public void SetDamageResistance(DamageType damageType, float resistancePercentage)
        {
            for (int i = 0; i < damageResistances.Length; i++)
            {
                if (damageResistances[i].DamageType == damageType)
                {
                    damageResistances[i].ResistancePercentage = Mathf.Clamp(resistancePercentage, 0f, 95f);
                    return;
                }
            }
            
            // Add new resistance
            var newResistances = new DamageResistance[damageResistances.Length + 1];
            System.Array.Copy(damageResistances, newResistances, damageResistances.Length);
            newResistances[damageResistances.Length] = new DamageResistance(damageType, resistancePercentage);
            damageResistances = newResistances;
        }
        
        public override void GenerateDefaultValues()
        {
            base.GenerateDefaultValues();
            
            // Adjust armor stats based on rarity, type, and material
            float rarityMultiplier = GetRarityMultiplier();
            float materialMultiplier = GetMaterialDefenseMultiplier();
            
            defense = Mathf.RoundToInt(defense * rarityMultiplier);
            magicResistance = Mathf.RoundToInt(magicResistance * rarityMultiplier);
            
            // Armor type specific adjustments
            switch (armorType)
            {
                case ArmorType.Helmet:
                    defense = Mathf.RoundToInt(defense * 0.7f);
                    magicResistance = Mathf.RoundToInt(magicResistance * 1.2f);
                    break;
                case ArmorType.Chestplate:
                    defense = Mathf.RoundToInt(defense * 1.5f);
                    movementSpeedModifier -= 0.1f;
                    break;
                case ArmorType.Leggings:
                    defense = Mathf.RoundToInt(defense * 1.2f);
                    movementSpeedModifier -= 0.05f;
                    break;
                case ArmorType.Boots:
                    defense = Mathf.RoundToInt(defense * 0.6f);
                    movementSpeedModifier += 0.1f;
                    break;
                case ArmorType.Gloves:
                    defense = Mathf.RoundToInt(defense * 0.5f);
                    break;
                case ArmorType.Shield:
                    defense = Mathf.RoundToInt(defense * 2f);
                    movementSpeedModifier -= 0.15f;
                    break;
            }
            
            // Generate damage resistances based on material
            GenerateMaterialResistances();
        }
        
        private void GenerateMaterialResistances()
        {
            damageResistances = armorMaterial switch
            {
                ArmorMaterial.Leather => new[] 
                { 
                    new DamageResistance(DamageType.Piercing, 5f) 
                },
                ArmorMaterial.Chainmail => new[] 
                { 
                    new DamageResistance(DamageType.Slashing, 10f),
                    new DamageResistance(DamageType.Piercing, 15f)
                },
                ArmorMaterial.Plate => new[] 
                { 
                    new DamageResistance(DamageType.Slashing, 20f),
                    new DamageResistance(DamageType.Bludgeoning, 15f)
                },
                ArmorMaterial.Magical => new[] 
                { 
                    new DamageResistance(DamageType.Magic, 25f),
                    new DamageResistance(DamageType.Elemental, 20f)
                },
                ArmorMaterial.Dragon => new[] 
                { 
                    new DamageResistance(DamageType.Fire, 30f),
                    new DamageResistance(DamageType.Magic, 15f)
                },
                _ => new DamageResistance[0]
            };
        }
        
        private float GetMaterialDefenseMultiplier()
        {
            return armorMaterial switch
            {
                ArmorMaterial.Leather => 0.8f,
                ArmorMaterial.Chainmail => 1.0f,
                ArmorMaterial.Plate => 1.3f,
                ArmorMaterial.Magical => 1.1f,
                ArmorMaterial.Dragon => 1.5f,
                _ => 1.0f
            };
        }
        
        private float GetMaterialMagicResistanceMultiplier()
        {
            return armorMaterial switch
            {
                ArmorMaterial.Leather => 1.0f,
                ArmorMaterial.Chainmail => 0.8f,
                ArmorMaterial.Plate => 0.7f,
                ArmorMaterial.Magical => 1.8f,
                ArmorMaterial.Dragon => 1.4f,
                _ => 1.0f
            };
        }
        
        private void OnValidate()
        {
            defense = Mathf.Max(0, defense);
            magicResistance = Mathf.Max(0, magicResistance);
            
            // Validate damage resistances
            for (int i = 0; i < damageResistances.Length; i++)
            {
                if (damageResistances[i] != null)
                {
                    damageResistances[i].Validate();
                }
            }
        }
    }
    
    /// <summary>
    /// Defines the different types of armor pieces.
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
    /// Defines the materials armor can be made from.
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
    /// Defines the different types of damage.
    /// </summary>
    public enum DamageType
    {
        Slashing,
        Piercing,
        Bludgeoning,
        Fire,
        Ice,
        Lightning,
        Magic,
        Elemental
    }
    
    /// <summary>
    /// Represents resistance to a specific damage type.
    /// </summary>
    [System.Serializable]
    public class DamageResistance
    {
        [SerializeField] private DamageType damageType;
        [SerializeField] private float resistancePercentage;
        
        public DamageResistance(DamageType damageType, float resistancePercentage)
        {
            this.damageType = damageType;
            this.resistancePercentage = Mathf.Clamp(resistancePercentage, 0f, 95f);
        }
        
        public DamageType DamageType => damageType;
        public float ResistancePercentage 
        { 
            get => resistancePercentage;
            set => resistancePercentage = Mathf.Clamp(value, 0f, 95f);
        }
        
        public void Validate()
        {
            resistancePercentage = Mathf.Clamp(resistancePercentage, 0f, 95f);
        }
    }
}