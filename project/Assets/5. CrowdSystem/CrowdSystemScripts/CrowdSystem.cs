using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Sammoh.CrowdSystem
{
// attach this to a gameobject and it should build a basic crowd. 
// make sure to yell at the user if they don't have a navmesh setup. 
// make sure to label and only spawn the layer that we want. 

//if doing a wait time, just yield out of any logic so that we don't have to keep processing. 

    public class CrowdSystem : MonoBehaviour
    {
        [Header("Components")] 
        [SerializeField] private Transform crowdParent;
        [SerializeField]private Transform staticWaypointParent;

        // [SerializeField] private Vector3[] _crowdWaypoints;
        [SerializeField] private SpawnLocation[] spawnLocations;
        [SerializeField] private float _agentSpeed = 1.2f;
        [SerializeField] private float _agentAngularSpeed = 200f;

        // Utilities
        private AgentFactory _agentFactory;
        private Camera _mainCamera;

        //animation instancing. 
        List<CrowdAgentAi> agentList = new List<CrowdAgentAi>();
  
        [Header("AI Behaviours")]
        [SerializeField] private CharacterDesigns[] currentCharacters;
        
        private Action<CrowdAgentAi> OnPathComplete = delegate {  };

        private void Start()
        {
            _agentFactory = new AgentFactory();
            
            SpawnAgents();
        }

        [SerializeField] private float timer;
        
        private void Update()
        {
            // this will only be called when the agent is on the navmesh.
            foreach (var agentAi in agentList)
            {
                if (agentAi.NavMeshAgent == null) 
                    continue;
                
                var agent = agentAi.NavMeshAgent;

                if (!agent.isOnNavMesh)
                    continue;
                
                if (agent.remainingDistance <= agent.stoppingDistance && agentAi.IsMoving && agent.isOnNavMesh || agent.pathStatus == NavMeshPathStatus.PathPartial && !agent.hasPath)
                {
                    OnPathComplete?.Invoke(agentAi);
                }
            }
        }
        
        private void SpawnAgents()
        {
            // spawn index is only for debugging purposes.
            var spawnIndex = 0;
            spawnLocations = staticWaypointParent.GetComponentsInChildren<SpawnLocation>();
            Debug.LogError($"Spawn Locations: {spawnLocations.Length}");

            if (currentCharacters.Length == 0)
            {
                Debug.LogError("No characters assigned to the crowd system. Please assign characters to the crowd system.");
                return;
            }

            foreach (var spawn in spawnLocations)
            {
                var randDesign = currentCharacters[Random.Range(0, currentCharacters.Length)];
                var agent = _agentFactory.CreateAgent(randDesign, spawn, crowdParent);
                
                agent.name += $"_{spawnIndex}_{spawn.name}";

                agent.OnMovementComplete += HandleAgentMovementComplete; // Subscribe to event
                agent.ExecuteBehaviour();
                
                agentList.Add(agent);

                spawnIndex++;
            }
        }
        
        private void HandleAgentMovementComplete(CrowdAgentAi agent)
        {
            // Delegate movement decision to AI Behavior
            agent.ExecuteBehaviour();
        }
    }
}