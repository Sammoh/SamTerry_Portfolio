
# Samuel Terry - Unity3D Senior Developer Portfolio

Welcome to my portfolio! I am Samuel Terry, a senior Unity3D developer with extensive experience in C# programming, game development, and complex system design. Below, you will find a collection of projects demonstrating my skills in procedural generation, multiplayer networking, AI development, performance optimization, and more. Each project is structured to showcase my expertise in both technical implementation and clean, maintainable code.

## 🚀 Quick Setup for Developers

### Requirements
- **Unity Version**: Unity 6000.0.49f1 (Unity 6 Preview)
- **Platform**: Windows, Mac, Linux
- **Dependencies**: Unity AI Navigation, Animation Rigging, UGui

### Installation Steps
1. Clone this repository
2. Install Unity Hub and Unity 6000.0.49f1
3. Open the `project` folder in Unity
4. Wait for initial asset import (5-10 minutes on first open)
5. Navigate to the demonstration scenes in each project folder

> ⚠️ **Important**: Unity 6 is preview software. Build times are longer than stable Unity versions. Never cancel Unity operations as this can corrupt the project.

---

## 📋 Table of Contents

1. [Global Data & Editor Tools](#global-data--editor-tools)
2. [UI, Flow, & Files (2D Canvas)](#ui-flow--files-2d-canvas)
3. [Advanced Interaction (3D or VR)](#advanced-interaction-3d-or-vr)
4. [Origin Shifting (3D or VR)](#origin-shifting-3d-or-vr)
5. [Crowd System](#crowd-system)
6. [Goal Oriented Action Planning (GOAP)](#goal-oriented-action-planning-goap)
7. [Multiplayer Netcode](#multiplayer-netcode)
8. [Turn-Based Strategy Game](#turn-based-strategy-game)
9. [Third-Party Assets](#third-party-assets)

---

## Global Data & Editor Tools

**Project**: [Global Data & Editor Tools](https://github.com/Sammoh/SamTerry_Portfolio/tree/main/project/Assets/1.%20Global%20Data%20%26%20Editor%20Tools)  
**Tech Stack**: Unity3D, C#, ScriptableObjects, Custom Editor Tools  
**Scene**: `ItemSearch.unity`  
**Description**: Developed a global data system that includes lists of adjectives, nouns, and colors (restricted to non-neutral hues) for generating unique game objects. Implemented an editor tool to manage these objects, enabling filtering, selection, and color-based display.

### 🎯 Key Features:
- **Procedural Generation**: Random color selection from a predefined list
- **Unique Naming**: Object naming from adjectives and nouns using proper case
- **Custom Editor Tool**: Display and filter objects in a scene by name and color
- **Interactive Selection**: Click-to-select feature for GameObjects in editor
- **ScriptableObject Architecture**: Global data management with runtime protection

### 🛠 Developer Setup:
1. Open `ItemSearch.unity` scene
2. Use the custom editor tool: `Window > Global Data Manager`
3. Generate objects using the `ItemManager` component
4. Filter and search objects in the editor tool

---

## UI, Flow, & Files (2D Canvas)

**Project**: [UI, Flow, & Files](https://github.com/Sammoh/SamTerry_Portfolio/tree/main/project/Assets/2.%20UI%2C%20Flow%2C%20%26%20Files%20(2D%20Canvas))  
**Tech Stack**: Unity3D, JSON, XML, C#, Unity UI System  
**Scene**: `UI_Flow.unity`  
**Description**: Created a 2D application using Unity's Canvas system to manage reports with data persistence in JSON and XML formats. The application supports listing reports, editing report details, and creating new reports with various metadata.

### 🎯 Key Features:
- **Report Management**: Dynamic report listing and filtering system
- **Data Persistence**: Switchable data formats (JSON/XML) with serialization
- **State Management**: Edit report state and priority in detailed view
- **Multi-Resolution UI**: Responsive canvas design for different screen sizes
- **File Operations**: Create, read, update reports with file system integration

### 🛠 Developer Setup:
1. Open `UI_Flow.unity` scene
2. Run in play mode to test the report management system
3. Reports are saved to `Application.persistentDataPath/Reports`
4. Toggle between JSON/XML serialization in `ReportManager` component

---

## Advanced Interaction (3D or VR)

**Project**: [Advanced Interaction](https://github.com/Sammoh/SamTerry_Portfolio/tree/main/project/Assets/3.%20Advanced%20Interaction%20(3D%20or%20VR))  
**Tech Stack**: Unity3D, VR SDK, C#, Unit Testing Framework  
**Scene**: `Knob.unity`  
**Description**: Designed a rotatable knob with various snapping and event-driven interactions, providing both free and fixed rotation options. The project includes unit tests for key methods and events to ensure robust functionality.

### 🎯 Key Features:
- **Rotatable Knob System**: Free rotation with 0.05 increment snapping (75-10 degrees)
- **Snap Positions**: Off, Ignition, and Test (momentary) positions
- **Dual Input Methods**: VR controller support and mouse interaction
- **Event System**: Value change events during use and completion
- **API Methods**: `SetValueID()`, `SetValue()`, `GetValueID()`, `GetValue()`
- **Unit Testing**: Comprehensive test coverage for reliability

### 🛠 Developer Setup:
1. Open `Knob.unity` scene
2. Run Unity Test Runner: `Window > General > Test Runner`
3. Execute "Advanced Interaction Tests" assembly
4. Test in play mode with mouse click and drag on knob objects
5. VR testing requires VR headset (optional for basic validation)

---

## Origin Shifting (3D or VR)

**Project**: [Origin Shifting](https://github.com/Sammoh/SamTerry_Portfolio/tree/main/project/Assets/4.%20Origin%20Shifting%20(3D%20or%20VR))  
**Tech Stack**: Unity3D, C#, Alstra_Infinite_Planes_LowPoly  
**Scene**: `OriginShift.unity`  
**Description**: Developed a shifting origin system for an aircraft simulation, allowing seamless navigation in an expansive environment. The project includes dual aircraft control, camera switching, and a mini-map with position tracking.

### 🎯 Key Features:
- **Origin Shifting**: Seamless movement in large worlds without floating-point precision issues
- **Dual Aircraft Control**: Two controllable aircraft with independent flight paths
- **Flight Mechanics**: Speed control (50-500 km/h), yaw control, 100m altitude flying
- **Camera System**: Switchable cameras between aircraft with smooth transitions
- **Mini-Map System**: Real-time position tracking with north indicator and range detection
- **GUI Debug Info**: Speed, position, and shifted position data display

### 🛠 Developer Setup:
1. Open `OriginShift.unity` scene
2. **Controls**: 
   - Up/Down arrows: Adjust throttle speed
   - Left/Right arrows: Adjust yaw
   - Space: Spawn second aircraft
   - Enter: Switch between aircraft
3. Requires `Alstra_Infinite_Planes_LowPoly` asset for aircraft models
4. Run Unity Test Runner for "Origin Shifting Tests" assembly

---

## Crowd System

**Project**: [Crowd System](https://github.com/Sammoh/SamTerry_Portfolio/tree/main/project/Assets/5.%20CrowdSystem)  
**Tech Stack**: Unity3D, C#, NavMesh, RoyalCod_SimpleCharacters, Kevin_Iglesias_Basic_Motions  
**Scene**: `CrowdTesting.unity`  
**Description**: The Crowd System simulates thousands of agents with various behaviors in an environment. It utilizes Unity's NavMesh for pathfinding, Command Patterns for behaviors, Factory Pattern for character generation, and instanced materials for performance optimization.

### 🎯 Key Features:
- **Massive Scale**: Thousands of agents with dynamic spawning and randomized appearances
- **Behavior System**: Multiple behavior types (static, random movement, crowd up, sitting, talking)
- **NavMesh Integration**: Unity's NavMesh system for realistic pathfinding and movement
- **Character Variants**: Procedural skin, body region type, and clothing color variations
- **Performance Optimization**: Instanced materials and efficient rendering systems
- **Factory Pattern**: Dynamic character design generation system

### 🛠 Developer Setup:
1. Open `CrowdTesting.unity` scene
2. Ensure NavMesh is baked: `Window > AI > Navigation > Bake`
3. Run in play mode to spawn crowd agents
4. Requires `RoyalCod_SimpleCharacters` for character models
5. Requires `Kevin_Iglesias_Basic_Motions` for animation sets
6. **Performance Note**: Start with smaller crowd sizes for testing

---

## Goal Oriented Action Planning (GOAP)

**Project**: [Goal Oriented Action Planning](https://github.com/Sammoh/SamTerry_Portfolio/tree/main/project/Assets/6.%20Goal%20Oriented%20Action%20Planning)
**Tech Stack**: Unity3D, C#, GOAP Architecture, AI Planning
**Description**: A complete Goal Oriented Action Planning system featuring a tamagotchi-like 3D character with autonomous behavior. The system demonstrates advanced AI planning techniques with a stable, extensible architecture designed for plugging in new behaviors without code churn.

* Key Features:
  - **6 Core Interfaces**: IAgentState, IWorldState, IGoal, IAction, IPlanner, IExecutor for maximum extensibility
  - **Deterministic Game Loop**: Fixed 5-10Hz tick rate with continuous execution updates
  - **Tamagotchi-like Character**: Autonomous agent with needs (hunger, thirst, sleep, play) and dynamic priority system
  - **Intelligent Planning**: A* pathfinding with obstacle avoidance through world state facts and POI system
  - **Real-time Debug Overlay**: Comprehensive visualization of agent state, current goals, and action plans
  - **Comprehensive Testing**: PlayMode smoke tests and acceptance criteria validation
  - **Stable Contracts**: New goals and actions can be added without modifying core interfaces
  - **Obstacle Handling**: Planning system accounts for doors and environmental barriers through world facts

---


## Multiplayer Netcode

**Project**: [Multiplayer Netcode Integration](https://github.com/Sammoh/Multiplayer-Base)  
**Tech Stack**: Unity3D, Unity Netcode, Vivox, C#  
**Status**: 🚧 WORK IN PROGRESS  
**Description**: Multiplayer Lobby Client/Host Platform designed to connect players with a comprehensive lobby system. Features integrated voice chat through Vivox and networked game state synchronization.

### 🎯 Key Features:
- **Lobby System**: Client/Host architecture with room management
- **Voice Chat Integration**: Full chat channels through Vivox SDK
- **Network Synchronization**: Synchronized lobby states across clients
- **Game Initialization**: Host and Client network data for maps and characters
- **Connection Management**: Robust connection handling and player management

### 🛠 Developer Setup:
> **Note**: This project is located in a separate repository and is currently under active development. Integration with the main portfolio is planned for future releases.

---

## Turn-Based Strategy Game

**Project**: [Turn-Based Strategy Game](https://github.com/Sammoh/SamTerry_Portfolio/tree/main/project/Assets/8.%20Turn-Based%20Strategy)
**Tech Stack**: Unity3D, C#, Factory Pattern, Turn-Based Systems  
**Description**: A complete turn-based strategy game system featuring character creation through factory patterns, strategic combat, and comprehensive game state management. The system is self-contained with cleanup/restart capabilities and includes both AI and player-controlled characters.

* Key Features:
  - Factory pattern for character creation with multiple classes (Warrior, Mage, Rogue, Healer)
  - Dynamic character stats and abilities system with health, mana, attack, defense, and speed
  - Turn-based combat with speed-based turn ordering
  - AI-controlled enemy characters with tactical decision making
  - Complete game state management with win/loss conditions
  - Self-contained system with cleanup and restart functionality
  - Comprehensive UI with ability selection and target choosing
  - Extensive test suite covering all major systems and edge cases
  - Modular design allowing for easy expansion and customization

---

## Third-Party Assets

This portfolio integrates several third-party assets to demonstrate realistic development workflows and asset integration skills. All assets are properly licensed and used within their respective license terms.

### ✈️ Alstra_Infinite_Planes_LowPoly
**Used in**: Origin Shifting (3D or VR)  
**Purpose**: Provides EaglePlane and LightPlane prefabs for aircraft simulation  
**Integration**: Aircraft models with custom flight controllers and origin shifting system

### 🚶 RoyalCod_SimpleCharacters  
**Used in**: Crowd System  
**Purpose**: Character models for crowd simulation agents  
**Integration**: Procedural character variant generation with different body types and materials

### 🎬 Kevin_Iglesias_Basic_Motions
**Used in**: Crowd System  
**Purpose**: Animation sets for character behaviors  
**Integration**: Behavior-driven animation system with multiple character states

### 🏃 Kevin_Iglesias_Villager_Animations
**Used in**: Crowd System  
**Purpose**: Additional animation sets for villager-specific behaviors  
**Integration**: Extended animation library for diverse crowd behaviors

### 🛠 Developer Notes
- All third-party assets are contained within their respective folders
- Asset integration demonstrates proper Unity workflow practices
- Custom scripts extend asset functionality without modifying original files
- Performance optimizations applied through instanced materials and efficient rendering

---

## Contact

Feel free to contact me via [LinkedIn](https://www.linkedin.com/in/sameats3d) or [Email](mailto:sameats3d@gmail.com) for any further inquiries or project collaborations!
