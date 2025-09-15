using NUnit.Framework;
using UnityEngine;
using Sammoh.GOAP;
using UnityEngine.AI;
using System.Collections.Generic;

namespace Sammoh.GOAP.Tests
{
    /// <summary>
    /// Unit tests for the new idle actions: WanderAction, InvestigateAction, and SayRandomLineAction
    /// </summary>
    public class IdleActionsTests
    {
        private BasicAgentState agentState;
        private BasicWorldState worldState;
        private GameObject testAgent;
        private Transform agentTransform;
        
        [SetUp]
        public void Setup()
        {
            // Create test agent
            testAgent = new GameObject("TestAgent");
            agentTransform = testAgent.transform;
            agentTransform.position = Vector3.zero;
            
            // Create test world state
            var worldGO = new GameObject("TestWorldState");
            worldState = worldGO.AddComponent<BasicWorldState>();
            
            // Create basic agent state
            agentState = new BasicAgentState();
        }
        
        [TearDown]
        public void TearDown()
        {
            if (testAgent != null)
                Object.DestroyImmediate(testAgent);
            if (worldState != null)
                Object.DestroyImmediate(worldState.gameObject);
        }
        
        #region WanderAction Tests
        
        [Test]
        public void WanderAction_HasCorrectProperties()
        {
            var wanderAction = new WanderAction();
            
            Assert.AreEqual("wander", wanderAction.ActionType);
            Assert.AreEqual(0.5f, wanderAction.Cost, 0.01f);
            Assert.IsFalse(wanderAction.IsExecuting);
        }
        
        [Test]
        public void WanderAction_CanExecuteWithValidAgent()
        {
            var wanderAction = new WanderAction();
            wanderAction.InjectAgent(agentTransform);
            
            Assert.IsTrue(wanderAction.CheckPreconditions(agentState, worldState));
        }
        
        [Test]
        public void WanderAction_CannotExecuteWithoutAgent()
        {
            var wanderAction = new WanderAction();
            // Don't inject agent
            
            Assert.IsFalse(wanderAction.CheckPreconditions(agentState, worldState));
        }
        
        [Test]
        public void WanderAction_HasCorrectEffects()
        {
            var wanderAction = new WanderAction();
            var effects = wanderAction.GetEffects();
            
            Assert.IsTrue(effects.ContainsKey("idle"));
            Assert.AreEqual(true, effects["idle"]);
        }
        
        [Test]
        public void WanderAction_StartsExecution()
        {
            var wanderAction = new WanderAction();
            wanderAction.InjectAgent(agentTransform);
            
            wanderAction.StartExecution(agentState, worldState);
            
            Assert.IsTrue(wanderAction.IsExecuting);
        }
        
        [Test]
        public void WanderAction_UpdateExecutionProgresses()
        {
            var wanderAction = new WanderAction(cost: 0.5f, radius: 5f, speed: 2f, wanderDuration: 2f);
            wanderAction.InjectAgent(agentTransform);
            
            wanderAction.StartExecution(agentState, worldState);
            
            // Update for less than duration - should be running
            var result1 = wanderAction.UpdateExecution(agentState, worldState, 1f);
            Assert.AreEqual(ActionResult.Running, result1);
            Assert.IsTrue(wanderAction.IsExecuting);
            
            // Update for remaining duration - should complete
            var result2 = wanderAction.UpdateExecution(agentState, worldState, 1.5f);
            Assert.AreEqual(ActionResult.Success, result2);
            Assert.IsFalse(wanderAction.IsExecuting);
        }
        
        [Test]
        public void WanderAction_CanBeCancelled()
        {
            var wanderAction = new WanderAction();
            wanderAction.InjectAgent(agentTransform);
            
            wanderAction.StartExecution(agentState, worldState);
            Assert.IsTrue(wanderAction.IsExecuting);
            
            wanderAction.CancelExecution();
            Assert.IsFalse(wanderAction.IsExecuting);
        }
        
        [Test]
        public void WanderAction_AppliesEffectsCorrectly()
        {
            var wanderAction = new WanderAction();
            
            wanderAction.ApplyEffects(agentState, worldState);
            
            Assert.IsTrue(worldState.GetFact("idle"));
        }
        
        #endregion
        
        #region InvestigateAction Tests
        
        [Test]
        public void InvestigateAction_HasCorrectProperties()
        {
            var investigateAction = new InvestigateAction();
            
            Assert.AreEqual("investigate", investigateAction.ActionType);
            Assert.AreEqual(0.3f, investigateAction.Cost, 0.01f);
            Assert.IsFalse(investigateAction.IsExecuting);
        }
        
        [Test]
        public void InvestigateAction_CanExecuteWithValidAgent()
        {
            var investigateAction = new InvestigateAction();
            investigateAction.InjectAgent(agentTransform);
            
            Assert.IsTrue(investigateAction.CheckPreconditions(agentState, worldState));
        }
        
        [Test]
        public void InvestigateAction_CannotExecuteWithoutAgent()
        {
            var investigateAction = new InvestigateAction();
            // Don't inject agent
            
            Assert.IsFalse(investigateAction.CheckPreconditions(agentState, worldState));
        }
        
        [Test]
        public void InvestigateAction_HasCorrectEffects()
        {
            var investigateAction = new InvestigateAction();
            var effects = investigateAction.GetEffects();
            
            Assert.IsTrue(effects.ContainsKey("idle"));
            Assert.AreEqual(true, effects["idle"]);
        }
        
        [Test]
        public void InvestigateAction_StartsExecution()
        {
            var investigateAction = new InvestigateAction();
            investigateAction.InjectAgent(agentTransform);
            
            investigateAction.StartExecution(agentState, worldState);
            
            Assert.IsTrue(investigateAction.IsExecuting);
        }
        
        [Test]
        public void InvestigateAction_UpdateExecutionProgresses()
        {
            var investigateAction = new InvestigateAction(cost: 0.3f, radius: 5f, investigateDuration: 2f);
            investigateAction.InjectAgent(agentTransform);
            
            investigateAction.StartExecution(agentState, worldState);
            
            // Update for less than duration - should be running
            var result1 = investigateAction.UpdateExecution(agentState, worldState, 1f);
            Assert.AreEqual(ActionResult.Running, result1);
            Assert.IsTrue(investigateAction.IsExecuting);
            
            // Update for remaining duration - should complete
            var result2 = investigateAction.UpdateExecution(agentState, worldState, 1.5f);
            Assert.AreEqual(ActionResult.Success, result2);
            Assert.IsFalse(investigateAction.IsExecuting);
        }
        
        [Test]
        public void InvestigateAction_CanBeCancelled()
        {
            var investigateAction = new InvestigateAction();
            investigateAction.InjectAgent(agentTransform);
            
            investigateAction.StartExecution(agentState, worldState);
            Assert.IsTrue(investigateAction.IsExecuting);
            
            investigateAction.CancelExecution();
            Assert.IsFalse(investigateAction.IsExecuting);
        }
        
        [Test]
        public void InvestigateAction_AppliesEffectsCorrectly()
        {
            var investigateAction = new InvestigateAction();
            
            investigateAction.ApplyEffects(agentState, worldState);
            
            Assert.IsTrue(worldState.GetFact("idle"));
        }
        
        #endregion
        
        #region SayRandomLineAction Tests
        
        [Test]
        public void SayRandomLineAction_HasCorrectProperties()
        {
            var sayAction = new SayRandomLineAction();
            
            Assert.AreEqual("say_random", sayAction.ActionType);
            Assert.AreEqual(0.1f, sayAction.Cost, 0.01f);
            Assert.IsFalse(sayAction.IsExecuting);
        }
        
        [Test]
        public void SayRandomLineAction_CanAlwaysExecute()
        {
            var sayAction = new SayRandomLineAction();
            
            Assert.IsTrue(sayAction.CheckPreconditions(agentState, worldState));
        }
        
        [Test]
        public void SayRandomLineAction_HasCorrectEffects()
        {
            var sayAction = new SayRandomLineAction();
            var effects = sayAction.GetEffects();
            
            Assert.IsTrue(effects.ContainsKey("idle"));
            Assert.AreEqual(true, effects["idle"]);
        }
        
        [Test]
        public void SayRandomLineAction_StartsExecution()
        {
            var sayAction = new SayRandomLineAction();
            
            sayAction.StartExecution(agentState, worldState);
            
            Assert.IsTrue(sayAction.IsExecuting);
        }
        
        [Test]
        public void SayRandomLineAction_UpdateExecutionProgresses()
        {
            var sayAction = new SayRandomLineAction(cost: 0.1f, speakDuration: 2f);
            
            sayAction.StartExecution(agentState, worldState);
            
            // Update for less than duration - should be running
            var result1 = sayAction.UpdateExecution(agentState, worldState, 1f);
            Assert.AreEqual(ActionResult.Running, result1);
            Assert.IsTrue(sayAction.IsExecuting);
            
            // Update for remaining duration - should complete
            var result2 = sayAction.UpdateExecution(agentState, worldState, 1.5f);
            Assert.AreEqual(ActionResult.Success, result2);
            Assert.IsFalse(sayAction.IsExecuting);
        }
        
        [Test]
        public void SayRandomLineAction_CanBeCancelled()
        {
            var sayAction = new SayRandomLineAction();
            
            sayAction.StartExecution(agentState, worldState);
            Assert.IsTrue(sayAction.IsExecuting);
            
            sayAction.CancelExecution();
            Assert.IsFalse(sayAction.IsExecuting);
        }
        
        [Test]
        public void SayRandomLineAction_AppliesEffectsCorrectly()
        {
            var sayAction = new SayRandomLineAction();
            
            sayAction.ApplyEffects(agentState, worldState);
            
            Assert.IsTrue(worldState.GetFact("idle"));
        }
        
        #endregion
        
        #region Enhanced IdleGoal Tests
        
        [Test]
        public void IdleGoal_HasCorrectBaseProperties()
        {
            var idleGoal = new IdleGoal();
            
            Assert.AreEqual("idle", idleGoal.GoalType);
            Assert.AreEqual(0.3f, idleGoal.Priority, 0.01f);
        }
        
        [Test]
        public void IdleGoal_PrioritySystemWorksCorrectly()
        {
            var idleGoal = new IdleGoal();
            
            // Test with low needs (satisfied) - should get priority boost
            agentState.SetNeed("hunger", 0.1f);
            agentState.SetNeed("thirst", 0.1f);
            agentState.SetNeed("sleep", 0.1f);
            agentState.SetNeed("play", 0.1f);
            
            float priorityLowNeeds = idleGoal.CalculatePriority(agentState, worldState);
            Assert.Greater(priorityLowNeeds, 0.3f, "Priority should be boosted when needs are satisfied");
            Assert.LessOrEqual(priorityLowNeeds, 0.8f, "Priority should not exceed 0.8");
            
            // Test with high needs - should stay at base priority
            agentState.SetNeed("hunger", 0.9f);
            agentState.SetNeed("thirst", 0.9f);
            agentState.SetNeed("sleep", 0.9f);
            agentState.SetNeed("play", 0.9f);
            
            float priorityHighNeeds = idleGoal.CalculatePriority(agentState, worldState);
            Assert.AreEqual(0.3f, priorityHighNeeds, 0.05f, "Priority should stay at base when needs are high");
        }
        
        [Test]
        public void IdleGoal_PriorityNeverExceedsMaximum()
        {
            var idleGoal = new IdleGoal();
            
            // Set extremely low needs to test priority cap
            agentState.SetNeed("hunger", 0.0f);
            agentState.SetNeed("thirst", 0.0f);
            agentState.SetNeed("sleep", 0.0f);
            agentState.SetNeed("play", 0.0f);
            
            float priority = idleGoal.CalculatePriority(agentState, worldState);
            Assert.LessOrEqual(priority, 0.8f, "Priority should never exceed 0.8");
        }
        
        #endregion
        
        #region Integration Tests
        
        [Test]
        public void IdleActions_IntegrateWithGOAPSystem()
        {
            // Test that new actions can be used with the GOAP planner
            var planner = new BasicPlanner();
            var idleGoal = new IdleGoal();
            
            var actions = new List<IAction>
            {
                new WanderAction(),
                new InvestigateAction(), 
                new SayRandomLineAction()
            };
            
            // Inject agent for actions that need it
            ((WanderAction)actions[0]).InjectAgent(agentTransform);
            ((InvestigateAction)actions[1]).InjectAgent(agentTransform);
            
            // Should be able to create a plan
            var plan = planner.CreatePlan(idleGoal, agentState, worldState, actions);
            
            Assert.IsNotNull(plan, "Planner should create a plan with new idle actions");
            Assert.AreEqual(idleGoal, plan.Goal, "Plan should target the idle goal");
            Assert.Greater(plan.Actions.Count, 0, "Plan should contain actions");
        }
        
        [Test]
        public void IdleActions_HaveCorrectCostOrdering()
        {
            var wanderAction = new WanderAction();
            var investigateAction = new InvestigateAction();
            var sayAction = new SayRandomLineAction();
            
            // Verify cost ordering: SayRandomLine (0.1) < Investigate (0.3) < Wander (0.5)
            Assert.Less(sayAction.Cost, investigateAction.Cost, "SayRandomLine should have lower cost than Investigate");
            Assert.Less(investigateAction.Cost, wanderAction.Cost, "Investigate should have lower cost than Wander");
        }
        
        #endregion
    }
}