# Crowd System

A comprehensive crowd simulation system designed to handle thousands of agents with diverse behaviors, appearances, and realistic pathfinding in Unity.

## 🎯 Overview

The Crowd System demonstrates advanced crowd simulation techniques using Unity's NavMesh system, Command Pattern for behaviors, Factory Pattern for character generation, and performance optimization through instanced materials. The system can spawn and manage thousands of agents simultaneously while maintaining smooth performance.

## ✨ Key Features

### 🏭 Factory Pattern Character Generation
- **Procedural Character Creation**: Dynamic generation of character variants
- **Appearance Variants**: Different skin tones, body types, and clothing colors
- **Material Instancing**: Performance-optimized rendering for large crowds
- **Randomized Attributes**: Unique speed and angular speed per agent

### 🧠 Behavior System (Command Pattern)
- **Static Behavior**: Agents remain stationary
- **Random Movement**: Wandering within defined areas
- **Forward/Back Movement**: Directional crowd flow
- **Crowd Up**: Agents move toward gathering points
- **Sitting Behavior**: Agents sit at designated locations
- **Talking Behavior**: Agents engage in conversation animations

### 🗺️ NavMesh Integration
- **Unity NavMesh**: Professional pathfinding and obstacle avoidance
- **Dynamic Navigation**: Agents navigate around obstacles and other agents
- **Path Completion Events**: Event-driven system for behavior changes
- **Customizable Speed**: Individual agent speed and angular speed settings

### 🎭 Character Design System
- **AgentDesigner Scene**: Visual character creation tool
- **Component-Based Design**: Modular character assembly
- **Real-time Preview**: See character variants in real-time
- **Export System**: Save character designs for crowd spawning

## 🛠 Technical Implementation

### Architecture
```
CrowdSystem/
├── AgentDesigner/           # Character design tool
├── CharacterComponents/     # Modular character parts
├── CharacterDesignerScripts/ # Character creation scripts
├── CrowdSystemScripts/      # Core crowd logic
│   ├── BehaviorCommands/    # Command pattern behaviors
│   ├── CharacterFactory/    # Factory pattern implementation
│   ├── CrowdManager/        # Main crowd controller
│   └── AgentController/     # Individual agent logic
├── CrowdTesting.unity       # Main demonstration scene
└── AgentDesigner.unity      # Character design scene
```

### Core Classes

#### `CrowdManager`
Main controller for the entire crowd system:
- Spawns and manages all agents
- Coordinates behavior assignments
- Handles performance optimization
- Manages agent lifecycle

#### `AgentController`
Individual agent behavior and state management:
- Executes assigned behaviors
- Manages NavMesh navigation
- Handles animation state changes
- Processes movement and rotation

#### `CharacterFactory`
Procedural character generation:
- Creates character variants
- Assigns random appearances
- Generates instanced materials
- Manages character prefab instantiation

#### `BehaviorCommand` (Interface)
Command pattern for agent behaviors:
- `StaticBehavior`: No movement
- `RandomMovementBehavior`: Wandering
- `CrowdUpBehavior`: Gathering movement
- `SittingBehavior`: Sitting animations
- `TalkingBehavior`: Conversation animations

## 🚀 Getting Started

### Prerequisites
- Unity 6000.0.49f1 or later
- **Required Assets**:
  - `RoyalCod_SimpleCharacters` - Character models
  - `Kevin_Iglesias_Basic_Motions` - Animation sets
  - `Kevin_Iglesias_Villager_Animations` - Additional animations
- NavMesh baked in the scene

### Quick Setup
1. **Open Scene**: Load `CrowdTesting.unity`
2. **Bake NavMesh**: 
   - Window > AI > Navigation
   - Select ground objects
   - Mark as "Navigation Static"
   - Click "Bake"
3. **Configure Crowd**: Adjust settings in `CrowdManager` component
4. **Play**: Enter play mode to see crowd simulation

### Character Designer Setup
1. **Open Scene**: Load `AgentDesigner.unity`
2. **Design Characters**: Use the visual tools to create character variants
3. **Export Designs**: Save character configurations for crowd spawning
4. **Test in Crowd**: Use exported designs in `CrowdTesting.unity`

## ⚙️ Configuration

### Crowd Manager Settings
```csharp
[SerializeField] private int maxAgents = 1000;           // Maximum concurrent agents
[SerializeField] private float spawnRadius = 50f;       // Spawn area radius
[SerializeField] private float behaviorChangeInterval = 10f; // Behavior switching time
[SerializeField] private bool usePerformanceMode = true; // Enable optimizations
```

### Agent Settings
```csharp
[SerializeField] private float baseSpeed = 3.5f;        // Default movement speed
[SerializeField] private float speedVariation = 1.5f;   // Speed randomization range
[SerializeField] private float angularSpeed = 120f;     // Rotation speed
[SerializeField] private float stoppingDistance = 1f;   // NavMesh stopping distance
```

### Behavior Configuration
```csharp
// Behavior weights for random selection
private readonly Dictionary<BehaviorType, float> behaviorWeights = new()
{
    { BehaviorType.Static, 0.2f },
    { BehaviorType.RandomMovement, 0.3f },
    { BehaviorType.CrowdUp, 0.2f },
    { BehaviorType.Sitting, 0.15f },
    { BehaviorType.Talking, 0.15f }
};
```

## 🎮 Usage Examples

### Basic Crowd Spawning
```csharp
// Get crowd manager reference
CrowdManager crowdManager = FindObjectOfType<CrowdManager>();

// Spawn 500 agents with random behaviors
crowdManager.SpawnCrowd(500);

// Spawn agents with specific behavior
crowdManager.SpawnAgentsWithBehavior(BehaviorType.CrowdUp, 100);
```

### Custom Character Creation
```csharp
// Create character factory
CharacterFactory factory = GetComponent<CharacterFactory>();

// Generate random character
GameObject character = factory.CreateRandomCharacter();

// Create character with specific appearance
CharacterAppearance appearance = new CharacterAppearance
{
    skinTone = SkinTone.Medium,
    bodyType = BodyType.Slim,
    clothingColor = Color.blue
};
GameObject customCharacter = factory.CreateCharacter(appearance);
```

### Behavior Management
```csharp
// Get agent controller
AgentController agent = agentGameObject.GetComponent<AgentController>();

// Assign new behavior
ICommand newBehavior = new RandomMovementBehavior(agent.transform, 10f);
agent.AssignBehavior(newBehavior);

// Change behavior dynamically
agent.ChangeBehavior(BehaviorType.Sitting);
```

## 🔧 Performance Optimization

### Instanced Materials
```csharp
// Automatic material instancing for performance
MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
propertyBlock.SetColor("_Color", randomColor);
renderer.SetPropertyBlock(propertyBlock);
```

### LOD System
- **Distance-based optimization**: Reduce detail for distant agents
- **Animation culling**: Disable animations for off-screen agents
- **Update rate scaling**: Lower update frequency for distant agents

### NavMesh Optimization
```csharp
// Optimize NavMesh settings for crowds
NavMeshAgent.radius = 0.3f;           // Smaller radius for tighter packing
NavMeshAgent.height = 1.8f;           // Standard human height
NavMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
```

## 🧪 Testing

### Performance Testing
- **Crowd Size Scaling**: Test with 100, 500, 1000, 2000+ agents
- **Behavior Distribution**: Ensure even behavior distribution
- **Frame Rate Monitoring**: Monitor FPS with large crowds
- **Memory Usage**: Check memory allocation patterns

### Behavior Testing
1. **Static Behavior**: Agents should remain motionless
2. **Random Movement**: Agents wander within bounds
3. **Crowd Up**: Agents move toward gathering points
4. **Sitting**: Agents sit at designated locations
5. **Talking**: Agents face each other and animate

### Navigation Testing
- **Obstacle Avoidance**: Agents navigate around obstacles
- **Path Finding**: Agents find optimal routes to destinations
- **Collision Handling**: Agents avoid overlapping
- **Boundary Respect**: Agents stay within defined areas

## 🐛 Troubleshooting

### Common Issues

#### No Agents Spawning
- **Check NavMesh**: Ensure NavMesh is baked and covers spawn areas
- **Verify Prefabs**: Confirm character prefabs are assigned in factory
- **Asset Dependencies**: Ensure all required assets are imported

#### Poor Performance
- **Reduce Agent Count**: Lower `maxAgents` in CrowdManager
- **Enable Performance Mode**: Turn on optimization settings
- **Check LOD Settings**: Ensure LOD system is configured

#### Animation Issues
- **Animation Controller**: Verify animator controllers are assigned
- **Animation Clips**: Ensure animation clips are properly imported
- **Layer Weights**: Check animation layer weights and blending

#### NavMesh Problems
- **Bake Quality**: Increase NavMesh bake resolution
- **Agent Radius**: Adjust agent radius for better pathfinding
- **Obstacle Layers**: Verify obstacle layers are correctly set

### Debug Tools
```csharp
// Enable debug visualization
public bool showDebugInfo = true;
public bool showNavMeshPaths = true;
public bool showBehaviorStates = true;

// Debug crowd statistics
Debug.Log($"Active Agents: {activeAgents.Count}");
Debug.Log($"Average FPS: {averageFrameRate}");
Debug.Log($"Memory Usage: {memoryUsage}MB");
```

## 🎯 Future Enhancements

### Planned Features
- **AI Behavior Trees**: More complex decision-making
- **Crowd Emotions**: Emotional state propagation
- **Weather Effects**: Environmental behavior changes
- **Social Groups**: Formation of temporary groups
- **Procedural Clothing**: Dynamic outfit generation

### Advanced Behaviors
- **Following**: Agents follow leaders
- **Fleeing**: Panic and evacuation behaviors
- **Shopping**: Market and commerce behaviors
- **Queuing**: Line formation and waiting

## 📁 Files Structure

```
5. CrowdSystem/
├── README.md                    # This documentation
├── AgentDesigner.unity          # Character design scene
├── CrowdTesting.unity          # Main demonstration scene
├── AgentDesigner/              # Character design tool components
├── CharacterComponents/        # Modular character parts
├── CharacterDesignerScripts/   # Character creation scripts
├── CrowdSystemScripts/         # Core crowd simulation logic
│   ├── Behaviors/              # Command pattern behaviors
│   ├── Factory/                # Character factory implementation
│   ├── Managers/               # System managers
│   └── Utils/                  # Utility scripts
└── test/                       # Test scenes and configurations
```

## 📋 Dependencies

### Unity Packages
- **AI Navigation** (`com.unity.ai.navigation`) - NavMesh system
- **Animation Rigging** (`com.unity.animation.rigging`) - Character animation

### Third-Party Assets
- **RoyalCod_SimpleCharacters** - Character models and rigs
- **Kevin_Iglesias_Basic_Motions** - Basic animation set
- **Kevin_Iglesias_Villager_Animations** - Villager-specific animations

### Technical Requirements
- **Unity Version**: 6000.0.49f1 or later
- **Platform**: Windows, Mac, Linux
- **Memory**: 4GB+ RAM recommended for large crowds
- **GPU**: DirectX 11 compatible for instanced rendering

## 🤝 Contributing

When extending the crowd system:

1. **Follow Patterns**: Use existing Command and Factory patterns
2. **Maintain Performance**: Consider performance impact of new features
3. **Test Thoroughly**: Validate with various crowd sizes
4. **Document Changes**: Update this README with new features

### Adding New Behaviors
```csharp
public class CustomBehavior : ICommand
{
    public void Execute()
    {
        // Implement custom behavior logic
    }
    
    public bool IsComplete()
    {
        // Return completion status
    }
}
```

### Creating Character Variants
```csharp
public class CustomCharacterVariant : ICharacterVariant
{
    public void ApplyToCharacter(GameObject character)
    {
        // Apply custom appearance modifications
    }
}
```

---

**Note**: This crowd system is designed for demonstration purposes and showcases advanced Unity development techniques. It can be adapted and extended for production use in games and simulations.