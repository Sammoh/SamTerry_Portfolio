using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Main GOAP agent that demonstrates the system
    /// Implements deterministic tick-based planning and execution
    /// </summary>
    public class GOAPAgent : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private float tickRate = 10f; // Hz (5-10Hz as specified)
        [SerializeField] private bool enableDebug = true;
        [SerializeField] private Transform agentTransform;
        
        [Header("Debug Info")]
        [SerializeField] private string currentGoalType = "none";
        [SerializeField] private string currentActionType = "none";
        [SerializeField] private int planActionsRemaining = 0;
        
        // Core GOAP systems
        private IAgentState agentState;
        private IWorldState worldState;
        private IPlanner planner;
        private IExecutor executor;
        
        // Available goals and actions
        private List<IGoal> availableGoals;
        private List<IAction> availableActions;
        
        // Timing
        private float lastTickTime;
        private float tickInterval;
        
        // Debug overlay
        private GOAPDebugOverlay debugOverlay;
        
        private void Awake()
        {
            if (agentTransform == null)
                agentTransform = transform;
                
            InitializeSystems();
            SetupGoalsAndActions();
            
            tickInterval = 1f / tickRate;
            lastTickTime = Time.time;
            
            if (enableDebug)
            {
                debugOverlay = FindObjectOfType<GOAPDebugOverlay>();
                if (debugOverlay == null)
                {
                    var debugGO = new GameObject("GOAP Debug Overlay");
                    debugOverlay = debugGO.AddComponent<GOAPDebugOverlay>();
                }
                debugOverlay.SetAgent(this);
            }
        }
        
        private void InitializeSystems()
        {
            // Create core systems
            agentState = new BasicAgentState();
            worldState = FindObjectOfType<BasicWorldState>();
            
            if (worldState == null)
            {
                var worldGO = new GameObject("World State");
                worldState = worldGO.AddComponent<BasicWorldState>();
            }
            
            planner = new BasicPlanner();
            executor = new BasicExecutor();
            
            // Setup event handlers
            executor.OnPlanCompleted += OnPlanCompleted;
            executor.OnActionFailed += OnActionFailed;
            executor.OnActionStarted += OnActionStarted;
            
            Debug.Log("GOAP Agent initialized successfully");
        }
        
        private void SetupGoalsAndActions()
        {
            // Initialize available goals
            availableGoals = new List<IGoal>
            {
                new IdleGoal(),
                new EatGoal(),
                new DrinkGoal(),
                new PlayGoal()
            };
            
            // Initialize available actions
            availableActions = new List<IAction>
            {
                new NoOpAction(),
                new MoveToAction(agentTransform, "food"),
                new MoveToAction(agentTransform, "water"),
                new MoveToAction(agentTransform, "toy"),
                new EatAction(),
                new DrinkAction()
            };
            
            Debug.Log($"Setup complete: {availableGoals.Count} goals, {availableActions.Count} actions");
        }
        
        private void Update()
        {
            // Update agent state continuously
            agentState.Update(Time.deltaTime);
            
            // Deterministic tick-based planning/execution
            if (Time.time >= lastTickTime + tickInterval)
            {
                PerformTick();
                lastTickTime = Time.time;
            }
            
            // Update executor every frame for smooth action execution
            executor.Update(agentState, worldState, Time.deltaTime);
            
            // Update debug info
            UpdateDebugInfo();
        }
        
        private void PerformTick()
        {
            // Check if we need a new plan
            if (!executor.IsExecuting || executor.IsComplete || executor.HasFailed)
            {
                SelectNewGoal();
            }
            // Check if current plan is still valid
            else if (!planner.IsPlanValid(executor.CurrentPlan, agentState, worldState))
            {
                Debug.Log("Current plan is no longer valid, replanning...");
                executor.Replan(agentState, worldState, availableActions, planner);
            }
        }
        
        private void SelectNewGoal()
        {
            IGoal bestGoal = null;
            float bestPriority = float.MinValue;
            
            // Find the highest priority goal that can be satisfied
            foreach (var goal in availableGoals)
            {
                if (goal.CanSatisfy(agentState, worldState))
                {
                    float priority = goal.CalculatePriority(agentState, worldState);
                    if (priority > bestPriority)
                    {
                        bestPriority = priority;
                        bestGoal = goal;
                    }
                }
            }
            
            if (bestGoal != null)
            {
                Debug.Log($"Selected new goal: {bestGoal.GoalType} (priority: {bestPriority:F2})");
                
                // Create plan for the goal
                var plan = planner.CreatePlan(bestGoal, agentState, worldState, availableActions);
                
                if (plan != null)
                {
                    executor.SetPlan(plan);
                    Debug.Log($"Plan created with {plan.Actions.Count} actions");
                }
                else
                {
                    Debug.LogWarning($"Failed to create plan for goal: {bestGoal.GoalType}");
                }
            }
            else
            {
                Debug.LogWarning("No suitable goal found");
            }
        }
        
        private void UpdateDebugInfo()
        {
            currentGoalType = executor.CurrentGoal?.GoalType ?? "none";
            currentActionType = executor.CurrentAction?.ActionType ?? "none";
            planActionsRemaining = executor.CurrentPlan?.Actions.Count ?? 0;
        }
        
        #region Event Handlers
        
        private void OnPlanCompleted(Plan plan, bool success)
        {
            Debug.Log($"Plan completed for goal '{plan.Goal.GoalType}'. Success: {success}");
        }
        
        private void OnActionFailed(IAction action)
        {
            Debug.LogError($"Action '{action.ActionType}' failed");
        }
        
        private void OnActionStarted(IAction action)
        {
            Debug.Log($"Action '{action.ActionType}' started");
        }
        
        #endregion
        
        #region Public API for Debug/Testing
        
        public IAgentState GetAgentState() => agentState;
        public IWorldState GetWorldState() => worldState;
        public IGoal GetCurrentGoal() => executor.CurrentGoal;
        public IAction GetCurrentAction() => executor.CurrentAction;
        public Plan GetCurrentPlan() => executor.CurrentPlan;
        public List<IGoal> GetAvailableGoals() => availableGoals;
        public List<IAction> GetAvailableActions() => availableActions;
        
        public void ForceReplan()
        {
            if (executor.CurrentGoal != null)
            {
                executor.Replan(agentState, worldState, availableActions, planner);
            }
        }
        
        #endregion
        
        private void OnDestroy()
        {
            // Cleanup
            if (executor != null)
            {
                executor.OnPlanCompleted -= OnPlanCompleted;
                executor.OnActionFailed -= OnActionFailed;
                executor.OnActionStarted -= OnActionStarted;
            }
        }
    }
}