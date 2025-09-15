using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Makes the agent wander around randomly when idle.
    /// Uses NavMesh to find random walkable positions around the agent.
    /// </summary>
    public class WanderAction : IAction
    {
        public string ActionType => "wander";
        public float Cost => 0.5f; // Low cost for idle activity
        public bool IsExecuting { get; private set; }

        private readonly float wanderRadius;
        private readonly float moveSpeed;
        private readonly float duration;
        
        private Transform agentTransform;
        private NavMeshAgent navAgent;
        private Vector3 targetPosition;
        private float timer;
        private bool hasValidTarget;

        public WanderAction(float wanderRadius = 10f, float moveSpeed = 2f, float duration = 5f)
        {
            this.wanderRadius = Mathf.Max(1f, wanderRadius);
            this.moveSpeed = Mathf.Max(0.1f, moveSpeed);
            this.duration = Mathf.Max(1f, duration);
        }

        // Injection methods for agent components
        public void InjectAgent(Transform agent, NavMeshAgent navMeshAgent = null)
        {
            agentTransform = agent;
            navAgent = navMeshAgent ?? agent?.GetComponent<NavMeshAgent>();
        }

        public bool CheckPreconditions(IAgentState agentState, IWorldState worldState)
        {
            // Can wander when idle (no specific location requirements)
            return agentTransform != null;
        }

        public Dictionary<string, object> GetEffects()
        {
            return new Dictionary<string, object>
            {
                { "wandering", true },
                { "idle", true }
            };
        }

        public void StartExecution(IAgentState agentState, IWorldState worldState)
        {
            IsExecuting = true;
            timer = 0f;
            hasValidTarget = FindRandomWanderTarget();
            
            if (hasValidTarget && navAgent != null && navAgent.isOnNavMesh)
            {
                navAgent.SetDestination(targetPosition);
                navAgent.speed = moveSpeed;
            }
        }

        public ActionResult UpdateExecution(IAgentState agentState, IWorldState worldState, float deltaTime)
        {
            if (!IsExecuting || agentTransform == null)
                return ActionResult.Failed;

            timer += deltaTime;

            // If we have NavMesh agent and valid target
            if (hasValidTarget && navAgent != null && navAgent.isOnNavMesh)
            {
                // Check if we've reached the destination or wandered long enough
                if (!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
                {
                    // Find a new target if we still have time
                    if (timer < duration)
                    {
                        if (FindRandomWanderTarget())
                        {
                            navAgent.SetDestination(targetPosition);
                            return ActionResult.Running;
                        }
                    }
                    return ActionResult.Success;
                }
                
                // Continue wandering if not at destination and within time limit
                if (timer < duration)
                    return ActionResult.Running;
                else
                    return ActionResult.Success;
            }
            else
            {
                // Fallback: just wait for the duration (stationary wandering)
                return timer >= duration ? ActionResult.Success : ActionResult.Running;
            }
        }

        public void CancelExecution()
        {
            IsExecuting = false;
            if (navAgent != null && navAgent.isOnNavMesh)
                navAgent.ResetPath();
        }

        public void ApplyEffects(IAgentState agentState, IWorldState worldState)
        {
            IsExecuting = false;
            worldState?.SetFact("wandering", false);
            worldState?.SetFact("idle", true);
            
            if (navAgent != null && navAgent.isOnNavMesh)
                navAgent.ResetPath();
        }

        public string GetDescription()
        {
            return $"Wander around randomly within {wanderRadius}m radius for up to {duration}s";
        }

        private bool FindRandomWanderTarget()
        {
            if (agentTransform == null) return false;

            // Try multiple times to find a valid NavMesh position
            for (int i = 0; i < 10; i++)
            {
                Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
                randomDirection += agentTransform.position;
                randomDirection.y = agentTransform.position.y; // Keep same height

                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
                {
                    targetPosition = hit.position;
                    return true;
                }
            }

            // If no valid NavMesh position found, stay in place
            targetPosition = agentTransform.position;
            return false;
        }
    }
}