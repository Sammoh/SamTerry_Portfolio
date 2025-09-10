using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    /// <summary>
    /// ScriptableObject representing an accessory equipment type.
    /// </summary>
    [CreateAssetMenu(fileName = "New_Accessory", menuName = "Equipment/Accessory", order = 2)]
    public class AccessorySO : EquipmentSO
    {
        [Header("Accessory Properties")]
        [SerializeField] private AccessoryType accessoryType = AccessoryType.Ring;
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
        
        public override void GenerateDefaultValues()
        {
            base.GenerateDefaultValues();
            
            // Ensure accessory goes in accessory slot
            slot = EquipmentSlot.Accessory;
            
            // Accessory type specific adjustments
            switch (accessoryType)
            {
                case AccessoryType.Ring:
                    // Rings are typically stackable
                    isStackable = true;
                    maxStackSize = 2;
                    break;
                case AccessoryType.Necklace:
                    // Necklaces are usually unique
                    isStackable = false;
                    maxStackSize = 1;
                    break;
                case AccessoryType.Amulet:
                    // Amulets provide powerful effects
                    isStackable = false;
                    maxStackSize = 1;
                    break;
                case AccessoryType.Bracelet:
                    // Bracelets can be worn on both arms
                    isStackable = true;
                    maxStackSize = 2;
                    break;
                case AccessoryType.Charm:
                    // Charms are small and stackable
                    isStackable = true;
                    maxStackSize = 3;
                    break;
            }
        }
        
        private void OnValidate()
        {
            maxStackSize = Mathf.Max(1, maxStackSize);
        }
    }
    
    /// <summary>
    /// Defines the different types of accessories.
    /// </summary>
    public enum AccessoryType
    {
        Ring,
        Necklace,
        Amulet,
        Bracelet,
        Charm
    }
}