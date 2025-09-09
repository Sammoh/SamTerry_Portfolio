# Equipment ScriptableObject System

This system provides a unified ScriptableObject-based workflow for all equipment types in the game, following the existing patterns from the Global Data & Editor Tools and UI Flow systems.

## Features Implemented

### Core Equipment System
- **Equipment Base Class**: Abstract ScriptableObject with common properties (name, description, icon, level, weight, value, rarity)
- **File-name-based IDs**: Equipment IDs are derived from asset file names for fast lookup tables
- **Type-specific Classes**: Weapon, Accessory, and Armor with specialized properties and behaviors
- **Rarity System**: Common, Uncommon, Rare, Epic, Legendary with corresponding stat multipliers

### Equipment Types

#### Weapons
- **Properties**: Damage, Attack Speed, Range, Critical Chance, Critical Multiplier
- **Types**: Sword, Axe, Mace, Dagger, Bow, Staff
- **Features**: DPS calculation, type-specific stat adjustments, procedural generation

#### Accessories  
- **Properties**: Stat modifiers (Health, Mana, Damage, etc.), Stacking rules
- **Types**: Ring, Necklace, Amulet, Bracelet, Charm
- **Features**: Multiple stat modifiers, percentage/flat modifier types, stacking logic

#### Armor
- **Properties**: Defense, Magic Resistance, Movement Speed Modifier, Damage Resistances
- **Types**: Helmet, Chestplate, Leggings, Boots, Gloves, Shield
- **Materials**: Leather, Chainmail, Plate, Magical, Dragon (with material-specific bonuses)
- **Features**: Material-based defense multipliers, damage type resistances

### Equipment Database
- **Centralized Management**: Single ScriptableObject containing all equipment references
- **Fast Lookups**: Dictionary-based lookup tables for ID, type, and rarity
- **Auto-referencing**: Automatically scans Resources folder for equipment assets
- **Statistics**: Provides counts and breakdowns by type and rarity

### Equipment Generator
- **Procedural Generation**: Creates equipment with procedurally generated names and stats
- **Asset Management**: Creates, updates, and manages ScriptableObject assets
- **Overwrite Protection**: Never overwrites existing assets unless explicitly selected
- **Batch Operations**: Can generate all default equipment or specific items

### Editor Tools
- **Equipment Manager Window**: Comprehensive UI for equipment management
  - Generation controls for creating new equipment
  - Filtering by type, rarity, and search queries
  - Batch selection and updating of equipment
  - Database management and statistics
  - Asset inspection and selection
- **Menu Integration**: Accessible via Tools/Equipment menu

## Directory Structure

```
Assets/2. UI, Flow, & Files (2D Canvas)/
├── Equipment/
│   ├── Scripts/
│   │   ├── Equipment.cs              # Base equipment class
│   │   ├── Weapon.cs                 # Weapon implementation
│   │   ├── Accessory.cs              # Accessory implementation
│   │   ├── Armor.cs                  # Armor implementation
│   │   ├── EquipmentDatabase.cs      # Database management
│   │   └── EquipmentGenerator.cs     # Asset generation
│   └── Editor/
│       └── EquipmentManagerWindow.cs # Editor UI
├── Resources/Equipment/
│   ├── Weapons/                      # Weapon assets
│   ├── Accessories/                  # Accessory assets
│   ├── Armor/                        # Armor assets
│   └── EquipmentDatabase.asset       # Main database
└── Tests/
    ├── EquipmentTests.cs             # Unit tests
    └── UI.Equipment.Tests.asmdef     # Test assembly
```

## Usage Instructions

### Creating Equipment Assets

1. **Generate All Defaults**:
   - Use menu: `Tools/Equipment/Generate Default Equipment`
   - Creates one of each equipment type/rarity combination

2. **Create Specific Equipment**:
   - Open: `Tools/Equipment/Equipment Manager`
   - Use "Equipment Generation" section
   - Select type, rarity, and optional custom name

3. **Manual Creation**:
   - Right-click in Project window
   - Create > Equipment > [Weapon/Accessory/Armor]

### Managing Equipment

1. **Equipment Manager Window**:
   - Filter by type, rarity, or search query
   - Select multiple items for batch operations
   - Update existing equipment with new values
   - View database statistics

2. **Database Management**:
   - Use `Tools/Equipment/Refresh Database` to update references
   - Database auto-updates when assets are created/modified

### Runtime Usage

```csharp
// Get the equipment database
var database = EquipmentGenerator.GetOrCreateEquipmentDatabase();
database.InitializeLookupTables();

// Find equipment by ID (file name)
Equipment sword = database.FindEquipmentById("Legendary_Sword_Sharp_Blade");

// Get all weapons
var allWeapons = database.GetEquipmentByType(EquipmentType.Weapon);

// Get rare armor pieces
var rareArmor = database.GetEquipmentByRarity(EquipmentRarity.Rare);

// Load directly from Resources
Weapon specificWeapon = Resources.Load<Weapon>("Equipment/Weapons/Epic_Bow_Ancient_Strike");
```

## Key Design Decisions

1. **Resources Folder**: Used instead of Addressables for better runtime performance as specified
2. **File-name IDs**: Asset file names serve as unique IDs for fast lookup tables
3. **No Overwriting**: Generation never overwrites existing assets unless explicitly selected
4. **Existing Patterns**: Follows established patterns from Global Data and UI Flow systems
5. **Future-Proofing**: Easy to extend with new equipment types by inheriting from Equipment base class

## Testing

The system includes comprehensive unit tests covering:
- Equipment ID assignment and retrieval
- Type-specific functionality (DPS calculation, stat modifiers, etc.)
- Database operations (add, remove, lookup)
- Asset generation and validation
- Rarity-based multipliers

Run tests via Unity Test Runner window.

## Extension Points

To add new equipment types:

1. Create new class inheriting from `Equipment`
2. Add to `EquipmentType` enum
3. Update `EquipmentGenerator` to handle the new type
4. Add UI support in `EquipmentManagerWindow`
5. Add to database auto-referencing logic

## Performance Considerations

- Dictionary-based lookups for O(1) equipment retrieval
- Resources folder used for faster runtime loading
- Lazy initialization of lookup tables
- Minimal serialization overhead with ScriptableObjects

## Validation Checklist

- [x] All equipment assets saved in Resources/Equipment directory structure
- [x] File-name-based ID system implemented
- [x] Generation scripts create/update all equipment types
- [x] Editor UI lists existing items with included/excluded status
- [x] EquipmentDatabase references all generated assets
- [x] Generation never overwrites unless explicitly selected
- [x] Intuitive editor UI for generation and management
- [x] Future-proofed for additional equipment types
- [x] Comprehensive test coverage
- [x] Following existing codebase patterns