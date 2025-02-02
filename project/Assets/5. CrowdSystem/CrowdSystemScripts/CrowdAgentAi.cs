using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Sammoh.CrowdSystem
{
    public class CrowdAgentAi : MonoBehaviour
    {
        [SerializeField] AgentParts agentParts;
        public AgentParts AgentParts => agentParts;
        
        public NavMeshAgent NavMeshAgent => _navAgent;
        private NavMeshAgent _navAgent;
        private AIBehavior _aiBehavior;

        private Animator _animator;

        private static readonly int Moving = Animator.StringToHash("Moving");
        private static readonly int State = Animator.StringToHash("IdleState");
        
        public event Action<CrowdAgentAi> OnMovementComplete;
        
        public bool IsMoving
        {
            get
            {
                if (_animator == null) return false;
                
                return _animator.GetBool(Moving);
            }
            set => _animator.SetBool(Moving, value);
        }
        
        public void AddBehaviour<T>(T behaviourType) where T : AIBehavior
        {
            _aiBehavior = behaviourType;
            _animator = gameObject.GetComponent<Animator>();

            var agent = gameObject.AddComponent<NavMeshAgent>();
            agent.speed = behaviourType.Speed;
            Debug.LogError($"Adding agent with speed {behaviourType.Speed}");
            agent.angularSpeed = behaviourType.AngularSpeed;
            // agent.areaMask = 1 << 4;
            agent.stoppingDistance = 0.2f;
     
            agent.Warp(transform.position);
            
            _navAgent = agent;
        }

        public void ExecuteBehaviour()
        {
            if (_aiBehavior == null) return;
            _aiBehavior.Control(this);
        }

        public void MoveTo(Vector3 destination)
        {
            if (_navAgent == null) return;
            
            Debug.LogError("Moving to destination");

            _navAgent.SetDestination(destination);
            IsMoving = true;
            StartCoroutine(CheckMovementCompletion());
        }

        private IEnumerator CheckMovementCompletion()
        {
            yield return new WaitUntil(() => _navAgent.remainingDistance <= _navAgent.stoppingDistance);

            IsMoving = false;
            var waitTime = Random.Range(0.5f, 2f);
            yield return new WaitForSeconds(waitTime);
            
            OnMovementComplete?.Invoke(this);
        }
        
    }
}