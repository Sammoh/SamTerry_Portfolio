# GOAP ScriptableObject Action System - Migration Guide

## Overview
This update introduces a ScriptableObject-based action system for GOAP, allowing dynamic configuration without code changes while maintaining full backward compatibility.

## Key Changes

### 1. New ScriptableObject Action System
- **NeedReductionActionSO**: Abstract base class for configurable actions
- **Specific Implementations**: EatActionSO, DrinkActionSO, SleepActionSO, PlayActionSO
- **ActionDatabase**: Container for managing collections of actions
- **Editor Tools**: Menu items under `GOAP > Setup` for database management

### 2. Enhanced GOAPAgent
- **ActionDatabase Support**: Loads actions from ScriptableObject database first
- **Fallback Compatibility**: Uses hardcoded actions if no ActionDatabase found
- **Validation**: Built-in validation when debugging is enabled

### 3. Testing & Validation
- **Comprehensive Test Suite**: New test files for ScriptableObject actions
- **Integration Tests**: GOAPAgent + ActionDatabase integration testing
- **Validation Tools**: Enhanced GOAPValidation with ActionDatabase support

## Migration Steps

### For Existing Projects
1. **No immediate action required** - existing hardcoded actions continue to work
2. **Optional**: Create ActionDatabase using `GOAP > Setup > Create Action Database`
3. **Optional**: Configure actions through Unity Inspector instead of code

### For New Projects
1. Use `GOAP > Setup > Create Action Database` to set up ScriptableObject actions
2. Configure actions in Inspector rather than hardcoding values
3. Add custom actions by extending `NeedReductionActionSO`

## Benefits

### Developer Experience
- **Visual Configuration**: Modify action parameters in Unity Inspector
- **No Compilation**: Change costs, durations, effects without code rebuild
- **Validation**: Built-in error checking and validation
- **Editor Tools**: Streamlined setup and management

### System Architecture
- **Consolidation**: Similar actions share common behavior through inheritance
- **Extensibility**: Easy to create new action types
- **Maintainability**: Centralized action management
- **Debugging**: Enhanced validation and error reporting

## Example Usage

### Creating Custom Actions
```csharp
[CreateAssetMenu(fileName = "RestAction", menuName = "GOAP/Actions/Rest Action")]
public class RestActionSO : NeedReductionActionSO
{
    protected override void OnValidate()
    {
        actionType = "rest";
        cost = 0.5f;
        duration = 4f;
        needType = "energy";
        requiredWorldFact = "at_rest_area";
        needWorldStateKey = "need_energy";
        
        base.OnValidate();
    }
}
```

### Configuring ActionDatabase
1. Create ActionDatabase asset: `GOAP > Setup > Create Action Database`
2. Add actions to database in Inspector
3. Assign ActionDatabase to GOAPAgent (optional - auto-loads from Resources)

## Backward Compatibility

### Existing Code
All existing hardcoded actions continue to work without modification:
- `EatAction`, `DrinkAction`, `SleepAction`, `PlayAction`, `NoOpAction`
- GOAPAgent falls back to hardcoded actions if no ActionDatabase found
- All existing interfaces and contracts remain unchanged

### Migration Path
- **Phase 1**: Keep existing hardcoded actions, optionally create ActionDatabase
- **Phase 2**: Replace hardcoded actions with ScriptableObject equivalents
- **Phase 3**: Deprecate hardcoded actions (future consideration)

## Troubleshooting

### Common Issues
1. **"ActionDatabase not found"**: Create using `GOAP > Setup > Create Action Database`
2. **"Actions not loading"**: Check ActionDatabase is in Resources folder or assigned to GOAPAgent
3. **"Validation errors"**: Use `GOAP > Setup > Validate Action Database` to check configuration

### Debug Tools
- **Debug Overlay**: Press F1 in play mode to see current actions
- **Console Logs**: Enable debugging on GOAPAgent for detailed validation
- **Validation Menu**: `GOAP > Setup > Validate Action Database` for configuration checks

## Performance Notes
- **Runtime**: No performance impact - same IAction interface used
- **Memory**: Minimal overhead from ScriptableObject references
- **Loading**: ActionDatabase loaded once at startup, cached thereafter

## Future Enhancements
- **Runtime Action Creation**: Support for dynamically creating actions at runtime
- **Action Pools**: Optimize memory usage for large numbers of actions
- **Serialization**: Save/load ActionDatabase configurations
- **Templates**: Pre-configured action templates for common use cases