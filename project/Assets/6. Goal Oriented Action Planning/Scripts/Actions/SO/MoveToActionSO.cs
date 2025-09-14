using System.Collections.Generic;
using Sammoh.GOAP;
using UnityEngine;

[CreateAssetMenu(fileName="MoveTo", menuName="GOAP/Actions/Move To (Per-Goal)")]
public class MoveToActionSO : ScriptableObject, IAction
{
    [SerializeField] private NeedReductionGoalSO targetGoal;
    [SerializeField] private float cost = 1f;
    [SerializeField] private float stoppingDistance = 0.5f;
    [SerializeField] private float moveSpeed = 3.5f;

    private Transform agent;
    private UnityEngine.AI.NavMeshAgent nav;
    private Transform target;
    private bool executing;

    public string ActionType => "move_to";
    public float Cost => cost;
    public bool IsExecuting => executing;

    public bool CheckPreconditions(IAgentState a, IWorldState w)
    {
        if (agent == null || targetGoal == null) return false;
        return POIUtility.TryGetNearestPOI(targetGoal, agent.position, out _);
    }

    public Dictionary<string, object> GetEffects()
    {
        // Static at plan-time: for goalType="eat" this is "at_eat" = true
        return new Dictionary<string, object> { { $"at_{targetGoal.GoalType}", true } };
    }

    public void StartExecution(IAgentState a, IWorldState w)
    {
        executing = POIUtility.TryGetNearestPOI(targetGoal, agent.position, out target);
        if (!executing) return;
        if (nav != null) { nav.stoppingDistance = Mathf.Max(nav.stoppingDistance, stoppingDistance); nav.SetDestination(target.position); }
    }

    public ActionResult UpdateExecution(IAgentState a, IWorldState w, float dt)
    {
        if (!executing || agent == null || target == null) return ActionResult.Failed;
        if (nav != null)
        {
            if (!nav.pathPending && nav.remainingDistance <= Mathf.Max(stoppingDistance, nav.stoppingDistance))
                return ActionResult.Success;
            return ActionResult.Running;
        }
        var to = target.position - agent.position;
        if (to.sqrMagnitude <= stoppingDistance * stoppingDistance) return ActionResult.Success;
        agent.position += to.normalized * moveSpeed * dt;
        return ActionResult.Running;
    }

    public void CancelExecution() { if (nav && nav.isOnNavMesh) nav.ResetPath(); executing = false; target = null; }
    public void ApplyEffects(IAgentState a, IWorldState w) { w.SetFact($"at_{targetGoal.GoalType}", true); }
    public string GetDescription() => $"MoveTo({targetGoal?.GoalType ?? "null"})";

    // Call these from your executor once per plan binding:
    public void InjectAgent(Transform t, UnityEngine.AI.NavMeshAgent n = null) { agent = t; nav = n; }
}
