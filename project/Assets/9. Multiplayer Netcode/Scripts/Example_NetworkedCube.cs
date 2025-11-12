using Unity.Netcode;
using UnityEngine;

namespace SamTerry.Multiplayer.Examples
{
    /// <summary>
    /// Example of a simple networked object that can be interacted with by players.
    /// Demonstrates NetworkVariable usage and ClientRpc/ServerRpc patterns.
    /// This is an EXAMPLE script to demonstrate how to extend the multiplayer system.
    /// </summary>
    public class Example_NetworkedCube : NetworkBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private float interactionDistance = 3f;
        [SerializeField] private Material[] colorMaterials;

        // NetworkVariable: automatically synchronized across all clients
        private NetworkVariable<int> colorIndex = new NetworkVariable<int>(
            0, 
            NetworkVariableReadPermission.Everyone, 
            NetworkVariableWritePermission.Server
        );

        private MeshRenderer meshRenderer;
        private bool isHovered = false;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            // Subscribe to changes in the network variable
            colorIndex.OnValueChanged += OnColorChanged;
            
            // Set initial color
            UpdateColor(colorIndex.Value);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            
            // Unsubscribe when despawned
            colorIndex.OnValueChanged -= OnColorChanged;
        }

        private void Update()
        {
            // Only allow local player to interact
            if (!IsOwner && NetworkManager.Singleton.LocalClient != null)
            {
                CheckForInteraction();
            }
        }

        private void CheckForInteraction()
        {
            // Find the local player
            var localPlayer = NetworkManager.Singleton.LocalClient?.PlayerObject;
            if (localPlayer == null) return;

            // Check distance to cube
            float distance = Vector3.Distance(transform.position, localPlayer.transform.position);
            isHovered = distance <= interactionDistance;

            // Interact on key press
            if (isHovered && Input.GetKeyDown(KeyCode.F))
            {
                RequestColorChange();
            }
        }

        /// <summary>
        /// Client requests the server to change the cube's color.
        /// ServerRpc can be called by clients and executes on the server.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        private void ChangeColorServerRpc(ServerRpcParams rpcParams = default)
        {
            // Cycle through colors
            int newIndex = (colorIndex.Value + 1) % colorMaterials.Length;
            colorIndex.Value = newIndex;

            // Notify all clients who changed the color
            NotifyColorChangeClientRpc(rpcParams.Receive.SenderClientId, newIndex);
        }

        /// <summary>
        /// Server notifies all clients about the color change.
        /// ClientRpc executes on all clients.
        /// </summary>
        [ClientRpc]
        private void NotifyColorChangeClientRpc(ulong changerClientId, int newColorIndex)
        {
            Debug.Log($"Client {changerClientId} changed cube color to {newColorIndex}");
        }

        private void OnColorChanged(int oldValue, int newValue)
        {
            UpdateColor(newValue);
        }

        private void UpdateColor(int index)
        {
            if (meshRenderer != null && colorMaterials != null && index < colorMaterials.Length)
            {
                meshRenderer.material = colorMaterials[index];
            }
        }

        private void RequestColorChange()
        {
            // Send request to server
            ChangeColorServerRpc();
        }

        private void OnGUI()
        {
            if (!isHovered) return;

            // Show interaction prompt
            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                normal = { textColor = Color.yellow }
            };

            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 50, 200, 30), 
                "Press F to change color", style);
        }

        private void OnDrawGizmosSelected()
        {
            // Visualize interaction distance
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionDistance);
        }
    }
}
