# Samuel Terry Unity3D Portfolio

**ALWAYS follow these instructions first and fallback to additional search and context gathering only if the information here is incomplete or found to be in error.**

Samuel Terry Unity3D Portfolio is a Unity 6 project showcasing advanced Unity development skills across multiple domains including procedural generation, UI systems, VR/3D interaction, origin shifting, crowd simulation, and multiplayer networking. The project contains 5 main portfolio demonstrations plus 3rd party asset examples.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Critical Requirements

### Unity Installation
- **Unity Version Required**: Unity 6000.0.49f1 (Unity 6 Preview)
- **NEVER** attempt to build without the exact Unity version installed
- Install Unity Hub first, then install Unity 6000.0.49f1 through the Hub
- Installation commands (requires internet access):
  ```bash
  # Download Unity Hub (Linux)
  wget -O UnityHub.AppImage "https://public-cdn.cloud.unity3d.com/hub/prod/UnityHub.AppImage"
  chmod +x UnityHub.AppImage
  ./UnityHub.AppImage
  ```
- **CRITICAL**: Unity Editor is required for ALL build, test, and run operations

### Build and Test Process
- **NEVER CANCEL BUILDS OR LONG-RUNNING COMMANDS**
- Project opening: 5-10 minutes on first open (Unity imports assets). NEVER CANCEL. Set timeout to 15+ minutes.
- Build time: 15-30 minutes depending on platform. NEVER CANCEL. Set timeout to 45+ minutes.
- Test execution: 5-10 minutes. NEVER CANCEL. Set timeout to 15+ minutes.
- **WARNING**: Force-closing Unity during asset import or build can corrupt the project

## Working Effectively

### Bootstrap and Setup
1. **Install Unity 6000.0.49f1** (see Critical Requirements above)
2. **Open project in Unity**:
   ```bash
   # From Unity Hub or command line (if Unity is in PATH)
   unity -projectPath "/path/to/project" -batchmode -quit -logFile
   ```
3. **Wait for initial import** - takes 5-10 minutes on first open. NEVER CANCEL.

### Building the Project
- **Build through Unity Editor only** - no command line build scripts available
- **Build platforms available**: Windows, Mac, Linux (configured in BuildSettings)
- **Build time**: 15-30 minutes per platform. NEVER CANCEL. Set timeout to 45+ minutes.
- **Build location**: `/project/Builds/` (create if needed)

### Running Tests
- **Use Unity Test Runner window** (Window > General > Test Runner)
- **Test assemblies**:
  - `Advanced Interaction Tests` - Tests for rotatable knob functionality
  - `Origin Shifting Tests` - Tests for origin shifting system
- **Test execution time**: 5-10 minutes. NEVER CANCEL. Set timeout to 15+ minutes.
- **Run tests before committing changes**

### Running Demonstrations
- **Play mode required** - run scenes in Unity Editor play mode
- **Cannot build standalone without Unity Pro license** (portfolio limitation)
- **Main demonstration scenes**:
  - `ItemSearch.unity` - Global Data & Editor Tools demo
  - `UI_Flow.unity` - 2D Canvas UI system demo
  - `Knob.unity` - Advanced 3D/VR interaction demo
  - `OriginShift.unity` - Origin shifting aircraft simulation
  - `CrowdTesting.unity` - Crowd system with thousands of agents
  - `AgentDesigner.unity` - Character design tool

## Project Structure

### Main Portfolio Projects (66 C# scripts total)
1. **Global Data & Editor Tools** (`Assets/1. Global Data & Editor Tools/`)
   - Procedural object generation with adjectives/nouns
   - Custom editor tools for scene management
   - ScriptableObject-based global data system

2. **UI, Flow, & Files** (`Assets/2. UI, Flow, & Files (2D Canvas)/`)
   - 2D Canvas-based report management system
   - JSON/XML data persistence
   - Dynamic UI generation and filtering

3. **Advanced Interaction** (`Assets/3. Advanced Interaction (3D or VR)/`)
   - Rotatable knob with snapping system
   - VR-compatible interaction framework
   - **HAS UNIT TESTS** - always run before changes

4. **Origin Shifting** (`Assets/4. Origin Shifting (3D or VR)/`)
   - Large world simulation with origin shifting
   - Dual aircraft control system
   - Mini-map and position tracking
   - **HAS UNIT TESTS** - always run before changes

5. **Crowd System** (`Assets/5. CrowdSystem/`)
   - Thousands of agents with NavMesh pathfinding
   - Command pattern behaviors
   - Factory pattern character generation
   - Instanced materials for performance

### Assembly Structure
- **Main assembly**: `Portfolio.asmdef` - Contains all main portfolio code
- **Test assemblies**: 
  - `Advanced Interaction Tests.asmdef`
  - `Origin Shifting Tests.asmdef`
- **Dependencies**: Unity.AI.Navigation, Unity.Animation.Rigging, NUnit for tests

## Validation Requirements

### Code Validation
- **Always run Unity Test Runner** after making changes to tested components
- **Test rotatable knob functionality** in `Knob.unity` scene:
  1. Enter play mode
  2. Click and drag knob to test rotation
  3. Verify snapping to positions: Off, Ignition, Test
  4. Verify "Test" position returns to "Off" after 2 seconds
- **Test crowd system** in `CrowdTesting.unity`:
  1. Enter play mode  
  2. Verify agents spawn and move with NavMesh
  3. Test different behavior types (static, random, crowd up, sitting, talking)
- **Test origin shifting** in `OriginShift.unity`:
  1. Enter play mode
  2. Verify aircraft movement and camera switching
  3. Test mini-map position tracking

### Build Validation
- **Always build after major changes** to verify no compilation errors
- **Build time**: 15-30 minutes. NEVER CANCEL. Set timeout to 45+ minutes.
- **Common build issues**:
  - Missing Unity packages (check Package Manager)
  - Assembly reference errors (check .asmdef files)
  - Platform-specific compilation errors

## Common Tasks

### Code Quality Validation
```bash
# Check for TODO/FIXME items before committing
cd project && grep -r "TODO\|FIXME\|HACK" Assets --include="*.cs"

# List all C# scripts by directory
find Assets -name "*.cs" -exec dirname {} \; | sort | uniq -c | sort -nr

# Count total scripts
find Assets -name "*.cs" | wc -l
```

### Working with Scenes
```bash
# List all scenes
find Assets -name "*.unity"
```
**Key scenes to know**:
- Portfolio demos: `ItemSearch.unity`, `UI_Flow.unity`, `Knob.unity`, `OriginShift.unity`, `CrowdTesting.unity`
- Asset demos: `Basic Motions - Playable Scene.unity`, `DemoScene.unity`, `Showcase.unity`

### Code Navigation
**Most frequently modified files**:
- `Assets/3. Advanced Interaction (3D or VR)/Interactable Objects/RotatableKnob.cs`
- `Assets/5. CrowdSystem/CrowdSystemScripts/` - Crowd behavior scripts
- `Assets/1. Global Data & Editor Tools/ItemManager.cs`
- `Assets/2. UI, Flow, & Files (2D Canvas)/Controllers/` - UI controllers

**Editor scripts** (custom tools):
- `Assets/1. Global Data & Editor Tools/Editor/`
- `Assets/3. Advanced Interaction (3D or VR)/Editor/KnobSnapPositionEditor.cs`

**Known code issues to be aware of**:
- RotatableKnob.cs has TODO items for snap value improvements
- MeshSpawner.cs doesn't work with scaled transforms (documented limitation)
- UI Flow system has TODO for display timing optimization

### Package Dependencies
**Required Unity packages** (auto-installed):
- `com.unity.ai.navigation` - NavMesh for crowd system
- `com.unity.animation.rigging` - Character animation
- `com.unity.ugui` - UI system
- `com.unity.multiplayer.center` - Multiplayer setup (future use)

**Verification commands**:
```bash
# Check package manifest
cat project/Packages/manifest.json

# Verify Unity version
cat project/ProjectSettings/ProjectVersion.txt

# Check assembly definitions
find project/Assets -name "*.asmdef" -exec echo "Assembly: {}" \; -exec cat {} \;
```

## Critical Warnings

### Build Process
- **NEVER CANCEL Unity operations** - can corrupt project
- **Always wait for "Importing assets" to complete** - 5-10 minutes on first open
- **Set timeouts to 45+ minutes for builds** and 15+ minutes for tests
- **Unity 6 is Preview software** - expect longer processing times than stable Unity

### Testing Requirements
- **ALWAYS test in play mode** after changes
- **Run unit tests** before committing changes to tested components
- **Test on target platform** if building for specific platform
- **Cannot test builds without Unity Pro license** - use play mode testing

### Performance Notes
- **Crowd system can spawn 1000+ agents** - performance testing required
- **Origin shifting handles large world coordinates** - test at extreme positions  
- **VR scenes require VR headset** for full testing (optional for basic validation)
- **MeshSpawner limitation**: Does not work for meshes with scaled transforms (documented in code)
- **Instanced materials used** for crowd rendering optimization

## Troubleshooting

### Common Issues
1. **"Unity not found"** - Install Unity 6000.0.49f1 exactly
2. **"Project version mismatch"** - Upgrade/downgrade Unity to 6000.0.49f1
3. **"Assembly reference errors"** - Check .asmdef files and package dependencies
4. **"Long import times"** - Normal for Unity 6 Preview, wait for completion
5. **"Build fails"** - Check Unity Console for specific errors, ensure all packages installed
6. **"NavMesh missing"** - Crowd system requires baked NavMesh (Window > AI > Navigation)
7. **"MeshSpawner errors"** - Does not work with scaled transform meshes (known limitation)
8. **"Test position returns to Off"** - Expected behavior in RotatableKnob after 2 seconds

### Build Times (Estimated)
- **First project open**: 5-10 minutes (asset import)
- **Incremental builds**: 5-15 minutes  
- **Full platform builds**: 15-30 minutes
- **Test execution**: 5-10 minutes
- **Package refresh**: 2-5 minutes

**NEVER CANCEL these operations** - set appropriate timeouts and wait for completion.

## Repository Structure Summary
```
/
├── README.md                 # Portfolio overview and project descriptions
├── project/                  # Unity project root
│   ├── Assets/              # All game assets and scripts
│   │   ├── 1. Global Data & Editor Tools/  # Procedural generation demo
│   │   ├── 2. UI, Flow, & Files (2D Canvas)/  # UI system demo  
│   │   ├── 3. Advanced Interaction (3D or VR)/  # VR interaction demo
│   │   ├── 4. Origin Shifting (3D or VR)/  # Large world demo
│   │   ├── 5. CrowdSystem/  # Crowd simulation demo
│   │   └── Portfolio.asmdef # Main assembly definition
│   ├── Packages/            # Unity package dependencies
│   └── ProjectSettings/     # Unity project configuration
└── .gitignore              # Excludes Unity build artifacts
```