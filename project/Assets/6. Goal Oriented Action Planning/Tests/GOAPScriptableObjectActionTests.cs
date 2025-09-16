using NUnit.Framework;
using UnityEngine;
using Sammoh.GOAP;
using System.Collections.Generic;

namespace Sammoh.GOAP.Tests
{
    /// <summary>
    /// Comprehensive tests for the ScriptableObject-based action system
    /// </summary>
    public class GOAPScriptableObjectActionTests
    {
        private BasicAgentState agentState;
        private BasicWorldState worldState;
        private ActionDatabase actionDatabase;
        private EatActionSO eatAction;
        private DrinkActionSO drinkAction;
        private SleepActionSO sleepAction;
        private PlayActionSO playAction;
        
        [SetUp]
        public void Setup()
        {
            // Create test world state
            var worldGO = new GameObject("TestWorldState");
            worldState = worldGO.AddComponent<BasicWorldState>();
            
            // Create basic agent state
            agentState = new BasicAgentState();
            
            // Create ActionDatabase
            actionDatabase = ScriptableObject.CreateInstance<ActionDatabase>();
            
            // Create ScriptableObject actions
            eatAction = ScriptableObject.CreateInstance<EatActionSO>();
            drinkAction = ScriptableObject.CreateInstance<DrinkActionSO>();
            sleepAction = ScriptableObject.CreateInstance<SleepActionSO>();
            playAction = ScriptableObject.CreateInstance<PlayActionSO>();
            
            // Add actions to database
            actionDatabase.AddAction(eatAction);
            actionDatabase.AddAction(drinkAction);
            actionDatabase.AddAction(sleepAction);
            actionDatabase.AddAction(playAction);
        }
        
        [TearDown]
        public void TearDown()
        {
            if (worldState != null)
                Object.DestroyImmediate(worldState.gameObject);
            if (actionDatabase != null)
                Object.DestroyImmediate(actionDatabase);
            if (eatAction != null)
                Object.DestroyImmediate(eatAction);
            if (drinkAction != null)
                Object.DestroyImmediate(drinkAction);
            if (sleepAction != null)
                Object.DestroyImmediate(sleepAction);
            if (playAction != null)
                Object.DestroyImmediate(playAction);
        }
        
        [Test]
        public void ActionDatabase_CreatesSuccessfully()
        {
            Assert.IsNotNull(actionDatabase);
            Assert.AreEqual(4, actionDatabase.Actions.Count);
        }
        
        [Test]
        public void ActionDatabase_ValidatesCorrectly()
        {
            var issues = actionDatabase.ValidateActions();
            Assert.AreEqual(0, issues.Count, "ActionDatabase should validate without issues");
        }
        
        [Test]
        public void NeedReductionActionSO_HasCorrectDefaults()
        {
            Assert.AreEqual("eat", eatAction.ActionType);
            Assert.AreEqual(1f, eatAction.Cost);
            Assert.IsFalse(eatAction.IsExecuting);
            
            Assert.AreEqual("drink", drinkAction.ActionType);
            Assert.AreEqual("sleep", sleepAction.ActionType);
            Assert.AreEqual("play", playAction.ActionType);
        }
        
        [Test]
        public void EatAction_ChecksPreconditions()
        {
            // Should fail without being at food
            Assert.IsFalse(eatAction.CheckPreconditions(agentState, worldState));
            
            // Should pass when at food
            worldState.SetFact("at_food", true);
            Assert.IsTrue(eatAction.CheckPreconditions(agentState, worldState));
        }
        
        [Test]
        public void EatAction_HasCorrectEffects()
        {
            var effects = eatAction.GetEffects();
            Assert.IsNotNull(effects);
            Assert.IsTrue(effects.ContainsKey("need_hunger"));
            Assert.IsTrue(effects.ContainsKey("at_food"));
            Assert.AreEqual(0f, effects["need_hunger"]);
            Assert.AreEqual(false, effects["at_food"]);
        }
        
        [Test]
        public void EatAction_ExecutesCorrectly()
        {
            // Setup preconditions
            worldState.SetFact("at_food", true);
            agentState.SetNeed("hunger", 0.8f);
            
            // Start execution
            eatAction.StartExecution(agentState, worldState);
            Assert.IsTrue(eatAction.IsExecuting);
            
            // Update execution (should still be running)
            var result = eatAction.UpdateExecution(agentState, worldState, 1f);
            Assert.AreEqual(ActionResult.Running, result);
            
            // Complete execution
            result = eatAction.UpdateExecution(agentState, worldState, 2f);
            Assert.AreEqual(ActionResult.Success, result);
            Assert.IsFalse(eatAction.IsExecuting);
        }
        
        [Test]
        public void EatAction_AppliesEffectsCorrectly()
        {
            // Setup
            worldState.SetFact("at_food", true);
            agentState.SetNeed("hunger", 0.8f);
            
            // Apply effects
            eatAction.ApplyEffects(agentState, worldState);
            
            // Verify effects
            Assert.AreEqual(0f, agentState.GetNeed("hunger"));
            Assert.IsFalse(worldState.GetFact("at_food"));
        }
        
        [Test]
        public void DrinkAction_WorksWithThirst()
        {
            // Setup preconditions
            worldState.SetFact("at_water", true);
            agentState.SetNeed("thirst", 0.7f);
            
            // Test preconditions
            Assert.IsTrue(drinkAction.CheckPreconditions(agentState, worldState));
            
            // Apply effects
            drinkAction.ApplyEffects(agentState, worldState);
            
            // Verify thirst is reduced
            Assert.AreEqual(0f, agentState.GetNeed("thirst"));
            Assert.IsFalse(worldState.GetFact("at_water"));
        }
        
        [Test]
        public void SleepAction_WorksWithSleep()
        {
            // Setup preconditions
            worldState.SetFact("at_bed", true);
            agentState.SetNeed("sleep", 0.9f);
            
            // Test preconditions
            Assert.IsTrue(sleepAction.CheckPreconditions(agentState, worldState));
            
            // Apply effects
            sleepAction.ApplyEffects(agentState, worldState);
            
            // Verify sleep need is reduced
            Assert.AreEqual(0f, agentState.GetNeed("sleep"));
            Assert.IsFalse(worldState.GetFact("at_bed"));
        }
        
        [Test]
        public void PlayAction_WorksWithPlay()
        {
            // Setup preconditions
            worldState.SetFact("at_toy", true);
            agentState.SetNeed("play", 0.6f);
            
            // Test preconditions
            Assert.IsTrue(playAction.CheckPreconditions(agentState, worldState));
            
            // Apply effects
            playAction.ApplyEffects(agentState, worldState);
            
            // Verify play need is reduced
            Assert.AreEqual(0f, agentState.GetNeed("play"));
            Assert.IsFalse(worldState.GetFact("at_toy"));
        }
        
        [Test]
        public void ActionDatabase_CanAddAndRemoveActions()
        {
            var testAction = ScriptableObject.CreateInstance<EatActionSO>();
            
            // Test adding
            Assert.IsTrue(actionDatabase.AddAction(testAction));
            Assert.AreEqual(5, actionDatabase.Actions.Count);
            
            // Test duplicate adding (should fail)
            Assert.IsFalse(actionDatabase.AddAction(testAction));
            Assert.AreEqual(5, actionDatabase.Actions.Count);
            
            // Test removing
            Assert.IsTrue(actionDatabase.RemoveAction(testAction));
            Assert.AreEqual(4, actionDatabase.Actions.Count);
            
            // Test removing non-existent (should fail)
            Assert.IsFalse(actionDatabase.RemoveAction(testAction));
            
            Object.DestroyImmediate(testAction);
        }
        
        [Test]
        public void ActionDatabase_GetActionsOfType_ReturnsCorrectType()
        {
            var eatActions = actionDatabase.GetActionsOfType<EatActionSO>();
            Assert.AreEqual(1, eatActions.Count);
            Assert.AreEqual(eatAction, eatActions[0]);
            
            var drinkActions = actionDatabase.GetActionsOfType<DrinkActionSO>();
            Assert.AreEqual(1, drinkActions.Count);
            Assert.AreEqual(drinkAction, drinkActions[0]);
        }
        
        [Test]
        public void ActionDatabase_ClearAll_RemovesAllActions()
        {
            Assert.AreEqual(4, actionDatabase.Actions.Count);
            
            actionDatabase.ClearAll();
            
            Assert.AreEqual(0, actionDatabase.Actions.Count);
        }
        
        [Test]
        public void ActionDatabase_ValidatesNullAndInvalidActions()
        {
            // Create database with problematic actions
            var testDB = ScriptableObject.CreateInstance<ActionDatabase>();
            
            // Add a null action (this should be caught by AddAction)
            Assert.IsFalse(testDB.AddAction(null));
            
            // Add a non-IAction ScriptableObject
            var invalidAction = ScriptableObject.CreateInstance<ScriptableObject>();
            Assert.IsFalse(testDB.AddAction(invalidAction));
            
            var issues = testDB.ValidateActions();
            Assert.IsTrue(issues.Count > 0, "Should detect empty database");
            Assert.IsTrue(issues[0].Contains("empty"));
            
            Object.DestroyImmediate(testDB);
            Object.DestroyImmediate(invalidAction);
        }
        
        [Test]
        public void ActionExecutionCancellation_WorksCorrectly()
        {
            worldState.SetFact("at_food", true);
            
            eatAction.StartExecution(agentState, worldState);
            Assert.IsTrue(eatAction.IsExecuting);
            
            eatAction.CancelExecution();
            Assert.IsFalse(eatAction.IsExecuting);
            
            // After cancellation, UpdateExecution should fail
            var result = eatAction.UpdateExecution(agentState, worldState, 1f);
            Assert.AreEqual(ActionResult.Failed, result);
        }
        
        [Test]
        public void BackwardCompatibility_OriginalActionStillWork()
        {
            // Test that original hardcoded actions still work alongside SO actions
            var hardcodedEat = new EatAction();
            var hardcodedDrink = new DrinkAction();
            
            // Setup conditions for both
            worldState.SetFact("at_food", true);
            worldState.SetFact("at_water", true);
            
            // Both should have same basic behavior
            Assert.IsTrue(hardcodedEat.CheckPreconditions(agentState, worldState));
            Assert.IsTrue(eatAction.CheckPreconditions(agentState, worldState));
            
            Assert.IsTrue(hardcodedDrink.CheckPreconditions(agentState, worldState));
            Assert.IsTrue(drinkAction.CheckPreconditions(agentState, worldState));
            
            // Both should have similar effects structure
            var hardcodedEffects = hardcodedEat.GetEffects();
            var soEffects = eatAction.GetEffects();
            
            Assert.IsTrue(hardcodedEffects.ContainsKey("need_hunger"));
            Assert.IsTrue(soEffects.ContainsKey("need_hunger"));
        }
        
        [Test]
        public void ConfigurableParameters_CanBeModified()
        {
            // Test that SO actions can be configured differently than hardcoded defaults
            var customEat = ScriptableObject.CreateInstance<EatActionSO>();
            
            // Access protected fields through reflection for testing
            var actionTypeField = typeof(NeedReductionActionSO).GetField("actionType", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var costField = typeof(NeedReductionActionSO).GetField("cost", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var durationField = typeof(NeedReductionActionSO).GetField("duration", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            // Modify configuration
            actionTypeField.SetValue(customEat, "custom_eat");
            costField.SetValue(customEat, 2.5f);
            durationField.SetValue(customEat, 5f);
            
            // Verify custom configuration
            Assert.AreEqual("custom_eat", customEat.ActionType);
            Assert.AreEqual(2.5f, customEat.Cost);
            
            Object.DestroyImmediate(customEat);
        }
    }
}