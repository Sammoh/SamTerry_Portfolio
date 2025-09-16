using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    public class GOAPAgent : MonoBehaviour
    {
        [Header("Goals/Actions (SO-driven)")]
        [SerializeField] private GoalDatabase goalDatabase;
        [SerializeField] private ActionDatabase actionDatabase;

        [Header("Configuration")]
        [SerializeField] private float tickRate = 10f;
        [SerializeField] private bool enableDebug = true;
        [SerializeField] private Transform agentTransform;

        [Header("Debug Info")]
        [SerializeField] private string currentGoalType = "none";
        [SerializeField] private string currentActionType = "none";
        [SerializeField] private int planActionsRemaining = 0;

        public string CurrentGoalType => currentGoalType;
        public string CurrentActionType => currentActionType;
        public int PlanActionsRemaining => planActionsRemaining;

        [Header("UI / Barking")]
        [SerializeField] private BarkBubble barkBubblePrefab;
        private BarkBubble barkBubble;

        private Dictionary<string, List<string>> cannedBarks;

        private IAgentState agentState;
        private IWorldState worldState;
        private IPlanner planner;
        private IExecutor executor;

        private List<IGoal> availableGoals;
        private List<IAction> availableActions;

        private float lastTickTime;
        private float tickInterval;

        private GOAPDebugOverlay debugOverlay;

        // Bark control
        private float lastBarkTime = -10f;
        private float barkMinInterval = 0.35f;

        private void Awake()
        {
            if (agentTransform == null)
                agentTransform = transform;

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
            agentState = new BasicAgentState();
            worldState = FindObjectOfType<BasicWorldState>();
            if (worldState == null)
            {
                var worldGO = new GameObject("World State");
                worldState = worldGO.AddComponent<BasicWorldState>();
            }

            planner = new BasicPlanner();
            executor = new BasicExecutor();

            executor.OnPlanCompleted += OnPlanCompleted;
            executor.OnActionFailed += OnActionFailed;
            executor.OnActionStarted += OnActionStarted;

            cannedBarks = new Dictionary<string, List<string>>();
            cannedBarks["Eat"] = new List<string> { "Yum!", "Delicious!", "More food, please!" };
            cannedBarks["Drink"] = new List<string> { "Ahh, refreshing!", "Thirst quenched!", "More water, please!" };
            cannedBarks["Sleep"] = new List<string> { "Zzz...", "So sleepy...", "Time for a nap!" };
            cannedBarks["Play"] = new List<string> { "Whee!", "This is fun!", "Let's play more!" };
            cannedBarks["Idle"] = new List<string> { "Just chilling...", "What a nice day!", "I'm bored..." };
            cannedBarks["MoveTo"] = new List<string> { "On my way!", "Heading out!", "Moving..." };

            if (barkBubblePrefab != null)
            {
                barkBubble = Instantiate(barkBubblePrefab, agentTransform);
                barkBubble.transform.localPosition = Vector3.zero;
                barkBubble.fadeInDuration = 0.2f;
                barkBubble.fadeOutDuration = 0.5f;
                barkBubble.disableAfterHide = true;
                barkBubble.SetFollowTarget(Camera.main != null ? Camera.main.transform : agentTransform);
                Bark("Ready.", 1.8f, "GOAP");
            }

            Debug.Log("GOAP Agent initialized successfully");
        }

        private void SetupGoalsAndActions()
        {
            // Setup Goals
            if (goalDatabase == null)
            {
                goalDatabase = Resources.Load<GoalDatabase>("GoalDatabase");
                if (goalDatabase == null)
                    Debug.LogWarning("GoalDatabase not found in Resources or not assigned");
            }

            if (goalDatabase != null && goalDatabase.Goals != null && goalDatabase.Goals.Count > 0)
                availableGoals = new List<IGoal>(goalDatabase.Goals);
            else
                availableGoals = new List<IGoal>();

            if (availableGoals.Count == 0)
            {
                availableGoals.Add(new IdleGoal());
                Debug.LogWarning("No goals found. Added fallback IdleGoal.");
            }

            // Setup Actions - try ActionDatabase first, fallback to hardcoded
            SetupActions();

            // Add MoveToAction for each NeedReductionGoalSO
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

            if (enableDebug)
            {
                var issues = GOAPValidation.ValidateSetup(availableGoals, availableActions);
                GOAPValidation.LogValidationResults(issues, "GOAP Agent Setup");

                if (goalDatabase != null)
                {
                    var dbIssues = GOAPValidation.ValidateGoalDatabase(goalDatabase);
                    GOAPValidation.LogValidationResults(dbIssues, "GOAP Goal Database");
                }
                
                if (actionDatabase != null)
                {
                    var actionIssues = GOAPValidation.ValidateActionDatabase(actionDatabase);
                    GOAPValidation.LogValidationResults(actionIssues, "GOAP Action Database");
                }
            }
        }
        
        /// <summary>
        /// Setup actions using ActionDatabase if available, otherwise fallback to hardcoded actions
        /// </summary>
        private void SetupActions()
        {
            availableActions = new List<IAction>();
            
            // Try to load ActionDatabase from Resources if not assigned
            if (actionDatabase == null)
            {
                actionDatabase = Resources.Load<ActionDatabase>("ActionDatabase");
            }
            
            // Use ScriptableObject actions if available
            if (actionDatabase != null)
            {
                var soActions = actionDatabase.Actions;
                if (soActions != null && soActions.Count > 0)
                {
                    availableActions.AddRange(soActions);
                    Debug.Log($"Loaded {soActions.Count} actions from ActionDatabase");
                }
                else
                {
                    Debug.LogWarning("ActionDatabase found but contains no valid actions. Using fallback.");
                    LoadFallbackActions();
                }
            }
            else
            {
                Debug.LogWarning("ActionDatabase not found in Resources or not assigned. Using fallback hardcoded actions.");
                LoadFallbackActions();
            }
            
            // Always add NoOpAction if not present
            bool hasNoOp = false;
            foreach (var action in availableActions)
            {
                if (action.ActionType == "noop")
                {
                    hasNoOp = true;
                    break;
                }
            }
            
            if (!hasNoOp)
            {
                availableActions.Add(new NoOpAction());
                Debug.Log("Added fallback NoOpAction");
            }
        }
        
        /// <summary>
        /// Load hardcoded fallback actions for backward compatibility
        /// </summary>
        private void LoadFallbackActions()
        {
            availableActions.AddRange(new List<IAction>
            {
                new NoOpAction(),
                new EatAction(),
                new DrinkAction(),
                new PlayAction(),
                new SleepAction()
            });
            Debug.Log("Loaded fallback hardcoded actions");
        }
                }
            }
        }

        private void Update()
        {
            agentState.Update(Time.deltaTime);

            if (Time.time >= lastTickTime + tickInterval)
            {
                PerformTick();
                lastTickTime = Time.time;
            }

            executor.Update(agentState, worldState, Time.deltaTime);
            UpdateDebugInfo();
        }

        private void PerformTick()
        {
            if (!executor.IsExecuting || executor.IsComplete || executor.HasFailed)
            {
                SelectNewGoal();
            }
            else if (!planner.IsPlanValid(executor.CurrentPlan, agentState, worldState))
            {
                Debug.Log("Current plan invalid. Replanning...");
                Bark("Replanning...", 1.2f, "Planner");
                executor.Replan(agentState, worldState, availableActions, planner);
            }
        }

        private void SelectNewGoal()
        {
            IGoal bestGoal = null;
            float bestPriority = float.MinValue;

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
                var plan = planner.CreatePlan(bestGoal, agentState, worldState, availableActions);
                if (plan != null)
                {
                    executor.SetPlan(plan);
                    currentGoalType = plan.Goal?.GoalType ?? "none";
                    planActionsRemaining = plan.Actions?.Count ?? 0;
                    Bark($"New goal: {bestGoal.GoalType}", 2f, "Goal");
                }
                else
                {
                    Bark($"Cannot plan {bestGoal.GoalType}", 2f, "Goal");
                    Debug.LogWarning($"Failed to create plan for goal: {bestGoal.GoalType}");
                }
            }
            // else
            // {
            //     Bark("No viable goal", 1.6f, "Goal");
            //     Debug.LogWarning("No suitable goal found");
            // }
        }

        private void UpdateDebugInfo()
        {
            currentGoalType = executor.CurrentGoal?.GoalType ?? "none";
            currentActionType = executor.CurrentAction?.ActionType ?? "none";
            planActionsRemaining = executor.CurrentPlan?.Actions.Count ?? 0;
        }

        private void OnPlanCompleted(Plan plan, bool success)
        {
            Debug.Log($"Plan completed for goal '{plan.Goal.GoalType}'. Success: {success}");
            currentActionType = "none";
            planActionsRemaining = 0;
            if (!success)
            {
                currentGoalType = "none";
                Bark($"Plan failed: {plan.Goal.GoalType}", 2f, "Plan");
            }
            else
            {
                Bark($"Done: {plan.Goal.GoalType}", 1.8f, "Plan");
            }
        }

        private void OnActionFailed(IAction action)
        {
            Debug.LogError($"Action '{action.ActionType}' failed");
            currentActionType = action?.ActionType ?? "none";
            Bark($"Fail {action.ActionType}", 1.8f, "Action");
        }

        private void OnActionStarted(IAction action)
        {
            Debug.Log($"Action '{action.ActionType}' started");
            currentActionType = action?.ActionType ?? "none";
            if (executor?.CurrentPlan?.Actions != null)
                planActionsRemaining = executor.CurrentPlan.Actions.Count;

            var line = GetRandomCanned(action.ActionType);
            Bark(line, 2f, action.ActionType);
        }

        // Bark helpers
        private void Bark(string message, float duration = 2f, string speaker = "Agent")
        {
            if (barkBubble == null) return;
            if (Time.time - lastBarkTime < barkMinInterval) return;
            lastBarkTime = Time.time;
            barkBubble.ShowBark(message, duration, speaker);
        }

        private string GetRandomCanned(string key)
        {
            if (cannedBarks != null && cannedBarks.TryGetValue(key, out var list) && list.Count > 0)
            {
                int idx = UnityEngine.Random.Range(0, list.Count);
                return list[idx];
            }
            return key;
        }

        #region Public API

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
                Bark("Force replan", 1.2f, "Debug");
                executor.Replan(agentState, worldState, availableActions, planner);
            }
        }

        #endregion

        private void OnDestroy()
        {
            if (executor != null)
            {
                executor.OnPlanCompleted -= OnPlanCompleted;
                executor.OnActionFailed -= OnActionFailed;
                executor.OnActionStarted -= OnActionStarted;
            }
        }
    }
}
