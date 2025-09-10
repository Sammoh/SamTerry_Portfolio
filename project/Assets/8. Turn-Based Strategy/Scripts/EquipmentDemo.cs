using UnityEngine;
using Sammoh.TurnBasedStrategy;

namespace Sammoh.TurnBasedStrategy
{
    /// <summary>
    /// Demonstration script showing how to use the equipment system
    /// </summary>
    public class EquipmentDemo : MonoBehaviour
    {
        [Header("Demo Configuration")]
        [SerializeField] private CharacterFactory characterFactory;
        
        private Character demoCharacter;

        void Start()
        {
            if (characterFactory == null)
            {
                Debug.LogError("Character Factory not assigned!");
                return;
            }

            DemonstrateEquipmentSystem();
        }

        void DemonstrateEquipmentSystem()
        {
            Debug.Log("=== Equipment System Demonstration ===");

            // Create a warrior character with default equipment
            demoCharacter = characterFactory.CreateCharacter(CharacterClass.Warrior, "Demo Warrior", true);
            
            Debug.Log($"Created {demoCharacter.CharacterName}:");
            LogCharacterStats("Initial Stats (with default equipment)");

            // Create some custom equipment
            Equipment magicSword = new Equipment(
                "Magic Sword of Power",
                "A legendary blade infused with magical energy",
                EquipmentSlot.Weapon,
                EquipmentRarity.Legendary,
                new EquipmentStats(10, 30, 5, -1, 5) // +10 health, +30 attack, +5 defense, -1 speed, +5 mana
            );

            Equipment dragonScaleArmor = new Equipment(
                "Dragon Scale Armor",
                "Armor crafted from the scales of an ancient dragon",
                EquipmentSlot.Armor,
                EquipmentRarity.Epic,
                new EquipmentStats(50, 0, 25, -3, 0) // +50 health, +25 defense, -3 speed
            );

            Equipment speedRing = new Equipment(
                "Ring of Swiftness",
                "A magical ring that enhances the wearer's agility",
                EquipmentSlot.Accessory,
                EquipmentRarity.Rare,
                new EquipmentStats(0, 5, 0, 10, 15) // +5 attack, +10 speed, +15 mana
            );

            // Replace default weapon with magic sword
            Debug.Log("\n--- Upgrading Equipment ---");
            Equipment oldWeapon = demoCharacter.GetEquippedItem(EquipmentSlot.Weapon);
            Debug.Log($"Replacing {oldWeapon?.EquipmentName} with {magicSword.EquipmentName}");
            
            bool equipped = demoCharacter.EquipItem(magicSword);
            if (equipped)
            {
                LogCharacterStats("After equipping Magic Sword");
            }

            // Add dragon scale armor
            Debug.Log($"\nEquipping {dragonScaleArmor.EquipmentName}");
            demoCharacter.EquipItem(dragonScaleArmor);
            LogCharacterStats("After equipping Dragon Scale Armor");

            // Add speed ring
            Debug.Log($"\nEquipping {speedRing.EquipmentName}");
            demoCharacter.EquipItem(speedRing);
            LogCharacterStats("After equipping Ring of Swiftness");

            // Show all equipped items
            Debug.Log("\n--- Final Equipment Loadout ---");
            var equippedItems = demoCharacter.Equipment.GetAllEquippedItems();
            foreach (var item in equippedItems)
            {
                Debug.Log($"• {item.EquipmentName} ({item.Rarity} {item.Slot}): {item.Description}");
            }

            Debug.Log("\n=== Equipment Demo Complete ===");
        }

        void LogCharacterStats(string label)
        {
            if (demoCharacter == null) return;

            var stats = demoCharacter.Stats;
            Debug.Log($"{label}:");
            Debug.Log($"  Health: {stats.CurrentHealth}/{stats.MaxHealth}");
            Debug.Log($"  Attack: {stats.Attack}");
            Debug.Log($"  Defense: {stats.Defense}");
            Debug.Log($"  Speed: {stats.Speed}");
            Debug.Log($"  Mana: {stats.CurrentMana}/{stats.Mana}");
        }

        void OnDestroy()
        {
            // Clean up the demo character
            if (demoCharacter != null && demoCharacter.gameObject != null)
            {
                DestroyImmediate(demoCharacter.gameObject);
            }
        }
    }
}