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
                new NoOpAction()
            };
            
            testGoals = new List<IGoal>
            {
                new IdleGoal()
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