using Unity.Netcode;
using UnityEngine;

namespace SamTerry.Multiplayer
{
    /// <summary>
    /// Bootstrap manager for peer-hosted multiplayer sessions.
    /// Handles player spawning and basic game session management.
    /// </summary>
    public class PeerHostedBootstrap : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private float spawnRadius = 2f;

        private NetworkManager networkManager;
        private int currentSpawnIndex = 0;

        private void Awake()
        {
            networkManager = NetworkManager.Singleton;
            
            if (networkManager == null)
            {
                Debug.LogError("NetworkManager not found in scene!");
                return;
            }
        }

        private void Start()
        {
            if (networkManager != null)
            {
                networkManager.OnClientConnectedCallback += OnClientConnected;
                networkManager.OnClientDisconnectCallback += OnClientDisconnected;
            }
        }

        private void OnDestroy()
        {
            if (networkManager != null)
            {
                networkManager.OnClientConnectedCallback -= OnClientConnected;
                networkManager.OnClientDisconnectCallback -= OnClientDisconnected;
            }
        }

        private void OnClientConnected(ulong clientId)
        {
            Debug.Log($"Client connected: {clientId}");

            // Only the server/host spawns players
            if (!networkManager.IsServer) return;

            SpawnPlayerForClient(clientId);
        }

        private void OnClientDisconnected(ulong clientId)
        {
            Debug.Log($"Client disconnected: {clientId}");
        }

        private void SpawnPlayerForClient(ulong clientId)
        {
            if (playerPrefab == null)
            {
                Debug.LogError("Player prefab is not assigned!");
                return;
            }

            Vector3 spawnPosition = GetSpawnPosition();
            Quaternion spawnRotation = Quaternion.identity;

            GameObject playerInstance = Instantiate(playerPrefab, spawnPosition, spawnRotation);
            NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();

            if (networkObject == null)
            {
                Debug.LogError("Player prefab must have a NetworkObject component!");
                Destroy(playerInstance);
                return;
            }

            networkObject.SpawnAsPlayerObject(clientId);
            Debug.Log($"Spawned player for client {clientId} at {spawnPosition}");
        }

        private Vector3 GetSpawnPosition()
        {
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                Transform spawnPoint = spawnPoints[currentSpawnIndex % spawnPoints.Length];
                currentSpawnIndex++;
                
                if (spawnPoint != null)
                {
                    return spawnPoint.position;
                }
            }

            // Fallback: spawn in a circle pattern
            float angle = currentSpawnIndex * (360f / 4f);
            Vector3 offset = Quaternion.Euler(0, angle, 0) * Vector3.forward * spawnRadius;
            currentSpawnIndex++;
            
            return offset;
        }
    }
}
