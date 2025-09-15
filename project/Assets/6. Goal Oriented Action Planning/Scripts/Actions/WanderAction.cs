using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Makes agents walk around randomly when idle.
    /// Uses NavMesh pathfinding, with a basic transformation movement as a fallback.
    /// </summary>
    public class WanderAction : IAction
    {
        public string ActionType => "wander";
        public float Cost { get; private set; }
        public bool IsExecuting { get; private set; }

        private readonly float wanderRadius;
        private readonly float moveSpeed;
        private readonly float duration;
        
        private Transform agentTransform;
        private NavMeshAgent navAgent;
        private Vector3 startPosition;
        private Vector3 targetPosition;
        private float timer;
        private bool hasValidTarget;
        private bool usingNavMesh;

        public WanderAction(float cost = 0.5f, float radius = 10f, float speed = 2f, float wanderDuration = 5f)
        {
            Cost = Mathf.Max(0.01f, cost);
            wanderRadius = Mathf.Max(1f, radius);
            moveSpeed = Mathf.Max(0.1f, speed);
            duration = Mathf.Max(1f, wanderDuration);
        }

        public void InjectAgent(Transform agent, NavMeshAgent navMeshAgent = null)
        {
            agentTransform = agent;
            navAgent = navMeshAgent ?? (agent != null ? agent.GetComponent<NavMeshAgent>() : null);
        }

        public bool CheckPreconditions(IAgentState agentState, IWorldState worldState)
        {
            // Can always wander if we have an agent transform
            return agentTransform != null;
        }

        public Dictionary<string, object> GetEffects()
        {
            return new Dictionary<string, object>
            {
                { "idle", true }
            };
        }

        public void StartExecution(IAgentState agentState, IWorldState worldState)
        {
            IsExecuting = true;
            timer = 0f;
            hasValidTarget = false;
            usingNavMesh = false;
            
            if (agentTransform == null)
            {
                Debug.LogWarning("WanderAction: No agent transform available");
                return;
            }

            startPosition = agentTransform.position;
            
            // Try to find a random walkable position
            if (TryFindWanderTarget(out Vector3 target))
            {
                targetPosition = target;
                hasValidTarget = true;
                
                // Try using NavMesh first
                if (navAgent != null && navAgent.enabled && navAgent.isOnNavMesh)
                {
                    navAgent.speed = moveSpeed;
                    navAgent.SetDestination(targetPosition);
                    usingNavMesh = true;
                    Debug.Log($"WanderAction: Using NavMesh to wander to {targetPosition}");
                }
                else
                {
                    // Fallback to manual movement
                    usingNavMesh = false;
                    Debug.Log($"WanderAction: Using manual movement to wander to {targetPosition}");
                }
            }
            else
            {
                Debug.Log("WanderAction: Could not find valid wander target, will idle in place");
            }
        }

        public ActionResult UpdateExecution(IAgentState agentState, IWorldState worldState, float deltaTime)
        {
            if (!IsExecuting || agentTransform == null)
                return ActionResult.Failed;

            timer += deltaTime;

            // Check if duration has elapsed
            if (timer >= duration)
            {
                IsExecuting = false;
                return ActionResult.Success;
            }

            // If no valid target, just wait
            if (!hasValidTarget)
            {
                return ActionResult.Running;
            }

            // Handle movement
            if (usingNavMesh && navAgent != null && navAgent.enabled)
            {
                // Using NavMesh - check if we've reached the destination or path is complete
                if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
                {
                    // Reached destination, try to find a new target if time remaining
                    if (timer < duration * 0.8f) // Only look for new target if we have 20% duration left
                    {
                        if (TryFindWanderTarget(out Vector3 newTarget))
                        {
                            targetPosition = newTarget;
                            navAgent.SetDestination(targetPosition);
                        }
                    }
                }
            }
            else
            {
                // Manual movement fallback
                Vector3 direction = (targetPosition - agentTransform.position);
                float distance = direction.magnitude;
                
                if (distance > 0.5f)
                {
                    // Move towards target
                    direction.Normalize();
                    Vector3 movement = direction * moveSpeed * deltaTime;
                    agentTransform.position += movement;
                    
                    // Rotate to face movement direction
                    if (movement != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(movement);
                        agentTransform.rotation = Quaternion.Slerp(agentTransform.rotation, targetRotation, 2f * deltaTime);
                    }
                }
                else
                {
                    // Reached target, try to find a new one if time remaining
                    if (timer < duration * 0.8f)
                    {
                        if (TryFindWanderTarget(out Vector3 newTarget))
                        {
                            targetPosition = newTarget;
                        }
                    }
                }
            }

            return ActionResult.Running;
        }

        public void CancelExecution()
        {
            IsExecuting = false;
            
            // Stop NavMesh agent if it was being used
            if (navAgent != null && navAgent.enabled && usingNavMesh)
            {
                navAgent.ResetPath();
            }
        }

        public void ApplyEffects(IAgentState agentState, IWorldState worldState)
        {
            worldState.SetFact("idle", true);
        }

        public string GetDescription()
        {
            return $"Wander around randomly within {wanderRadius}m for {duration}s";
        }

        private bool TryFindWanderTarget(out Vector3 target)
        {
            target = Vector3.zero;
            
            if (agentTransform == null)
                return false;

            // Try several random positions within the wander radius
            for (int i = 0; i < 10; i++)
            {
                Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
                Vector3 randomPosition = startPosition + new Vector3(randomCircle.x, 0, randomCircle.y);
                
                // If using NavMesh, check if the position is valid
                if (navAgent != null && navAgent.enabled)
                {
                    if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                    {
                        target = hit.position;
                        return true;
                    }
                }
                else
                {
                    // For manual movement, just use the random position
                    // Could add additional validation here (e.g., raycast down to find ground)
                    target = randomPosition;
                    return true;
                }
            }
            
            return false;
        }
    }
}