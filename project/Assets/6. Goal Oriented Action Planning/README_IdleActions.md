# New Idle Actions Implementation

This implementation adds three new idle actions to the GOAP system, providing rich ambient behaviors for agents when no higher-priority goals are available.

## Overview

### New Actions

1. **WanderAction** - Random movement with NavMesh integration
2. **InvestigateAction** - Looking around and examining nearby objects  
3. **SayRandomLineAction** - Context-aware dialogue and ambient chatter

### Enhanced IdleGoal

The IdleGoal has been upgraded with a sophisticated priority system that ensures idle behaviors have a reasonable chance to execute while not interfering with important needs.

## Implementation Details

### WanderAction

**Purpose**: Makes agents walk around randomly when idle

**Features**:
- Uses NavMesh pathfinding with transform movement fallback
- Configurable radius (default: 10m), speed (default: 2m/s), duration (default: 5s)
- Finds random walkable positions within specified radius
- Graceful fallback to stationary behavior if no valid positions found
- Low cost (0.5) ensures selection when no urgent needs exist

**Configuration**:
```csharp
var wanderAction = new WanderAction(
    cost: 0.5f,        // GOAP planner cost
    radius: 10f,       // Wander radius in meters
    speed: 2f,         // Movement speed in m/s
    wanderDuration: 5f // Duration in seconds
);
```

**Usage**:
- Requires agent injection: `wanderAction.InjectAgent(transform, navMeshAgent)`
- Automatically finds valid NavMesh positions
- Falls back to manual transform movement if NavMesh unavailable

### InvestigateAction

**Purpose**: Makes agents look around and investigate nearby objects

**Features**:
- Scans for interesting objects within configurable radius (default: 5m)
- Prioritizes objects with POIMarker components and specific tags (Prop, Furniture, Interactable)
- 30% chance to target specific objects, 70% aimless looking around
- Smoothly rotates agent to face target objects during investigation
- Very low cost (0.3) for ambient world interaction

**Configuration**:
```csharp
var investigateAction = new InvestigateAction(
    cost: 0.3f,               // GOAP planner cost
    radius: 5f,               // Investigation radius in meters
    investigateDuration: 5f,   // Duration in seconds
    rotSpeed: 45f             // Rotation speed in degrees/second
);
```

**Usage**:
- Requires agent injection: `investigateAction.InjectAgent(transform)`
- Automatically scans for POIMarker components
- Falls back to random looking if no interesting objects found

### SayRandomLineAction

**Purpose**: Makes agents speak random dialogue lines

**Features**:
- Context-aware dialogue selection based on world state and agent needs
- Includes conversational topics like "What are we doing today?" and "Anyone else around?"
- Integrates seamlessly with existing bark/dialogue system
- Lowest cost (0.1) for ambient chatter
- Multiple dialogue categories: general, morning, evening, safe, crowded, alone, tired, energetic

**Configuration**:
```csharp
var sayAction = new SayRandomLineAction(
    cost: 0.1f,           // GOAP planner cost
    speakDuration: 3f,    // How long the action runs
    displayDuration: 2f   // How long the bark message displays
);
```

**Dialogue Categories**:
- **General**: "What are we doing today?", "Anyone else around?"
- **Morning**: "Good morning!", "What a lovely morning!"
- **Evening**: "Getting late...", "Long day today."
- **Safe**: "All seems peaceful.", "Nice and quiet here."
- **Crowded**: "Lots of people around today.", "Busy place, this."
- **Alone**: "Enjoying the solitude.", "Nice to have some quiet time."
- **Tired**: "Feeling a bit tired...", "Could use a rest soon."
- **Energetic**: "Feeling good today!", "Ready for anything!"

### Enhanced IdleGoal Priority System

**Purpose**: Provide better priority balancing for idle activities

**Features**:
- Base priority of 0.3 ensures idle activities have reasonable chance to execute
- Additional priority boost when agent needs are satisfied (max 0.8 total)
- Prevents idle behaviors from being completely suppressed by high-priority needs

**Priority Calculation**:
```
Base Priority: 0.3
Need Satisfaction Bonus: (1 - maxNeed) * 0.5
Final Priority: min(basePriority + bonus, 0.8)
```

**Examples**:
- All needs satisfied (0.0): Priority = 0.3 + 0.5 = 0.8
- Moderate needs (0.5): Priority = 0.3 + 0.25 = 0.55
- High needs (0.9): Priority = 0.3 + 0.05 = 0.35
- Critical needs (1.0): Priority = 0.3 + 0.0 = 0.3

## Integration with Existing Systems

### Bark System

New dialogue categories added to `cannedBarks` dictionary in GOAPAgent:
- `"wander"`: "Time for a walk...", "Let's explore a bit.", etc.
- `"investigate"`: "What's that over there?", "Let me take a closer look.", etc.
- `"say_random"`: "Just thinking out loud...", "Hmm...", etc.

### GOAP Agent

New actions automatically added to available actions during initialization:
```csharp
// In SetupGoalsAndActions()
var wanderAction = new WanderAction();
wanderAction.InjectAgent(agentTransform, navMeshAgent);
availableActions.Add(wanderAction);

var investigateAction = new InvestigateAction();
investigateAction.InjectAgent(agentTransform);
availableActions.Add(investigateAction);

availableActions.Add(new SayRandomLineAction());
```

### NavMesh Integration

WanderAction fully integrates with existing movement systems:
- Uses NavMeshAgent when available and enabled
- Falls back to manual transform movement
- Respects NavMesh constraints and pathfinding

### POI System

InvestigateAction leverages existing Point of Interest markers:
- Automatically finds POIMarker components
- Checks for tagged objects (Prop, Furniture, Interactable)
- Integrates with existing POI registration system

## Testing

Comprehensive unit tests validate all functionality:

### Test Coverage
- **Basic Properties**: ActionType, Cost, IsExecuting
- **Preconditions**: Validation of execution requirements
- **Effects**: Verification of world state changes
- **Execution Lifecycle**: Start, Update, Cancel, ApplyEffects
- **Priority System**: IdleGoal priority calculation across different need levels
- **Integration**: GOAP system compatibility and cost ordering

### Running Tests
Tests are located in `Assets/6. Goal Oriented Action Planning/Tests/IdleActionsTests.cs`

Use Unity Test Runner:
1. Open Window > General > Test Runner
2. Select Play Mode or Edit Mode tests
3. Run "IdleActionsTests" test suite

## Usage Example

Agents will automatically perform idle behaviors when:
1. No higher-priority goals are available (low hunger/thirst/sleep needs)
2. The GOAP planner selects the IdleGoal
3. One of the three idle actions is chosen to fulfill the goal

**No additional setup required** - behaviors activate automatically in existing GOAP scenes.

### Manual Usage
```csharp
// Create and configure actions
var wanderAction = new WanderAction(cost: 0.5f, radius: 8f, speed: 2.5f);
wanderAction.InjectAgent(agentTransform, navMeshAgent);

var investigateAction = new InvestigateAction(cost: 0.3f, radius: 6f);
investigateAction.InjectAgent(agentTransform);

var sayAction = new SayRandomLineAction(cost: 0.1f, speakDuration: 3f);

// Add to available actions
availableActions.AddRange(new IAction[] { wanderAction, investigateAction, sayAction });
```

## Code Quality

### Architecture
- All actions implement IAction interface consistently
- Clean separation of concerns between movement, investigation, and dialogue
- Proper dependency injection for agent components
- Graceful fallback behaviors when systems unavailable

### Performance
- Minimal overhead during execution
- Efficient POI scanning with radius limiting
- Optimized NavMesh usage with transform fallback
- Reasonable default durations to prevent excessive action switching

### Maintainability
- Comprehensive documentation and comments
- Configurable parameters with sensible defaults
- Consistent naming conventions
- Proper Unity integration with meta files

## Future Enhancements

Potential improvements:
- Animation integration for wandering and investigating
- Audio cues for dialogue and investigation
- Group behaviors (agents investigating together)
- Environmental context awareness (weather, time of day)
- Memory system (remember interesting locations)
- Social interaction between agents during idle time