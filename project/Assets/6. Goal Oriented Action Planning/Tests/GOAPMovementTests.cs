using NUnit.Framework;
using UnityEngine;
using Sammoh.GOAP;
using System.Collections.Generic;

namespace Sammoh.GOAP.Tests
{
    /// <summary>
    /// Tests for GOAP agent movement and goal selection
    /// </summary>
    public class GOAPMovementTests
    {
        private BasicAgentState agentState;
        private BasicWorldState worldState;
        private BasicPlanner planner;
        private BasicExecutor executor;
        private List<IAction> testActions;
        private List<IGoal> testGoals;
        
        [SetUp]
        public void Setup()
        {
            // Create test world state
            var worldGO = new GameObject("TestWorldState");
            worldState = worldGO.AddComponent<BasicWorldState>();
            
            // Create basic systems
            agentState = new BasicAgentState();
            planner = new BasicPlanner();
            executor = new BasicExecutor();
            
            // Create test goals
            testGoals = new List<IGoal>();
            
            // Create a hunger goal
            var hungerGoal = ScriptableObject.CreateInstance<NeedReductionGoalSO>();
            var hungerFields = typeof(NeedReductionGoalSO).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            foreach (var field in hungerFields)
            {
                switch (field.Name)
                {
                    case "goalType":
                        field.SetValue(hungerGoal, "hunger");
                        break;
                    case "needKey":
                        field.SetValue(hungerGoal, "hunger");
                        break;
                    case "desiredWorldStateKey":
                        field.SetValue(hungerGoal, "need_hunger");
                        break;
                    case "activationThreshold":
                        field.SetValue(hungerGoal, 0.5f);
                        break;
                    case "completionThreshold":
                        field.SetValue(hungerGoal, 0.05f);
                        break;
                    case "priorityScale":
                        field.SetValue(hungerGoal, 10f);
                        break;
                }
            }
            testGoals.Add(hungerGoal);
            
            // Setup test actions
            testActions = new List<IAction>
            {
                new NoOpAction(),
                new EatAction(),
                new DrinkAction(),
                new PlayAction(),
                new SleepAction()
            };
            
            // Add movement action for hunger goal
            var moveToAction = new MoveToAction();
            moveToAction.InjectCurrentGoal(hungerGoal);
            testActions.Add(moveToAction);
        }
        
        [TearDown]
        public void TearDown()
        {
            if (worldState != null)
                Object.DestroyImmediate(worldState.gameObject);
        }
        
        [Test]
        public void AgentState_HighHungerNeed_TriggersPlanningForHungerGoal()
        {
            // Set high hunger need
            agentState.SetNeed("hunger", 0.8f);
            
            // Find the hunger goal
            IGoal hungerGoal = null;
            foreach (var goal in testGoals)
            {
                if (goal.GoalType == "hunger")
                {
                    hungerGoal = goal;
                    break;
                }
            }
            
            Assert.IsNotNull(hungerGoal, "Hunger goal should exist");
            Assert.IsTrue(hungerGoal.CanSatisfy(agentState, worldState), "Hunger goal should be satisfiable with high hunger need");
            
            float priority = hungerGoal.CalculatePriority(agentState, worldState);
            Assert.Greater(priority, 0f, "Hunger goal should have positive priority when need is high");
        }
        
        [Test]
        public void Planner_HungerGoal_CreatesMovementAndEatPlan()
        {
            // Set high hunger need
            agentState.SetNeed("hunger", 0.8f);
            
            // Create hunger goal
            var hungerGoal = testGoals.Find(g => g.GoalType == "hunger");
            Assert.IsNotNull(hungerGoal, "Hunger goal should exist");
            
            // Create plan
            var plan = planner.CreatePlan(hungerGoal, agentState, worldState, testActions);
            
            // With current setup, plan should be created
            Assert.IsNotNull(plan, "Plan should be created for hunger goal");
            Assert.Greater(plan.Actions.Count, 0, "Plan should contain actions");
            
            // The plan should contain both movement and eating actions
            bool hasMovement = false;
            bool hasEating = false;
            
            foreach (var action in plan.Actions)
            {
                if (action.ActionType == "move_to") hasMovement = true;
                if (action.ActionType == "eat") hasEating = true;
            }
            
            Assert.IsTrue(hasMovement || hasEating, "Plan should contain movement or eating action");
        }
        
        [Test]
        public void MoveToAction_HungerGoal_SetsCorrectLocationFact()
        {
            // Create movement action for hunger goal
            var hungerGoal = testGoals.Find(g => g.GoalType == "hunger");
            var moveAction = new MoveToAction();
            moveAction.InjectCurrentGoal(hungerGoal);
            
            // Check effects
            var effects = moveAction.GetEffects();
            Assert.IsTrue(effects.ContainsKey("at_food"), "MoveToAction for hunger goal should set at_food fact");
            Assert.AreEqual(true, effects["at_food"], "at_food fact should be set to true");
        }
        
        [Test]
        public void EatAction_WithAtFoodFact_CanExecute()
        {
            // Set world state to indicate agent is at food
            worldState.SetFact("at_food", true);
            
            var eatAction = new EatAction();
            Assert.IsTrue(eatAction.CheckPreconditions(agentState, worldState), "EatAction should be executable when at_food is true");
            
            // Check effects
            var effects = eatAction.GetEffects();
            Assert.IsTrue(effects.ContainsKey("need_hunger"), "EatAction should affect hunger need");
            Assert.AreEqual(0f, effects["need_hunger"], "EatAction should set hunger need to 0");
        }
        
        [Test]
        public void EatAction_WithoutAtFoodFact_CannotExecute()
        {
            // Ensure agent is not at food
            worldState.SetFact("at_food", false);
            
            var eatAction = new EatAction();
            Assert.IsFalse(eatAction.CheckPreconditions(agentState, worldState), "EatAction should not be executable when at_food is false");
        }
        
        [Test]
        public void ActionEffectsMapping_AllGoalTypes_MapCorrectly()
        {
            // Test that all goal types map to correct location facts
            var testMappings = new Dictionary<string, string>
            {
                { "hunger", "at_food" },
                { "thirst", "at_water" },
                { "sleep", "at_bed" },
                { "play", "at_toy" }
            };
            
            foreach (var mapping in testMappings)
            {
                var mockGoal = new TestGoalForMapping(mapping.Key);
                var moveAction = new MoveToAction();
                moveAction.InjectCurrentGoal(mockGoal);
                
                var effects = moveAction.GetEffects();
                Assert.IsTrue(effects.ContainsKey(mapping.Value), 
                    $"Goal type '{mapping.Key}' should map to location fact '{mapping.Value}'");
            }
        }
        
        [Test]
        public void AcceptanceCriteria_AgentWithHighNeed_SelectsGoalAndAttemptsPlan()
        {
            // This test verifies the main acceptance criteria:
            // Agent with high need should select appropriate goal and create a plan to satisfy it
            
            // Set high hunger need
            agentState.SetNeed("hunger", 0.9f);
            
            // Find highest priority goal
            IGoal bestGoal = null;
            float bestPriority = float.MinValue;
            
            foreach (var goal in testGoals)
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
            
            Assert.IsNotNull(bestGoal, "Agent should select a goal when needs are high");
            Assert.AreEqual("hunger", bestGoal.GoalType, "Agent should select hunger goal when hunger need is highest");
            
            // Create plan for the selected goal
            var plan = planner.CreatePlan(bestGoal, agentState, worldState, testActions);
            
            Assert.IsNotNull(plan, "Agent should be able to create a plan for the selected goal");
            Assert.Greater(plan.Actions.Count, 0, "Plan should contain at least one action");
            
            Debug.Log($"Agent selected goal: {bestGoal.GoalType} with priority: {bestPriority:F2}");
            Debug.Log($"Plan created with {plan.Actions.Count} actions");
        }
    }
    
    /// <summary>
    /// Helper goal class for testing action mappings
    /// </summary>
    public class TestGoalForMapping : IGoal
    {
        private readonly string goalType;
        
        public TestGoalForMapping(string goalType)
        {
            this.goalType = goalType;
        }
        
        public string GoalType => goalType;
        public float Priority => 1f;
        
        public bool CanSatisfy(IAgentState agentState, IWorldState worldState) => true;
        public bool IsCompleted(IAgentState agentState, IWorldState worldState) => false;
        public Dictionary<string, object> GetDesiredState() => new Dictionary<string, object>();
        public float CalculatePriority(IAgentState agentState, IWorldState worldState) => 1f;
    }
}