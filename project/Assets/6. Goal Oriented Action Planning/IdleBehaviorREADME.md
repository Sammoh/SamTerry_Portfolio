# Idle Behavior System for GOAP Agents

This implementation adds enhanced idle behaviors to the Goal Oriented Action Planning (GOAP) system. When agents have no urgent needs to fulfill, they can now perform more interesting and lifelike idle activities.

## New Idle Actions

### 1. WanderAction
- **Purpose**: Makes agents walk around randomly when idle
- **Behavior**: Uses NavMesh to find random walkable positions within a configurable radius
- **Features**:
  - Configurable wander radius (default: 10m)
  - Configurable move speed (default: 2m/s)
  - Configurable duration (default: 5s)
  - Falls back to stationary behavior if no NavMesh is available
- **Cost**: 0.5 (low priority for idle activity)

### 2. InvestigateAction
- **Purpose**: Makes agents look around and investigate nearby objects
- **Behavior**: Finds interesting objects within radius and rotates to look at them
- **Features**:
  - Configurable investigation radius (default: 5m)
  - Configurable investigation time (default: 3s)
  - Prioritizes objects with POIMarker components
  - Recognizes tagged objects (Prop, Furniture, Interactable)
  - Smooth rotation towards targets
- **Cost**: 0.3 (very low cost for idle activity)

### 3. SayRandomLineAction
- **Purpose**: Makes agents say random dialogue lines when idle
- **Behavior**: Selects from predefined dialogue arrays and speaks random lines
- **Features**:
  - Context-aware dialogue (different lines based on world state)
  - Integrates with existing bark/dialogue system
  - Configurable duration (default: 2s)
  - Idle conversation topics and curious observations
- **Cost**: 0.1 (lowest cost for ambient dialogue)

## Enhanced IdleGoal

The IdleGoal has been improved to provide better priority balancing:

- **Base Priority**: 0.3 (ensures idle activities have reasonable chance to execute)
- **Need-Based Adjustment**: Additional priority based on how satisfied the agent's needs are
- **Maximum Priority**: 0.8 when all needs are fully satisfied
- **Minimum Priority**: 0.3 even when needs are high (allows occasional idle behavior)

## Integration with Existing Systems

### Bark System Enhancement
Added new dialogue lines to the cannedBarks dictionary:
- `"wander"`: Walking-related barks
- `"investigate"`: Investigation-related barks  
- `"say_random"`: Conversational dialogue lines

### GOAPAgent Integration
New actions are automatically added to the agent's available actions list during initialization:
- Actions are properly injected with agent transform and NavMeshAgent references
- Actions integrate seamlessly with the existing GOAP planning system
- All actions satisfy the IdleGoal, providing multiple options for idle behavior

## Usage

The idle behaviors activate automatically when:
1. The agent has no higher-priority goals to pursue
2. The IdleGoal is selected by the planner
3. The planner selects one of the idle actions to fulfill the goal

No additional setup is required - the behaviors are automatically available once the system is integrated.

## Testing

Comprehensive tests are provided:
- **Unit Tests**: `IdleBehaviorTests.cs` - Tests individual action behavior
- **Integration Tests**: `IdleBehaviorIntegrationTest.cs` - Tests system integration
- **Test Coverage**:
  - Action creation and configuration
  - Precondition checking
  - Effect specification
  - Goal compatibility
  - Priority calculation
  - Execution lifecycle

## Configuration

Each action can be configured through constructor parameters:

```csharp
// Wander in 15m radius at 3m/s for 8 seconds
var wanderAction = new WanderAction(wanderRadius: 15f, moveSpeed: 3f, duration: 8f);

// Investigate within 8m for 4 seconds with faster rotation
var investigateAction = new InvestigateAction(investigationRadius: 8f, investigationTime: 4f, rotationSpeed: 3f);

// Say random lines for 3 seconds
var sayRandomAction = new SayRandomLineAction(duration: 3f);
```

## Future Enhancements

Potential improvements for the idle behavior system:
- Social interactions between multiple agents
- Memory system to avoid repeating the same actions
- Seasonal or time-based dialogue variations
- More complex investigation behaviors (touching, moving objects)
- Group idle activities (following other agents, gathering)