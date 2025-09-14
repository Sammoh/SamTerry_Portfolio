# Origin Shifting (3D or VR)

A sophisticated aircraft simulation system demonstrating origin shifting techniques for seamless navigation in large-scale virtual environments.

## 🎯 Overview

This project showcases an advanced origin shifting system that allows aircraft to navigate vast virtual worlds without encountering floating-point precision issues. The system features dual aircraft control, dynamic camera switching, comprehensive flight mechanics, and real-time mini-map tracking with position visualization.

## ✨ Key Features

### ✈️ Aircraft Simulation
- **Dual Aircraft System**: Two independently controllable aircraft with seamless switching
- **Realistic Flight Mechanics**: Speed control (50-500 km/h), yaw adjustment, 100m altitude maintenance
- **No Aerodynamic Physics**: Simplified flight model focusing on navigation and control
- **Continuous Flight**: Aircraft continue flying when not actively controlled

### 🌍 Origin Shifting Technology
- **Seamless World Navigation**: Prevents floating-point precision errors in large worlds
- **Dynamic Coordinate System**: Automatically shifts world origin to maintain precision
- **Position Tracking**: Maintains both relative and absolute position data
- **Smooth Transitions**: Imperceptible origin shifts during gameplay

### 📡 Navigation Systems
- **Mini-Map Display**: Real-time position tracking with north indicator
- **Range Detection**: Shows aircraft in and out of mini-map range
- **GUI Debug Information**: Speed, position, and shifted position data
- **Dual Aircraft Tracking**: Visual indicators for both aircraft positions

### 🎮 Control System
- **Intuitive Controls**: Arrow keys for throttle and yaw control
- **Aircraft Spawning**: Space bar spawns second aircraft ahead of current position
- **Camera Switching**: Enter key toggles between aircraft cameras
- **Speed Management**: Throttle-style speed control (not gas pedal mechanics)

## 🏗️ Technical Implementation

### Architecture
```
4. Origin Shifting (3D or VR)/
├── Scripts/
│   ├── Aircraft/
│   │   ├── AircraftController.cs       # Individual aircraft control
│   │   ├── FlightMechanics.cs         # Flight physics and movement
│   │   └── AircraftSpawner.cs         # Aircraft creation and management
│   ├── Origin Shifting/
│   │   ├── OriginShiftManager.cs      # Core origin shifting logic
│   │   ├── WorldCoordinate.cs         # Coordinate system management
│   │   └── ShiftableObject.cs         # Objects that participate in shifting
│   ├── Navigation/
│   │   ├── MiniMapController.cs       # Mini-map display and tracking
│   │   ├── CompassSystem.cs           # North indicator and orientation
│   │   └── PositionTracker.cs         # Position data management
│   ├── Camera/
│   │   ├── CameraSwitcher.cs          # Aircraft camera management
│   │   └── SmoothFollow.cs            # Camera follow mechanics
│   └── UI/
│       ├── FlightHUD.cs               # Flight information display
│       └── DebugInfoPanel.cs          # Debug information GUI
├── Prefabs/
│   ├── Aircraft/                      # Aircraft prefab configurations
│   └── Environment/                   # World environment objects
├── Scenes/
│   └── OriginShift.unity             # Main demonstration scene
└── Tests/
    ├── OriginShiftTests.cs           # Origin shifting unit tests
    └── AircraftControlTests.cs       # Aircraft control tests
```

### Core Systems

#### `OriginShiftManager`
```csharp
public class OriginShiftManager : MonoBehaviour
{
    [Header("Shift Settings")]
    [SerializeField] private float shiftThreshold = 5000f;    // Distance before shift
    [SerializeField] private Vector3 playerPosition;          // Current player position
    [SerializeField] private Vector3 worldOffset;             // Current world offset
    
    [Header("Tracking")]
    [SerializeField] private Transform primaryAircraft;       // Main aircraft reference
    [SerializeField] private Transform secondaryAircraft;     // Second aircraft reference
    
    // Core functionality
    public void CheckAndPerformShift();
    public Vector3 GetAbsolutePosition(Vector3 localPosition);
    public Vector3 GetRelativePosition(Vector3 absolutePosition);
    
    // Events
    public event System.Action<Vector3> OnOriginShift;
}
```

#### `AircraftController`
```csharp
public class AircraftController : MonoBehaviour
{
    [Header("Flight Settings")]
    [SerializeField] private float currentSpeed = 0f;         // Current speed in km/h
    [SerializeField] private float minSpeed = 50f;            // Minimum flight speed
    [SerializeField] private float maxSpeed = 500f;           // Maximum flight speed
    [SerializeField] private float speedChangeRate = 25f;     // Speed change per second
    [SerializeField] private float yawRate = 30f;             // Degrees per second
    
    [Header("Flight Constraints")]
    [SerializeField] private float flightAltitude = 100f;     // Fixed altitude in meters
    [SerializeField] private bool maintainAltitude = true;    // Auto-maintain altitude
    
    // Control methods
    public void AdjustThrottle(float input);    // -1 to 1 for throttle control
    public void AdjustYaw(float input);         // -1 to 1 for yaw control
    public void SetSpeed(float targetSpeed);    // Direct speed setting
    
    // State queries
    public float GetSpeedKmh() => currentSpeed;
    public Vector3 GetVelocity() => transform.forward * (currentSpeed / 3.6f);
}
```

#### `MiniMapController`
```csharp
public class MiniMapController : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private float mapRange = 10000f;         // Map display range in meters
    [SerializeField] private RectTransform miniMapRect;       // UI mini-map container
    [SerializeField] private Transform northIndicator;        // North direction arrow
    
    [Header("Aircraft Markers")]
    [SerializeField] private RectTransform primaryMarker;     // Primary aircraft marker
    [SerializeField] private RectTransform secondaryMarker;   // Secondary aircraft marker
    [SerializeField] private Color inRangeColor = Color.green;
    [SerializeField] private Color outOfRangeColor = Color.red;
    
    // Map functionality
    public void UpdateAircraftPositions();
    public void SetAircraftInRange(bool primary, bool secondary);
    public Vector2 WorldToMapPosition(Vector3 worldPos);
}
```

### World Environment

#### Ground System
- **Large Ground Plane**: 1000x1000 meter terrain
- **Tileable Textures**: Seamlessly repeating ground textures that shift with origin
- **Texture Coordination**: Ground textures maintain visual continuity during shifts
- **LOD Management**: Level-of-detail system for performance optimization

#### Aircraft Models
- **Alstra_Infinite_Planes_LowPoly Integration**:
  - **EaglePlane**: Primary aircraft model
  - **LightPlane**: Secondary aircraft model
- **Custom Flight Controllers**: Extended with origin-shifting awareness
- **Camera Mounting**: Pre-configured camera positions for optimal viewing

## 🚀 Getting Started

### Prerequisites
- Unity 6000.0.49f1 or later
- **Required Asset**: `Alstra_Infinite_Planes_LowPoly` (aircraft models)
- Understanding of 3D coordinate systems and floating-point precision

### Setup Instructions
1. **Open Scene**: Load `OriginShift.unity`
2. **Verify Assets**: Ensure aircraft models are properly imported
3. **Run Tests**: Execute "Origin Shifting Tests" in Unity Test Runner
4. **Flight Testing**:
   - Use arrow keys to control aircraft
   - Spawn second aircraft with Space
   - Switch cameras with Enter
   - Monitor debug information in GUI

### Quick Setup Example
```csharp
// Create origin shift manager
GameObject manager = new GameObject("Origin Shift Manager");
OriginShiftManager originShift = manager.AddComponent<OriginShiftManager>();

// Configure aircraft
GameObject aircraft = Instantiate(aircraftPrefab);
AircraftController controller = aircraft.GetComponent<AircraftController>();
originShift.SetPrimaryAircraft(aircraft.transform);

// Setup mini-map
MiniMapController miniMap = FindObjectOfType<MiniMapController>();
miniMap.SetTrackedAircraft(aircraft.transform);
```

## 🎮 Controls & Usage

### Flight Controls
| Input | Action | Description |
|-------|--------|-------------|
| **Up Arrow** | Increase Throttle | Accelerate aircraft (throttle, not gas pedal) |
| **Down Arrow** | Decrease Throttle | Decelerate aircraft |
| **Left Arrow** | Yaw Left | Turn aircraft left |
| **Right Arrow** | Yaw Right | Turn aircraft right |
| **Space** | Spawn Aircraft | Create second aircraft 10m ahead |
| **Enter** | Switch Aircraft | Toggle between aircraft cameras |

### Flight Mechanics
```csharp
// Throttle control (not gas pedal)
void Update()
{
    float throttleInput = 0f;
    
    if (Input.GetKey(KeyCode.UpArrow))
        throttleInput = 1f;     // Increase throttle
    else if (Input.GetKey(KeyCode.DownArrow))
        throttleInput = -1f;    // Decrease throttle
    
    aircraft.AdjustThrottle(throttleInput);
    
    // Yaw control
    float yawInput = 0f;
    if (Input.GetKey(KeyCode.LeftArrow))
        yawInput = -1f;
    else if (Input.GetKey(KeyCode.RightArrow))
        yawInput = 1f;
        
    aircraft.AdjustYaw(yawInput);
}
```

### Aircraft Spawning System
```csharp
public void SpawnSecondAircraft()
{
    Vector3 spawnPosition = primaryAircraft.position + 
                           primaryAircraft.forward * 10f; // 10 meters ahead
    
    GameObject newAircraft = Instantiate(aircraftPrefab, spawnPosition, 
                                       primaryAircraft.rotation);
    
    // Match current speed
    AircraftController newController = newAircraft.GetComponent<AircraftController>();
    newController.SetSpeed(primaryController.GetSpeedKmh());
    
    secondaryAircraft = newAircraft.transform;
}
```

## ⚙️ Configuration

### Origin Shifting Settings
```csharp
[Header("Origin Shift Configuration")]
public float shiftThreshold = 5000f;           // Distance before shifting (meters)
public float safetyMargin = 1000f;             // Additional margin for precision
public bool enableAutomaticShifting = true;    // Auto-shift when threshold reached
public bool showDebugVisuals = false;          // Show shift zones in scene view
```

### Aircraft Configuration
```csharp
[Header("Flight Parameters")]
public float minSpeed = 50f;                   // Minimum speed (km/h)
public float maxSpeed = 500f;                  // Maximum speed (km/h)
public float acceleration = 25f;               // Speed change rate (km/h per second)
public float yawRate = 30f;                    // Turn rate (degrees per second)
public float flightAltitude = 100f;            // Fixed flight altitude (meters)

[Header("Physics Settings")]
public bool usePhysics = false;                // Enable/disable physics simulation
public float dragCoefficient = 0.1f;           // Air resistance simulation
public bool enableTurbulence = false;          // Random flight variations
```

### Mini-Map Configuration
```csharp
[Header("Mini-Map Display")]
public float displayRange = 10000f;            // Map range in meters
public float updateFrequency = 10f;            // Updates per second
public bool showDistanceRings = true;          // Display range indicators
public bool showHeadingLines = true;           // Show aircraft heading vectors

[Header("Marker Settings")]
public float markerSize = 5f;                  // Aircraft marker size
public Color primaryColor = Color.blue;        // Primary aircraft color
public Color secondaryColor = Color.green;     // Secondary aircraft color
public Color outOfRangeColor = Color.red;      // Out-of-range indicator
```

## 🧪 Testing Framework

### Origin Shifting Tests
```csharp
[Test]
public void OriginShift_WhenThresholdReached_ShiftsWorldOrigin()
{
    // Arrange
    var originShift = CreateOriginShiftManager();
    var aircraft = CreateTestAircraft();
    aircraft.transform.position = new Vector3(6000f, 100f, 0f); // Beyond threshold
    
    // Act
    originShift.CheckAndPerformShift();
    
    // Assert
    Assert.AreNotEqual(Vector3.zero, originShift.WorldOffset);
    Assert.Less(aircraft.transform.position.magnitude, 2000f); // Shifted closer to origin
}

[Test]
public void AbsolutePosition_AfterOriginShift_RemainsConsistent()
{
    // Test position consistency through origin shifts
    var originalAbsolute = new Vector3(7500f, 100f, 2500f);
    var localPos = originShift.GetRelativePosition(originalAbsolute);
    
    originShift.PerformShift(new Vector3(-5000f, 0f, -2000f));
    
    var calculatedAbsolute = originShift.GetAbsolutePosition(localPos);
    Assert.AreEqual(originalAbsolute, calculatedAbsolute, Vector3.kEpsilon);
}
```

### Aircraft Control Tests
```csharp
[Test]
public void AircraftController_SpeedControl_StaysWithinLimits()
{
    var aircraft = CreateTestAircraft();
    
    // Test maximum speed limit
    aircraft.AdjustThrottle(1f);
    UpdateForSeconds(aircraft, 20f); // 20 seconds at full throttle
    
    Assert.LessOrEqual(aircraft.GetSpeedKmh(), aircraft.maxSpeed);
    
    // Test minimum speed limit
    aircraft.AdjustThrottle(-1f);
    UpdateForSeconds(aircraft, 20f);
    
    Assert.GreaterOrEqual(aircraft.GetSpeedKmh(), aircraft.minSpeed);
}
```

### Integration Tests
```csharp
[Test]
public void DualAircraftSystem_IndependentControl_MaintainsConsistency()
{
    var primary = CreateTestAircraft();
    var secondary = CreateTestAircraft();
    
    // Set different speeds and headings
    primary.SetSpeed(200f);
    primary.transform.rotation = Quaternion.Euler(0f, 45f, 0f);
    
    secondary.SetSpeed(350f);
    secondary.transform.rotation = Quaternion.Euler(0f, -30f, 0f);
    
    // Simulate flight
    UpdateForSeconds(primary, 10f);
    UpdateForSeconds(secondary, 10f);
    
    // Verify independent operation
    Assert.AreNotEqual(primary.transform.position, secondary.transform.position);
    Assert.AreEqual(200f, primary.GetSpeedKmh(), 0.1f);
    Assert.AreEqual(350f, secondary.GetSpeedKmh(), 0.1f);
}
```

## 🎯 Current Status & Features

### ✅ Completed Features
- [x] Origin shifting system with automatic threshold detection
- [x] Dual aircraft control and spawning
- [x] Camera switching between aircraft
- [x] Mini-map with real-time position tracking
- [x] Flight mechanics with speed and yaw control
- [x] GUI debug information display
- [x] Unit testing framework
- [x] Large world ground plane with tileable textures

### 🚧 In Progress
- [ ] **VR Integration**: VR headset support for immersive flight
- [ ] **Enhanced Physics**: Optional aerodynamic simulation
- [ ] **Weather System**: Wind effects and environmental conditions
- [ ] **Autopilot**: AI-assisted flight control

### 📋 Future Enhancements
- **Multi-Aircraft Support**: More than two aircraft simultaneously
- **Formation Flying**: Coordinated aircraft movement patterns
- **Terrain System**: Procedural terrain generation with origin shifting
- **Collision Detection**: Aircraft and terrain collision systems
- **Recording/Playback**: Flight path recording and replay functionality

## 🐛 Troubleshooting

### Common Issues

#### Floating-Point Precision Errors
- **Symptoms**: Jittery movement, inaccurate positioning at large distances
- **Solution**: Ensure origin shifting is enabled and threshold is appropriate
- **Prevention**: Monitor absolute positions and shift before reaching precision limits

#### Aircraft Not Responding
- **Check Input**: Verify input system is receiving key presses
- **Controller State**: Ensure aircraft controller is enabled and configured
- **Physics**: Confirm Rigidbody settings if physics are enabled

#### Mini-Map Issues
- **Position Tracking**: Verify aircraft references are set in MiniMapController
- **UI Scaling**: Check Canvas scaler settings for proper UI display
- **Range Calculation**: Ensure map range covers expected flight area

#### Camera Switching Problems
- **Camera References**: Confirm camera objects are properly assigned
- **Active State**: Check that only one camera is active at a time
- **Smooth Transitions**: Verify camera follow settings for smooth movement

### Debug Tools
```csharp
[System.Diagnostics.Conditional("UNITY_EDITOR")]
public void DisplayDebugInfo()
{
    Debug.Log($"Primary Aircraft - Speed: {primaryAircraft.GetSpeedKmh():F1} km/h");
    Debug.Log($"Primary Position: {primaryAircraft.transform.position}");
    Debug.Log($"World Offset: {originShiftManager.WorldOffset}");
    Debug.Log($"Absolute Position: {originShiftManager.GetAbsolutePosition(primaryAircraft.transform.position)}");
    
    if (secondaryAircraft != null)
    {
        Debug.Log($"Secondary Aircraft - Speed: {secondaryAircraft.GetSpeedKmh():F1} km/h");
        Debug.Log($"Distance Between Aircraft: {Vector3.Distance(primaryAircraft.transform.position, secondaryAircraft.transform.position):F1}m");
    }
}
```

## 📁 Files Structure

```
4. Origin Shifting (3D or VR)/
├── README.md                           # This documentation
├── _README_.txt                       # Original requirements (legacy)
├── OriginShift.unity                  # Main demonstration scene
├── Scripts/
│   ├── Aircraft/
│   │   ├── AircraftController.cs       # Aircraft control and movement
│   │   ├── AircraftSpawner.cs         # Aircraft creation system
│   │   └── FlightMechanics.cs         # Flight physics calculations
│   ├── OriginShifting/
│   │   ├── OriginShiftManager.cs      # Core origin shifting logic
│   │   ├── ShiftableObject.cs         # Objects that participate in shifts
│   │   ├── WorldCoordinate.cs         # Coordinate system utilities
│   │   └── PrecisionManager.cs        # Floating-point precision management
│   ├── Navigation/
│   │   ├── MiniMapController.cs       # Mini-map display system
│   │   ├── CompassSystem.cs           # North indicator and navigation
│   │   ├── PositionTracker.cs         # Position tracking and logging
│   │   └── NavigationHUD.cs           # Navigation UI elements
│   ├── Camera/
│   │   ├── CameraSwitcher.cs          # Aircraft camera management
│   │   ├── SmoothFollow.cs            # Camera follow mechanics
│   │   └── CameraTransitions.cs       # Smooth camera transitions
│   ├── Environment/
│   │   ├── GroundTextureShifter.cs    # Ground texture origin shifting
│   │   ├── SkyboxController.cs        # Sky and atmospheric effects
│   │   └── LODManager.cs              # Level-of-detail management
│   └── UI/
│       ├── FlightHUD.cs               # Main flight interface
│       ├── DebugInfoPanel.cs          # Debug information display
│       ├── MiniMapUI.cs               # Mini-map UI components
│       └── InputDisplayer.cs          # Control input visualization
├── Prefabs/
│   ├── Aircraft/
│   │   ├── EaglePlane_Configured.prefab    # Primary aircraft setup
│   │   └── LightPlane_Configured.prefab   # Secondary aircraft setup
│   ├── Environment/
│   │   ├── GroundPlane_Large.prefab       # 1000x1000m ground plane
│   │   └── SkySystem.prefab               # Sky and atmospheric setup
│   └── UI/
│       ├── FlightHUD.prefab               # Complete flight interface
│       └── MiniMap.prefab                 # Mini-map system
├── Materials/
│   ├── GroundTextures/                    # Tileable ground materials
│   ├── AircraftMaterials/                 # Aircraft visual materials
│   └── UI/                                # UI styling materials
├── Tests/
│   ├── Runtime/
│   │   ├── OriginShiftTests.cs            # Origin shifting unit tests
│   │   ├── AircraftControlTests.cs        # Aircraft control tests
│   │   ├── CoordinateSystemTests.cs       # Coordinate conversion tests
│   │   └── IntegrationTests.cs            # System integration tests
│   └── TestUtilities/
│       ├── TestHelpers.cs                 # Test utility methods
│       └── MockObjects.cs                 # Mock objects for testing
└── Documentation/
    ├── FlightControls.md                  # Detailed control documentation
    ├── OriginShiftingGuide.md            # Technical implementation guide
    └── TroubleshootingGuide.md           # Common issues and solutions
```

## 📋 Dependencies

### Unity Packages
- **AI Navigation** (`com.unity.ai.navigation`) - Advanced navigation systems
- **Test Framework** (`com.unity.test-framework`) - Unit testing
- **Mathematics** (`com.unity.mathematics`) - High-precision calculations

### Third-Party Assets
- **Alstra_Infinite_Planes_LowPoly** - Aircraft models (EaglePlane, LightPlane)

### Technical Requirements
- **Unity Version**: 6000.0.49f1 or later
- **Platform**: Windows, Mac, Linux, VR platforms
- **Memory**: 4GB+ RAM for large world simulation
- **Storage**: 2GB+ for aircraft assets and terrain textures

### Optional Dependencies
- **VR SDK**: OpenXR, Oculus, SteamVR for VR flight simulation
- **Terrain Tools**: Unity Terrain tools for advanced ground systems

---

**Note**: This origin shifting system demonstrates production-ready techniques for large-scale virtual environments, suitable for flight simulators, open-world games, and professional training applications.