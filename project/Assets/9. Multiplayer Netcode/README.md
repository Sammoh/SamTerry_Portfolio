# Multiplayer Netcode - Peer-Hosted System

## Overview

This multiplayer system implements a **peer-hosted (client-host)** network topology using Unity Netcode for GameObjects, with support for Unity Relay and Lobby services. This allows one player to act as both host and player, while other players connect as clients through NAT traversal using Unity Relay.

## Features

- **Peer-Hosted Architecture**: One player acts as host/server while playing
- **Unity Relay Integration**: NAT traversal for connecting players across different networks
- **Unity Lobby System**: Join code-based matchmaking system
- **Easy-to-Use UI**: Simple interface for hosting and joining games
- **Network Player Controller**: Basic movement synchronization demonstration

## Network Topology

```
Host Player (Peer-Hosted)
    ├── Acts as Server
    ├── Plays as Client
    └── Unity Relay (for NAT traversal)
            ├── Client 1 connects via Lobby Code
            ├── Client 2 connects via Lobby Code
            └── Client 3 connects via Lobby Code
```

This is different from a dedicated server architecture where a separate server instance runs the game.

## Components

### ConnectionManager
Manages the entire connection lifecycle:
- Initializes Unity Gaming Services (Authentication, Relay, Lobby)
- Allocates relay servers for the host
- Creates lobbies with join codes
- Handles client connection via lobby codes
- Manages disconnection and cleanup

### LobbyUI
Provides the user interface for:
- Hosting a new game
- Joining an existing game via lobby code
- Displaying connection status
- Copying lobby codes to clipboard

### PeerHostedBootstrap
Handles game session management:
- Spawns players when they connect
- Manages spawn points
- Handles player disconnection

### NetworkPlayer
Basic networked player implementation:
- Movement synchronization
- Player identification (local vs remote)
- Visual differentiation between players

## Setup Instructions

### Prerequisites
1. Unity 6000.0.49f1 (Unity 6)
2. Unity Gaming Services configured for your project
3. Required packages (automatically added via manifest.json):
   - Unity Netcode for GameObjects (2.0.0+)
   - Unity Transport (2.3.0+)
   - Unity Services Lobby (1.2.2+)
   - Unity Services Relay (1.1.0+)

### Scene Setup

1. **Create the Bootstrap Scene**:
   - Create a new scene: `9. Multiplayer Netcode/Scenes/PeerHostedBootstrap.unity`
   - This will be your multiplayer entry point

2. **Add NetworkManager**:
   - Create empty GameObject named "NetworkManager"
   - Add `NetworkManager` component (from Unity Netcode)
   - Add `UnityTransport` component
   - In NetworkManager, set Transport to the UnityTransport component

3. **Add ConnectionManager**:
   - Create empty GameObject named "ConnectionManager"
   - Add `ConnectionManager` script
   - Configure lobby name and max players

4. **Add Bootstrap Manager**:
   - Create empty GameObject named "Bootstrap"
   - Add `PeerHostedBootstrap` script
   - Create a player prefab and assign it
   - (Optional) Create spawn point GameObjects and assign them

5. **Add UI**:
   - Create Canvas for UI
   - Create panels for: Main Menu, Host Panel, Client Panel, Connected Panel
   - Add buttons and text fields as required by LobbyUI
   - Create GameObject with `LobbyUI` script
   - Assign all UI elements in the inspector

6. **Create Player Prefab**:
   - Create a capsule or character model
   - Add `NetworkObject` component
   - Add `NetworkPlayer` script
   - Add to prefab
   - In NetworkManager, add this prefab to the "Network Prefabs" list

### Unity Gaming Services Configuration

1. **Link Your Project**:
   - Open Unity Editor
   - Go to Edit > Project Settings > Services
   - Link to a Unity Project ID or create a new one

2. **Enable Required Services**:
   - Enable Relay service in Unity Dashboard
   - Enable Lobby service in Unity Dashboard
   - No additional API keys required for anonymous authentication

### Usage

1. **As Host**:
   - Run the scene
   - Click "Host Game"
   - Wait for lobby code to generate
   - Share the lobby code with other players
   - Wait for players to join

2. **As Client**:
   - Run the scene
   - Click "Join Game"
   - Enter the lobby code from the host
   - Click "Connect"
   - Wait to join the game

## Testing

### Local Testing
- Build the game for your platform
- Run one instance as host
- Run another instance as client
- Join using the lobby code

### Network Testing
- Test with friends on different networks
- Verify relay connection works across NAT
- Test disconnection handling
- Test with maximum number of players

## Architecture Notes

### Why Peer-Hosted?
- **No dedicated server costs**: Host player provides server resources
- **Lower latency for host**: Host has zero network latency to themselves
- **Simpler deployment**: No need to maintain server infrastructure
- **Good for small player counts**: Works well for 2-8 players

### Limitations
- **Host dependency**: If host disconnects, game ends for everyone
- **Host advantage**: Host has zero latency, creating slight advantage
- **Resource requirements**: Host needs more CPU/bandwidth
- **Scalability**: Not suitable for large player counts (30+)

### When to Use Dedicated Server Instead
- Large player counts (20+ simultaneous players)
- Persistent game worlds
- Need for authoritative server logic
- Professional/competitive gameplay requiring fairness

## Future Enhancements

Potential improvements for this system:
- [ ] Host migration (transfer host role if original host disconnects)
- [ ] Lobby browser (list of available games)
- [ ] Player customization (names, colors, avatars)
- [ ] Voice chat integration (Vivox)
- [ ] Game state synchronization
- [ ] Match configuration (game mode, map selection)
- [ ] Reconnection handling
- [ ] Player kick/ban functionality

## Troubleshooting

### "Failed to initialize Unity Services"
- Ensure project is linked to Unity Gaming Services
- Check internet connection
- Verify Relay and Lobby services are enabled in Unity Dashboard

### "Failed to start host"
- Check NetworkManager is properly configured
- Ensure UnityTransport is attached and referenced
- Verify no firewall blocking Unity services

### "Failed to join game"
- Verify lobby code is correct (case-sensitive)
- Ensure host is still connected
- Check both players are using the same game version
- Verify relay service is working

### Players can't see each other
- Ensure player prefab has NetworkObject component
- Check player prefab is in NetworkManager's "Network Prefabs" list
- Verify NetworkPlayer script is properly attached

## Related Documentation

- [Unity Netcode for GameObjects Documentation](https://docs-multiplayer.unity3d.com/netcode/current/about/)
- [Unity Relay Documentation](https://docs.unity.com/relay/)
- [Unity Lobby Documentation](https://docs.unity.com/lobby/)
- [Unity Transport Documentation](https://docs.unity.com/transport/)

## Support

For issues or questions:
1. Check Unity Multiplayer forums
2. Review Unity Netcode samples
3. Contact: sameats3d@gmail.com
