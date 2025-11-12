using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobby;
using Unity.Services.Lobby.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace SamTerry.Multiplayer
{
    /// <summary>
    /// Manages network connections for peer-hosted multiplayer using Unity Relay and Lobby services.
    /// Handles lobby creation, joining, and relay allocation for NAT traversal.
    /// </summary>
    public class ConnectionManager : MonoBehaviour
    {
        [Header("Lobby Settings")]
        [SerializeField] private string lobbyName = "MyLobby";
        [SerializeField] private int maxPlayers = 4;
        
        private Lobby currentLobby;
        private UnityTransport transport;
        private const float LobbyHeartbeatInterval = 15f;
        private float lobbyHeartbeatTimer;

        public static ConnectionManager Instance { get; private set; }
        
        public event Action<string> OnLobbyCodeGenerated;
        public event Action<bool> OnConnectionStatusChanged;
        public event Action<string> OnErrorOccurred;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            transport = FindObjectOfType<UnityTransport>();
            if (transport == null)
            {
                Debug.LogError("UnityTransport component not found in scene!");
            }
        }

        private void Update()
        {
            HandleLobbyHeartbeat();
        }

        /// <summary>
        /// Initializes Unity Gaming Services. Must be called before using lobby/relay features.
        /// </summary>
        public async Task<bool> InitializeUnityServices()
        {
            try
            {
                if (UnityServices.State == ServicesInitializationState.Initialized)
                {
                    return true;
                }

                await UnityServices.InitializeAsync();
                
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
                
                Debug.Log("Unity Services initialized successfully");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to initialize Unity Services: {e.Message}");
                OnErrorOccurred?.Invoke($"Service initialization failed: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Starts the game as a host using Unity Relay for NAT traversal.
        /// Creates a lobby and generates a join code for other players.
        /// </summary>
        public async Task<string> StartHostWithRelay()
        {
            try
            {
                // Initialize services if not already done
                if (!await InitializeUnityServices())
                {
                    return null;
                }

                // Allocate relay
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
                
                // Get join code for relay
                string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                
                // Configure transport with relay
                transport.SetHostRelayData(
                    allocation.RelayServer.IpV4,
                    (ushort)allocation.RelayServer.Port,
                    allocation.AllocationIdBytes,
                    allocation.Key,
                    allocation.ConnectionData
                );

                // Start host
                if (!NetworkManager.Singleton.StartHost())
                {
                    Debug.LogError("Failed to start host");
                    OnErrorOccurred?.Invoke("Failed to start as host");
                    return null;
                }

                // Create lobby
                await CreateLobby(relayJoinCode);
                
                Debug.Log($"Host started successfully. Lobby Code: {currentLobby?.LobbyCode}");
                OnConnectionStatusChanged?.Invoke(true);
                
                return currentLobby?.LobbyCode;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to start host with relay: {e.Message}");
                OnErrorOccurred?.Invoke($"Failed to start host: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Joins a game as a client using the provided lobby code.
        /// </summary>
        public async Task<bool> JoinGameWithLobbyCode(string lobbyCode)
        {
            try
            {
                // Initialize services if not already done
                if (!await InitializeUnityServices())
                {
                    return false;
                }

                // Join lobby by code
                currentLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode);
                
                // Get relay join code from lobby data
                string relayJoinCode = currentLobby.Data["RelayJoinCode"].Value;
                
                // Join relay allocation
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
                
                // Configure transport with relay
                transport.SetClientRelayData(
                    joinAllocation.RelayServer.IpV4,
                    (ushort)joinAllocation.RelayServer.Port,
                    joinAllocation.AllocationIdBytes,
                    joinAllocation.Key,
                    joinAllocation.ConnectionData,
                    joinAllocation.HostConnectionData
                );

                // Start client
                if (!NetworkManager.Singleton.StartClient())
                {
                    Debug.LogError("Failed to start client");
                    OnErrorOccurred?.Invoke("Failed to connect to game");
                    return false;
                }

                Debug.Log($"Successfully joined game with lobby code: {lobbyCode}");
                OnConnectionStatusChanged?.Invoke(true);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to join game: {e.Message}");
                OnErrorOccurred?.Invoke($"Failed to join game: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Disconnects from the current game session and cleans up lobby.
        /// </summary>
        public async void Disconnect()
        {
            try
            {
                if (NetworkManager.Singleton != null)
                {
                    NetworkManager.Singleton.Shutdown();
                }

                if (currentLobby != null)
                {
                    if (NetworkManager.Singleton.IsHost)
                    {
                        await Lobbies.Instance.DeleteLobbyAsync(currentLobby.Id);
                    }
                    else
                    {
                        await Lobbies.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);
                    }
                    
                    currentLobby = null;
                }
                
                OnConnectionStatusChanged?.Invoke(false);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error during disconnect: {e.Message}");
            }
        }

        private async Task CreateLobby(string relayJoinCode)
        {
            try
            {
                CreateLobbyOptions options = new CreateLobbyOptions
                {
                    IsPrivate = false,
                    Data = new System.Collections.Generic.Dictionary<string, DataObject>
                    {
                        { "RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                    }
                };

                currentLobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
                
                Debug.Log($"Lobby created with code: {currentLobby.LobbyCode}");
                OnLobbyCodeGenerated?.Invoke(currentLobby.LobbyCode);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create lobby: {e.Message}");
                throw;
            }
        }

        private void HandleLobbyHeartbeat()
        {
            if (currentLobby == null || !NetworkManager.Singleton.IsHost)
            {
                return;
            }

            lobbyHeartbeatTimer += Time.deltaTime;
            if (lobbyHeartbeatTimer >= LobbyHeartbeatInterval)
            {
                lobbyHeartbeatTimer = 0f;
                SendLobbyHeartbeat();
            }
        }

        private async void SendLobbyHeartbeat()
        {
            try
            {
                await Lobbies.Instance.SendHeartbeatPingAsync(currentLobby.Id);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to send lobby heartbeat: {e.Message}");
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Disconnect();
            }
        }
    }
}
