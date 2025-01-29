using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Sammoh.CrowdSystem
{
    public class CrowdAgentAi : MonoBehaviour
    {
        
        public NavMeshAgent CurrentAgent => _currentAgent;
        private NavMeshAgent _currentAgent;
        
        private float _agentPace;
        public float AgentPace => _agentPace;
        private Coroutine _currentAction;
        private Animator _animator;
        // public GameObject[] MeshList;
        public List<string> SeedValue;

        // [SerializeField] private BehaviourBase _currentBehaviour;
        // [SerializeField] private IdleState _currentIdle;
        [SerializeField] private int _step;
        private static readonly int Moving = Animator.StringToHash("Moving");
        private static readonly int State = Animator.StringToHash("IdleState");
        
        // constructor
        public CrowdAgentAi()
        {
        }
        public CrowdAgentAi(GameObject[] meshList)
        {
            // MeshList = meshList;
        }
        
        // public void Init<T>(T behaviourBase, int spawnIndex = 0) where T : BehaviourBase
        // {
        //     if (behaviourBase == null)
        //     {
        //         Debug.LogError("BehaviourBase is null");
        //         return;
        //     }
        //     
        //     // get the behaviour type from the behaviour base class type.
        //     var behaviourType = behaviourBase.GetType();
        //
        //     gameObject.name = $"{behaviourType}_{spawnIndex}";
        //
        //     _currentBehaviour = behaviourBase;
        //     // _currentBehaviour.AgentAi = this;
        //     _agentPace = Random.Range(_currentBehaviour.PaceRangeMin, _currentBehaviour.PaceRangeMax);
        //
        //     if (!TryGetComponent(out _animator))
        //     {
        //         Debug.LogError("There is no animator");
        //         return;
        //     }
        //
        //     SetIdleState(behaviourBase);
        //
        //     if (behaviourBase is SittingBehaviour) return;
        //
        //     if (!TryGetComponent(out _currentAgent))
        //     {
        //         Debug.LogError($"There is no NavMeshAgent for {name}");
        //         return;
        //     }
        //
        //     if (_currentAgent.isOnNavMesh)
        //     {
        //         // Debug.LogError($"Agent [{gameObject.name}] is READY!!");
        //         _currentAction = StartCoroutine(DoBehaviour(behaviourBase));
        //     }
        //     else
        //     {
        //         Debug.LogError($"Agent [{gameObject.name}] is not on the nav mesh");
        //     }
        // }
        
        public void OnPathComplete(CrowdAgentAi agentAi)
        {
            if (agentAi != this)
                return;
        
            IsMoving = false;
            // TODO -- Add this back in.
            // _currentAction = StartCoroutine(DoBehaviour(agentAi._currentBehaviour));
        }
        
        public void AddAgent(NavMeshAgent agent, Vector3 spawnPosition)
        {
            _currentAgent = agent;
            _currentAgent.Warp(spawnPosition);
        }
 
        // Sammoh TODO - This is the old behavior that requires static states. Move this to the new behavior system.
        // private IEnumerator DoBehaviour<T>(T behaviourBase) where T : BehaviourBase
        // {
        //     yield return new WaitForSeconds(_agentPace);
        //
        //     switch (behaviourBase)
        //     {
        //         case ForwardAndBackBehaviour fwdbk:
        //             _currentAgent.SetDestination(fwdbk.GetBehaviourDestination(_step));
        //             _animator.SetFloat("InputY", 1);
        //             _animator.SetBool(Moving, true);
        //             break;
        //         case StaticBehaviour:
        //             break;
        //         case RandomBehaviour:
        //             break;
        //         case CrowdingBehaviour crowd:
        //             SetIdleState(crowd);
        //             break;
        //         case SittingBehaviour:
        //             break;
        //         case TalkingBehaviour:
        //             _currentIdle = GetNewState(IdleState.talking_1, IdleState.talking_2);
        //             break;
        //         default:
        //             throw new ArgumentOutOfRangeException();
        //     }
        //     _step++;
        //
        //     NewIdleState = (int)_currentIdle;
        // }

        // private void SetIdleState<T>(T behaviour) where T : BehaviourBase
        // {
        //     switch (behaviour)
        //     {
        //         case ForwardAndBackBehaviour _:
        //             _currentIdle = GetNewState(IdleState.idle_1, IdleState.idle_2);
        //             break;
        //         case StaticBehaviour:
        //             break;
        //         case RandomBehaviour:
        //             break;
        //         case CrowdingBehaviour:
        //             _currentIdle = GetNewState(IdleState.idle_1, IdleState.idle_2);
        //             break;
        //         case SittingBehaviour:
        //             _currentIdle = GetNewState(IdleState.sitting_1, IdleState.sitting_2);
        //             break;
        //         case TalkingBehaviour:
        //             _currentIdle = GetNewState(IdleState.talking_1, IdleState.talking_2);
        //             break;
        //         default:
        //             throw new ArgumentOutOfRangeException();
        //     }
        //     NewIdleState = (int)_currentIdle;
        // }

        IdleState GetNewState(IdleState state1, IdleState state2)
        {
            return Random.Range(0, 100) >= 50 ? state1 : state2;
        }

        public bool IsMoving
        {
            get
            {
                if (_animator == null) return false;
                
                return _animator.GetBool(Moving);
            }
            set => _animator.SetBool(Moving, value);
        }

        private int NewIdleState
        {
            get { return _animator.GetInteger(State); }
            set { _animator.SetInteger(State, value); }
        }

        // private void OnDrawGizmos()
        // {
        //     if (_currentBehaviour == null) return;
        //
        //     if (_currentBehaviour is not ForwardAndBackBehaviour) return;
        //
        //     Debug.DrawLine(_currentAgent.destination, _currentAgent.destination + Vector3.up * 2, Color.magenta);
        // }
    }

    public enum IdleState
    {
        idle_1,
        idle_2,
        talking_1,
        talking_2,
        sitting_1,
        sitting_2
    }

    // public enum BehaviourType
    // {
    //     Static,
    //     Random,
    //     ForwardAndBack,
    //     CrowdUp,
    //     Sitting,
    //     Talking
    // }
}