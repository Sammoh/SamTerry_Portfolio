using NUnit.Framework;
using UnityEngine;
using Sammoh.GOAP;
using System.Collections.Generic;

namespace Sammoh.GOAP.Tests
{
    /// <summary>
    /// Tests for the new idle behavior actions: WanderAction, InvestigateAction, and SayRandomLineAction
    /// </summary>
    public class IdleBehaviorTests
    {
        private BasicAgentState agentState;
        private BasicWorldState worldState;
        private GameObject agentGameObject;
        private Transform agentTransform;

        [SetUp]
        public void Setup()
        {
            // Create test agent object
            agentGameObject = new GameObject("TestAgent");
            agentTransform = agentGameObject.transform;
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
            if (agentGameObject != null)
                Object.DestroyImmediate(agentGameObject);
            if (worldState != null)
                Object.DestroyImmediate(worldState.gameObject);
        }

        [Test]
        public void WanderAction_CheckPreconditions_ReturnsTrueWithValidAgent()
        {
            // Arrange
            var wanderAction = new WanderAction();
            wanderAction.InjectAgent(agentTransform);

            // Act
            bool result = wanderAction.CheckPreconditions(agentState, worldState);

            // Assert
            Assert.IsTrue(result, "WanderAction should return true for preconditions when agent is valid");
        }

        [Test]
        public void WanderAction_GetEffects_ReturnsCorrectEffects()
        {
            // Arrange
            var wanderAction = new WanderAction();

            // Act
            var effects = wanderAction.GetEffects();

            // Assert
            Assert.IsTrue(effects.ContainsKey("wandering"), "WanderAction should have 'wandering' effect");
            Assert.IsTrue(effects.ContainsKey("idle"), "WanderAction should have 'idle' effect");
            Assert.AreEqual(true, effects["wandering"], "Wandering effect should be true");
            Assert.AreEqual(true, effects["idle"], "Idle effect should be true");
        }

        [Test]
        public void WanderAction_StartExecution_SetsIsExecutingToTrue()
        {
            // Arrange
            var wanderAction = new WanderAction();
            wanderAction.InjectAgent(agentTransform);

            // Act
            wanderAction.StartExecution(agentState, worldState);

            // Assert
            Assert.IsTrue(wanderAction.IsExecuting, "WanderAction should be executing after StartExecution");
        }

        [Test]
        public void InvestigateAction_CheckPreconditions_ReturnsTrueWithValidAgent()
        {
            // Arrange
            var investigateAction = new InvestigateAction();
            investigateAction.InjectAgent(agentTransform);

            // Act
            bool result = investigateAction.CheckPreconditions(agentState, worldState);

            // Assert
            Assert.IsTrue(result, "InvestigateAction should return true for preconditions when agent is valid");
        }

        [Test]
        public void InvestigateAction_GetEffects_ReturnsCorrectEffects()
        {
            // Arrange
            var investigateAction = new InvestigateAction();

            // Act
            var effects = investigateAction.GetEffects();

            // Assert
            Assert.IsTrue(effects.ContainsKey("investigating"), "InvestigateAction should have 'investigating' effect");
            Assert.IsTrue(effects.ContainsKey("idle"), "InvestigateAction should have 'idle' effect");
            Assert.AreEqual(true, effects["investigating"], "Investigating effect should be true");
            Assert.AreEqual(true, effects["idle"], "Idle effect should be true");
        }

        [Test]
        public void InvestigateAction_UpdateExecution_CompletesAfterDuration()
        {
            // Arrange
            var investigateAction = new InvestigateAction(5f, 1f); // 5m radius, 1s duration
            investigateAction.InjectAgent(agentTransform);
            investigateAction.StartExecution(agentState, worldState);

            // Act
            var result1 = investigateAction.UpdateExecution(agentState, worldState, 0.5f);
            var result2 = investigateAction.UpdateExecution(agentState, worldState, 0.6f);

            // Assert
            Assert.AreEqual(ActionResult.Running, result1, "Action should be running before duration expires");
            Assert.AreEqual(ActionResult.Success, result2, "Action should succeed after duration expires");
        }

        [Test]
        public void SayRandomLineAction_CheckPreconditions_AlwaysReturnsTrue()
        {
            // Arrange
            var sayAction = new SayRandomLineAction();

            // Act
            bool result = sayAction.CheckPreconditions(agentState, worldState);

            // Assert
            Assert.IsTrue(result, "SayRandomLineAction should always return true for preconditions");
        }

        [Test]
        public void SayRandomLineAction_GetEffects_ReturnsCorrectEffects()
        {
            // Arrange
            var sayAction = new SayRandomLineAction();

            // Act
            var effects = sayAction.GetEffects();

            // Assert
            Assert.IsTrue(effects.ContainsKey("talking"), "SayRandomLineAction should have 'talking' effect");
            Assert.IsTrue(effects.ContainsKey("idle"), "SayRandomLineAction should have 'idle' effect");
            Assert.AreEqual(true, effects["talking"], "Talking effect should be true");
            Assert.AreEqual(true, effects["idle"], "Idle effect should be true");
        }

        [Test]
        public void SayRandomLineAction_UpdateExecution_CompletesAfterDuration()
        {
            // Arrange
            var sayAction = new SayRandomLineAction(1f); // 1 second duration
            sayAction.InjectAgent(agentTransform);
            sayAction.StartExecution(agentState, worldState);

            // Act
            var result1 = sayAction.UpdateExecution(agentState, worldState, 0.5f);
            var result2 = sayAction.UpdateExecution(agentState, worldState, 0.6f);

            // Assert
            Assert.AreEqual(ActionResult.Running, result1, "Action should be running before duration expires");
            Assert.AreEqual(ActionResult.Success, result2, "Action should succeed after duration expires");
        }

        [Test]
        public void AllIdleActions_HaveLowCost()
        {
            // Arrange
            var wanderAction = new WanderAction();
            var investigateAction = new InvestigateAction();
            var sayAction = new SayRandomLineAction();

            // Assert
            Assert.LessOrEqual(wanderAction.Cost, 1f, "WanderAction should have low cost for idle activity");
            Assert.LessOrEqual(investigateAction.Cost, 1f, "InvestigateAction should have low cost for idle activity");
            Assert.LessOrEqual(sayAction.Cost, 1f, "SayRandomLineAction should have low cost for idle activity");
        }

        [Test]
        public void IdleGoal_HasIncreasedPriorityWhenNoNeeds()
        {
            // Arrange
            var idleGoal = new IdleGoal();
            agentState.SetNeed("hunger", 0f);
            agentState.SetNeed("thirst", 0f);
            agentState.SetNeed("sleep", 0f);

            // Act
            float priority = idleGoal.CalculatePriority(agentState, worldState);

            // Assert
            Assert.Greater(priority, 0.3f, "IdleGoal should have decent priority when no needs are present");
            Assert.LessOrEqual(priority, 1f, "IdleGoal priority should not exceed maximum");
        }

        [Test]
        public void IdleGoal_HasLowerPriorityWithHighNeeds()
        {
            // Arrange
            var idleGoal = new IdleGoal();
            agentState.SetNeed("hunger", 0.9f);
            agentState.SetNeed("thirst", 0.8f);

            // Act
            float priority = idleGoal.CalculatePriority(agentState, worldState);

            // Assert
            Assert.Greater(priority, 0f, "IdleGoal should still have some priority even with high needs");
            Assert.Less(priority, 0.5f, "IdleGoal should have lower priority when needs are high");
        }
    }
}