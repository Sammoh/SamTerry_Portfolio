using UnityEngine;
using UnityEditor;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

namespace SamTerry.Multiplayer.Editor
{
    /// <summary>
    /// Editor utility to validate the multiplayer scene setup.
    /// Helps identify common configuration issues before testing.
    /// </summary>
    public class MultiplayerSetupValidator : EditorWindow
    {
        private Vector2 scrollPosition;
        private bool hasValidated = false;
        private System.Collections.Generic.List<ValidationResult> validationResults = 
            new System.Collections.Generic.List<ValidationResult>();

        private struct ValidationResult
        {
            public string message;
            public MessageType type;
        }

        [MenuItem("Tools/Multiplayer/Validate Setup")]
        public static void ShowWindow()
        {
            var window = GetWindow<MultiplayerSetupValidator>("Multiplayer Setup Validator");
            window.minSize = new Vector2(400, 300);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Multiplayer Setup Validation", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (GUILayout.Button("Run Validation", GUILayout.Height(30)))
            {
                ValidateSetup();
            }

            EditorGUILayout.Space();

            if (hasValidated)
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                
                foreach (var result in validationResults)
                {
                    EditorGUILayout.HelpBox(result.message, result.type);
                }
                
                EditorGUILayout.EndScrollView();
            }
        }

        private void ValidateSetup()
        {
            validationResults.Clear();
            hasValidated = true;

            AddResult("=== Starting Validation ===", MessageType.Info);

            // Check for NetworkManager
            ValidateNetworkManager();

            // Check for ConnectionManager
            ValidateConnectionManager();

            // Check for Bootstrap
            ValidateBootstrap();

            // Check for UI
            ValidateLobbyUI();

            // Check for player prefab
            ValidatePlayerPrefab();

            // Check Unity Gaming Services
            ValidateUnityServices();

            // Summary
            int errorCount = validationResults.FindAll(r => r.type == MessageType.Error).Count;
            int warningCount = validationResults.FindAll(r => r.type == MessageType.Warning).Count;

            EditorGUILayout.Space();
            if (errorCount == 0 && warningCount == 0)
            {
                AddResult("✓ All checks passed! Setup looks good.", MessageType.Info);
            }
            else
            {
                AddResult($"Validation complete: {errorCount} error(s), {warningCount} warning(s)", MessageType.Info);
            }

            Repaint();
        }

        private void ValidateNetworkManager()
        {
            AddResult("--- Checking NetworkManager ---", MessageType.None);

            var networkManager = FindObjectOfType<NetworkManager>();
            if (networkManager == null)
            {
                AddResult("✗ NetworkManager not found in scene! Please add one.", MessageType.Error);
                return;
            }

            AddResult("✓ NetworkManager found", MessageType.Info);

            // Check transport
            if (networkManager.NetworkConfig == null || networkManager.NetworkConfig.NetworkTransport == null)
            {
                AddResult("✗ NetworkTransport not configured in NetworkManager", MessageType.Error);
            }
            else
            {
                var transport = networkManager.NetworkConfig.NetworkTransport;
                if (transport is UnityTransport)
                {
                    AddResult("✓ UnityTransport configured", MessageType.Info);
                }
                else
                {
                    AddResult($"! Transport is {transport.GetType().Name}, expected UnityTransport", MessageType.Warning);
                }
            }

            // Check network prefabs
            if (networkManager.NetworkConfig == null || networkManager.NetworkConfig.Prefabs == null ||
                networkManager.NetworkConfig.Prefabs.Prefabs.Count == 0)
            {
                AddResult("! No network prefabs registered. Player prefab should be added.", MessageType.Warning);
            }
            else
            {
                AddResult($"✓ {networkManager.NetworkConfig.Prefabs.Prefabs.Count} network prefab(s) registered", MessageType.Info);
            }
        }

        private void ValidateConnectionManager()
        {
            AddResult("--- Checking ConnectionManager ---", MessageType.None);

            var connectionManager = FindObjectOfType<ConnectionManager>();
            if (connectionManager == null)
            {
                AddResult("✗ ConnectionManager not found in scene! Please add one.", MessageType.Error);
                return;
            }

            AddResult("✓ ConnectionManager found", MessageType.Info);
        }

        private void ValidateBootstrap()
        {
            AddResult("--- Checking PeerHostedBootstrap ---", MessageType.None);

            var bootstrap = FindObjectOfType<PeerHostedBootstrap>();
            if (bootstrap == null)
            {
                AddResult("! PeerHostedBootstrap not found. Add it if you want automatic player spawning.", MessageType.Warning);
                return;
            }

            AddResult("✓ PeerHostedBootstrap found", MessageType.Info);

            // Check if player prefab is assigned
            var playerPrefabField = bootstrap.GetType().GetField("playerPrefab", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (playerPrefabField != null)
            {
                var playerPrefab = playerPrefabField.GetValue(bootstrap) as GameObject;
                if (playerPrefab == null)
                {
                    AddResult("✗ Player prefab not assigned to PeerHostedBootstrap", MessageType.Error);
                }
                else
                {
                    AddResult("✓ Player prefab assigned", MessageType.Info);
                    
                    // Check if prefab has NetworkObject
                    var networkObject = playerPrefab.GetComponent<NetworkObject>();
                    if (networkObject == null)
                    {
                        AddResult("✗ Player prefab missing NetworkObject component!", MessageType.Error);
                    }
                    else
                    {
                        AddResult("✓ Player prefab has NetworkObject", MessageType.Info);
                    }
                }
            }
        }

        private void ValidateLobbyUI()
        {
            AddResult("--- Checking LobbyUI ---", MessageType.None);

            var lobbyUI = FindObjectOfType<LobbyUI>();
            if (lobbyUI == null)
            {
                AddResult("! LobbyUI not found. Add it if you want the UI system.", MessageType.Warning);
                return;
            }

            AddResult("✓ LobbyUI found", MessageType.Info);

            // Could add more detailed UI validation here
            // For now, just confirm it exists
        }

        private void ValidatePlayerPrefab()
        {
            AddResult("--- Checking Player Prefab ---", MessageType.None);

            // This checks network prefabs already registered
            var networkManager = FindObjectOfType<NetworkManager>();
            if (networkManager != null && networkManager.NetworkConfig != null && 
                networkManager.NetworkConfig.Prefabs != null)
            {
                bool foundPlayerPrefab = false;
                foreach (var prefab in networkManager.NetworkConfig.Prefabs.Prefabs)
                {
                    if (prefab != null && prefab.Prefab != null)
                    {
                        var networkPlayer = prefab.Prefab.GetComponent<NetworkPlayer>();
                        if (networkPlayer != null)
                        {
                            AddResult($"✓ Found NetworkPlayer prefab: {prefab.Prefab.name}", MessageType.Info);
                            foundPlayerPrefab = true;
                            break;
                        }
                    }
                }

                if (!foundPlayerPrefab)
                {
                    AddResult("! No NetworkPlayer prefab found in registered network prefabs", MessageType.Warning);
                }
            }
        }

        private void ValidateUnityServices()
        {
            AddResult("--- Checking Unity Gaming Services ---", MessageType.None);

            // Check if project is linked
            string projectId = UnityEditor.CloudProjectSettings.projectId;
            if (string.IsNullOrEmpty(projectId))
            {
                AddResult("✗ Project not linked to Unity Gaming Services! Go to Edit > Project Settings > Services", MessageType.Error);
                AddResult("  Without this, Relay and Lobby services won't work.", MessageType.Error);
            }
            else
            {
                AddResult($"✓ Project linked to Unity Gaming Services (ID: {projectId.Substring(0, 8)}...)", MessageType.Info);
                AddResult("  Make sure Relay and Lobby services are enabled in Unity Dashboard", MessageType.Info);
            }
        }

        private void AddResult(string message, MessageType type)
        {
            validationResults.Add(new ValidationResult { message = message, type = type });
        }
    }
}
