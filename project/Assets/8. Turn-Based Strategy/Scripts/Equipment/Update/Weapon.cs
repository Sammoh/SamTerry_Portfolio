using UnityEngine;

namespace Sammoh.Two
{
    /// <summary>
    /// ScriptableObject representing a weapon equipment type.
    /// </summary>
    [CreateAssetMenu(fileName = "New_Weapon", menuName = "Equipment/Weapon", order = 1)]
    public class Weapon : Equipment
    {
        [Header("Weapon Properties")]
        [SerializeField] private WeaponType weaponType = WeaponType.Sword;
        [SerializeField] private int damage = 10;
        [SerializeField] private float attackSpeed = 1.0f;
        [SerializeField] private float range = 1.0f;
        [SerializeField] private int criticalChance = 5; // Percentage
        [SerializeField] private float criticalMultiplier = 1.5f;
        
        public override EquipmentType Type => EquipmentType.Weapon;
        
        /// <summary>
        /// Gets or sets the weapon type (sword, bow, staff, etc.).
        /// </summary>
        public WeaponType WeaponType 
        { 
            get => weaponType; 
            set => weaponType = value; 
        }
        
        /// <summary>
        /// Gets or sets the base damage of the weapon.
        /// </summary>
        public int Damage 
        { 
            get => damage; 
            set => damage = Mathf.Max(1, value); 
        }
        
        /// <summary>
        /// Gets or sets the attack speed (attacks per second).
        /// </summary>
        public float AttackSpeed 
        { 
            get => attackSpeed; 
            set => attackSpeed = Mathf.Max(0.1f, value); 
        }
        
        /// <summary>
        /// Gets or sets the attack range of the weapon.
        /// </summary>
        public float Range 
        { 
            get => range; 
            set => range = Mathf.Max(0.1f, value); 
        }
        
        /// <summary>
        /// Gets or sets the critical hit chance percentage (0-100).
        /// </summary>
        public int CriticalChance 
        { 
            get => criticalChance; 
            set => criticalChance = Mathf.Clamp(value, 0, 100); 
        }
        
        /// <summary>
        /// Gets or sets the critical hit damage multiplier.
        /// </summary>
        public float CriticalMultiplier 
        { 
            get => criticalMultiplier; 
            set => criticalMultiplier = Mathf.Max(1f, value); 
        }
        
        /// <summary>
        /// Calculates the DPS (Damage Per Second) of this weapon.
        /// </summary>
        public float CalculateDPS()
        {
            float baseDPS = damage * attackSpeed;
            float criticalBonus = (criticalChance / 100f) * (criticalMultiplier - 1f);
            return baseDPS * (1f + criticalBonus);
        }
        
        public override void GenerateDefaultValues()
        {
            base.GenerateDefaultValues();
            
            // Adjust weapon stats based on rarity and type
            float rarityMultiplier = GetRarityMultiplier(rarity);
            damage = Mathf.RoundToInt(damage * rarityMultiplier);
            
            // Weapon type specific adjustments
            switch (weaponType)
            {
                case WeaponType.Dagger:
                    attackSpeed *= 1.5f;
                    damage = Mathf.RoundToInt(damage * 0.8f);
                    criticalChance += 5;
                    break;
                case WeaponType.Sword:
                    // Balanced stats - no changes
                    break;
                case WeaponType.Axe:
                    damage = Mathf.RoundToInt(damage * 1.3f);
                    attackSpeed *= 0.8f;
                    break;
                case WeaponType.Bow:
                    range *= 3f;
                    criticalChance += 3;
                    break;
                case WeaponType.Staff:
                    range *= 2f;
                    damage = Mathf.RoundToInt(damage * 1.1f);
                    break;
                case WeaponType.Mace:
                    damage = Mathf.RoundToInt(damage * 1.2f);
                    attackSpeed *= 0.9f;
                    break;
            }
        }
        
        private void OnValidate()
        {
            damage = Mathf.Max(1, damage);
            attackSpeed = Mathf.Max(0.1f, attackSpeed);
            range = Mathf.Max(0.1f, range);
            criticalChance = Mathf.Clamp(criticalChance, 0, 100);
            criticalMultiplier = Mathf.Max(1f, criticalMultiplier);
        }
    }
    
    /// <summary>
    /// Defines the different types of weapons.
    /// </summary>
    public enum WeaponType
    {
        Sword,
        Axe,
        Mace,
        Dagger,
        Bow,
        Staff
    }
}