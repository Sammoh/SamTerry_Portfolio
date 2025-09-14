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
        [Header("Goals/Actions (SO-driven)")]
        [SerializeField] private GoalDatabase goalDatabase;
        
        [Header("Configuration")]
        [SerializeField] private float tickRate = 10f; // Hz (5-10Hz as specified)
        [SerializeField] private bool enableDebug = true;
        [SerializeField] private Transform agentTransform;
        
        [Header("Debug Info")]
        [SerializeField] private string currentGoalType = "none";
        [SerializeField] private string currentActionType = "none";
        [SerializeField] private int planActionsRemaining = 0;
        
        // Public read-only accessors for monitoring
        public string CurrentGoalType => currentGoalType;
        public string CurrentActionType => currentActionType;
        public int PlanActionsRemaining => planActionsRemaining;
        
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
            
            // Ensure agent has NavMeshAgent for movement
            if (agentTransform.GetComponent<UnityEngine.AI.NavMeshAgent>() == null)
            {
                var navAgent = agentTransform.gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
                navAgent.speed = 3.5f;
                navAgent.stoppingDistance = 0.5f;
                navAgent.enabled = true;
            }
                
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
            // Try to load GoalDatabase from Resources if not assigned
            if (goalDatabase == null)
            {
                goalDatabase = Resources.Load<GoalDatabase>("GoalDatabase");
                if (goalDatabase == null)
                {
                    Debug.LogWarning("GoalDatabase not found in Resources folder or assigned to agent");
                }
            }
            
            // Goals from ScriptableObject database
            if (goalDatabase != null && goalDatabase.Goals != null && goalDatabase.Goals.Count > 0)
                availableGoals = new List<IGoal>(goalDatabase.Goals);
            else
                availableGoals = new List<IGoal>();
            
            // If no goals loaded from database, add fallback goals
            if (availableGoals.Count == 0)
            {
                availableGoals.Add(new IdleGoal());
                availableGoals.Add(new CommunicationGoal());
                Debug.LogWarning("No goals found in GoalDatabase, using fallback IdleGoal and CommunicationGoal");
            }

            // Initialize available actions list
            availableActions = new List<IAction>();

            // Add basic actions that work with any goal
            availableActions.Add(new NoOpAction());
            availableActions.Add(new EatAction());
            availableActions.Add(new DrinkAction()); 
            availableActions.Add(new PlayAction());
            availableActions.Add(new SleepAction());
            availableActions.Add(new BarkAction());

            // Create movement actions for each goal that requires POIs
            foreach (var goal in availableGoals)
            {
                if (goal is NeedReductionGoalSO)
                {
                    var moveToAction = new MoveToAction();
                    moveToAction.InjectAgent(agentTransform, agentTransform.GetComponent<UnityEngine.AI.NavMeshAgent>());
                    moveToAction.InjectCurrentGoal(goal);
                    availableActions.Add(moveToAction);
                }
            }
            
            Debug.Log($"Setup complete: {availableGoals.Count} goals, {availableActions.Count} actions");
            
            // Validate setup and log any issues
            if (enableDebug)
            {
                var issues = GOAPValidation.ValidateSetup(availableGoals, availableActions);
                GOAPValidation.LogValidationResults(issues, "GOAP Agent Setup");
                
                if (goalDatabase != null)
                {
                    var dbIssues = GOAPValidation.ValidateGoalDatabase(goalDatabase);
                    GOAPValidation.LogValidationResults(dbIssues, "GOAP Goal Database");
                }
            }
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
                // Debug.Log($"Selected new goal: {bestGoal.GoalType} (priority: {bestPriority:F2})");
                
                // Create plan for the goal
                var plan = planner.CreatePlan(bestGoal, agentState, worldState, availableActions);
                
                if (plan != null)
                {
                    executor.SetPlan(plan); 
                    currentGoalType = plan.Goal?.GoalType ?? "none";
                    planActionsRemaining = plan.Actions?.Count ?? 0;
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
            currentActionType = "none";
            planActionsRemaining = 0;
            if (!success)
            {
                currentGoalType = "none";
            }
        }
        
        private void OnActionFailed(IAction action)
        {
            Debug.LogError($"Action '{action.ActionType}' failed");
            currentActionType = action?.ActionType ?? "none";
        }
        
        private void OnActionStarted(IAction action)
        {
            Debug.Log($"Action '{action.ActionType}' started");
            currentActionType = action?.ActionType ?? "none";
            if (executor?.CurrentPlan?.Actions != null)
            {
                planActionsRemaining = executor.CurrentPlan.Actions.Count;
            }
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