# Goal Oriented Action Planning (GOAP) System

A complete Goal Oriented Action Planning system for Unity that demonstrates a tamagotchi-like 3D character with autonomous behavior based on needs and goals.

## Core Features

### ✅ Stable Core Interfaces
- **IAgentState**: Manages agent needs, inventory, and effects
- **IWorldState**: Handles world facts and Points of Interest (POIs)
- **IGoal**: Defines what the agent wants to achieve
- **IAction**: Represents executable behaviors
- **IPlanner**: Creates action sequences to achieve goals
- **IExecutor**: Manages plan execution and failure handling

### ✅ Deterministic Game Loop
- Fixed tick rate (5-10Hz configurable)
- Continuous execution updates for smooth actions
- Automatic replanning when conditions change

### ✅ Tamagotchi-like Character System
- **Needs**: Hunger, Thirst, Sleep, Play (0.0-1.0 scale)
- **Dynamic Priority**: Higher needs = higher goal priority
- **Need Decay**: Needs gradually increase over time

### ✅ Planning with Obstacles
- World state facts for condition checking
- POI (Point of Interest) system for navigation
- Path validation for obstacle avoidance
- Automatic replanning on failure

### ✅ Debug and Testing
- Real-time debug overlay (Press F1)
- Comprehensive test suite
- PlayMode smoke tests
- Visual feedback for agent state

## Quick Start

### 1. Setup Scene
```csharp
// Add to empty GameObject in scene
var sceneSetup = gameObject.AddComponent<GOAPSceneSetup>();
// This will create agent, world state, and POIs automatically
```

### 2. Manual Setup
```csharp
// Create World State
var worldState = GameObject.Find("World State")?.GetComponent<BasicWorldState>() 
                ?? new GameObject("World State").AddComponent<BasicWorldState>();

// Create Agent
var agentGO = new GameObject("GOAP Agent");
var agent = agentGO.AddComponent<GOAPAgent>();

// POIs are automatically registered by POIMarker components
```

### 3. Run and Debug
1. Press Play in Unity
2. Press F1 to toggle debug overlay
3. Watch agent select goals and execute actions based on needs

## Adding Custom Goals

```csharp
public class CustomGoal : IGoal
{
    public string GoalType => "custom_goal";
    public float Priority => 5f;
    
    public bool CanSatisfy(IAgentState agentState, IWorldState worldState)
    {
        // Define when this goal is available
        return agentState.GetNeed("custom_need") > 0.5f;
    }
    
    public bool IsCompleted(IAgentState agentState, IWorldState worldState)
    {
        // Define completion condition
        return agentState.GetNeed("custom_need") < 0.2f;
    }
    
    public Dictionary<string, object> GetDesiredState()
    {
        return new Dictionary<string, object>
        {
            { "need_custom_need", 0f }
        };
    }
    
    public float CalculatePriority(IAgentState agentState, IWorldState worldState)
    {
        return agentState.GetNeed("custom_need") * 10f;
    }
}
```

## Adding Custom Actions

```csharp
public class CustomAction : IAction
{
    public string ActionType => "custom_action";
    public float Cost => 2f;
    public bool IsExecuting { get; private set; }
    
    public bool CheckPreconditions(IAgentState agentState, IWorldState worldState)
    {
        // Define when this action can be executed
        return worldState.GetFact("can_do_custom_action");
    }
    
    public Dictionary<string, object> GetEffects()
    {
        return new Dictionary<string, object>
        {
            { "need_custom_need", 0f }
        };
    }
    
    public void StartExecution(IAgentState agentState, IWorldState worldState)
    {
        IsExecuting = true;
        // Initialize action execution
    }
    
    public ActionResult UpdateExecution(IAgentState agentState, IWorldState worldState, float deltaTime)
    {
        // Update action progress
        // Return Success, Failed, or Running
        IsExecuting = false;
        return ActionResult.Success;
    }
    
    public void ApplyEffects(IAgentState agentState, IWorldState worldState)
    {
        // Apply the action's effects to the world/agent
        agentState.SetNeed("custom_need", 0f);
    }
    
    // ... other required methods
}
```

## System Architecture

```
GOAPAgent (MonoBehaviour)
├── BasicAgentState (IAgentState)
├── BasicWorldState (IWorldState)
├── BasicPlanner (IPlanner)
├── BasicExecutor (IExecutor)
├── Goals: IdleGoal, EatGoal, DrinkGoal, PlayGoal
└── Actions: NoOpAction, MoveToAction, EatAction, DrinkAction
```

## Testing

Run tests through Unity Test Runner:
- Window > General > Test Runner
- Select "PlayMode" tab
- Run "GOAP.Tests" assembly

Key test: `AcceptanceCriteria_AgentSelectsGoalAndExecutesNoOpAction`

## Performance Notes

- **Tick Rate**: 5-10Hz for planning (configurable)
- **Execution**: 60Hz for smooth action updates
- **Scalability**: Single agent demonstration (easily extensible to multiple agents)
- **Memory**: Minimal allocation during runtime

## Extensibility

The system is designed for easy extension:

1. **New Goals**: Implement IGoal interface
2. **New Actions**: Implement IAction interface  
3. **Custom Needs**: Add to agent state
4. **World Facts**: Add through world state
5. **POI Types**: Register new types through POIMarker

No changes to core interfaces required for common extensions.

## Debug Commands

- **F1**: Toggle debug overlay
- **Force Replan**: Button in debug overlay
- **Console Logs**: Detailed action/goal information

## Files Structure

```
Assets/6. Goal Oriented Action Planning/
├── Scripts/
│   ├── Core/               # Core interfaces
│   ├── Implementations/    # Basic implementations
│   ├── Goals/             # Goal definitions
│   ├── Actions/           # Action definitions
│   ├── GOAPAgent.cs       # Main controller
│   ├── GOAPDebugOverlay.cs # Debug UI
│   └── GOAPSceneSetup.cs  # Scene setup helper
├── Tests/                 # Unit tests
├── Scenes/               # Demo scenes
├── Prefabs/              # Agent/POI prefabs
└── GOAP.asmdef           # Assembly definition
```

## Acceptance Criteria ✅

- ✅ Agent selects trivial goal and executes no-op action without errors
- ✅ Contracts are stable for adding actions without interface changes
- ✅ Deterministic tick-based execution
- ✅ Debug overlay shows current goal and plan
- ✅ PlayMode smoke tests pass