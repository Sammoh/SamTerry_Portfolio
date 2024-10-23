using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum BehaviourType
{
    Static = 0,
    Random = 1,
    ForwardAndBack = 2,
    CrowdUp = 3,
    Sitting = 4,
    Talking = 5
}

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

namespace CrowdSystem
{

    //attach this to a gameobject and it should build a basic crowd. 
    //make sure to yell at the user if they don't have a navmesh setup. 
    // make sure to label and only spawn the layer that we want. 

    //if doing a wait time, just yield out of any logic so that we don't have to keep processing. 

    public class CrowdSystem : MonoBehaviour
    {
        [Header("Components")] 
        [SerializeField] private Transform crowdParent;
        [SerializeField]private Transform staticWaypointParent;

        [SerializeField] private Vector3[] _crowdWaypoints;
        [SerializeField] private SpawnLocation[] spawnLocations;
        [SerializeField] private float _agentSpeed = 1.2f;
        [SerializeField] private float _agentAngularSpeed = 200f;

        // Utilities
        private AgentDesigner _agentDesigner;


        //animation instancing. 
        List<CrowdAgentAi> agentList = new List<CrowdAgentAi>();
        private CrowdAgentAi _currentAgent;
  
        [Header("AI Behaviours")]
        [SerializeField] private CharacterDesigns[] currentCharacters;
        [SerializeField] private BehaviourBase[] BehaviourTypes;
        
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
                var agent = agentAi._myAgent;

                if (!agent.isOnNavMesh)
                    continue;
                
                if (agent.remainingDistance <= agent.stoppingDistance && agentAi.IsMoving && agent.isOnNavMesh || agent.pathStatus == NavMeshPathStatus.PathPartial && !agent.hasPath)
                {
                    agentAi.OnPathComplete(agentAi);
                }
            }
        }
        
        private void SpawnAgents()
        {
//            _crowdWaypoints = new Vector3 [waypointCount];
            spawnLocations = staticWaypointParent.GetComponentsInChildren<SpawnLocation>();
//            
//            for (var i = 0; i < waypointCount; i++)
//            {
//                _crowdWaypoints[i] = _meshSpawner.GenerateRandomSpawnPoint(_walkableAreaMesh);
//            }

            foreach (var spawn in spawnLocations)
            {
                var randDesign = currentCharacters[Random.Range(0, currentCharacters.Length)];
                var newBehaviour = CreateNewBehaviour((int)spawn.CurrentSpawn);
                var obj = Instantiate(randDesign.instancedCharacter, crowdParent);
                obj.name = newBehaviour.Behaviour.ToString();

                var spawnPoint = spawn;
                var t = spawnPoint.transform;

                _currentAgent = obj.GetComponent<CrowdAgentAi>();

                if (newBehaviour.Behaviour != BehaviourType.Sitting)
                {
                    var agent = obj.AddComponent<NavMeshAgent>();
                    agent.speed = _agentSpeed;
                    agent.angularSpeed = _agentAngularSpeed;
                    agent.areaMask = 1 << 4;
                    agent.stoppingDistance = 0.2f;
                    
                    _currentAgent._myAgent = agent;
                    _currentAgent._myAgent.Warp(t.transform.position);
                    agentList.Add(_currentAgent);
                }
                else
                    obj.transform.position = t.transform.position;
                
                obj.transform.rotation = spawn.transform.rotation;

                OnPathComplete += _currentAgent.OnPathComplete;
                _currentAgent.Init(newBehaviour);
                newBehaviour.InitBehaviour(_crowdWaypoints, newBehaviour.Behaviour);

                _agentDesigner.AssignRandomCharacter(_currentAgent, randDesign);
                
            }
        }

        private BehaviourBase CreateNewBehaviour(int type)
        {
            var randBehaviour = ScriptableObject.CreateInstance<BehaviourBase>();

            // this just needs the same index as the list. Little janky, but I got other things to do. 
            var newB = BehaviourTypes[type];

//            var newB = _staticWaypoints[1].CurrentSpawn;
            randBehaviour.Behaviour = newB.Behaviour;
            randBehaviour.name = newB.Behaviour.ToString();
            randBehaviour.PaceRangeMin = newB.PaceRangeMin;
            randBehaviour. PaceRangeMax = newB.PaceRangeMax;

            return randBehaviour;
        }
        
        
    }
}

