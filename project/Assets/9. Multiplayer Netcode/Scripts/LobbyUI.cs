using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SamTerry.Multiplayer
{
    /// <summary>
    /// UI controller for the peer-hosted multiplayer lobby system.
    /// Provides interface for hosting games and joining via lobby code.
    /// </summary>
    public class LobbyUI : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject hostPanel;
        [SerializeField] private GameObject clientPanel;
        [SerializeField] private GameObject connectedPanel;

        [Header("Main Menu")]
        [SerializeField] private Button hostButton;
        [SerializeField] private Button joinButton;

        [Header("Host Panel")]
        [SerializeField] private TMP_Text lobbyCodeDisplay;
        [SerializeField] private Button copyCodeButton;
        [SerializeField] private Button hostCancelButton;
        [SerializeField] private TMP_Text hostStatusText;

        [Header("Client Panel")]
        [SerializeField] private TMP_InputField lobbyCodeInput;
        [SerializeField] private Button connectButton;
        [SerializeField] private Button clientCancelButton;
        [SerializeField] private TMP_Text clientStatusText;

        [Header("Connected Panel")]
        [SerializeField] private TMP_Text connectionInfoText;
        [SerializeField] private Button disconnectButton;

        private ConnectionManager connectionManager;
        private bool isConnecting;

        private void Start()
        {
            connectionManager = ConnectionManager.Instance;
            
            if (connectionManager == null)
            {
                Debug.LogError("ConnectionManager not found! Make sure it exists in the scene.");
                return;
            }

            SetupEventListeners();
            ShowMainMenu();
        }

        private void SetupEventListeners()
        {
            // Main menu buttons
            hostButton.onClick.AddListener(OnHostButtonClicked);
            joinButton.onClick.AddListener(OnJoinButtonClicked);

            // Host panel buttons
            copyCodeButton.onClick.AddListener(OnCopyCodeButtonClicked);
            hostCancelButton.onClick.AddListener(OnCancelButtonClicked);

            // Client panel buttons
            connectButton.onClick.AddListener(OnConnectButtonClicked);
            clientCancelButton.onClick.AddListener(OnCancelButtonClicked);

            // Connected panel buttons
            disconnectButton.onClick.AddListener(OnDisconnectButtonClicked);

            // Connection manager events
            connectionManager.OnLobbyCodeGenerated += OnLobbyCodeGenerated;
            connectionManager.OnConnectionStatusChanged += OnConnectionStatusChanged;
            connectionManager.OnErrorOccurred += OnErrorOccurred;
        }

        private void OnDestroy()
        {
            if (connectionManager != null)
            {
                connectionManager.OnLobbyCodeGenerated -= OnLobbyCodeGenerated;
                connectionManager.OnConnectionStatusChanged -= OnConnectionStatusChanged;
                connectionManager.OnErrorOccurred -= OnErrorOccurred;
            }
        }

        private void ShowMainMenu()
        {
            SetPanelActive(mainMenuPanel, true);
            SetPanelActive(hostPanel, false);
            SetPanelActive(clientPanel, false);
            SetPanelActive(connectedPanel, false);
        }

        private void ShowHostPanel()
        {
            SetPanelActive(mainMenuPanel, false);
            SetPanelActive(hostPanel, true);
            SetPanelActive(clientPanel, false);
            SetPanelActive(connectedPanel, false);
            
            hostStatusText.text = "Initializing host...";
            lobbyCodeDisplay.text = "Generating code...";
        }

        private void ShowClientPanel()
        {
            SetPanelActive(mainMenuPanel, false);
            SetPanelActive(hostPanel, false);
            SetPanelActive(clientPanel, true);
            SetPanelActive(connectedPanel, false);
            
            clientStatusText.text = "Enter lobby code to join";
            lobbyCodeInput.text = "";
        }

        private void ShowConnectedPanel(bool isHost)
        {
            SetPanelActive(mainMenuPanel, false);
            SetPanelActive(hostPanel, false);
            SetPanelActive(clientPanel, false);
            SetPanelActive(connectedPanel, true);
            
            connectionInfoText.text = isHost ? "Connected as Host" : "Connected as Client";
        }

        private void SetPanelActive(GameObject panel, bool active)
        {
            if (panel != null)
            {
                panel.SetActive(active);
            }
        }

        private async void OnHostButtonClicked()
        {
            if (isConnecting) return;
            
            isConnecting = true;
            ShowHostPanel();
            
            string lobbyCode = await connectionManager.StartHostWithRelay();
            
            isConnecting = false;
            
            if (string.IsNullOrEmpty(lobbyCode))
            {
                hostStatusText.text = "Failed to start host. Check console for details.";
            }
        }

        private void OnJoinButtonClicked()
        {
            ShowClientPanel();
        }

        private async void OnConnectButtonClicked()
        {
            if (isConnecting) return;
            
            string lobbyCode = lobbyCodeInput.text.Trim().ToUpper();
            
            if (string.IsNullOrEmpty(lobbyCode))
            {
                clientStatusText.text = "Please enter a lobby code";
                return;
            }

            isConnecting = true;
            clientStatusText.text = "Connecting...";
            connectButton.interactable = false;
            
            bool success = await connectionManager.JoinGameWithLobbyCode(lobbyCode);
            
            isConnecting = false;
            connectButton.interactable = true;
            
            if (!success)
            {
                clientStatusText.text = "Failed to connect. Please check the code and try again.";
            }
        }

        private void OnCancelButtonClicked()
        {
            if (isConnecting) return;
            ShowMainMenu();
        }

        private void OnDisconnectButtonClicked()
        {
            connectionManager.Disconnect();
            ShowMainMenu();
        }

        private void OnCopyCodeButtonClicked()
        {
            if (!string.IsNullOrEmpty(lobbyCodeDisplay.text) && 
                lobbyCodeDisplay.text != "Generating code...")
            {
                GUIUtility.systemCopyBuffer = lobbyCodeDisplay.text;
                hostStatusText.text = "Lobby code copied to clipboard!";
            }
        }

        private void OnLobbyCodeGenerated(string lobbyCode)
        {
            lobbyCodeDisplay.text = lobbyCode;
            hostStatusText.text = "Waiting for players to join...";
        }

        private void OnConnectionStatusChanged(bool isConnected)
        {
            if (isConnected)
            {
                bool isHost = Unity.Netcode.NetworkManager.Singleton.IsHost;
                ShowConnectedPanel(isHost);
            }
            else
            {
                ShowMainMenu();
            }
        }

        private void OnErrorOccurred(string errorMessage)
        {
            Debug.LogError($"Connection error: {errorMessage}");
            
            if (hostPanel.activeSelf)
            {
                hostStatusText.text = $"Error: {errorMessage}";
            }
            else if (clientPanel.activeSelf)
            {
                clientStatusText.text = $"Error: {errorMessage}";
            }
        }
    }
}
