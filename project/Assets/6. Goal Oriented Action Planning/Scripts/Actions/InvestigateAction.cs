using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Makes the agent investigate nearby objects when idle.
    /// The agent will look around and "investigate" objects by looking at them.
    /// </summary>
    public class InvestigateAction : IAction
    {
        public string ActionType => "investigate";
        public float Cost => 0.3f; // Very low cost for idle activity
        public bool IsExecuting { get; private set; }

        private readonly float investigationRadius;
        private readonly float investigationTime;
        private readonly float rotationSpeed;
        
        private Transform agentTransform;
        private Transform targetObject;
        private float timer;
        private bool hasTarget;
        private Quaternion startRotation;
        private Quaternion targetRotation;

        public InvestigateAction(float investigationRadius = 5f, float investigationTime = 3f, float rotationSpeed = 2f)
        {
            this.investigationRadius = Mathf.Max(1f, investigationRadius);
            this.investigationTime = Mathf.Max(1f, investigationTime);
            this.rotationSpeed = Mathf.Max(0.1f, rotationSpeed);
        }

        // Injection method for agent components
        public void InjectAgent(Transform agent, UnityEngine.AI.NavMeshAgent navMeshAgent = null)
        {
            agentTransform = agent;
        }

        public bool CheckPreconditions(IAgentState agentState, IWorldState worldState)
        {
            // Can investigate when idle and there are objects around
            return agentTransform != null;
        }

        public Dictionary<string, object> GetEffects()
        {
            return new Dictionary<string, object>
            {
                { "investigating", true },
                { "idle", true }
            };
        }

        public void StartExecution(IAgentState agentState, IWorldState worldState)
        {
            IsExecuting = true;
            timer = 0f;
            hasTarget = FindObjectToInvestigate();
            
            if (hasTarget && agentTransform != null)
            {
                startRotation = agentTransform.rotation;
                Vector3 directionToTarget = (targetObject.position - agentTransform.position).normalized;
                directionToTarget.y = 0; // Keep horizontal rotation only
                targetRotation = Quaternion.LookRotation(directionToTarget);
            }
        }

        public ActionResult UpdateExecution(IAgentState agentState, IWorldState worldState, float deltaTime)
        {
            if (!IsExecuting || agentTransform == null)
                return ActionResult.Failed;

            timer += deltaTime;

            // Rotate towards target if we have one
            if (hasTarget && targetObject != null)
            {
                agentTransform.rotation = Quaternion.Slerp(
                    agentTransform.rotation, 
                    targetRotation, 
                    rotationSpeed * deltaTime
                );
            }
            else
            {
                // If no target, just look around randomly
                float randomYRotation = Mathf.Sin(Time.time * 0.5f) * 30f; // Slow head movement
                Vector3 currentEuler = agentTransform.eulerAngles;
                agentTransform.rotation = Quaternion.Euler(currentEuler.x, currentEuler.y + randomYRotation * deltaTime, currentEuler.z);
            }

            // Complete investigation after the specified time
            return timer >= investigationTime ? ActionResult.Success : ActionResult.Running;
        }

        public void CancelExecution()
        {
            IsExecuting = false;
        }

        public void ApplyEffects(IAgentState agentState, IWorldState worldState)
        {
            IsExecuting = false;
            worldState?.SetFact("investigating", false);
            worldState?.SetFact("idle", true);
        }

        public string GetDescription()
        {
            return $"Investigate nearby objects within {investigationRadius}m for {investigationTime}s";
        }

        private bool FindObjectToInvestigate()
        {
            if (agentTransform == null) return false;

            // Find all colliders within investigation radius
            Collider[] nearbyObjects = Physics.OverlapSphere(agentTransform.position, investigationRadius);
            
            // Filter for interesting objects (not the agent itself)
            List<Transform> potentialTargets = new List<Transform>();
            
            foreach (var collider in nearbyObjects)
            {
                if (collider.transform != agentTransform && 
                    collider.transform.parent != agentTransform &&
                    !collider.isTrigger)
                {
                    // Prefer objects with POIMarker or specific tags
                    if (collider.GetComponent<POIMarker>() != null ||
                        collider.CompareTag("Prop") ||
                        collider.CompareTag("Furniture") ||
                        collider.CompareTag("Interactable"))
                    {
                        potentialTargets.Add(collider.transform);
                    }
                    // Also include generic objects but with lower priority
                    else if (potentialTargets.Count < 3)
                    {
                        potentialTargets.Add(collider.transform);
                    }
                }
            }

            // Select a random target from potential objects
            if (potentialTargets.Count > 0)
            {
                targetObject = potentialTargets[Random.Range(0, potentialTargets.Count)];
                return true;
            }

            return false;
        }
    }
}