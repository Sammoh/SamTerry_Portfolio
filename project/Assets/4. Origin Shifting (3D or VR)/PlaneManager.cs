using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Sammoh.Four
{
    public class PlaneManager : MonoBehaviour
    {
        public static PlaneManager Instance { get; private set; }

        public Action<PlaneController> OnControlSwitch; // Event to notify control switch with the new player's plane name.

        public GameObject playerPlanePrefab;
        public GameObject aiPlanePrefab;

        public Transform spawnPointPlayer;

        private List<PlaneController> _planes = new List<PlaneController>();
        public List<PlaneController> Planes = new List<PlaneController>();
        public PlaneController PlayerPlane => _planes[0];

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
            
            SpawnPlanes();
        }
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SwitchControl();
            }
        }

        public void SpawnPlanes()
        {
            var playerPlane = PlaneFactory.CreatePlane(playerPlanePrefab, spawnPointPlayer, Quaternion.identity,
                new PlayerControlStrategy());
            _planes.Add(playerPlane);

            var aiPlane = PlaneFactory.CreatePlane(aiPlanePrefab,
                spawnPointPlayer, Quaternion.identity,
                new AIControlStrategy(), Vector3.forward * distanceBetweenPlanes);
            _planes.Add(aiPlane);

            OnControlSwitch?.Invoke(playerPlane);
        }

        private void SwitchControl()
        {
            if (_planes.Count > 1)
            {
                _planes[0].SetControlStrategy(new AIControlStrategy());
                _planes[1].SetControlStrategy(new PlayerControlStrategy());

                // Notify listeners of the new active player plane
                OnControlSwitch.Invoke(_planes[1]);

                // Swap references
                (_planes[0], _planes[1]) = (_planes[1], _planes[0]);
            }
        }
    }
}