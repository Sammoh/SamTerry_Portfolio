using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Sammoh.CrowdSystem
{
    // public enum BehaviourType
    // {
    //     Static = 0,
    //     Random = 1,
    //     ForwardAndBack = 2,
    //     CrowdUp = 3,
    //     Sitting = 4,
    //     Talking = 5
    // }

    public enum ColorScheme
    {
        Dark = 0,
        Midtone = 1,
        Pastel = 2
    }

    public enum PartType
    {
        Head = 0,
        Body = 1,
        Legs = 2
    }

//attach this to a gameobject and it should build a basic crowd. 
//make sure to yell at the user if they don't have a navmesh setup. 
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
        private AgentDesigner _agentDesigner;
        private Camera _mainCamera;

        //animation instancing. 
        List<CrowdAgentAi> agentList = new List<CrowdAgentAi>();
        private CrowdAgentAi _currentAgent;
  
        [Header("AI Behaviours")]
        [SerializeField] private CharacterDesigns[] currentCharacters;
        // [SerializeField] private BehaviourBase[] behaviourTypes;
        
        public Action<CrowdAgentAi> OnPathComplete = delegate {  };

        private void Start()
        {
            _agentDesigner = new AgentDesigner();
            
            SpawnAgents();
        }

        [SerializeField] private float timer;
        
        private void Update()
        {
            foreach (var agentAi in agentList)
            {
                var agent = agentAi.CurrentAgent;

                if (!agent.isOnNavMesh)
                    continue;
                
                if (agent.remainingDistance <= agent.stoppingDistance && agentAi.IsMoving && agent.isOnNavMesh || agent.pathStatus == NavMeshPathStatus.PathPartial && !agent.hasPath)
                {
                    agentAi.OnPathComplete(agentAi);
                }
            }
        }
        
        // Spawning agents should use a factory pattern

        private void SpawnAgents()
        {

            // var playerPlane = PlaneFactory.CreatePlane(playerPlanePrefab, spawnPointPlayer, Quaternion.identity,
            //     new PlayerControlStrategy());
            // _planes.Add(playerPlane);

            var spawnIndex = 0;
//            _crowdWaypoints = new Vector3 [waypointCount];
            spawnLocations = staticWaypointParent.GetComponentsInChildren<SpawnLocation>();
//            
//            for (var i = 0; i < waypointCount; i++)
//            {
//                _crowdWaypoints[i] = _meshSpawner.GenerateRandomSpawnPoint(_walkableAreaMesh);
//            }

            if (currentCharacters.Length == 0)
            {
                Debug.LogError(
                    "No characters assigned to the crowd system. Please assign characters to the crowd system.");
                return;
            }

            foreach (var spawn in spawnLocations)
            {
                var randDesign = currentCharacters[Random.Range(0, currentCharacters.Length - 1)];
                
                // creating a new behaviour for the agent that can be adjusted.
                var behaviourType = spawn.SpawnBehaviour;

                if (behaviourType == null)
                {
                    behaviourType = new AiBehavior_Idle();
                }
                
                _currentAgent = _agentDesigner.CreateAgent(randDesign, spawn.SpawnBehaviour);
                var obj = Instantiate(randDesign.instancedCharacter, crowdParent);
                obj.name = behaviourType.ToString();

                var t = spawn.transform;
                
                
                if (_currentAgent == null)
                {
                    Debug.LogError("No agent assigned to the crowd system. Please assign agents to the crowd system.");
                    return;
                }
                
                if (behaviourType is not AiBehavior_Sitting)
                {
                    var agent = obj.AddComponent<NavMeshAgent>();
                    agent.speed = _agentSpeed;
                    agent.angularSpeed = _agentAngularSpeed;
                    // agent.areaMask = 1 << 4;
                    agent.stoppingDistance = 0.2f;
                    
                    _currentAgent.AddAgent(agent, t.transform.position);
                    agentList.Add(_currentAgent);
                }
                else
                    obj.transform.position = t.transform.position;
                
                obj.transform.rotation = spawn.transform.rotation;
                
                OnPathComplete += _currentAgent.OnPathComplete;
                
                // _currentAgent.Init(newBehaviour, spawnIndex);
                // newBehaviour.InitBehaviour(_currentAgent, _crowdWaypoints);
                
                _agentDesigner.AssignRandomCharacter(_currentAgent, randDesign);
                
                spawnIndex++;
            }

            // foreach (var spawn in spawnLocations)
            // {
            //     var randDesign = currentCharacters[Random.Range(0, currentCharacters.Length - 1)];
            //     
            //     // creating a new behaviour for the agent that can be adjusted
            //     var behaviourType = spawn.spawnBehaviour.GetType();
            //     var newBehaviour = ScriptableObject.CreateInstance(behaviourType) as BehaviourBase;
            //     
            //     if (newBehaviour == null)
            //     {
            //         Debug.LogError("No behaviours assigned to the crowd system. Please assign behaviours to the crowd system.");
            //         return;
            //     }
            //     
            //     var obj = Instantiate(randDesign.instancedCharacter, crowdParent);
            //     obj.name = newBehaviour.ToString();
            //
            //     var spawnPoint = spawn;
            //     var t = spawnPoint.transform;
            //
            //     _currentAgent = obj.GetComponent<CrowdAgentAi>();
            //
            //     if (_currentAgent == null)
            //     {
            //         Debug.LogError("No agent assigned to the crowd system. Please assign agents to the crowd system.");
            //         return;
            //     }
            //
            //     if (newBehaviour is not SittingBehaviour)
            //     {
            //         var agent = obj.AddComponent<NavMeshAgent>();
            //         agent.speed = _agentSpeed;
            //         agent.angularSpeed = _agentAngularSpeed;
            //         // agent.areaMask = 1 << 4;
            //         agent.stoppingDistance = 0.2f;
            //         
            //         _currentAgent.AddAgent(agent, t.transform.position);
            //         agentList.Add(_currentAgent);
            //     }
            //     else
            //         obj.transform.position = t.transform.position;
            //     
            //     obj.transform.rotation = spawn.transform.rotation;
            //
            //     OnPathComplete += _currentAgent.OnPathComplete;
            //     
            //     _currentAgent.Init(newBehaviour, spawnIndex);
            //     newBehaviour.InitBehaviour(_currentAgent, _crowdWaypoints);
            //
            //     _agentDesigner.AssignRandomCharacter(_currentAgent, randDesign);
            //     
            //     spawnIndex++;
            // }
        }

//         private BehaviourBase CreateNewBehaviour(BehaviourType type)
//         {
//             if (behaviourTypes.Length == 0)
//             {
//                 Debug.LogError("No behaviours assigned to the crowd system. Please assign behaviours to the crowd system.");
//                 return null;
//             }
//             
//             var randBehaviour = ScriptableObject.CreateInstance<BehaviourBase>();
//             var newB = behaviourTypes.FirstOrDefault(behaviour => behaviour.Behaviour == type);
//
//             if (newB == null)
//             {
//                 Debug.LogError("No behaviours assigned to the crowd system. Please assign behaviours to the crowd system.");
//             }
//
// //            var newB = _staticWaypoints[1].SpawnBehaviour;
//             randBehaviour.Behaviour = newB.Behaviour;
//             randBehaviour.name = newB.Behaviour.ToString();
//             randBehaviour.PaceRangeMin = newB.PaceRangeMin;
//             randBehaviour. PaceRangeMax = newB.PaceRangeMax;
//
//             return randBehaviour;
//         }
        
        
    }
}