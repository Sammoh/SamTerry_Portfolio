using NUnit.Framework;
using UnityEngine;
using Sammoh.GOAP;
using System.Collections.Generic;

namespace Sammoh.GOAP.Tests
{
    /// <summary>
    /// Tests for ScriptableObject-based actions to verify they work correctly
    /// </summary>
    public class GOAPScriptableObjectActionTests
    {
        private BasicAgentState agentState;
        private BasicWorldState worldState;
        
        [SetUp]
        public void Setup()
        {
            // Create test world state
            var worldGO = new GameObject("TestWorldState");
            worldState = worldGO.AddComponent<BasicWorldState>();
            
            // Create basic agent state
            agentState = new BasicAgentState();
        }
        
        [TearDown]
        public void TearDown()
        {
            if (worldState != null)
                Object.DestroyImmediate(worldState.gameObject);
        }
        
        [Test]
        public void EatActionSO_HasCorrectConfiguration()
        {
            var eatAction = ScriptableObject.CreateInstance<EatActionSO>();
            
            // Verify default configuration
            Assert.AreEqual("eat", eatAction.ActionType);
            Assert.AreEqual(1f, eatAction.Cost);
            Assert.IsFalse(eatAction.IsExecuting);
        }
        
        [Test]
        public void EatActionSO_CheckPreconditions_RequiresAtFood()
        {
            var eatAction = ScriptableObject.CreateInstance<EatActionSO>();
            
            // Should fail when not at food
            worldState.SetFact("at_food", false);
            Assert.IsFalse(eatAction.CheckPreconditions(agentState, worldState));
            
            // Should pass when at food
            worldState.SetFact("at_food", true);
            Assert.IsTrue(eatAction.CheckPreconditions(agentState, worldState));
        }
        
        [Test]
        public void EatActionSO_GetEffects_ReturnsCorrectEffects()
        {
            var eatAction = ScriptableObject.CreateInstance<EatActionSO>();
            
            var effects = eatAction.GetEffects();
            
            Assert.IsNotNull(effects);
            Assert.IsTrue(effects.ContainsKey("need_hunger"));
            Assert.IsTrue(effects.ContainsKey("at_food"));
            Assert.AreEqual(0f, effects["need_hunger"]);
            Assert.AreEqual(false, effects["at_food"]);
        }
        
        [Test]
        public void DrinkActionSO_HasCorrectConfiguration()
        {
            var drinkAction = ScriptableObject.CreateInstance<DrinkActionSO>();
            
            // Verify default configuration
            Assert.AreEqual("drink", drinkAction.ActionType);
            Assert.AreEqual(1f, drinkAction.Cost);
            Assert.IsFalse(drinkAction.IsExecuting);
        }
        
        [Test]
        public void DrinkActionSO_CheckPreconditions_RequiresAtWater()
        {
            var drinkAction = ScriptableObject.CreateInstance<DrinkActionSO>();
            
            // Should fail when not at water
            worldState.SetFact("at_water", false);
            Assert.IsFalse(drinkAction.CheckPreconditions(agentState, worldState));
            
            // Should pass when at water
            worldState.SetFact("at_water", true);
            Assert.IsTrue(drinkAction.CheckPreconditions(agentState, worldState));
        }
        
        [Test]
        public void PlayActionSO_ExecutionCompletes()
        {
            var playAction = ScriptableObject.CreateInstance<PlayActionSO>();
            
            // Set preconditions
            worldState.SetFact("at_toy", true);
            agentState.SetNeed("play", 1f);
            
            // Start execution
            Assert.IsTrue(playAction.CheckPreconditions(agentState, worldState));
            playAction.StartExecution(agentState, worldState);
            Assert.IsTrue(playAction.IsExecuting);
            
            // Simulate execution for duration (2 seconds)
            var result = playAction.UpdateExecution(agentState, worldState, 2.1f);
            Assert.AreEqual(ActionResult.Success, result);
            
            // Apply effects
            playAction.ApplyEffects(agentState, worldState);
            Assert.AreEqual(0f, agentState.GetNeed("play"));
            Assert.IsFalse(worldState.GetFact("at_toy"));
        }
        
        [Test]
        public void SleepActionSO_ExecutionCompletes()
        {
            var sleepAction = ScriptableObject.CreateInstance<SleepActionSO>();
            
            // Set preconditions
            worldState.SetFact("at_bed", true);
            agentState.SetNeed("sleep", 1f);
            
            // Start execution
            Assert.IsTrue(sleepAction.CheckPreconditions(agentState, worldState));
            sleepAction.StartExecution(agentState, worldState);
            Assert.IsTrue(sleepAction.IsExecuting);
            
            // Simulate execution for duration (3 seconds)
            var result = sleepAction.UpdateExecution(agentState, worldState, 3.1f);
            Assert.AreEqual(ActionResult.Success, result);
            
            // Apply effects
            sleepAction.ApplyEffects(agentState, worldState);
            Assert.AreEqual(0f, agentState.GetNeed("sleep"));
            Assert.IsFalse(worldState.GetFact("at_bed"));
        }
        
        [Test]
        public void ActionDatabase_CanStoreAndRetrieveActions()
        {
            var actionDatabase = ScriptableObject.CreateInstance<ActionDatabase>();
            
            // Create test actions
            var eatAction = ScriptableObject.CreateInstance<EatActionSO>();
            var drinkAction = ScriptableObject.CreateInstance<DrinkActionSO>();
            
            // Add actions to database (would normally be done in Inspector)
            var needReductionActions = new List<NeedReductionActionSO> { eatAction, drinkAction };
            
            // Use reflection to set the private field for testing
            var needReductionActionsField = typeof(ActionDatabase).GetField("needReductionActions", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            needReductionActionsField?.SetValue(actionDatabase, needReductionActions);
            
            // Test retrieval
            var allActions = actionDatabase.GetAllActions();
            Assert.AreEqual(2, allActions.Count);
            
            var foundEatAction = actionDatabase.FindByType("eat");
            Assert.IsNotNull(foundEatAction);
            Assert.AreEqual("eat", foundEatAction.ActionType);
            
            var foundDrinkAction = actionDatabase.FindByType("drink");
            Assert.IsNotNull(foundDrinkAction);
            Assert.AreEqual("drink", foundDrinkAction.ActionType);
        }
    }
}