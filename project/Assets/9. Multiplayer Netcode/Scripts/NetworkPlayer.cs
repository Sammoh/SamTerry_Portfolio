using Unity.Netcode;
using UnityEngine;

namespace SamTerry.Multiplayer
{
    /// <summary>
    /// Basic networked player controller demonstrating peer-hosted multiplayer functionality.
    /// Handles movement, spawning, and basic player state synchronization.
    /// </summary>
    public class NetworkPlayer : NetworkBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 180f;

        [Header("Visual Settings")]
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Color localPlayerColor = Color.green;
        [SerializeField] private Color remotePlayerColor = Color.blue;

        private CharacterController characterController;
        private Camera playerCamera;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            if (characterController == null)
            {
                characterController = gameObject.AddComponent<CharacterController>();
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            // Set player color based on ownership
            if (meshRenderer != null)
            {
                Material mat = meshRenderer.material;
                mat.color = IsOwner ? localPlayerColor : remotePlayerColor;
            }

            // Only enable camera and input for the local player
            if (IsOwner)
            {
                SetupLocalPlayer();
            }
            else
            {
                // Disable camera for remote players
                if (playerCamera != null)
                {
                    playerCamera.gameObject.SetActive(false);
                }
            }

            Debug.Log($"Player spawned: {(IsOwner ? "Local" : "Remote")} player {OwnerClientId}");
        }

        private void SetupLocalPlayer()
        {
            // Get or create camera for local player
            playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera == null)
            {
                GameObject cameraObj = new GameObject("PlayerCamera");
                cameraObj.transform.SetParent(transform);
                cameraObj.transform.localPosition = new Vector3(0, 1.6f, -3);
                cameraObj.transform.localRotation = Quaternion.Euler(15, 0, 0);
                playerCamera = cameraObj.AddComponent<Camera>();
            }
            
            playerCamera.gameObject.SetActive(true);
        }

        private void Update()
        {
            // Only process input for the local player
            if (!IsOwner) return;

            HandleMovement();
        }

        private void HandleMovement()
        {
            // Get input
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // Calculate movement direction
            Vector3 moveDirection = transform.forward * vertical + transform.right * horizontal;
            moveDirection = moveDirection.normalized * moveSpeed;

            // Apply gravity
            moveDirection.y = -9.81f;

            // Move character
            if (characterController != null)
            {
                characterController.Move(moveDirection * Time.deltaTime);
            }

            // Handle rotation
            if (Input.GetKey(KeyCode.Q))
            {
                transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            }
        }

        private void OnGUI()
        {
            if (!IsOwner) return;

            // Display simple instructions for the local player
            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                normal = { textColor = Color.white }
            };

            GUI.Label(new Rect(10, 10, 300, 30), "WASD - Move", style);
            GUI.Label(new Rect(10, 30, 300, 30), "Q/E - Rotate", style);
            GUI.Label(new Rect(10, 50, 300, 30), $"Player ID: {OwnerClientId}", style);
        }
    }
}
