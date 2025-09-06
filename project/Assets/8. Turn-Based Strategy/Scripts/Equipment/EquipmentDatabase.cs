using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    [CreateAssetMenu(fileName = "New Equipment Database", menuName = "Turn-Based Strategy/Equipment Database")]
    public class EquipmentDatabase : ScriptableObject
    {
        [SerializeField] private Equipment[] allEquipment;

        public Equipment[] AllEquipment => allEquipment;

        public Equipment GetEquipmentByName(string name)
        {
            foreach (var equipment in allEquipment)
            {
                if (equipment.EquipmentName == name)
                    return equipment;
            }
            return null;
        }

        public Equipment[] GetEquipmentBySlot(EquipmentSlot slot)
        {
            return System.Array.FindAll(allEquipment, e => e.Slot == slot);
        }

        /// <summary>
        /// Creates default equipment for testing and demonstration
        /// </summary>
        public static Equipment[] CreateDefaultEquipment()
        {
            return new Equipment[]
            {
                // Weapons
                new Equipment("Iron Sword", EquipmentSlot.Weapon, 
                    new StatModifier[] { 
                        new StatModifier(StatType.Attack, 5, ModifierType.Additive) 
                    }, 
                    "A sturdy iron sword that increases attack power."),

                new Equipment("Steel Blade", EquipmentSlot.Weapon, 
                    new StatModifier[] { 
                        new StatModifier(StatType.Attack, 8, ModifierType.Additive),
                        new StatModifier(StatType.Speed, 2, ModifierType.Additive)
                    }, 
                    "A sharp steel blade that boosts attack and speed."),

                new Equipment("Flame Sword", EquipmentSlot.Weapon, 
                    new StatModifier[] { 
                        new StatModifier(StatType.Attack, 12, ModifierType.Additive),
                        new StatModifier(StatType.Mana, -5, ModifierType.Additive)
                    }, 
                    "A magical sword wreathed in flames. High attack but drains mana."),

                // Armor
                new Equipment("Leather Armor", EquipmentSlot.Armor, 
                    new StatModifier[] { 
                        new StatModifier(StatType.Defense, 3, ModifierType.Additive),
                        new StatModifier(StatType.MaxHealth, 10, ModifierType.Additive)
                    }, 
                    "Basic leather protection that provides moderate defense and health."),

                new Equipment("Chain Mail", EquipmentSlot.Armor, 
                    new StatModifier[] { 
                        new StatModifier(StatType.Defense, 6, ModifierType.Additive),
                        new StatModifier(StatType.MaxHealth, 15, ModifierType.Additive),
                        new StatModifier(StatType.Speed, -2, ModifierType.Additive)
                    }, 
                    "Heavy chain mail with excellent defense but reduces speed."),

                new Equipment("Mage Robes", EquipmentSlot.Armor, 
                    new StatModifier[] { 
                        new StatModifier(StatType.Mana, 20, ModifierType.Additive),
                        new StatModifier(StatType.Defense, -2, ModifierType.Additive)
                    }, 
                    "Magical robes that boost mana but offer little physical protection."),

                // Accessories
                new Equipment("Power Ring", EquipmentSlot.Accessory, 
                    new StatModifier[] { 
                        new StatModifier(StatType.Attack, 20, ModifierType.Multiplicative)
                    }, 
                    "A mystical ring that increases attack power by 20%."),

                new Equipment("Health Amulet", EquipmentSlot.Accessory, 
                    new StatModifier[] { 
                        new StatModifier(StatType.MaxHealth, 30, ModifierType.Additive)
                    }, 
                    "An enchanted amulet that greatly increases maximum health."),

                new Equipment("Speed Boots", EquipmentSlot.Accessory, 
                    new StatModifier[] { 
                        new StatModifier(StatType.Speed, 5, ModifierType.Additive)
                    }, 
                    "Enchanted boots that make the wearer faster in combat.")
            };
        }
    }
}