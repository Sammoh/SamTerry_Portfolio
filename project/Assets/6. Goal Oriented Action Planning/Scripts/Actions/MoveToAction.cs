using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Moves the agent to a POI that supports the current goal (SO-only).
    /// Declares/sets world-state fact: "at_<goalType>" = true on success.
    /// </summary>
    public class MoveToAction : IAction
    {
        public string ActionType => "move_to";
        public float Cost => cost;
        public bool IsExecuting { get; private set; }

        private readonly float cost;
        private readonly float stoppingDistance;
        private readonly float moveSpeed;

        private Transform agentTransform;
        private NavMeshAgent nav;
        private IGoal currentGoal;

        private Transform target;
        private bool targetLocked;

        // NavMesh flags
        private bool useNav;
        private bool warnedNavUnavailable;

        // Rotation speed for manual movement
        private float rotationSpeed = 10f;

        public MoveToAction(float cost = 1f, float stoppingDistance = 0.5f, float moveSpeed = 3.5f)
        {
            this.cost = Mathf.Max(0.01f, cost);
            this.stoppingDistance = Mathf.Max(0.01f, stoppingDistance);
            this.moveSpeed = Mathf.Max(0.01f, moveSpeed);
        }

        // ---- Injection from executor ----
        public void InjectAgent(Transform agent, NavMeshAgent navAgent = null)
        {
            agentTransform = agent;
            nav = navAgent != null ? navAgent : agent != null ? agent.GetComponent<NavMeshAgent>() : null;
        }
        public void InjectCurrentGoal(IGoal goal) => currentGoal = goal;

        public bool CheckPreconditions(IAgentState agentState, IWorldState worldState)
        {
            if (agentTransform == null || currentGoal == null) return false;
            return POIUtility.TryGetNearestPOI(currentGoal, agentTransform.position, out _);
        }

        public Dictionary<string, object> GetEffects()
        {
            if (currentGoal == null) return Empty;

            string locationFact = currentGoal.GoalType switch
            {
                "hunger" => "at_food",
                "thirst" => "at_water",
                "sleep" => "at_bed",
                "play" => "at_toy",
                _ => $"at_{currentGoal.GoalType}"
            };

            return new Dictionary<string, object> { { locationFact, true } };
        }

        public void StartExecution(IAgentState agentState, IWorldState worldState)
        {
            IsExecuting = TryResolveAndLockTarget();
            if (!IsExecuting) return;

            useNav = EnsureNavUsable();

            if (useNav)
            {
                nav.stoppingDistance = Mathf.Max(nav.stoppingDistance, stoppingDistance);
                nav.SetDestination(target.position);
            }
            else
            {
                WarnOnceNoNav();
            }
        }

        public ActionResult UpdateExecution(IAgentState agentState, IWorldState worldState, float deltaTime)
        {
            if (!IsExecuting || agentTransform == null) return ActionResult.Failed;

            if (target == null || !target.gameObject.activeInHierarchy)
            {
                if (!TryResolveAndLockTarget()) return ActionResult.Failed;
                if (useNav) nav.SetDestination(target.position);
            }

            if (useNav)
            {
                if (!EnsureNavUsable())
                {
                    useNav = false;
                    WarnOnceNoNav();
                }
                else
                {
                    if (!nav.pathPending && nav.remainingDistance <= Mathf.Max(stoppingDistance, nav.stoppingDistance))
                        return ActionResult.Success;
                    return ActionResult.Running;
                }
            }

            // ---- Manual movement + rotation fallback ----
            Vector3 to = target.position - agentTransform.position;
            float sqrDist = to.sqrMagnitude;

            if (sqrDist <= stoppingDistance * stoppingDistance)
                return ActionResult.Success;

            // Rotate smoothly toward the target
            if (to != Vector3.zero)
            {
                Quaternion desiredRot = Quaternion.LookRotation(to.normalized, Vector3.up);
                agentTransform.rotation = Quaternion.Slerp(agentTransform.rotation, desiredRot, rotationSpeed * deltaTime);
            }

            // Translate forward
            agentTransform.position += to.normalized * moveSpeed * deltaTime;

            return ActionResult.Running;
        }

        public void CancelExecution()
        {
            if (useNav && nav != null && nav.isOnNavMesh)
                nav.ResetPath();

            IsExecuting = false;
            target = null;
            targetLocked = false;
            useNav = false;
        }

        public void ApplyEffects(IAgentState agentState, IWorldState worldState)
        {
            if (currentGoal == null) return;

            string locationFact = currentGoal.GoalType switch
            {
                "hunger" => "at_food",
                "thirst" => "at_water",
                "sleep" => "at_bed",
                "play" => "at_toy",
                _ => $"at_{currentGoal.GoalType}"
            };

            worldState.SetFact(locationFact, true);
        }

        public string GetDescription()
        {
            string tgt = target ? target.name : "(none)";
            string mode = useNav ? "NavMesh" : "Direct";
            return $"MoveToAction → mode={mode} goal={currentGoal?.GoalType ?? "(none)"} target={tgt} stop={stoppingDistance}";
        }

        private bool TryResolveAndLockTarget()
        {
            if (agentTransform == null || currentGoal == null) return false;
            if (POIUtility.TryGetNearestPOI(currentGoal, agentTransform.position, out var t))
            {
                target = t; targetLocked = true; return true;
            }
            target = null; targetLocked = false; return false;
        }

        private bool EnsureNavUsable()
        {
            if (nav == null || !nav.enabled || !nav.gameObject.activeInHierarchy)
                return false;

            if (!nav.isOnNavMesh)
            {
                if (NavMesh.SamplePosition(nav.transform.position, out var hit, 2f, NavMesh.AllAreas))
                {
                    if (!nav.Warp(hit.position))
                        return false;
                }
                else
                {
                    return false;
                }
            }

            var dummyPath = new NavMeshPath();
            bool pathOk = NavMesh.CalculatePath(nav.transform.position, nav.transform.position, NavMesh.AllAreas, dummyPath);
            return pathOk;
        }

        private void WarnOnceNoNav()
        {
            if (warnedNavUnavailable) return;
            warnedNavUnavailable = true;

            string goal = currentGoal?.GoalType ?? "(none)";
            Debug.LogWarning($"[MoveToAction] NavMesh unavailable or agent off-mesh; falling back to direct Transform movement for goal '{goal}'.");
        }

        private static readonly Dictionary<string, object> Empty = new();
    }
}
