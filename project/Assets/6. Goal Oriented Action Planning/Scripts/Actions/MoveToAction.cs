using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Moves the agent to a POI that supports the current goal (SO-based only).
    /// </summary>
    public class MoveToAction : IAction
    {
        // ===================== CONFIG =====================
        private readonly float stoppingDistance;   // World units
        private readonly float moveSpeed;          // Used when not using NavMeshAgent
        private readonly float cost;

        // ===================== RUNTIME =====================
        private Transform agentTransform;
        private NavMeshAgent nav;
        private IGoal currentGoal;

        private Transform target;
        private bool targetLocked;

        public bool IsExecuting { get; private set; }

        // ===================== CTOR =====================
        public MoveToAction(float cost = 1f, float stoppingDistance = 0.5f, float moveSpeed = 3.5f)
        {
            this.cost = Mathf.Max(0.01f, cost);
            this.stoppingDistance = Mathf.Max(0.01f, stoppingDistance);
            this.moveSpeed = Mathf.Max(0.01f, moveSpeed);
        }

        // ===================== INJECTION HOOKS =====================
        public void InjectAgent(Transform agent, NavMeshAgent navAgent = null)
        {
            agentTransform = agent;
            nav = navAgent;
        }

        public void InjectCurrentGoal(IGoal goal)
        {
            currentGoal = goal;
        }

        // ===================== IAction =====================
        public string ActionType => "move_to";
        public float Cost => cost;

        public bool CheckPreconditions(IAgentState agentState, IWorldState worldState)
        {
            if (agentTransform == null || currentGoal == null) return false;
            return POIUtility.TryGetNearestPOI(currentGoal, agentTransform.position, out _);
        }

        public Dictionary<string, object> GetEffects()
        {
            // Pure movement has no direct effects — effects are applied by subsequent actions (e.g., Eat, Sleep).
            return EmptyDict;
        }

        public void StartExecution(IAgentState agentState, IWorldState worldState)
        {
            IsExecuting = true;
            targetLocked = TryResolveAndLockTarget();

            if (!targetLocked)
            {
                IsExecuting = false;
                return;
            }

            if (nav != null)
            {
                nav.stoppingDistance = Mathf.Max(nav.stoppingDistance, stoppingDistance);
                nav.SetDestination(target.position);
            }
        }

        public ActionResult UpdateExecution(IAgentState agentState, IWorldState worldState, float deltaTime)
        {
            if (!IsExecuting) return ActionResult.Failed;
            if (agentTransform == null) return ActionResult.Failed;

            // Reacquire if target is lost
            if (target == null || !target.gameObject.activeInHierarchy)
            {
                targetLocked = TryResolveAndLockTarget();
                if (!targetLocked) return ActionResult.Failed;

                if (nav != null) nav.SetDestination(target.position);
            }

            // NavMeshAgent pathing
            if (nav != null)
            {
                if (!nav.pathPending && nav.remainingDistance <= Mathf.Max(stoppingDistance, nav.stoppingDistance))
                    return ActionResult.Success;

                return ActionResult.Running;
            }

            // Manual transform move
            Vector3 to = target.position - agentTransform.position;
            float distSq = to.sqrMagnitude;
            float stopSq = stoppingDistance * stoppingDistance;

            if (distSq <= stopSq)
                return ActionResult.Success;

            Vector3 step = to.normalized * moveSpeed * deltaTime;
            agentTransform.position += step;

            return ActionResult.Running;
        }

        public void CancelExecution()
        {
            if (nav != null && nav.isOnNavMesh)
            {
                nav.ResetPath();
            }
            IsExecuting = false;
            target = null;
            targetLocked = false;
        }

        public void ApplyEffects(IAgentState agentState, IWorldState worldState)
        {
            // No direct world state effects from movement.
        }

        public string GetDescription()
        {
            string tgt = target != null ? target.name : "(none)";
            string goal = currentGoal != null ? currentGoal.GoalType : "(no-goal)";
            return $"MoveToAction → target: {tgt}, goal: {goal}, stopDist: {stoppingDistance}";
        }

        // ===================== INTERNALS =====================
        private bool TryResolveAndLockTarget()
        {
            if (agentTransform == null || currentGoal == null) return false;
            if (POIUtility.TryGetNearestPOI(currentGoal, agentTransform.position, out var tGoal))
            {
                target = tGoal;
                return true;
            }

            target = null;
            return false;
        }

        private static readonly Dictionary<string, object> EmptyDict = new Dictionary<string, object>();
    }
}
