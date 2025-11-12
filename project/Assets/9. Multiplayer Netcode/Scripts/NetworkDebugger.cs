using Unity.Netcode;
using UnityEngine;

namespace SamTerry.Multiplayer
{
    /// <summary>
    /// Displays debug information about the current network state.
    /// Useful for testing and troubleshooting multiplayer connections.
    /// </summary>
    public class NetworkDebugger : MonoBehaviour
    {
        [Header("Display Settings")]
        [SerializeField] private bool showDebugInfo = true;
        [SerializeField] private int fontSize = 14;
        [SerializeField] private Color textColor = Color.white;
        [SerializeField] private Color backgroundColor = new Color(0, 0, 0, 0.5f);

        private GUIStyle labelStyle;
        private GUIStyle boxStyle;
        private NetworkManager networkManager;

        private void Start()
        {
            networkManager = NetworkManager.Singleton;
        }

        private void OnGUI()
        {
            if (!showDebugInfo) return;

            InitializeStyles();
            DrawDebugPanel();
        }

        private void InitializeStyles()
        {
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = fontSize,
                    normal = { textColor = textColor }
                };
            }

            if (boxStyle == null)
            {
                boxStyle = new GUIStyle(GUI.skin.box)
                {
                    normal = { background = MakeTex(2, 2, backgroundColor) }
                };
            }
        }

        private void DrawDebugPanel()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 400), boxStyle);
            GUILayout.Label("=== NETWORK DEBUG INFO ===", labelStyle);
            GUILayout.Space(10);

            if (networkManager == null)
            {
                GUILayout.Label("NetworkManager: Not Found", labelStyle);
                GUILayout.EndArea();
                return;
            }

            // Connection State
            GUILayout.Label($"Is Running: {networkManager.IsListening}", labelStyle);
            GUILayout.Label($"Is Server: {networkManager.IsServer}", labelStyle);
            GUILayout.Label($"Is Host: {networkManager.IsHost}", labelStyle);
            GUILayout.Label($"Is Client: {networkManager.IsClient}", labelStyle);
            GUILayout.Space(10);

            // Client/Player Info
            if (networkManager.IsListening)
            {
                GUILayout.Label($"Connected Clients: {networkManager.ConnectedClients.Count}", labelStyle);
                GUILayout.Label($"Local Client ID: {networkManager.LocalClientId}", labelStyle);
                GUILayout.Space(10);

                // List all connected clients
                GUILayout.Label("Connected Players:", labelStyle);
                foreach (var client in networkManager.ConnectedClientsIds)
                {
                    string clientInfo = $"  Client {client}";
                    if (client == networkManager.LocalClientId)
                    {
                        clientInfo += " (You)";
                    }
                    if (client == 0)
                    {
                        clientInfo += " (Host)";
                    }
                    GUILayout.Label(clientInfo, labelStyle);
                }
            }
            else
            {
                GUILayout.Label("Not Connected", labelStyle);
            }

            GUILayout.Space(10);

            // Network Stats
            if (networkManager.IsListening)
            {
                GUILayout.Label("=== NETWORK STATS ===", labelStyle);
                var transport = networkManager.NetworkConfig.NetworkTransport;
                if (transport != null)
                {
                    GUILayout.Label($"Transport: {transport.GetType().Name}", labelStyle);
                }
            }

            GUILayout.EndArea();
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        /// <summary>
        /// Toggle debug info display at runtime
        /// </summary>
        public void ToggleDebugInfo()
        {
            showDebugInfo = !showDebugInfo;
        }
    }
}
