# Turn-Based Strategy Game System

A comprehensive turn-based strategy game system built using Unity3D and C#, featuring a factory pattern for character creation, strategic combat mechanics, complete game state management, and an integrated equipment system that enhances character capabilities.

## Features

### Core Systems
- **Factory Pattern**: Character creation through `CharacterFactory` with support for multiple character classes and automatic equipment assignment
- **Turn-Based Combat**: Speed-based turn ordering with strategic ability usage
- **Character Stats**: Health, Mana, Attack, Defense, Speed system with damage calculation enhanced by equipment bonuses
- **Ability System**: Multiple ability types (Attack, Heal, Defend, Special) with mana costs
- **Equipment System**: Comprehensive equipment management with stat modifications, multiple equipment slots, and rarity tiers
- **Game State Management**: Complete state tracking with win/loss conditions
- **AI System**: Intelligent enemy decision-making
- **UI Integration**: Button-based interface for ability selection and target choosing

### Equipment System
The equipment system provides deep character customization through:
- **Equipment Slots**: Weapon, Armor, and Accessory slots
- **Stat Modifications**: Equipment provides bonuses/penalties to all character stats
- **Equipment Rarity**: Common, Uncommon, Rare, Epic, and Legendary tiers
- **Class Integration**: Equipment restrictions and bonuses tailored to character classes
- **Dynamic Updates**: Real-time stat recalculation when equipment changes

### Character Classes
1. **Warrior**: High health and attack, strong defense, lower speed and mana
   - Default Equipment: Iron Sword (+5 attack), Leather Armor (+15 health, +8 defense, -1 speed)
2. **Mage**: High mana and magical damage, lower health and defense
   - Default Equipment: Oak Staff (+2 attack, +10 mana), Mage Robes (+5 health, +2 defense, +1 speed, +15 mana)
3. **Rogue**: High speed and critical damage, moderate stats overall
   - Default Equipment: Steel Dagger (+4 attack, +2 speed), Studded Leather (+8 health, +4 defense, +3 speed)
4. **Healer**: Balanced stats with powerful healing abilities
   - Default Equipment: Healing Rod (+5 health, +1 attack, +2 defense, +8 mana), Blessed Garments (+10 health, +5 defense, +10 mana)

### Self-Contained Design
- Complete cleanup and restart functionality
- No dependencies on external systems
- Modular architecture for easy expansion
- Comprehensive error handling and validation

## Usage

### Quick Start
1. Add the `TurnBasedGameManager` prefab to your scene
2. Attach the `GameUI` component for player interactions
3. Call `StartNewGame()` to begin a match

### Programmatic Usage
```csharp
// Create characters using the factory
CharacterFactory factory = GetComponent<CharacterFactory>();
Character warrior = factory.CreateCharacter(CharacterClass.Warrior, "Hero", true);
Character mage = factory.CreateCharacter(CharacterClass.Mage, "Enemy", false);

// Start a game
TurnBasedGameManager gameManager = GetComponent<TurnBasedGameManager>();
gameManager.StartNewGame();

// Use abilities during player turn
CharacterAbility ability = gameManager.CurrentCharacter.Abilities[0];
List<Character> targets = gameManager.GetValidTargets(ability);
gameManager.UseAbility(ability, targets[0]);
```

### Demo Script
Use the `TurnBasedGameDemo` component to see the system in action:
```csharp
// Attach to a GameObject and it will automatically start a demo game
TurnBasedGameDemo demo = gameObject.AddComponent<TurnBasedGameDemo>();
demo.StartDemoGame();
```

## Architecture

### Class Structure
```
TurnBasedStrategy/
├── Characters/
│   ├── ICharacter.cs - Character interface
│   ├── Character.cs - Main character implementation with equipment integration
│   ├── CharacterStats.cs - Stats and health/mana management with equipment bonuses
│   ├── CharacterAbility.cs - Ability system
│   ├── Equipment.cs - Equipment data structures and item definitions
│   └── EquipmentManager.cs - Equipment slot management and stat calculation
├── Factory/
│   └── CharacterFactory.cs - Factory pattern for character creation with default equipment
├── Game/
│   ├── TurnBasedGameManager.cs - Main game controller
│   ├── TurnManager.cs - Turn order and management
│   └── GameState.cs - Game state tracking
└── UI/
    └── GameUI.cs - User interface controller
```

### Events System
The game manager provides several events for integration:
- `OnGameStateChanged` - Fired when game state changes
- `OnGameMessage` - Game notifications and messages
- `OnCharacterDefeated` - When a character is defeated

### Factory Pattern Implementation
The `CharacterFactory` creates characters with predefined stats and abilities:
```csharp
public Character CreateCharacter(CharacterClass characterClass, string name, bool isPlayerControlled = false, Transform parent = null)
```

## Testing

Comprehensive test suite covers all major systems:
- **CharacterTests**: Character behavior and state management
- **CharacterAbilityTests**: Ability execution and validation
- **EquipmentTests**: Equipment system functionality, stat modifications, and equipment management
- **FactoryTests**: Character creation, factory pattern, and default equipment assignment
- **GameManagerTests**: Game flow and state management
- **TurnManagerTests**: Turn order and management

Run tests through Unity Test Runner or programmatically validate the system.

## Customization

### Adding New Character Classes
1. Add new enum value to `CharacterClass`
2. Update `CreateStatsForClass()` in `CharacterFactory`
3. Update `CreateAbilitiesForClass()` in `CharacterFactory`
4. Add default equipment in `CreateDefaultWeapon()` and `CreateDefaultArmor()`

### Creating Custom Abilities
```csharp
var customAbility = new CharacterAbility(
    "Lightning Bolt", 
    AbilityType.Special, 
    35, // power
    20, // mana cost
    "Devastating lightning attack"
);
```

### Creating Custom Equipment
```csharp
var customWeapon = new Equipment(
    "Flaming Sword",
    "A sword wreathed in flames",
    EquipmentSlot.Weapon,
    EquipmentRarity.Epic,
    new EquipmentStats(0, 25, 0, -2, 0) // +25 attack, -2 speed
);

// Equip to character
character.EquipItem(customWeapon);
```

### Equipment Management
```csharp
// Get equipped item
Equipment currentWeapon = character.GetEquippedItem(EquipmentSlot.Weapon);

// Unequip item
Equipment removedArmor = character.UnequipItem(EquipmentSlot.Armor);

// Check if character has any equipment
bool hasEquipment = character.Equipment.HasEquipment();
```

### Extending Game Rules
The modular design allows easy extension:
- Override `TurnBasedGameManager` for custom game rules
- Implement `ICharacter` for custom character types
- Extend `CharacterAbility` for complex ability effects

## Integration

The system is designed to integrate seamlessly with Unity projects:
- Uses Unity's component system
- Compatible with Unity UI
- Follows Unity coding conventions
- Includes proper serialization support

## Performance

- Efficient turn-based processing
- Minimal garbage collection during gameplay
- Scalable to larger team sizes
- Optimized for mobile and desktop platforms