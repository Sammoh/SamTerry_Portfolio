# Equipment Abilities Feature

This document describes the new equipment abilities feature that allows equipment items to provide abilities to characters, similar to how characters have their own abilities.

## Overview

The equipment system has been extended to support abilities, allowing equipment items to grant additional abilities to characters when equipped. This creates more strategic depth and variety in character builds.

## Implementation Details

### Equipment Class Changes

The base `Equipment` class now includes:

```csharp
[SerializeField] protected CharacterAbility[] abilities = Array.Empty<CharacterAbility>();

public CharacterAbility[] Abilities
{
    get => abilities ?? Array.Empty<CharacterAbility>();
    set => abilities = value ?? Array.Empty<CharacterAbility>();
}
```

### EquipmentManager Changes

New methods in `EquipmentManager`:

```csharp
// Get all abilities from equipped items
public CharacterAbility[] GetEquipmentAbilities()

// Get abilities from a specific equipment slot
public CharacterAbility[] GetAbilitiesFromSlot(EquipmentSlot slot)
```

### Character Class Changes

New methods in `Character`:

```csharp
// Get all abilities (character + equipment)
public CharacterAbility[] GetAllAbilities()

// Get only equipment abilities
public CharacterAbility[] GetEquipmentAbilities()
```

## Usage Examples

### Basic Usage

```csharp
// Create a weapon with abilities
var fireStrike = new CharacterAbility("Fire Strike", AbilityType.Attack, 20, 10, "A fiery weapon attack");
weapon.Abilities = new CharacterAbility[] { fireStrike };

// Equip the weapon
character.EquipItem(weapon);

// Get all available abilities
var allAbilities = character.GetAllAbilities(); // Includes character + equipment abilities
var equipmentAbilities = character.GetEquipmentAbilities(); // Only equipment abilities
```

### Equipment Manager Usage

```csharp
// Get abilities from all equipped items
var equipmentAbilities = equipmentManager.GetEquipmentAbilities();

// Get abilities from specific slot
var weaponAbilities = equipmentManager.GetAbilitiesFromSlot(EquipmentSlot.Weapon);
var armorAbilities = equipmentManager.GetAbilitiesFromSlot(EquipmentSlot.Armor);
```

### Using Abilities in Combat

```csharp
public void UseAbility(Character character, string abilityName, Character target = null)
{
    var allAbilities = character.GetAllAbilities();
    var ability = allAbilities.FirstOrDefault(a => a.AbilityName == abilityName);
    
    if (ability != null && ability.CanUse(character.Stats))
    {
        var result = ability.Execute(character.Stats, target?.Stats);
        Debug.Log($"Used {abilityName}, result: {result}");
    }
}
```

## Example Equipment with Abilities

### Flaming Sword
- **Stat Modifiers**: +15 Attack, +5 Critical Chance
- **Abilities**: 
  - Fire Strike (Attack, Power: 20, Mana: 10): "Deals fire damage to target"
  - Flame Aura (Special, Power: 10, Mana: 15): "Burns nearby enemies"

### Guardian's Plate
- **Stat Modifiers**: +20 Defense, +50 Max Health
- **Abilities**:
  - Shield Wall (Defend, Power: 25, Mana: 12): "Greatly increases defense for one turn"
  - Taunt (Special, Power: 0, Mana: 8): "Forces enemies to target this character"

### Ring of Healing
- **Stat Modifiers**: +10 Mana Regeneration
- **Abilities**:
  - Quick Heal (Heal, Power: 15, Mana: 8): "Instant minor healing"
  - Regeneration (Heal, Power: 30, Mana: 20): "Healing over time effect"

## Testing

Comprehensive tests have been added to validate the functionality:

### EquipmentSoTests.cs
- `Equipment_WithAbilities_StoresAbilitiesCorrectly()`
- `Equipment_Abilities_DefaultsToEmpty()`
- `EquipmentManager_GetEquipmentAbilities_ReturnsAllAbilities()`
- `EquipmentManager_GetAbilitiesFromSlot_ReturnsCorrectAbilities()`

### CharacterTests.cs
- `Character_GetAllAbilities_ReturnsCharacterAndEquipmentAbilities()`
- `Character_GetEquipmentAbilities_ReturnsOnlyEquipmentAbilities()`
- `Character_GetEquipmentAbilities_ReturnsEmptyWhenNoEquipment()`

## Backward Compatibility

The implementation maintains full backward compatibility:
- Existing equipment without abilities continues to work normally
- All existing code continues to function without changes
- The `abilities` field defaults to an empty array
- Null safety is handled throughout the implementation

## Future Enhancements

Possible future additions:
- Ability cooldowns for equipment abilities
- Conditional abilities (e.g., only usable when health is low)
- Equipment set bonuses that unlock additional abilities
- Ability upgrade systems tied to equipment enhancement
- Passive abilities that activate automatically

## Demo Scripts

Two demo scripts are provided to showcase the functionality:

1. **EquipmentAbilityDemo.cs**: Demonstrates the core equipment abilities functionality
2. **CharacterEquipmentExample.cs**: Shows how characters integrate equipment abilities with their own abilities

These can be added to GameObjects in the scene and used via context menu options to see the feature in action.