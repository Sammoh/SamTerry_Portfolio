# Equipment and Skill Tree System

This document describes the new Equipment and Skill Tree systems added to the Turn-Based Strategy project.

## Overview

The equipment and skill tree systems allow characters to:
- Equip items that modify their base stats
- Learn and swap abilities through a skill tree interface
- View stat comparisons when changing equipment or abilities
- Maintain character progression and customization

## Equipment System

### Equipment Classes

**Equipment**: Represents an item that can be equipped
- `EquipmentName`: Display name of the item
- `Slot`: Which slot it occupies (Weapon, Armor, Accessory)
- `StatModifiers`: Array of stat bonuses/penalties
- `Description`: Flavor text describing the item

**StatModifier**: Defines how equipment affects character stats
- `StatType`: Which stat to modify (MaxHealth, Attack, Defense, Speed, Mana)
- `Value`: Amount of modification
- `ModifierType`: How to apply (Additive: +5, Multiplicative: +20%)

**EquipmentManager**: Handles equipped items for a character
- Manages equipment in 3 slots: Weapon, Armor, Accessory
- Calculates combined stat bonuses from all equipped items
- Supports both additive and multiplicative modifiers

### Character Stats Integration

Characters now have separate base stats and effective stats:
- **Base Stats**: The character's natural attributes (e.g., `BaseAttack`)
- **Effective Stats**: Base stats + equipment bonuses (e.g., `Attack`)

Example:
```csharp
// Character has 15 base attack
character.Stats.BaseAttack; // Returns 15

// Equip weapon with +5 attack
character.EquipItem(ironSword);
character.Stats.Attack; // Returns 20 (15 + 5)
```

### Equipment Usage

```csharp
// Create equipment
var weapon = new Equipment("Iron Sword", EquipmentSlot.Weapon, 
    new StatModifier[] { 
        new StatModifier(StatType.Attack, 5, ModifierType.Additive) 
    }, 
    "A sturdy iron sword");

// Equip item
bool success = character.EquipItem(weapon);

// Unequip item
Equipment unequipped = character.UnequipItem(EquipmentSlot.Weapon);

// Check what's equipped
Equipment currentWeapon = character.EquipmentManager.GetEquippedItem(EquipmentSlot.Weapon);
```

## Skill Tree System

### Skill Tree Classes

**SkillTreeNode**: Represents a learnable ability
- `NodeId`: Unique identifier for the ability
- `Ability`: The actual ability (damage, mana cost, etc.)
- `Category`: Which slot it can occupy (BasicAttack, Skill, Secondary, Special)
- `Prerequisites`: Other nodes that must be unlocked first
- `IsUnlocked`: Whether the player can use this ability

**SkillTree**: Manages a character's available and equipped abilities
- Maintains a collection of all nodes
- Tracks which abilities are equipped in each of 4 slots
- Handles ability replacement and prerequisites

### Ability Categories

Characters have 4 ability slots corresponding to categories:
1. **BasicAttack** (Slot 0): Primary combat abilities
2. **Skill** (Slot 1): Magic spells and special techniques  
3. **Secondary** (Slot 2): Defensive abilities and utilities
4. **Special** (Slot 3): Ultimate abilities with high requirements

### Skill Tree Usage

```csharp
// Get character's skill tree
SkillTree skillTree = character.SkillTree;

// Check current abilities
CharacterAbility[] currentAbilities = skillTree.CurrentAbilities;

// Add new nodes to the tree
var newNode = new SkillTreeNode("fireball", fireballAbility, AbilityCategory.Skill, 
    new string[] { "basic_heal" });
skillTree.AddNode(newNode);

// Unlock a node (if prerequisites met)
skillTree.UnlockNode("fireball");

// Get available abilities for a category
var skillAbilities = skillTree.GetAvailableNodesForCategory(AbilityCategory.Skill);

// Replace an ability
skillTree.ReplaceAbility(1, fireballAbility); // Replace slot 1 with fireball
```

## UI Integration

### Skill Tree UI

The `SkillTreeUI` component provides the interface for managing abilities:

**Current Abilities Section**: Shows abilities equipped in each slot
- Click an ability to see available replacements for that category
- Displays ability name, power, and mana cost

**Available Abilities Section**: Shows unlocked abilities for the selected category
- Only displays when a current ability is selected
- Click to select a replacement ability

**Replacement Panel**: Allows comparison and confirmation
- Shows stats for current vs new ability
- Displays power and mana cost differences
- "Replace" button to confirm, "Cancel" to abort

### Game UI Integration

The main game UI (`TurnBasedGameUI`) includes:
- Skill Tree button (available during player turns)
- Integration with character selection
- Button enabling/disabling based on game state

### Opening the Skill Tree

```csharp
// From UI button click
skillTreeUI.SetCharacter(currentCharacter);
skillTreeUI.ShowSkillTree();

// Programmatically
if (currentCharacter != null && currentCharacter.IsPlayerControlled)
{
    skillTreeUI.SetCharacter(currentCharacter);
    skillTreeUI.ShowSkillTree();
}
```

## Example Equipment and Abilities

### Sample Equipment

The `EquipmentDatabase.CreateDefaultEquipment()` method provides examples:

**Weapons**:
- Iron Sword: +5 Attack
- Steel Blade: +8 Attack, +2 Speed
- Flame Sword: +12 Attack, -5 Mana

**Armor**:
- Leather Armor: +3 Defense, +10 Health
- Chain Mail: +6 Defense, +15 Health, -2 Speed
- Mage Robes: +20 Mana, -2 Defense

**Accessories**:
- Power Ring: +20% Attack (multiplicative)
- Health Amulet: +30 Health
- Speed Boots: +5 Speed

### Sample Skill Tree

The `SkillTreeDatabase.CreateDemoSkillTreeNodes()` method provides a complete skill tree:

**Basic Attacks**: Basic Attack → Power Attack → Fury Strike
**Skills**: Basic Heal → Greater Heal, Flame Bolt, Ice Shard
**Secondary**: Defend → Shield Wall, Counter Attack, Meditation
**Special**: Special Strike, Meteor, Divine Intervention, Berserker Rage

## Testing

Comprehensive test coverage is provided in:
- `EquipmentTests.cs`: Tests equipment creation, stat modification, and integration
- `SkillTreeTests.cs`: Tests skill tree functionality, node unlocking, and ability replacement
- `CharacterTests.cs`: Updated to test equipment integration with characters

## Demo and Usage

Run the `EquipmentSkillTreeDemo` component to see the systems in action:
- Demonstrates stat changes from equipment
- Shows skill tree progression and ability replacement
- Provides logging of all operations for debugging

### Console Commands

Use the context menu in the Inspector:
- "Run Equipment and Skill Tree Demo": Full system demonstration
- "Reset Character": Returns character to default state

## Integration Notes

### Backward Compatibility

The systems maintain full backward compatibility:
- Existing characters work without modification
- Old save data remains valid
- Character abilities fall back to the original array if skill tree is empty

### Performance Considerations

- Stat calculations are cached until equipment changes
- UI elements are created/destroyed as needed to minimize memory usage
- Equipment and skill tree data can be stored in ScriptableObjects for easy content management

### Extensibility

The system is designed to be easily extended:
- Add new `StatType` enum values for new stats
- Create new `ModifierType` values for different calculation methods
- Add new `AbilityCategory` values for additional ability slots
- Equipment and abilities are data-driven through ScriptableObjects