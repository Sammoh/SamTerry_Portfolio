# Turn-Based Strategy Game System

A comprehensive turn-based strategy game system built using Unity3D and C#, featuring a factory pattern for character creation, strategic combat mechanics, and complete game state management.

## 🎯 Overview

**Tech Stack**: Unity3D, C#, Factory Pattern, Turn-Based Systems, ScriptableObjects  
**Scene**: Create scene with `TurnBasedGameManager` prefab  
**Description**: A complete turn-based strategy game system featuring character creation through factory patterns, strategic combat, and comprehensive game state management. The system is self-contained with cleanup/restart capabilities and includes both AI and player-controlled characters.

### 🛠 Developer Setup:
1. Add `TurnBasedGameManager` prefab to your scene
2. Attach `GameUI` component for player interactions
3. Call `StartNewGame()` to begin a match
4. Use Unity Test Runner for comprehensive test suite
5. Access Equipment Editor via `Tools > Turn-Based Strategy > Equipment Editor`

## ✨ Features

### 🏭 Core Systems
- **Factory Pattern**: Character creation through `CharacterFactory` with support for multiple character classes
- **Turn-Based Combat**: Speed-based turn ordering with strategic ability usage
- **Character Stats**: Health, Mana, Attack, Defense, Speed system with damage calculation
- **Equipment System**: Comprehensive equipment system with stat modifications and character customization
- **Ability System**: Multiple ability types (Attack, Heal, Defend, Special) with mana costs
- **Game State Management**: Complete state tracking with win/loss conditions
- **AI System**: Intelligent enemy decision-making
- **UI Integration**: Button-based interface for ability selection and target choosing

### ⚔️ Equipment System
The equipment system allows characters to equip items that modify their base stats, providing character progression and customization.

#### Equipment Classes
- **Equipment**: Represents an item that can be equipped with stat modifiers
- **StatModifier**: Defines how equipment affects character stats (additive or multiplicative)
- **EquipmentManager**: Handles equipped items for a character (3 slots: Weapon, Armor, Accessory)
- **EquipmentDatabase**: ScriptableObject for storing and managing equipment items

#### Character Stats Integration
Characters now have separate base stats and effective stats:
- **Base Stats**: The character's natural attributes (e.g., `BaseAttack`)
- **Effective Stats**: Base stats + equipment bonuses (e.g., `Attack`)

Example:
```csharp
// Character has 15 base attack
character.Stats.BaseAttack; // Returns 15

// Equip weapon with +5 attack
var weapon = new Equipment("Iron Sword", EquipmentSlot.Weapon, 
    new StatModifier[] { 
        new StatModifier(StatType.Attack, 5, ModifierType.Additive) 
    }, 
    "A sturdy iron sword");
character.EquipItem(weapon);
character.Stats.Attack; // Returns 20 (15 + 5)
```

#### Equipment Editor
Custom Unity Editor tool for creating and managing equipment:
- **Equipment Creation**: Form-based interface for creating weapons, armor, and accessories
- **Stat Assignment**: Dropdowns for stat types, input fields for values, toggles for modifier types
- **Preview Panel**: Shows resulting stat changes when equipping items
- **Database Integration**: Saves items to `EquipmentDatabase` ScriptableObject

Access via: **Tools > Turn-Based Strategy > Equipment Editor**

#### Example Equipment
```csharp
// Create equipment with multiple modifiers
var steelBlade = new Equipment("Steel Blade", EquipmentSlot.Weapon,
    new StatModifier[] {
        new StatModifier(StatType.Attack, 8, ModifierType.Additive),
        new StatModifier(StatType.Speed, 2, ModifierType.Additive)
    },
    "A sharp steel blade");

// Multiplicative modifier example
var powerRing = new Equipment("Power Ring", EquipmentSlot.Accessory,
    new StatModifier[] {
        new StatModifier(StatType.Attack, 20, ModifierType.Multiplicative)
    },
    "A ring that amplifies physical strength");
```

### Character Classes
1. **Warrior**: High health and attack, strong defense, lower speed and mana
2. **Mage**: High mana and magical damage, lower health and defense
3. **Rogue**: High speed and critical damage, moderate stats overall
4. **Healer**: Balanced stats with powerful healing abilities

### Self-Contained Design
- Complete cleanup and restart functionality
- No dependencies on external systems
- Modular architecture for easy expansion
- Comprehensive error handling and validation

## 🚀 Usage

### 🎮 Quick Start
1. Add the `TurnBasedGameManager` prefab to your scene
2. Attach the `GameUI` component for player interactions
3. Call `StartNewGame()` to begin a match

### 💻 Programmatic Usage
```csharp
// Create characters using the factory
CharacterFactory factory = GetComponent<CharacterFactory>();
Character warrior = factory.CreateCharacter(CharacterClass.Warrior, "Hero", true);
Character mage = factory.CreateCharacter(CharacterClass.Mage, "Enemy", false);

// Create and equip items
var ironSword = new Equipment("Iron Sword", EquipmentSlot.Weapon, 
    new StatModifier[] { 
        new StatModifier(StatType.Attack, 5, ModifierType.Additive) 
    }, 
    "A sturdy iron sword");

bool success = warrior.EquipItem(ironSword);

// Check what's equipped
Equipment currentWeapon = warrior.EquipmentManager.GetEquippedItem(EquipmentSlot.Weapon);

// Unequip item
Equipment unequipped = warrior.UnequipItem(EquipmentSlot.Weapon);

// Start a game
TurnBasedGameManager gameManager = GetComponent<TurnBasedGameManager>();
gameManager.StartNewGame();

// Use abilities during player turn
CharacterAbility ability = gameManager.CurrentCharacter.Abilities[0];
List<Character> targets = gameManager.GetValidTargets(ability);
gameManager.UseAbility(ability, targets[0]);
```

### ⚔️ Equipment Usage
```csharp
// Create equipment database
EquipmentDatabase database = ScriptableObject.CreateInstance<EquipmentDatabase>();
database.CreateDefaultEquipment();

// Get equipment from database
var weapons = database.GetEquipmentBySlot(EquipmentSlot.Weapon);
var ironSword = database.FindEquipmentByName("Iron Sword");

// Equip items and see stat changes
character.EquipItem(ironSword);
Debug.Log($"Base Attack: {character.Stats.BaseAttack}");
Debug.Log($"Effective Attack: {character.Stats.Attack}");
```

### Demo Scripts
Use the demo components to see the systems in action:
```csharp
// Turn-based game demo
TurnBasedGameDemo demo = gameObject.AddComponent<TurnBasedGameDemo>();
demo.StartDemoGame();

// Equipment system demo
EquipmentDemo equipDemo = gameObject.AddComponent<EquipmentDemo>();
equipDemo.RunEquipmentDemo();
```

## 🏗️ Architecture

### Class Structure
```
TurnBasedStrategy/
├── Characters/
│   ├── ICharacter.cs - Character interface
│   ├── Character.cs - Main character implementation
│   ├── CharacterStats.cs - Stats and health/mana management
│   └── CharacterAbility.cs - Ability system
├── Equipment/
│   ├── Equipment.cs - Equipment item implementation
│   ├── EquipmentManager.cs - Character equipment management
│   ├── EquipmentDatabase.cs - ScriptableObject equipment storage
│   ├── EquipmentDemo.cs - Equipment system demonstration
│   ├── StatModifier.cs - Stat modification system
│   ├── StatType.cs - Enumeration of modifiable stats
│   ├── ModifierType.cs - Additive/Multiplicative modifier types
│   ├── EquipmentSlot.cs - Equipment slot enumeration
│   └── Editor/
│       └── EquipmentEditor.cs - Custom Unity Editor tool
├── Factory/
│   └── CharacterFactory.cs - Factory pattern for character creation
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

## 🧪 Testing

Comprehensive test suite covers all major systems:
- **CharacterTests**: Character behavior and state management
- **CharacterAbilityTests**: Ability execution and validation
- **EquipmentTests**: Equipment system functionality and stat modifications
- **FactoryTests**: Character creation and factory pattern
- **GameManagerTests**: Game flow and state management
- **TurnManagerTests**: Turn order and management

Run tests through Unity Test Runner or programmatically validate the system.

## 🔧 Customization

### Adding New Character Classes
1. Add new enum value to `CharacterClass`
2. Update `CreateStatsForClass()` in `CharacterFactory`
3. Update `CreateAbilitiesForClass()` in `CharacterFactory`

### Creating Custom Equipment
```csharp
// Create equipment with custom modifiers
var customSword = new Equipment("Legendary Blade", EquipmentSlot.Weapon,
    new StatModifier[] {
        new StatModifier(StatType.Attack, 15, ModifierType.Additive),
        new StatModifier(StatType.Speed, 10, ModifierType.Multiplicative),
        new StatModifier(StatType.Mana, -5, ModifierType.Additive)
    },
    "A legendary weapon with complex effects");

// Add to database
database.AddEquipment(customSword);
```

### Adding New Stat Types
1. Add new enum value to `StatType`
2. Update `CharacterStats` to include the new stat
3. Update `GetBaseStat()` method in equipment editor
4. Update character creation to set the new stat

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

### Extending Game Rules
The modular design allows easy extension:
- Override `TurnBasedGameManager` for custom game rules
- Implement `ICharacter` for custom character types
- Extend `CharacterAbility` for complex ability effects

## 🔗 Integration

The system is designed to integrate seamlessly with Unity projects:
- Uses Unity's component system
- Compatible with Unity UI
- Follows Unity coding conventions
- Includes proper serialization support

### Backward Compatibility

The equipment system maintains full backward compatibility:
- Existing characters work without modification
- Old save data remains valid
- All existing tests continue to pass
- Equipment is optional - characters function normally without it

### Equipment Database Management

- Equipment data stored in ScriptableObjects for easy content management
- Hot-reloadable in Unity Editor
- Easily shareable between projects
- Version control friendly

## ⚡ Performance

- Efficient turn-based processing
- Minimal garbage collection during gameplay
- Stat calculations cached until equipment changes
- Scalable to larger team sizes
- Equipment modifiers calculated only when needed
- Optimized for mobile and desktop platforms

## 🖥️ Console Commands

Equipment Demo component provides context menu commands:
- **"Run Equipment Demo"**: Demonstrates equipment system functionality
- **"Reset Character"**: Returns character to default state without equipment

Access via right-click on `EquipmentDemo` component in Inspector.