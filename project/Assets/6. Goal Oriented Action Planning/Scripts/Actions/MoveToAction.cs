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
            nav = navAgent;
        }
        public void InjectCurrentGoal(IGoal goal) => currentGoal = goal;

        public bool CheckPreconditions(IAgentState agentState, IWorldState worldState)
        {
            if (agentTransform == null || currentGoal == null) return false;
            return POIUtility.TryGetNearestPOI(currentGoal, agentTransform.position, out _);
        }

        // Important: planner needs to see the "at_<goalType>" effect to sequence next action.
        public Dictionary<string, object> GetEffects()
        {
            if (currentGoal == null) return Empty;
            return new Dictionary<string, object> { { $"at_{currentGoal.GoalType}", true } };
        }

        public void StartExecution(IAgentState agentState, IWorldState worldState)
        {
            IsExecuting = TryResolveAndLockTarget();
            if (!IsExecuting) return;

            if (nav != null)
            {
                nav.stoppingDistance = Mathf.Max(nav.stoppingDistance, stoppingDistance);
                nav.SetDestination(target.position);
            }
        }

        public ActionResult UpdateExecution(IAgentState agentState, IWorldState worldState, float deltaTime)
        {
            if (!IsExecuting || agentTransform == null) return ActionResult.Failed;

            if (target == null || !target.gameObject.activeInHierarchy)
            {
                if (!TryResolveAndLockTarget()) return ActionResult.Failed;
                if (nav != null) nav.SetDestination(target.position);
            }

            if (nav != null)
            {
                if (!nav.pathPending && nav.remainingDistance <= Mathf.Max(stoppingDistance, nav.stoppingDistance))
                    return ActionResult.Success;
                return ActionResult.Running;
            }

            // Manual translation
            Vector3 to = target.position - agentTransform.position;
            if (to.sqrMagnitude <= stoppingDistance * stoppingDistance)
                return ActionResult.Success;

            agentTransform.position += to.normalized * moveSpeed * deltaTime;
            return ActionResult.Running;
        }

        public void CancelExecution()
        {
            if (nav != null && nav.isOnNavMesh) nav.ResetPath();
            IsExecuting = false;
            target = null;
            targetLocked = false;
        }

        public void ApplyEffects(IAgentState agentState, IWorldState worldState)
        {
            if (currentGoal == null) return;
            worldState.SetFact($"at_{currentGoal.GoalType}", true);
        }

        public string GetDescription()
        {
            string tgt = target ? target.name : "(none)";
            return $"MoveToAction → goal={currentGoal?.GoalType ?? "(none)"} target={tgt} stop={stoppingDistance}";
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

        private static readonly Dictionary<string, object> Empty = new();
    }
}
