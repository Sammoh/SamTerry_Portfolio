# Unity Netcode Multiplayer Patterns Reference

This document provides quick reference examples for common multiplayer networking patterns used with Unity Netcode for GameObjects in a peer-hosted architecture.

## Table of Contents
1. [Network Variables](#network-variables)
2. [Server RPCs](#server-rpcs)
3. [Client RPCs](#client-rpcs)
4. [Ownership](#ownership)
5. [Spawning Objects](#spawning-objects)
6. [Player Input Synchronization](#player-input-synchronization)
7. [Network Transform](#network-transform)
8. [Common Patterns](#common-patterns)

---

## Network Variables

Network Variables are automatically synchronized from server to all clients.

### Basic Network Variable
```csharp
using Unity.Netcode;

public class MyScript : NetworkBehaviour
{
    // Synchronized automatically
    private NetworkVariable<int> health = new NetworkVariable<int>(100);
    
    public override void OnNetworkSpawn()
    {
        // Subscribe to changes
        health.OnValueChanged += OnHealthChanged;
    }
    
    private void OnHealthChanged(int oldValue, int newValue)
    {
        Debug.Log($"Health changed from {oldValue} to {newValue}");
    }
}
```

### Network Variable with Permissions
```csharp
// Only server can write, everyone can read
private NetworkVariable<float> serverTime = new NetworkVariable<float>(
    0f,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server
);

// Only owner can write
private NetworkVariable<string> playerName = new NetworkVariable<string>(
    "",
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner
);
```

### Complex Types in Network Variables
```csharp
using Unity.Netcode;

public struct PlayerData : INetworkSerializable
{
    public int score;
    public float health;
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref score);
        serializer.SerializeValue(ref health);
    }
}

private NetworkVariable<PlayerData> playerData = new NetworkVariable<PlayerData>();
```

---

## Server RPCs

ServerRpc methods are called by clients but execute on the server.

### Basic Server RPC
```csharp
// Client calls this, server executes
[ServerRpc]
private void RequestJumpServerRpc()
{
    // Execute jump on server
    PerformJump();
}

// Client code
private void Update()
{
    if (IsOwner && Input.GetKeyDown(KeyCode.Space))
    {
        RequestJumpServerRpc();
    }
}
```

### Server RPC with Parameters
```csharp
[ServerRpc]
private void FireWeaponServerRpc(Vector3 direction, float force)
{
    // Server spawns projectile
    SpawnProjectile(direction, force);
}
```

### Server RPC Without Ownership Requirement
```csharp
// Any client can call this, even if they don't own the object
[ServerRpc(RequireOwnership = false)]
private void InteractWithObjectServerRpc(ServerRpcParams rpcParams = default)
{
    ulong senderClientId = rpcParams.Receive.SenderClientId;
    Debug.Log($"Client {senderClientId} interacted with object");
}
```

---

## Client RPCs

ClientRpc methods are called by the server and execute on all clients.

### Basic Client RPC
```csharp
[ClientRpc]
private void PlayEffectClientRpc()
{
    // Executes on all clients
    PlayParticleEffect();
}

// Server code
private void OnDestroyed()
{
    if (IsServer)
    {
        PlayEffectClientRpc();
    }
}
```

### Client RPC with Parameters
```csharp
[ClientRpc]
private void ShowDamageNumberClientRpc(int damageAmount, Vector3 position)
{
    // All clients show damage number at position
    DamageNumberUI.Show(damageAmount, position);
}
```

### Target Specific Client
```csharp
[ClientRpc]
private void SendMessageToClientRpc(string message, ClientRpcParams clientRpcParams = default)
{
    Debug.Log($"Received message: {message}");
}

// Server sends to specific client
private void SendToClient(ulong clientId, string message)
{
    ClientRpcParams clientRpcParams = new ClientRpcParams
    {
        Send = new ClientRpcSendParams
        {
            TargetClientIds = new ulong[] { clientId }
        }
    };
    
    SendMessageToClientRpc(message, clientRpcParams);
}
```

---

## Ownership

### Check Ownership
```csharp
private void Update()
{
    // Only execute for the player who owns this object
    if (!IsOwner) return;
    
    HandleInput();
}
```

### Transfer Ownership
```csharp
// Server transfers ownership
if (IsServer)
{
    NetworkObject.ChangeOwnership(newClientId);
}

// Remove ownership (return to server)
if (IsServer)
{
    NetworkObject.RemoveOwnership();
}
```

### Request Ownership
```csharp
[ServerRpc(RequireOwnership = false)]
private void RequestOwnershipServerRpc(ServerRpcParams rpcParams = default)
{
    NetworkObject.ChangeOwnership(rpcParams.Receive.SenderClientId);
}
```

---

## Spawning Objects

### Spawn Network Object (Server Only)
```csharp
if (IsServer)
{
    GameObject obj = Instantiate(prefab, position, rotation);
    NetworkObject networkObject = obj.GetComponent<NetworkObject>();
    networkObject.Spawn();
}
```

### Spawn with Ownership
```csharp
if (IsServer)
{
    GameObject obj = Instantiate(prefab, position, rotation);
    NetworkObject networkObject = obj.GetComponent<NetworkObject>();
    networkObject.SpawnWithOwnership(clientId);
}
```

### Spawn as Player Object
```csharp
if (IsServer)
{
    GameObject player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
    NetworkObject networkObject = player.GetComponent<NetworkObject>();
    networkObject.SpawnAsPlayerObject(clientId);
}
```

### Despawn Network Object
```csharp
if (IsServer)
{
    NetworkObject.Despawn();
    // Optionally destroy
    // NetworkObject.Despawn(destroy: true);
}
```

---

## Player Input Synchronization

### Pattern 1: Client Authority (Predicted)
```csharp
// Client handles input immediately (prediction)
// Server validates and corrects if needed
private void Update()
{
    if (!IsOwner) return;
    
    // Client processes input
    Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    Move(input);
    
    // Send to server for validation
    SendMovementServerRpc(input);
}

[ServerRpc]
private void SendMovementServerRpc(Vector3 input)
{
    // Server validates and potentially corrects
    if (ValidateMovement(input))
    {
        // Update server state
        Move(input);
    }
}
```

### Pattern 2: Server Authority
```csharp
// Client sends input, server processes
private void Update()
{
    if (!IsOwner) return;
    
    Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    
    if (input != Vector3.zero)
    {
        SendInputServerRpc(input);
    }
}

[ServerRpc]
private void SendInputServerRpc(Vector3 input)
{
    // Server processes movement
    transform.position += input * speed * Time.deltaTime;
}
```

---

## Network Transform

NetworkTransform automatically synchronizes position/rotation.

### Basic Setup
```csharp
// Add NetworkTransform component in Unity Editor
// Or in code:
public class MyScript : NetworkBehaviour
{
    private void Awake()
    {
        // NetworkTransform handles position sync automatically
        // Just modify transform as normal on the owner/server
    }
}
```

### Configure Sync Options
```csharp
// In Unity Editor, on NetworkTransform component:
// - Sync Position X/Y/Z
// - Sync Rotation X/Y/Z
// - Sync Scale X/Y/Z
// - Interpolate: Enable for smooth movement
// - Use Half Floats: Enable to reduce bandwidth
```

---

## Common Patterns

### Player State Management
```csharp
public class NetworkPlayerState : NetworkBehaviour
{
    private NetworkVariable<PlayerState> state = new NetworkVariable<PlayerState>();
    
    public enum PlayerState
    {
        Idle,
        Walking,
        Jumping,
        Dead
    }
    
    [ServerRpc]
    private void ChangeStateServerRpc(PlayerState newState)
    {
        state.Value = newState;
    }
    
    private void Update()
    {
        if (!IsOwner) return;
        
        // Client requests state change
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeStateServerRpc(PlayerState.Jumping);
        }
    }
}
```

### Collectible Items
```csharp
public class Collectible : NetworkBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    private void CollectServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong collectorId = rpcParams.Receive.SenderClientId;
        
        // Award to player
        AwardToPlayer(collectorId);
        
        // Remove from world
        NetworkObject.Despawn(true);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<NetworkPlayer>();
            if (player != null && player.IsOwner)
            {
                CollectServerRpc();
            }
        }
    }
}
```

### Synchronized Timer
```csharp
public class GameTimer : NetworkBehaviour
{
    private NetworkVariable<float> timeRemaining = new NetworkVariable<float>(60f);
    
    private void Update()
    {
        // Only server updates time
        if (!IsServer) return;
        
        timeRemaining.Value -= Time.deltaTime;
        
        if (timeRemaining.Value <= 0)
        {
            EndGameClientRpc();
        }
    }
    
    [ClientRpc]
    private void EndGameClientRpc()
    {
        Debug.Log("Game Over!");
    }
}
```

### Network Event System
```csharp
public class NetworkEventManager : NetworkBehaviour
{
    [ClientRpc]
    private void BroadcastEventClientRpc(string eventName)
    {
        // All clients process event
        ProcessEvent(eventName);
    }
    
    public void TriggerNetworkEvent(string eventName)
    {
        if (IsServer)
        {
            BroadcastEventClientRpc(eventName);
        }
    }
}
```

---

## Best Practices

### 1. Minimize Network Traffic
- Use NetworkVariables for data that changes frequently
- Use RPCs for one-time events
- Send only necessary data
- Use compression where possible

### 2. Server Authority
- Important game logic should execute on server
- Clients should request actions, not perform them directly
- Server validates all client requests

### 3. Client Prediction
- Allow clients to predict movement for responsiveness
- Server corrects prediction errors when needed
- Use NetworkTransform interpolation

### 4. Error Handling
```csharp
[ServerRpc]
private void ProcessActionServerRpc(ServerRpcParams rpcParams = default)
{
    ulong senderId = rpcParams.Receive.SenderClientId;
    
    // Validate sender
    if (!NetworkManager.ConnectedClients.ContainsKey(senderId))
    {
        Debug.LogError($"Invalid sender: {senderId}");
        return;
    }
    
    // Process action
}
```

### 5. Connection Callbacks
```csharp
public override void OnNetworkSpawn()
{
    if (IsServer)
    {
        NetworkManager.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.OnClientDisconnectCallback += OnClientDisconnected;
    }
}

public override void OnNetworkDespawn()
{
    if (IsServer)
    {
        NetworkManager.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.OnClientDisconnectCallback -= OnClientDisconnected;
    }
}
```

---

## Performance Tips

1. **Batch RPCs**: Send multiple pieces of data in one RPC call
2. **Use NetworkVariable Sparingly**: For frequently changing data, consider sending updates less often
3. **Compress Data**: Use smaller data types when possible (byte instead of int)
4. **Limit RPC Frequency**: Add cooldowns or rate limiting
5. **Clean Up**: Always unsubscribe from events in OnNetworkDespawn

---

## Testing Tips

1. **Use NetworkDebugger**: Add the NetworkDebugger component to see connection state
2. **Test Locally**: Build and run multiple instances on one machine
3. **Simulate Lag**: Use Network Simulator tools to test with latency
4. **Test Edge Cases**: Connection loss, host migration, rapid connects/disconnects
5. **Verify Synchronization**: Ensure all clients see the same game state

---

## Additional Resources

- [Unity Netcode Documentation](https://docs-multiplayer.unity3d.com/netcode/current/about/)
- [Netcode API Reference](https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@latest)
- [Unity Multiplayer Samples](https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop)
- [Netcode Community Contributions](https://github.com/Unity-Technologies/multiplayer-community-contributions)
