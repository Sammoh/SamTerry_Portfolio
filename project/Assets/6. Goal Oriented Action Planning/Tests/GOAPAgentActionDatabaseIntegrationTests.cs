using NUnit.Framework;
using UnityEngine;
using Sammoh.GOAP;
using System.Collections.Generic;

namespace Sammoh.GOAP.Tests
{
    /// <summary>
    /// Integration tests for GOAPAgent with ActionDatabase
    /// </summary>
    public class GOAPAgentActionDatabaseIntegrationTests
    {
        private GameObject agentGO;
        private GOAPAgent goapAgent;
        private ActionDatabase actionDatabase;
        private GoalDatabase goalDatabase;
        
        [SetUp]
        public void Setup()
        {
            // Create agent GameObject
            agentGO = new GameObject("TestAgent");
            goapAgent = agentGO.AddComponent<GOAPAgent>();
            
            // Create ActionDatabase with test actions
            actionDatabase = ScriptableObject.CreateInstance<ActionDatabase>();
            var eatAction = ScriptableObject.CreateInstance<EatActionSO>();
            var drinkAction = ScriptableObject.CreateInstance<DrinkActionSO>();
            actionDatabase.AddAction(eatAction);
            actionDatabase.AddAction(drinkAction);
            
            // Create GoalDatabase with test goals
            goalDatabase = ScriptableObject.CreateInstance<GoalDatabase>();
            var hungerGoal = ScriptableObject.CreateInstance<NeedReductionGoalSO>();
            goalDatabase.AddGoal(hungerGoal);
            
            // Setup agent with databases - use reflection to access private fields
            var goalDBField = typeof(GOAPAgent).GetField("goalDatabase", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var actionDBField = typeof(GOAPAgent).GetField("actionDatabase", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            goalDBField.SetValue(goapAgent, goalDatabase);
            actionDBField.SetValue(goapAgent, actionDatabase);
        }
        
        [TearDown]
        public void TearDown()
        {
            if (agentGO != null)
                Object.DestroyImmediate(agentGO);
            if (actionDatabase != null)
                Object.DestroyImmediate(actionDatabase);
            if (goalDatabase != null)
                Object.DestroyImmediate(goalDatabase);
        }
        
        [Test]
        public void GOAPAgent_LoadsActionsFromDatabase()
        {
            // Use reflection to access private setup method
            var setupMethod = typeof(GOAPAgent).GetMethod("SetupGoalsAndActions", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            // This should not throw and should use ActionDatabase
            Assert.DoesNotThrow(() => setupMethod.Invoke(goapAgent, null));
            
            // Verify agent has loaded actions
            var actionsField = typeof(GOAPAgent).GetField("availableActions", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var actions = actionsField.GetValue(goapAgent) as List<IAction>;
            
            Assert.IsNotNull(actions);
            Assert.IsTrue(actions.Count >= 2, "Should have at least the 2 SO actions");
            
            // Verify we have the expected action types
            bool hasEat = false, hasDrink = false;
            foreach (var action in actions)
            {
                if (action.ActionType == "eat") hasEat = true;
                if (action.ActionType == "drink") hasDrink = true;
            }
            Assert.IsTrue(hasEat, "Should have eat action from ActionDatabase");
            Assert.IsTrue(hasDrink, "Should have drink action from ActionDatabase");
        }
        
        [Test]
        public void GOAPAgent_FallsBackToHardcodedWhenNoDatabaseFound()
        {
            // Clear the ActionDatabase reference
            var actionDBField = typeof(GOAPAgent).GetField("actionDatabase", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            actionDBField.SetValue(goapAgent, null);
            
            // Setup without ActionDatabase
            var setupMethod = typeof(GOAPAgent).GetMethod("SetupGoalsAndActions", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.DoesNotThrow(() => setupMethod.Invoke(goapAgent, null));
            
            // Verify agent has fallback hardcoded actions
            var actionsField = typeof(GOAPAgent).GetField("availableActions", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var actions = actionsField.GetValue(goapAgent) as List<IAction>;
            
            Assert.IsNotNull(actions);
            Assert.IsTrue(actions.Count >= 5, "Should have hardcoded fallback actions");
            
            // Verify we have the expected hardcoded action types
            var actionTypes = new HashSet<string>();
            foreach (var action in actions)
            {
                actionTypes.Add(action.ActionType);
            }
            
            Assert.IsTrue(actionTypes.Contains("noop"));
            Assert.IsTrue(actionTypes.Contains("eat"));
            Assert.IsTrue(actionTypes.Contains("drink"));
            Assert.IsTrue(actionTypes.Contains("Sleep"));  // Note: hardcoded actions use different naming
            Assert.IsTrue(actionTypes.Contains("Play"));
        }
        
        [Test]
        public void GOAPAgent_ValidatesActionDatabaseWhenDebuggingEnabled()
        {
            // Enable debugging
            var debugField = typeof(GOAPAgent).GetField("enableDebug", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            debugField.SetValue(goapAgent, true);
            
            // This should call validation without throwing
            var setupMethod = typeof(GOAPAgent).GetMethod("SetupGoalsAndActions", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.DoesNotThrow(() => setupMethod.Invoke(goapAgent, null));
            
            // Validation should have been called (verified by no exceptions)
            // The actual validation output is logged to Debug.Log
        }
        
        [Test]
        public void GOAPAgent_HandlesEmptyActionDatabase()
        {
            // Create empty ActionDatabase
            var emptyDB = ScriptableObject.CreateInstance<ActionDatabase>();
            var actionDBField = typeof(GOAPAgent).GetField("actionDatabase", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            actionDBField.SetValue(goapAgent, emptyDB);
            
            // Setup should fallback to hardcoded actions
            var setupMethod = typeof(GOAPAgent).GetMethod("SetupGoalsAndActions", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.DoesNotThrow(() => setupMethod.Invoke(goapAgent, null));
            
            // Should have fallback actions
            var actionsField = typeof(GOAPAgent).GetField("availableActions", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var actions = actionsField.GetValue(goapAgent) as List<IAction>;
            
            Assert.IsNotNull(actions);
            Assert.IsTrue(actions.Count > 0, "Should fallback to hardcoded actions when ActionDatabase is empty");
            
            Object.DestroyImmediate(emptyDB);
        }
        
        [Test]
        public void GOAPAgent_AlwaysHasNoOpAction()
        {
            // Test both scenarios - with and without ActionDatabase
            
            // Scenario 1: With ActionDatabase (no NoOp in DB)
            var setupMethod = typeof(GOAPAgent).GetMethod("SetupGoalsAndActions", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            setupMethod.Invoke(goapAgent, null);
            
            var actionsField = typeof(GOAPAgent).GetField("availableActions", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var actions = actionsField.GetValue(goapAgent) as List<IAction>;
            
            bool hasNoOp = false;
            foreach (var action in actions)
            {
                if (action.ActionType == "noop")
                {
                    hasNoOp = true;
                    break;
                }
            }
            Assert.IsTrue(hasNoOp, "Agent should always have NoOp action available");
            
            // Scenario 2: Without ActionDatabase
            var actionDBField = typeof(GOAPAgent).GetField("actionDatabase", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            actionDBField.SetValue(goapAgent, null);
            
            setupMethod.Invoke(goapAgent, null);
            actions = actionsField.GetValue(goapAgent) as List<IAction>;
            
            hasNoOp = false;
            foreach (var action in actions)
            {
                if (action.ActionType == "noop")
                {
                    hasNoOp = true;
                    break;
                }
            }
            Assert.IsTrue(hasNoOp, "Agent should have NoOp action even with fallback actions");
        }
        
        [Test]
        public void Integration_FullAgentSetupWithBothDatabases()
        {
            // Test complete integration with both GoalDatabase and ActionDatabase
            var setupMethod = typeof(GOAPAgent).GetMethod("SetupGoalsAndActions", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            // Should complete without errors
            Assert.DoesNotThrow(() => setupMethod.Invoke(goapAgent, null));
            
            // Verify both goals and actions are loaded
            var goalsField = typeof(GOAPAgent).GetField("availableGoals", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var actionsField = typeof(GOAPAgent).GetField("availableActions", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            var goals = goalsField.GetValue(goapAgent) as List<IGoal>;
            var actions = actionsField.GetValue(goapAgent) as List<IAction>;
            
            Assert.IsNotNull(goals);
            Assert.IsNotNull(actions);
            Assert.IsTrue(goals.Count > 0);
            Assert.IsTrue(actions.Count > 0);
            
            Debug.Log($"Agent setup complete: {goals.Count} goals, {actions.Count} actions");
        }
    }
}