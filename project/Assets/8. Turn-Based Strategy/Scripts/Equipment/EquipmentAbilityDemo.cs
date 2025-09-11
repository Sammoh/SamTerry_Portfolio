using UnityEngine;
using Sammoh.TurnBasedStrategy;

namespace Sammoh.TurnBasedStrategy
{
    /// <summary>
    /// Demonstration script showing how equipment abilities work in practice
    /// </summary>
    [CreateAssetMenu(fileName = "EquipmentAbilityDemo", menuName = "Turn-Based Strategy/Equipment Ability Demo", order = 100)]
    public class EquipmentAbilityDemo : ScriptableObject
    {
        [Header("Demo Configuration")]
        [SerializeField] private Equipment demoWeapon;
        [SerializeField] private Equipment demoArmor;
        [SerializeField] private Equipment demoAccessory;
        
        [Header("Results (Read Only)")]
        [SerializeField, TextArea(5, 10)] private string demonstrationOutput;
        
        /// <summary>
        /// Demonstrates how equipment abilities work
        /// </summary>
        [ContextMenu("Run Equipment Abilities Demo")]
        public void RunDemo()
        {
            var output = "=== Equipment Abilities Demonstration ===\n\n";
            
            // Create a test character and equipment manager
            var equipmentManager = new EquipmentManager();
            var stats = new CharacterStats(100, 15, 10, 12, 50);
            
            // Create demo equipment with abilities if not already set
            if (demoWeapon == null)
            {
                CreateDemoEquipment();
            }
            
            output += "1. Equipment with Abilities:\n";
            output += ShowEquipmentAbilities(demoWeapon, "Weapon");
            output += ShowEquipmentAbilities(demoArmor, "Armor");
            output += ShowEquipmentAbilities(demoAccessory, "Accessory");
            
            output += "\n2. Equipping Items:\n";
            
            equipmentManager.EquipItem(demoWeapon);
            output += $"✓ Equipped {demoWeapon?.EquipmentName ?? "Demo Weapon"}\n";
            
            equipmentManager.EquipItem(demoArmor);
            output += $"✓ Equipped {demoArmor?.EquipmentName ?? "Demo Armor"}\n";
            
            equipmentManager.EquipItem(demoAccessory);
            output += $"✓ Equipped {demoAccessory?.EquipmentName ?? "Demo Accessory"}\n";
            
            output += "\n3. Getting Equipment Abilities:\n";
            var equipmentAbilities = equipmentManager.GetEquipmentAbilities();
            output += $"Total equipment abilities found: {equipmentAbilities.Length}\n";
            
            foreach (var ability in equipmentAbilities)
            {
                output += $"  - {ability.AbilityName} ({ability.AbilityType}): {ability.Description}\n";
                output += $"    Power: {ability.Power}, Mana Cost: {ability.ManaCost}\n";
            }
            
            output += "\n4. Getting Abilities by Slot:\n";
            var weaponAbilities = equipmentManager.GetAbilitiesFromSlot(EquipmentSlot.Weapon);
            var armorAbilities = equipmentManager.GetAbilitiesFromSlot(EquipmentSlot.Armor);
            var accessoryAbilities = equipmentManager.GetAbilitiesFromSlot(EquipmentSlot.Accessory);
            
            output += $"Weapon abilities: {weaponAbilities.Length}\n";
            output += $"Armor abilities: {armorAbilities.Length}\n";
            output += $"Accessory abilities: {accessoryAbilities.Length}\n";
            
            output += "\n=== Demo Complete ===\n";
            output += "Equipment can now provide abilities to characters!\n";
            output += "Use GetEquipmentAbilities() to get all abilities from equipped items.\n";
            output += "Use GetAbilitiesFromSlot() to get abilities from specific equipment slots.";
            
            demonstrationOutput = output;
            
            Debug.Log(output);
        }
        
        private string ShowEquipmentAbilities(Equipment equipment, string slotName)
        {
            if (equipment == null)
                return $"  {slotName}: No equipment set\n";
                
            var abilities = equipment.Abilities;
            var output = $"  {slotName} ({equipment.EquipmentName}): {abilities.Length} abilities\n";
            
            foreach (var ability in abilities)
            {
                output += $"    - {ability.AbilityName}: {ability.Description}\n";
            }
            
            return output;
        }
        
        private void CreateDemoEquipment()
        {
            // This would typically be done through the Unity Editor or ScriptableObject creation
            // This is just for demonstration purposes
            Debug.LogWarning("Demo equipment not configured. Please assign equipment assets in the inspector.");
        }
        
        private void OnValidate()
        {
            if (Application.isPlaying && !string.IsNullOrEmpty(demonstrationOutput))
            {
                // Keep the output visible in the inspector
            }
        }
    }
}