using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Makes agents look around and investigate nearby objects.
    /// Scans for interesting objects within configurable radius, prioritizes POIMarker components 
    /// and specific tags, but mostly walking around aimlessly (30/70 split).
    /// </summary>
    public class InvestigateAction : IAction
    {
        public string ActionType => "investigate";
        public float Cost { get; private set; }
        public bool IsExecuting { get; private set; }

        private readonly float investigateRadius;
        private readonly float rotationSpeed;
        private readonly float duration;
        private readonly string[] interestingTags = { "Prop", "Furniture", "Interactable" };
        
        private Transform agentTransform;
        private Transform currentTarget;
        private float timer;
        private float lookTimer;
        private bool hasTarget;
        private Quaternion originalRotation;
        private Quaternion targetRotation;
        private bool isRotating;

        public InvestigateAction(float cost = 0.3f, float radius = 5f, float investigateDuration = 5f, float rotSpeed = 45f)
        {
            Cost = Mathf.Max(0.01f, cost);
            investigateRadius = Mathf.Max(1f, radius);
            duration = Mathf.Max(1f, investigateDuration);
            rotationSpeed = Mathf.Max(1f, rotSpeed);
        }

        public void InjectAgent(Transform agent)
        {
            agentTransform = agent;
        }

        public bool CheckPreconditions(IAgentState agentState, IWorldState worldState)
        {
            // Can always investigate if we have an agent transform
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
            lookTimer = 0f;
            hasTarget = false;
            isRotating = false;
            
            if (agentTransform == null)
            {
                Debug.LogWarning("InvestigateAction: No agent transform available");
                return;
            }

            originalRotation = agentTransform.rotation;
            
            // 30% chance to look for interesting objects, 70% chance to just look around aimlessly
            if (Random.value < 0.3f)
            {
                FindInterestingTarget();
            }
            else
            {
                StartAimlessLooking();
            }
            
            Debug.Log($"InvestigateAction: Started investigating for {duration}s");
        }

        public ActionResult UpdateExecution(IAgentState agentState, IWorldState worldState, float deltaTime)
        {
            if (!IsExecuting || agentTransform == null)
                return ActionResult.Failed;

            timer += deltaTime;
            lookTimer += deltaTime;

            // Check if duration has elapsed
            if (timer >= duration)
            {
                IsExecuting = false;
                return ActionResult.Success;
            }

            // Handle rotation behavior
            if (isRotating)
            {
                // Smoothly rotate towards target
                agentTransform.rotation = Quaternion.RotateTowards(
                    agentTransform.rotation, 
                    targetRotation, 
                    rotationSpeed * deltaTime
                );

                // Check if rotation is complete
                if (Quaternion.Angle(agentTransform.rotation, targetRotation) < 1f)
                {
                    isRotating = false;
                    lookTimer = 0f;
                }
            }
            else
            {
                // Look at target for a bit, then find a new one
                if (lookTimer >= Random.Range(1f, 3f))
                {
                    if (hasTarget && currentTarget != null)
                    {
                        // We were looking at a specific target, now look around aimlessly
                        StartAimlessLooking();
                    }
                    else
                    {
                        // We were looking aimlessly, try to find a target or continue aimless looking
                        if (Random.value < 0.4f) // 40% chance to look for target
                        {
                            FindInterestingTarget();
                        }
                        else
                        {
                            StartAimlessLooking();
                        }
                    }
                }
            }

            return ActionResult.Running;
        }

        public void CancelExecution()
        {
            IsExecuting = false;
        }

        public void ApplyEffects(IAgentState agentState, IWorldState worldState)
        {
            worldState.SetFact("idle", true);
        }

        public string GetDescription()
        {
            return $"Investigate nearby objects within {investigateRadius}m for {duration}s";
        }

        private void FindInterestingTarget()
        {
            if (agentTransform == null)
                return;

            Transform bestTarget = null;
            float closestDistance = float.MaxValue;

            // First priority: Look for POIMarker components
            var poiMarkers = Object.FindObjectsOfType<POIMarker>();
            foreach (var poi in poiMarkers)
            {
                if (poi == null || !poi.isActiveAndEnabled) continue;
                
                float distance = Vector3.Distance(agentTransform.position, poi.transform.position);
                if (distance <= investigateRadius && distance < closestDistance)
                {
                    closestDistance = distance;
                    bestTarget = poi.transform;
                }
            }

            // Second priority: Look for objects with interesting tags
            if (bestTarget == null)
            {
                foreach (string tag in interestingTags)
                {
                    var taggedObjects = GameObject.FindGameObjectsWithTag(tag);
                    foreach (var obj in taggedObjects)
                    {
                        if (obj == null) continue;
                        
                        float distance = Vector3.Distance(agentTransform.position, obj.transform.position);
                        if (distance <= investigateRadius && distance < closestDistance)
                        {
                            closestDistance = distance;
                            bestTarget = obj.transform;
                        }
                    }
                }
            }

            if (bestTarget != null)
            {
                LookAtTarget(bestTarget);
                Debug.Log($"InvestigateAction: Found interesting target '{bestTarget.name}' at distance {closestDistance:F1}m");
            }
            else
            {
                // No interesting targets found, look around aimlessly
                StartAimlessLooking();
            }
        }

        private void LookAtTarget(Transform target)
        {
            currentTarget = target;
            hasTarget = true;
            
            Vector3 directionToTarget = (target.position - agentTransform.position).normalized;
            // Only rotate horizontally (Y-axis)
            directionToTarget.y = 0;
            
            if (directionToTarget != Vector3.zero)
            {
                targetRotation = Quaternion.LookRotation(directionToTarget);
                isRotating = true;
                lookTimer = 0f;
            }
        }

        private void StartAimlessLooking()
        {
            currentTarget = null;
            hasTarget = false;
            
            // Generate a random direction to look towards
            float randomAngle = Random.Range(0f, 360f);
            Vector3 randomDirection = Quaternion.AngleAxis(randomAngle, Vector3.up) * Vector3.forward;
            
            targetRotation = Quaternion.LookRotation(randomDirection);
            isRotating = true;
            lookTimer = 0f;
            
            Debug.Log("InvestigateAction: Looking around aimlessly");
        }
    }
}