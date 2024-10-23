using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace CrowdSystem
{
    public class CrowdAgentAi : MonoBehaviour
    {
        public NavMeshAgent _myAgent;
        public float agentPace;
        private Coroutine _currentAction;
        private Animator _animator;
        private string walkingParameterString;
        public GameObject[] meshList;
        public List<string> seedValue;

        [SerializeField] private BehaviourBase _currentBehaviour;
        [SerializeField] private IdleState _currentIdle;
        [SerializeField] private int step;
        private static readonly int Moving = Animator.StringToHash("IsMoving");
        private static readonly int State = Animator.StringToHash("IdleState");

        public void Init(BehaviourBase behaviourBase)
        {
            _currentBehaviour = behaviourBase;
            _currentBehaviour.AgentAi = this;
            agentPace = Random.Range(_currentBehaviour.PaceRangeMin, _currentBehaviour.PaceRangeMax);
            if (!TryGetComponent(out _animator))
                Debug.LogError("There is no animator");

            SetIdleState(behaviourBase.Behaviour);

            if (!TryGetComponent(out _myAgent)) return;

            if (_myAgent.isOnNavMesh)
            {
                _currentAction = StartCoroutine(DoBehaviour());
            }
        }

        public void OnPathComplete(CrowdAgentAi agentAi)
        {
            if (agentAi != this)
                return;

            IsMoving = false;
            _currentAction = StartCoroutine(DoBehaviour());
        }

        private IEnumerator DoBehaviour()
        {
            yield return new WaitForSeconds(agentPace);

            switch (_currentBehaviour.Behaviour)
            {
                case BehaviourType.Static:
                case BehaviourType.Random:
                case BehaviourType.ForwardAndBack:
                case BehaviourType.CrowdUp:
                    SetIdleState(_currentBehaviour.Behaviour);
                    break;
                case BehaviourType.Sitting:
                    break;
                case BehaviourType.Talking:
                    _currentIdle = GetNewState(IdleState.talking_1, IdleState.talking_2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            step++;

            NewIdleState = (int)_currentIdle;
        }

        private void SetIdleState(BehaviourType behaviour)
        {
            switch (behaviour)
            {
                case BehaviourType.Static:
                case BehaviourType.Random:
                case BehaviourType.ForwardAndBack:
                case BehaviourType.CrowdUp:
                    _currentIdle = GetNewState(IdleState.idle_1, IdleState.idle_2);
                    break;
                case BehaviourType.Sitting:
                    _currentIdle = GetNewState(IdleState.sitting_1, IdleState.sitting_2);
                    break;
                case BehaviourType.Talking:
                    _currentIdle = GetNewState(IdleState.talking_1, IdleState.talking_2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            NewIdleState = (int)_currentIdle;
        }

        IdleState GetNewState(IdleState state1, IdleState state2)
        {
            return Random.Range(0, 100) >= 50 ? state1 : state2;
        }

        public bool IsMoving
        {
            get => _animator.GetBool(Moving);
            set => _animator.SetBool(Moving, value);
        }

        private int NewIdleState
        {
            get { return _animator.GetInteger(State); }
            set { _animator.SetInteger(State, (int)_currentIdle); }
        }

        private void OnDrawGizmos()
        {
            if (_currentBehaviour == null) return;

            if (_currentBehaviour.Behaviour == BehaviourType.Sitting) return;
            if (_currentBehaviour.Behaviour == BehaviourType.Static) return;

            Debug.DrawLine(_myAgent.destination, _myAgent.destination + Vector3.up * 2, Color.magenta);
        }
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
}