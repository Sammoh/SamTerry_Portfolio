# Global Data & Editor Tools

A comprehensive system demonstrating Unity's ScriptableObject architecture, procedural object generation, and custom editor tool development.

## 🎯 Overview

This project showcases a global data management system that generates unique game objects using procedural naming and color assignment. It includes a custom Unity editor tool for managing, filtering, and selecting these objects in the scene.

## ✨ Key Features

### 🗂️ Global Data System
- **ScriptableObject Architecture**: Centralized data management with runtime protection
- **10 Adjectives**: Predefined list for procedural naming
- **10 Nouns**: Noun collection for object generation
- **10 Colors**: Curated color palette (100% alpha, no black/white/grays, no red hues)

### 🎲 Procedural Object Generation
- **Unique Naming**: Combines adjectives and nouns using proper case
- **Random Color Assignment**: Selects from the global color list
- **GameObject Naming**: Converts to lowerCamelCase for GameObject names
- **Uniqueness Guarantee**: Ensures no duplicate names in the scene

### 🛠️ Custom Editor Tool
- **Scene Object Listing**: Displays all ItemManager MonoBehaviours in the scene
- **Color-Coded Display**: Shows each item's name in its assigned color
- **Search Functionality**: Filter list with partial name matching
- **Color Filtering**: Filter objects by their assigned colors
- **Interactive Selection**: Click to select associated GameObject in scene

## 🏗️ Technical Implementation

### Architecture
```
1. Global Data & Editor Tools/
├── Scripts/
│   ├── GlobalData.cs           # ScriptableObject data container
│   ├── ItemManager.cs          # MonoBehaviour for object generation
│   └── ItemGenerator.cs        # Procedural generation logic
├── Editor/
│   ├── GlobalDataEditor.cs     # Custom editor tool
│   └── ItemManagerEditor.cs    # Inspector customization
├── Data/
│   └── GlobalDataAsset.asset   # ScriptableObject instance
└── ItemSearch.unity            # Demonstration scene
```

### Core Components

#### `GlobalData` (ScriptableObject)
```csharp
[CreateAssetMenu(fileName = "GlobalData", menuName = "Portfolio/Global Data")]
public class GlobalData : ScriptableObject
{
    [SerializeField] private string[] adjectives = new string[10];
    [SerializeField] private string[] nouns = new string[10];
    [SerializeField] private Color[] colors = new Color[10];
    
    // Properties with runtime protection (sealed)
    public IReadOnlyList<string> Adjectives => adjectives;
    public IReadOnlyList<string> Nouns => nouns;
    public IReadOnlyList<Color> Colors => colors;
}
```

#### `ItemManager` (MonoBehaviour)
```csharp
public class ItemManager : MonoBehaviour
{
    [SerializeField] private string itemName;
    [SerializeField] private Color itemColor;
    
    public string ItemName => itemName;
    public Color ItemColor => itemColor;
    
    [ContextMenu("Generate New Item")]
    public void Generate()
    {
        // Procedural generation logic
    }
}
```

### Color Restrictions
The color system enforces specific constraints:
- **Alpha**: Always 100% (no transparency)
- **Excluded**: No black, white, or gray values
- **Excluded**: No red hues to maintain visual consistency
- **Validation**: Editor tools validate color assignments

## 🚀 Getting Started

### Prerequisites
- Unity 6000.0.49f1 or later
- Basic understanding of ScriptableObjects and Unity Editor tools

### Setup Instructions
1. **Open Scene**: Load `ItemSearch.unity`
2. **Access Editor Tool**: 
   - Window > Portfolio > Global Data Manager
   - OR use the ItemManager Inspector buttons
3. **Generate Objects**: 
   - Right-click in Hierarchy > Portfolio > Create Item
   - OR use existing ItemManager "Generate" button
4. **Test Filtering**: Use the editor tool to search and filter objects

### Quick Start Example
```csharp
// Access global data
GlobalData data = Resources.Load<GlobalData>("GlobalDataAsset");

// Generate new item
ItemManager newItem = new GameObject("NewItem").AddComponent<ItemManager>();
newItem.Generate();

// Access generated properties
string name = newItem.ItemName;        // e.g., "Shiny Crystal"
Color color = newItem.ItemColor;       // Random color from global list
```

## 🎮 Usage Examples

### Manual Object Creation
```csharp
// Create ItemManager programmatically
GameObject obj = new GameObject();
ItemManager item = obj.AddComponent<ItemManager>();
item.Generate();

// Access properties
Debug.Log($"Generated: {item.ItemName} with color {item.ItemColor}");
```

### Using the Editor Tool
1. **Open Tool**: Window > Portfolio > Global Data Manager
2. **View Objects**: See all items listed with colored names
3. **Search**: Type partial names to filter results
4. **Filter by Color**: Use color buttons to show specific colored items
5. **Select**: Click item names to select GameObjects in scene

### Custom Generation
```csharp
public class CustomItemGenerator : MonoBehaviour
{
    [SerializeField] private GlobalData globalData;
    
    public void CreateCustomItem()
    {
        // Access global data
        string adjective = globalData.Adjectives[Random.Range(0, globalData.Adjectives.Count)];
        string noun = globalData.Nouns[Random.Range(0, globalData.Nouns.Count)];
        Color color = globalData.Colors[Random.Range(0, globalData.Colors.Count)];
        
        // Create custom item
        GameObject item = new GameObject($"{adjective.ToLower()}{noun}");
        ItemManager manager = item.AddComponent<ItemManager>();
        // Set properties through reflection or custom methods
    }
}
```

## ⚙️ Configuration

### Global Data Setup
```csharp
// Configure adjectives
private static readonly string[] DefaultAdjectives = 
{
    "Shiny", "Ancient", "Mystical", "Glowing", "Crystal",
    "Golden", "Silver", "Ethereal", "Enchanted", "Radiant"
};

// Configure nouns
private static readonly string[] DefaultNouns = 
{
    "Gem", "Orb", "Crystal", "Stone", "Artifact",
    "Relic", "Treasure", "Pendant", "Amulet", "Charm"
};

// Color restrictions (no black/white/gray/red)
private static readonly Color[] ValidColors = 
{
    new Color(0f, 1f, 0f, 1f),      // Pure Green
    new Color(0f, 0f, 1f, 1f),      // Pure Blue
    new Color(1f, 1f, 0f, 1f),      // Yellow
    new Color(1f, 0f, 1f, 1f),      // Magenta
    new Color(0f, 1f, 1f, 1f),      // Cyan
    // ... additional valid colors
};
```

### Editor Tool Customization
```csharp
[CustomEditor(typeof(ItemManager))]
public class ItemManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        ItemManager item = (ItemManager)target;
        
        if (GUILayout.Button("Generate New Item"))
        {
            item.Generate();
        }
        
        if (GUILayout.Button("Open Global Data Manager"))
        {
            GlobalDataManagerWindow.ShowWindow();
        }
    }
}
```

## 🧪 Testing

### Unit Testing
```csharp
[Test]
public void Generate_CreatesUniqueNames()
{
    // Test unique name generation
    HashSet<string> generatedNames = new HashSet<string>();
    
    for (int i = 0; i < 100; i++)
    {
        ItemManager item = CreateTestItem();
        item.Generate();
        
        Assert.IsFalse(generatedNames.Contains(item.ItemName));
        generatedNames.Add(item.ItemName);
    }
}

[Test]
public void Generate_UsesValidColors()
{
    ItemManager item = CreateTestItem();
    item.Generate();
    
    // Verify alpha is 100%
    Assert.AreEqual(1f, item.ItemColor.a, 0.001f);
    
    // Verify not black/white/gray
    Color color = item.ItemColor;
    bool isGrayscale = Mathf.Approximately(color.r, color.g) && 
                      Mathf.Approximately(color.g, color.b);
    Assert.IsFalse(isGrayscale);
}
```

### Scene Testing
1. **Generate 20+ Objects**: Create multiple items to test uniqueness
2. **Verify Colors**: Ensure all colors meet the restrictions
3. **Test Editor Tool**: Verify filtering and selection functionality
4. **Performance**: Test with larger numbers of objects

## 🔧 Design Decisions

### ScriptableObject Choice
- **Global Variables Alternative**: ScriptableObjects provide better data management than static variables
- **Runtime Protection**: Properties are sealed to prevent accidental runtime modification
- **Designer Friendly**: Non-programmers can modify data through Inspector

### Naming Convention
- **Proper Case Generation**: "Shiny Crystal" for display names
- **lowerCamelCase GameObjects**: "shinyCrystal" for GameObject names
- **Uniqueness Enforcement**: Prevents duplicate names in scene

### Color Restrictions
- **Visual Consistency**: Eliminating reds creates cohesive color palette
- **Accessibility**: Avoiding grayscale improves visual distinction
- **Alpha Consistency**: 100% alpha ensures solid, visible objects

## 🐛 Troubleshooting

### Common Issues

#### No Objects Generated
- **Check GlobalData Asset**: Ensure ScriptableObject is properly configured
- **Verify Arrays**: Adjectives and nouns arrays must be populated
- **Scene References**: Confirm ItemManager components are in scene

#### Duplicate Names
- **Check Scene State**: Clear existing items before mass generation
- **Verify Logic**: Ensure uniqueness checking is working correctly
- **Large Scale Testing**: Generate many items to verify algorithm

#### Editor Tool Not Working
- **Assembly References**: Verify editor scripts are in Editor folder
- **Compilation Errors**: Check for script compilation issues
- **Menu Path**: Confirm menu items are registered correctly

### Debug Tools
```csharp
[System.Diagnostics.Conditional("UNITY_EDITOR")]
public static void LogGenerationStats()
{
    ItemManager[] allItems = FindObjectsOfType<ItemManager>();
    
    Debug.Log($"Total Items: {allItems.Length}");
    Debug.Log($"Unique Names: {allItems.Select(i => i.ItemName).Distinct().Count()}");
    Debug.Log($"Color Distribution: {GetColorDistribution(allItems)}");
}
```

## 🎯 Future Enhancements

### Planned Features
- **Procedural Stats**: Generate numerical properties for items
- **Rarity System**: Common, rare, legendary item classifications
- **Category System**: Group items by type (weapons, armor, etc.)
- **Export System**: Save item configurations to external files

### Advanced Features
- **Localization**: Support for multiple languages
- **Asset References**: Link to 3D models and textures
- **Animation Support**: Procedural animation assignments
- **Sound Integration**: Audio clips for item interactions

## 📁 Files Structure

```
1. Global Data & Editor Tools/
├── README.md                    # This documentation
├── _README_.txt                # Original requirements (legacy)
├── ItemSearch.unity            # Main demonstration scene
├── Scripts/
│   ├── GlobalData.cs           # ScriptableObject data container
│   ├── ItemManager.cs          # Object generation component
│   ├── ItemGenerator.cs        # Generation algorithms
│   └── GlobalDataValidator.cs  # Data validation utilities
├── Editor/
│   ├── GlobalDataEditor.cs     # Custom editor window
│   ├── ItemManagerEditor.cs    # Custom inspector
│   └── GlobalDataManagerWindow.cs # Main editor tool
└── Data/
    └── GlobalDataAsset.asset   # Configured ScriptableObject
```

## 📋 Dependencies

### Unity Packages
- **Unity Editor** (Built-in) - Custom editor tools
- **Unity UI** (Built-in) - Inspector customization

### Technical Requirements
- **Unity Version**: 6000.0.49f1 or later
- **Platform**: Editor only (design-time tool)
- **API Compatibility**: .NET Standard 2.1

## 💡 Learning Outcomes

This project demonstrates:
- **ScriptableObject Architecture**: Data-driven design patterns
- **Custom Editor Development**: Unity editor extension techniques
- **Procedural Generation**: Algorithmic content creation
- **Runtime Protection**: Preventing accidental data modification
- **Code Organization**: Separation of data, logic, and presentation

---

**Note**: This system serves as a foundation for more complex procedural generation systems and demonstrates best practices for Unity tool development.