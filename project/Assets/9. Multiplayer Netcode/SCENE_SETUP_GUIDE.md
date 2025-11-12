# Scene Setup Guide for Peer-Hosted Bootstrap

This guide provides step-by-step instructions for setting up the PeerHostedBootstrap scene in Unity Editor.

## Step 1: Create the Scene

1. In Unity Editor, go to **File > New Scene**
2. Choose **Basic (Built-in)** or **Basic (URP)** depending on your render pipeline
3. Save the scene as: `Assets/9. Multiplayer Netcode/Scenes/PeerHostedBootstrap.unity`

## Step 2: Setup NetworkManager

1. **Create NetworkManager GameObject**:
   - Right-click in Hierarchy > Create Empty
   - Name it "NetworkManager"
   - Position: (0, 0, 0)

2. **Add NetworkManager Component**:
   - With NetworkManager selected, click **Add Component**
   - Search for "NetworkManager" (from Unity.Netcode)
   - Add the component

3. **Add UnityTransport Component**:
   - With NetworkManager still selected, click **Add Component**
   - Search for "Unity Transport"
   - Add the component

4. **Configure NetworkManager**:
   - In NetworkManager component:
     - Set **NetworkTransport** field to the UnityTransport component on the same GameObject
     - Expand **Network Prefabs** section (we'll add the player prefab here later)

## Step 3: Setup ConnectionManager

1. **Create ConnectionManager GameObject**:
   - Right-click in Hierarchy > Create Empty
   - Name it "ConnectionManager"

2. **Add ConnectionManager Script**:
   - Select ConnectionManager GameObject
   - Add Component > Search for "Connection Manager"
   - Configure settings:
     - **Lobby Name**: "MyGameLobby" (or your preferred name)
     - **Max Players**: 4 (or your desired maximum)

## Step 4: Setup PeerHostedBootstrap

1. **Create Bootstrap GameObject**:
   - Right-click in Hierarchy > Create Empty
   - Name it "Bootstrap"

2. **Add PeerHostedBootstrap Script**:
   - Select Bootstrap GameObject
   - Add Component > Search for "Peer Hosted Bootstrap"

3. **Create Spawn Points** (Optional but recommended):
   - Create 4 empty GameObjects as children of Bootstrap
   - Name them: "SpawnPoint1", "SpawnPoint2", "SpawnPoint3", "SpawnPoint4"
   - Position them in a square pattern:
     - SpawnPoint1: (2, 0, 2)
     - SpawnPoint2: (-2, 0, 2)
     - SpawnPoint3: (-2, 0, -2)
     - SpawnPoint4: (2, 0, -2)

4. **Configure PeerHostedBootstrap**:
   - **Player Prefab**: (assign after creating player prefab in Step 6)
   - **Spawn Points**: Drag all SpawnPoint GameObjects into this array
   - **Spawn Radius**: 2.0

## Step 5: Setup UI

### 5.1 Create Canvas

1. **Create UI Canvas**:
   - Right-click in Hierarchy > UI > Canvas
   - Name it "LobbyCanvas"
   - In Canvas component:
     - **Render Mode**: Screen Space - Overlay
     - **Canvas Scaler**: Scale With Screen Size
     - **Reference Resolution**: 1920x1080

2. **Add Event System** (if not already present):
   - Right-click in Hierarchy > UI > Event System

### 5.2 Create Main Menu Panel

1. **Create Panel**:
   - Right-click on LobbyCanvas > UI > Panel
   - Name it "MainMenuPanel"
   - Color: Semi-transparent background (R:0, G:0, B:0, A:200)

2. **Add Title Text**:
   - Right-click on MainMenuPanel > UI > Text - TextMeshPro
   - Name: "TitleText"
   - Text: "Multiplayer Lobby"
   - Font Size: 48
   - Position: Top center
   - Alignment: Center

3. **Add Host Button**:
   - Right-click on MainMenuPanel > UI > Button - TextMeshPro
   - Name: "HostButton"
   - Position: Center, slightly above middle
   - Button Text: "Host Game"
   - Size: 300x60

4. **Add Join Button**:
   - Duplicate Host Button (Ctrl+D)
   - Name: "JoinButton"
   - Position: Below Host Button (spacing: 20px)
   - Button Text: "Join Game"

### 5.3 Create Host Panel

1. **Create Panel**:
   - Duplicate MainMenuPanel
   - Name: "HostPanel"
   - Set Active: **Unchecked** (disabled by default)

2. **Add Status Text**:
   - Right-click on HostPanel > UI > Text - TextMeshPro
   - Name: "HostStatusText"
   - Text: "Initializing host..."
   - Font Size: 24
   - Position: Upper center
   - Alignment: Center

3. **Add Lobby Code Display**:
   - Right-click on HostPanel > UI > Text - TextMeshPro
   - Name: "LobbyCodeDisplay"
   - Text: "XXXXXX"
   - Font Size: 64
   - Position: Center
   - Alignment: Center
   - Color: Bright color (e.g., yellow or green)

4. **Add Copy Button**:
   - Right-click on HostPanel > UI > Button - TextMeshPro
   - Name: "CopyCodeButton"
   - Position: Below lobby code
   - Button Text: "Copy Code"
   - Size: 200x50

5. **Add Cancel Button**:
   - Right-click on HostPanel > UI > Button - TextMeshPro
   - Name: "HostCancelButton"
   - Position: Bottom of panel
   - Button Text: "Cancel"
   - Size: 200x50

### 5.4 Create Client Panel

1. **Create Panel**:
   - Duplicate MainMenuPanel
   - Name: "ClientPanel"
   - Set Active: **Unchecked** (disabled by default)

2. **Add Status Text**:
   - Right-click on ClientPanel > UI > Text - TextMeshPro
   - Name: "ClientStatusText"
   - Text: "Enter lobby code to join"
   - Font Size: 24
   - Position: Upper center

3. **Add Lobby Code Input**:
   - Right-click on ClientPanel > UI > Input Field - TextMeshPro
   - Name: "LobbyCodeInput"
   - Position: Center
   - Placeholder Text: "Enter Lobby Code"
   - Character Limit: 6
   - Size: 400x60
   - Font Size: 36

4. **Add Connect Button**:
   - Right-click on ClientPanel > UI > Button - TextMeshPro
   - Name: "ConnectButton"
   - Position: Below input field
   - Button Text: "Connect"
   - Size: 300x60

5. **Add Cancel Button**:
   - Right-click on ClientPanel > UI > Button - TextMeshPro
   - Name: "ClientCancelButton"
   - Position: Bottom of panel
   - Button Text: "Cancel"
   - Size: 200x50

### 5.5 Create Connected Panel

1. **Create Panel**:
   - Duplicate MainMenuPanel
   - Name: "ConnectedPanel"
   - Set Active: **Unchecked** (disabled by default)

2. **Add Connection Info Text**:
   - Right-click on ConnectedPanel > UI > Text - TextMeshPro
   - Name: "ConnectionInfoText"
   - Text: "Connected"
   - Font Size: 36
   - Position: Center
   - Alignment: Center

3. **Add Disconnect Button**:
   - Right-click on ConnectedPanel > UI > Button - TextMeshPro
   - Name: "DisconnectButton"
   - Position: Bottom of panel
   - Button Text: "Disconnect"
   - Size: 300x60

### 5.6 Setup LobbyUI Component

1. **Create LobbyUI GameObject**:
   - Right-click in Hierarchy > Create Empty (as child of Canvas or separate)
   - Name: "LobbyUIManager"

2. **Add LobbyUI Script**:
   - Select LobbyUIManager
   - Add Component > Search for "Lobby UI"

3. **Assign UI References**:
   - **Main Menu Panel**: MainMenuPanel
   - **Host Panel**: HostPanel
   - **Client Panel**: ClientPanel
   - **Connected Panel**: ConnectedPanel
   - **Host Button**: HostButton from MainMenuPanel
   - **Join Button**: JoinButton from MainMenuPanel
   - **Lobby Code Display**: LobbyCodeDisplay from HostPanel
   - **Copy Code Button**: CopyCodeButton from HostPanel
   - **Host Cancel Button**: HostCancelButton from HostPanel
   - **Host Status Text**: HostStatusText from HostPanel
   - **Lobby Code Input**: LobbyCodeInput from ClientPanel
   - **Connect Button**: ConnectButton from ClientPanel
   - **Client Cancel Button**: ClientCancelButton from ClientPanel
   - **Client Status Text**: ClientStatusText from ClientPanel
   - **Connection Info Text**: ConnectionInfoText from ConnectedPanel
   - **Disconnect Button**: DisconnectButton from ConnectedPanel

## Step 6: Create Player Prefab

1. **Create Player GameObject**:
   - Right-click in Hierarchy > 3D Object > Capsule
   - Name it "NetworkPlayer"
   - Scale: (1, 1, 1)
   - Position: (0, 1, 0)

2. **Add NetworkObject Component**:
   - Select NetworkPlayer
   - Add Component > Search for "Network Object"
   - This component is essential for network synchronization

3. **Add NetworkPlayer Script**:
   - Select NetworkPlayer
   - Add Component > Search for "Network Player"
   - Configure:
     - **Move Speed**: 5
     - **Rotation Speed**: 180
     - **Mesh Renderer**: Drag the MeshRenderer from the Capsule
     - **Local Player Color**: Green
     - **Remote Player Color**: Blue

4. **Add CharacterController** (optional, script will add if missing):
   - Add Component > Character Controller
   - **Height**: 2
   - **Radius**: 0.5
   - **Center**: (0, 0, 0)

5. **Create Prefab**:
   - Create folder: `Assets/9. Multiplayer Netcode/Prefabs`
   - Drag NetworkPlayer from Hierarchy to Prefabs folder
   - Delete NetworkPlayer from Hierarchy (we only want it as a prefab)

6. **Register Player Prefab with NetworkManager**:
   - Select NetworkManager in Hierarchy
   - In NetworkManager component, expand **Network Prefabs** section
   - Click **+** to add new entry
   - Drag NetworkPlayer prefab into the new slot

7. **Assign Player Prefab to Bootstrap**:
   - Select Bootstrap GameObject
   - In PeerHostedBootstrap component:
     - **Player Prefab**: Drag NetworkPlayer prefab into this field

## Step 7: Setup Environment (Optional)

1. **Add Ground Plane**:
   - Right-click in Hierarchy > 3D Object > Plane
   - Name: "Ground"
   - Scale: (10, 1, 10)
   - Position: (0, 0, 0)

2. **Add Lighting**:
   - The default Directional Light should be sufficient
   - Adjust as needed for visibility

3. **Add Camera** (Temporary - players will have their own):
   - Create a temporary camera positioned to view the spawn area
   - This will be replaced by player cameras when they spawn
   - Position: (0, 10, -10)
   - Rotation: (45, 0, 0)

## Step 8: Configure Unity Gaming Services

1. **Link Project**:
   - Go to Edit > Project Settings > Services
   - Click "Create Unity Project ID" or link to existing project
   - Select your organization

2. **Enable Services**:
   - In Unity Dashboard (open in web browser)
   - Navigate to your project
   - Enable **Relay** service
   - Enable **Lobby** service

3. **No API Keys Required**:
   - The system uses anonymous authentication
   - No additional configuration needed

## Step 9: Test the Scene

1. **Save the Scene**: File > Save Scene (Ctrl+S)

2. **Test in Editor**:
   - Press Play
   - Click "Host Game"
   - Wait for lobby code to appear
   - Note the lobby code

3. **Test Build**:
   - Build the game: File > Build Settings > Build
   - Run one instance in Editor (as host)
   - Run another instance from the build (as client)
   - Enter the lobby code in the client instance

## Step 10: Build Settings

1. **Add Scene to Build**:
   - File > Build Settings
   - Click "Add Open Scenes" to add PeerHostedBootstrap scene
   - Ensure it's checked in the scenes list

2. **Configure Player Settings**:
   - In Build Settings, click "Player Settings"
   - Set Company Name and Product Name
   - Configure any platform-specific settings

## Troubleshooting

### "NetworkManager not found"
- Ensure NetworkManager GameObject exists in scene
- Verify NetworkManager component is attached
- Check that it's not disabled

### "UnityTransport component not found"
- Add UnityTransport component to NetworkManager GameObject
- Ensure it's referenced in NetworkManager's NetworkTransport field

### "Failed to initialize Unity Services"
- Verify project is linked to Unity Gaming Services
- Check internet connection
- Ensure Relay and Lobby services are enabled in Unity Dashboard

### UI buttons not responding
- Check EventSystem is present in scene
- Verify button onClick events are properly set up in LobbyUI script
- Ensure panels are children of Canvas

### Player doesn't spawn
- Verify player prefab has NetworkObject component
- Check player prefab is in NetworkManager's Network Prefabs list
- Ensure player prefab is assigned in PeerHostedBootstrap
- Check spawn points are correctly positioned

## Next Steps

After completing this setup:
1. Test hosting and joining functionality
2. Test with multiple clients
3. Add game-specific logic to NetworkPlayer
4. Extend ConnectionManager for your game's needs
5. Customize UI to match your game's style
6. Add error handling and user feedback
7. Implement host migration if needed
8. Add player names, customization, etc.

## Reference Screenshots

(Note: In a production environment, you would include screenshots here showing:
- Completed scene hierarchy
- NetworkManager configuration
- UI layout examples
- Inspector settings for each component)
