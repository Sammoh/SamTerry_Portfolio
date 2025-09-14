using NUnit.Framework;
using UnityEngine;
using Sammoh.GOAP;
using System.Collections.Generic;

namespace Sammoh.GOAP.Tests
{
    /// <summary>
    /// Core GOAP system tests to verify acceptance criteria
    /// </summary>
    public class GOAPSystemTests
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
            
            // Setup test actions and goals
            testActions = new List<IAction>
            {
                new NoOpAction(),
                new BarkAction()
            };
            
            testGoals = new List<IGoal>
            {
                new IdleGoal(),
                new CommunicationGoal()
            };
        }
        
        [TearDown]
        public void TearDown()
        {
            if (worldState != null)
                Object.DestroyImmediate(worldState.gameObject);
        }
        
        [Test]
        public void AgentState_InitializesWithBasicNeeds()
        {
            // Verify agent has basic needs
            Assert.IsTrue(agentState.GetNeed("hunger") >= 0f);
            Assert.IsTrue(agentState.GetNeed("thirst") >= 0f);
            Assert.IsTrue(agentState.GetNeed("sleep") >= 0f);
            Assert.IsTrue(agentState.GetNeed("play") >= 0f);
        }
        
        [Test]
        public void NoOpAction_CanExecute_ReturnsTrue()
        {
            var noOpAction = new NoOpAction();
            
            // No-op action should always be executable
            Assert.IsTrue(noOpAction.CheckPreconditions(agentState, worldState));
        }
        
        [Test]
        public void IdleGoal_CanSatisfy_ReturnsTrue()
        {
            var idleGoal = new IdleGoal();
            
            // Idle goal should always be satisfiable
            Assert.IsTrue(idleGoal.CanSatisfy(agentState, worldState));
        }
        
        [Test]
        public void Planner_CreatesPlanForIdleGoal()
        {
            var idleGoal = new IdleGoal();
            
            var plan = planner.CreatePlan(idleGoal, agentState, worldState, testActions);
            
            Assert.IsNotNull(plan);
            Assert.AreEqual(idleGoal, plan.Goal);
        }
        
        [Test]
        public void Executor_AcceptsPlan()
        {
            var idleGoal = new IdleGoal();
            var plan = planner.CreatePlan(idleGoal, agentState, worldState, testActions);
            
            executor.SetPlan(plan);
            
            Assert.AreEqual(plan, executor.CurrentPlan);
            Assert.AreEqual(idleGoal, executor.CurrentGoal);
        }
        
        [Test]
        public void Executor_ExecutesNoOpActionWithoutErrors()
        {
            var idleGoal = new IdleGoal();
            var plan = planner.CreatePlan(idleGoal, agentState, worldState, testActions);
            
            executor.SetPlan(plan);
            
            // Execute one update cycle
            executor.Update(agentState, worldState, 0.1f);
            
            // Should not throw errors and should be executing
            Assert.IsTrue(executor.IsExecuting || executor.IsComplete);
        }
        
        /// <summary>
        /// Core acceptance criteria test:
        /// "Play hits 'Run' and a dummy agent selects a trivial goal and executes a no-op action without errors"
        /// </summary>
        [Test]
        public void AcceptanceCriteria_AgentSelectsGoalAndExecutesNoOpAction()
        {
            // This test simulates the acceptance criteria
            
            // 1. Agent selects a trivial goal (idle)
            var idleGoal = new IdleGoal();
            Assert.IsTrue(idleGoal.CanSatisfy(agentState, worldState), "Agent should be able to select idle goal");
            
            // 2. Create plan for the goal
            var plan = planner.CreatePlan(idleGoal, agentState, worldState, testActions);
            Assert.IsNotNull(plan, "Planner should create a plan");
            Assert.Greater(plan.Actions.Count, 0, "Plan should contain at least one action");
            
            // 3. Execute the plan
            executor.SetPlan(plan);
            Assert.IsNotNull(executor.CurrentPlan, "Executor should accept the plan");
            
            // 4. Update executor multiple times to complete the no-op action
            bool actionCompleted = false;
            for (int i = 0; i < 100; i++) // Max 100 iterations to prevent infinite loop
            {
                executor.Update(agentState, worldState, 0.1f);
                
                if (executor.IsComplete)
                {
                    actionCompleted = true;
                    break;
                }
            }
            
            Assert.IsTrue(actionCompleted, "No-op action should complete without errors");
        }
        
        [Test]
        public void ContractsAreStableForExtension()
        {
            // Test that we can create custom goals and actions using the interfaces
            var customGoal = new TestCustomGoal();
            var customAction = new TestCustomAction();
            
            // These should work with the existing systems
            Assert.DoesNotThrow(() => customGoal.CanSatisfy(agentState, worldState));
            Assert.DoesNotThrow(() => customAction.CheckPreconditions(agentState, worldState));
            
            // Should be able to plan with custom actions
            var customActions = new List<IAction> { customAction };
            Assert.DoesNotThrow(() => planner.CreatePlan(customGoal, agentState, worldState, customActions));
        }
        
        [Test]
        public void BarkAction_CheckPreconditions_WorksCorrectly()
        {
            var barkAction = new BarkAction();
            
            // Should be able to bark when not sleeping
            Assert.IsTrue(barkAction.CheckPreconditions(agentState, worldState));
            
            // Should not be able to bark when sleeping
            agentState.ApplyEffect("sleeping");
            Assert.IsFalse(barkAction.CheckPreconditions(agentState, worldState));
        }
        
        [Test]
        public void BarkAction_ExecutesSuccessfully()
        {
            var barkAction = new BarkAction();
            
            // Action should not be executing initially
            Assert.IsFalse(barkAction.IsExecuting);
            
            // Start execution
            barkAction.StartExecution(agentState, worldState);
            Assert.IsTrue(barkAction.IsExecuting);
            
            // Update execution - should complete after enough time
            var result = ActionResult.Running;
            for (int i = 0; i < 20; i++) // Enough time for 1 second duration
            {
                result = barkAction.UpdateExecution(agentState, worldState, 0.1f);
                if (result != ActionResult.Running) break;
            }
            
            Assert.AreEqual(ActionResult.Success, result);
            Assert.IsFalse(barkAction.IsExecuting);
        }
        
        [Test]
        public void BarkAction_AppliesEffectsCorrectly()
        {
            var barkAction = new BarkAction();
            
            // Apply effects
            barkAction.ApplyEffects(agentState, worldState);
            
            // Check that bark effects are applied
            Assert.IsTrue(worldState.GetFact("has_barked"));
            Assert.IsTrue(worldState.GetFact("communicated"));
        }
        
        [Test]
        public void CommunicationGoal_CanSatisfy_WorksCorrectly()
        {
            var commGoal = new CommunicationGoal();
            
            // Should be able to communicate when not sleeping and hasn't barked
            Assert.IsTrue(commGoal.CanSatisfy(agentState, worldState));
            
            // Should not be able to communicate when sleeping
            agentState.ApplyEffect("sleeping");
            Assert.IsFalse(commGoal.CanSatisfy(agentState, worldState));
            
            // Reset sleeping effect
            agentState.RemoveEffect("sleeping");
            
            // Should not be able to communicate when already barked
            worldState.SetFact("has_barked", true);
            Assert.IsFalse(commGoal.CanSatisfy(agentState, worldState));
        }
        
        [Test]
        public void CommunicationGoal_IsCompleted_WorksCorrectly()
        {
            var commGoal = new CommunicationGoal();
            
            // Should not be completed initially
            Assert.IsFalse(commGoal.IsCompleted(agentState, worldState));
            
            // Should be completed when has_barked is set
            worldState.SetFact("has_barked", true);
            Assert.IsTrue(commGoal.IsCompleted(agentState, worldState));
            
            // Reset and test with communicated fact
            worldState.SetFact("has_barked", false);
            worldState.SetFact("communicated", true);
            Assert.IsTrue(commGoal.IsCompleted(agentState, worldState));
        }
        
        [Test]
        public void BarkSystem_Integration_CreatesAndExecutesPlan()
        {
            // Test integration: CommunicationGoal should create a plan with BarkAction
            var commGoal = new CommunicationGoal();
            var barkAction = new BarkAction();
            var actionsWithBark = new List<IAction> { new NoOpAction(), barkAction };
            
            // Set up conditions where communication goal has high priority
            agentState.SetNeed("hunger", 0.8f); // High hunger should increase communication priority
            agentState.SetNeed("thirst", 0.8f); // High thirst too
            
            var plan = planner.CreatePlan(commGoal, agentState, worldState, actionsWithBark);
            
            Assert.IsNotNull(plan, "Planner should create a plan for communication goal");
            Assert.AreEqual(commGoal, plan.Goal);
            
            // The plan should include the bark action since it satisfies the communication goal
            bool containsBarkAction = false;
            foreach (var action in plan.Actions)
            {
                if (action.ActionType == "bark")
                {
                    containsBarkAction = true;
                    break;
                }
            }
            Assert.IsTrue(containsBarkAction, "Plan should contain bark action for communication goal");
        }
    }
    
    /// <summary>
    /// Test custom goal to verify contract stability
    /// </summary>
    public class TestCustomGoal : IGoal
    {
        public string GoalType => "test_custom";
        public float Priority => 1f;
        
        public bool CanSatisfy(IAgentState agentState, IWorldState worldState) => true;
        public bool IsCompleted(IAgentState agentState, IWorldState worldState) => false;
        public Dictionary<string, object> GetDesiredState() => new Dictionary<string, object>();
        public float CalculatePriority(IAgentState agentState, IWorldState worldState) => 1f;
    }
    
    /// <summary>
    /// Test custom action to verify contract stability
    /// </summary>
    public class TestCustomAction : IAction
    {
        public string ActionType => "test_custom";
        public float Cost => 1f;
        public bool IsExecuting { get; private set; }
        
        public bool CheckPreconditions(IAgentState agentState, IWorldState worldState) => true;
        public Dictionary<string, object> GetEffects() => new Dictionary<string, object>();
        public void StartExecution(IAgentState agentState, IWorldState worldState) => IsExecuting = true;
        public ActionResult UpdateExecution(IAgentState agentState, IWorldState worldState, float deltaTime)
        {
            IsExecuting = false;
            return ActionResult.Success;
        }
        public void CancelExecution() => IsExecuting = false;
        public void ApplyEffects(IAgentState agentState, IWorldState worldState) { }
        public string GetDescription() => "Test custom action";
    }
}