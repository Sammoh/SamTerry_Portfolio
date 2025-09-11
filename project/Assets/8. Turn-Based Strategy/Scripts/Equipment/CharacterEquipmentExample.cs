using UnityEngine;
using System.Linq;
using Sammoh.TurnBasedStrategy;

namespace Sammoh.TurnBasedStrategy
{
    /// <summary>
    /// Example usage of Character with equipment abilities
    /// </summary>
    public class CharacterEquipmentExample : MonoBehaviour
    {
        [Header("Character Setup")]
        [SerializeField] private Character character;
        
        [Header("Example Equipment")]
        [SerializeField] private Equipment exampleWeapon;
        [SerializeField] private Equipment exampleArmor;
        [SerializeField] private Equipment exampleAccessory;
        
        [Header("Demo Results")]
        [SerializeField, TextArea(5, 10)] private string lastDemoResults;
        
        /// <summary>
        /// Demonstrates character using both their own abilities and equipment abilities
        /// </summary>
        [ContextMenu("Demo Character Equipment Abilities")]
        public void DemoCharacterEquipmentAbilities()
        {
            if (character == null)
            {
                Debug.LogError("No character assigned for demo!");
                return;
            }
            
            var output = "=== Character Equipment Abilities Demo ===\n\n";
            
            // Show initial character abilities
            var initialAbilities = character.Abilities;
            output += $"Character '{character.CharacterName}' initial abilities: {initialAbilities.Length}\n";
            
            foreach (var ability in initialAbilities)
            {
                output += $"  - {ability.AbilityName} ({ability.AbilityType})\n";
            }
            
            output += "\nEquipping items...\n";
            
            // Equip items if available
            if (exampleWeapon != null)
            {
                character.EquipItem(exampleWeapon);
                output += $"✓ Equipped {exampleWeapon.EquipmentName}\n";
                
                var weaponAbilities = exampleWeapon.Abilities;
                if (weaponAbilities.Length > 0)
                {
                    output += $"  Weapon abilities: {weaponAbilities.Length}\n";
                    foreach (var ability in weaponAbilities)
                    {
                        output += $"    - {ability.AbilityName} ({ability.AbilityType})\n";
                    }
                }
            }
            
            if (exampleArmor != null)
            {
                character.EquipItem(exampleArmor);
                output += $"✓ Equipped {exampleArmor.EquipmentName}\n";
                
                var armorAbilities = exampleArmor.Abilities;
                if (armorAbilities.Length > 0)
                {
                    output += $"  Armor abilities: {armorAbilities.Length}\n";
                    foreach (var ability in armorAbilities)
                    {
                        output += $"    - {ability.AbilityName} ({ability.AbilityType})\n";
                    }
                }
            }
            
            if (exampleAccessory != null)
            {
                character.EquipItem(exampleAccessory);
                output += $"✓ Equipped {exampleAccessory.EquipmentName}\n";
                
                var accessoryAbilities = exampleAccessory.Abilities;
                if (accessoryAbilities.Length > 0)
                {
                    output += $"  Accessory abilities: {accessoryAbilities.Length}\n";
                    foreach (var ability in accessoryAbilities)
                    {
                        output += $"    - {ability.AbilityName} ({ability.AbilityType})\n";
                    }
                }
            }
            
            output += "\n=== All Available Abilities ===\n";
            
            // Get all abilities (character + equipment)
            var allAbilities = character.GetAllAbilities();
            output += $"Total abilities available: {allAbilities.Length}\n";
            
            foreach (var ability in allAbilities)
            {
                var canUse = ability.CanUse(character.Stats) ? "✓" : "✗";
                output += $"  {canUse} {ability.AbilityName} ({ability.AbilityType}) - Power: {ability.Power}, Mana: {ability.ManaCost}\n";
            }
            
            output += "\n=== Equipment-Only Abilities ===\n";
            
            // Get only equipment abilities
            var equipmentAbilities = character.GetEquipmentAbilities();
            output += $"Equipment abilities: {equipmentAbilities.Length}\n";
            
            foreach (var ability in equipmentAbilities)
            {
                var canUse = ability.CanUse(character.Stats) ? "✓" : "✗";
                output += $"  {canUse} {ability.AbilityName} ({ability.AbilityType}) - {ability.Description}\n";
            }
            
            output += "\n=== Ability Usage Example ===\n";
            
            // Try to use an equipment ability if available
            if (equipmentAbilities.Length > 0)
            {
                var firstEquipmentAbility = equipmentAbilities[0];
                if (firstEquipmentAbility.CanUse(character.Stats))
                {
                    output += $"Using equipment ability: {firstEquipmentAbility.AbilityName}\n";
                    
                    // In a real game, you'd call ability.Execute(character.Stats, targetStats)
                    output += $"  - Would consume {firstEquipmentAbility.ManaCost} mana\n";
                    output += $"  - Would deal/heal {firstEquipmentAbility.Power} points\n";
                }
                else
                {
                    output += $"Cannot use {firstEquipmentAbility.AbilityName} - insufficient mana\n";
                }
            }
            
            output += "\n=== Summary ===\n";
            output += $"Character has {initialAbilities.Length} base abilities\n";
            output += $"Equipment provides {equipmentAbilities.Length} additional abilities\n";
            output += $"Total abilities available: {allAbilities.Length}\n";
            output += "\nEquipment abilities integrate seamlessly with character abilities!";
            
            lastDemoResults = output;
            Debug.Log(output);
        }
        
        private void Start()
        {
            if (character == null)
            {
                character = GetComponent<Character>();
            }
        }
        
        /// <summary>
        /// Example of how to use abilities in combat or gameplay
        /// </summary>
        public void UseAbilityExample(string abilityName, Character target = null)
        {
            var allAbilities = character.GetAllAbilities();
            var ability = allAbilities.FirstOrDefault(a => a.AbilityName == abilityName);
            
            if (ability == null)
            {
                Debug.LogWarning($"Ability '{abilityName}' not found!");
                return;
            }
            
            if (!ability.CanUse(character.Stats))
            {
                Debug.LogWarning($"Cannot use ability '{abilityName}' - insufficient mana or character is dead!");
                return;
            }
            
            Debug.Log($"Using ability: {ability.AbilityName}");
            
            // Execute the ability
            var result = ability.Execute(character.Stats, target?.Stats);
            Debug.Log($"Ability result: {result}");
        }
    }
}