
# Samuel Terry - Unity3D Senior Developer Portfolio

Welcome to my portfolio! I am Samuel Terry, a senior Unity3D developer with extensive experience in C# programming, game development, and complex system design. Below, you will find a collection of projects demonstrating my skills in procedural generation, multiplayer networking, AI development, performance optimization, and more. Each project is structured to showcase my expertise in both technical implementation and clean, maintainable code.

---

## Table of Contents

1. [Global Data & Editor Tools](#global-data--editor-tools)
2. [UI, Flow, & Files (2D Canvas)](#ui-flow--files-2d-canvas)
3. [Advanced Interaction (3D or VR)](#advanced-interaction-3d-or-vr)
4. [Origin Shifting (3D or VR)](#origin-shifting-3d-or-vr)
5. [Crowd System](#crowd-system)
6. [Multiplayer Netcode](#multiplayer-netcode)
7. [Turn-Based Strategy Game](#turn-based-strategy-game)

---

## Global Data & Editor Tools

**Project**: [Global Data & Editor Tools](https://github.com/Sammoh/SamTerry_Portfolio/tree/main/project/Assets/1.%20Global%20Data%20%26%20Editor%20Tools)
**Tech Stack**: Unity3D, C#  
**Description**: Developed a global data system that includes lists of adjectives, nouns, and colors (restricted to non-neutral hues) for generating unique game objects. Implemented an editor tool to manage these objects, enabling filtering, selection, and color-based display.

* Key Features:
  - Random color selection from a predefined list.
  - Unique object naming from adjectives and nouns.
  - Editor tool to display and filter objects in a scene by name and color.
  - Interactive click-to-select feature for GameObjects.

---

## UI, Flow, & Files (2D Canvas)

**Project**: [UI, Flow, & Files](https://github.com/Sammoh/SamTerry_Portfolio/tree/main/project/Assets/2.%20UI%2C%20Flow%2C%20%26%20Files%20(2D%20Canvas))
**Tech Stack**: Unity3D, JSON, XML, C#  
**Description**: Created a 2D application using Unity's Canvas system to manage reports with data persistence in JSON and XML formats. The application supports listing reports, editing report details, and creating new reports with various metadata.

* Key Features:
  - Dynamic report listing and filtering.
  - Edit report state and priority in a detailed view.
  - New report creation with serialized data storage.
  - Switchable data formats (JSON/XML).

---

## Advanced Interaction (3D or VR)

**Project**: [Advanced Interaction](https://github.com/Sammoh/SamTerry_Portfolio/tree/main/project/Assets/3.%20Advanced%20Interaction%20(3D%20or%20VR))
**Tech Stack**: Unity3D, VR (optional), C#  
**Description**: Designed a rotatable knob with various snapping and event-driven interactions, providing both free and fixed rotation options. The project includes unit tests for key methods and events to ensure robust functionality.

* Key Features:
  - Snapping and free rotation with adjustable values.
  - Event-based responses on value changes.
  - Custom methods for direct value setting and retrieval.
  - Multiple knobs with distinct values and snap positions.

---

## Origin Shifting (3D or VR)

**Project**: [Origin Shifting](https://github.com/Sammoh/SamTerry_Portfolio/tree/main/project/Assets/4.%20Origin%20Shifting%20(3D%20or%20VR))
**Tech Stack**: Unity3D, C#  
**Description**: Developed a shifting origin system for an aircraft simulation, allowing seamless navigation in an expansive environment. The project includes dual aircraft control, camera switching, and a mini-map with position tracking.

* Key Features:
  - Origin shifting for continuous movement in large worlds.
  - Dual aircraft control with camera and speed management.
  - Mini-map displaying relative and absolute positioning.
  - GUI elements for speed, position, and shifted position data.

---

## Crowd System

**Project**: [Crowd System](https://github.com/Sammoh/SamTerry_Portfolio/tree/main/project/Assets/5.%20CrowdSystem)
**Tech Stack**: Unity3D, C#  
**Description**: The Crowd System is designed to simulate a crowd of thousands of agents with various behaviors in an environment. It utilizes Unity's NavMesh for pathfinding and movement, and Command Patterns for defining different behaviors. A factory pattern to generate character designs, and instanced materals to optimize rendering. The system allows for dynamic spawning of agents with randomized appearances and behaviors, providing a realistic crowd simulation.

* Key Features:
  - Dynamic agent spawning with randomized character designs.
  - Multiple behavior types for agents, including static, random movement, forward and back, crowd up, sitting, and talking.
  - Integration with Unity's NavMesh for pathfinding and movement.
  - Customizable agent speed and angular speed.
  - Event-driven system for handling path completion.
  - Variants include, skin, body region type, and clothing color.
  
  https://github.com/user-attachments/assets/c8db3f9a-3be1-4190-a2c1-034acd2ad8f7

---


## Multiplayer Netcode

**Project**: [Multiplayer Netcode Integration](https://github.com/Sammoh/Multiplayer-Base)
**Tech Stack**: Unity3D, Networking, Netcode, C#  
**Description**: WORK IN PROGRESS: Multiplayer Lobby Client/Host Platform. Used to connect players with a lobby chat system through Vivox. 

* Key Features:
  - Networked synchronization of lobby states.
  - Full chat channels.
  - Host and Client network data for game initialization (maps, characters).

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

## Contact

Feel free to contact me via [LinkedIn](https://www.linkedin.com/in/sameats3d) or [Email](mailto:sameats3d@gmail.com) for any further inquiries or project collaborations!
