using UnityEngine;
using UnityEngine.Events;

namespace Sammoh.Four
{
    public class PlaneManager : MonoBehaviour
    {
        public static PlaneManager Instance { get; private set; }

        public UnityEvent<PlaneController>
            OnControlSwitch; // Event to notify control switch with the new player's plane name.

        public GameObject playerPlanePrefab;
        public GameObject aiPlanePrefab;
        public Transform spawnPointPlayer;
        // public Transform spawnPointAI;

        private PlaneController playerPlane;
        public PlaneController PlayerPlane => playerPlane;
        private PlaneController aiPlane;

        [SerializeField] private float distanceBetweenPlanes = 10f;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            OnControlSwitch = new UnityEvent<PlaneController>();
        }

        void Start()
        {
            SpawnPlanes();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SwitchControl();
            }
        }

        private void SpawnPlanes()
        {
            playerPlane = PlaneFactory.CreatePlane(playerPlanePrefab, spawnPointPlayer.position, Quaternion.identity,
                new PlayerControlStrategy());
            aiPlane = PlaneFactory.CreatePlane(aiPlanePrefab,
                spawnPointPlayer.position + Vector3.forward * distanceBetweenPlanes, Quaternion.identity,
                new AIControlStrategy());

            OnControlSwitch?.Invoke(playerPlane);
        }

        private void SwitchControl()
        {
            if (playerPlane != null && aiPlane != null)
            {
                playerPlane.SetControlStrategy(new AIControlStrategy());
                aiPlane.SetControlStrategy(new PlayerControlStrategy());

                // Notify listeners of the new active player plane
                OnControlSwitch.Invoke(aiPlane);

                // Swap references
                (playerPlane, aiPlane) = (aiPlane, playerPlane);
            }
        }
    }
}