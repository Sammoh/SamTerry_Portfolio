using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Integration test to validate that the new idle actions work correctly with the GOAP system
    /// </summary>
    public class IdleBehaviorIntegrationTest : MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField] private bool runTestOnStart = false;
        [SerializeField] private bool verbose = true;

        private void Start()
        {
            if (runTestOnStart)
            {
                RunIntegrationTest();
            }
        }

        [ContextMenu("Run Idle Behavior Integration Test")]
        public void RunIntegrationTest()
        {
            Log("Starting Idle Behavior Integration Test...");

            // Test 1: Verify actions can be created and configured
            TestActionCreation();

            // Test 2: Verify actions have correct effects
            TestActionEffects();

            // Test 3: Verify idle goal works with new actions
            TestIdleGoalIntegration();

            // Test 4: Verify all actions can satisfy idle goal
            TestActionGoalCompatibility();

            Log("Idle Behavior Integration Test completed successfully!");
        }

        private void TestActionCreation()
        {
            Log("Test 1: Action Creation");

            var wanderAction = new WanderAction();
            var investigateAction = new InvestigateAction();
            var sayRandomAction = new SayRandomLineAction();

            Assert(wanderAction != null, "WanderAction should be created successfully");
            Assert(investigateAction != null, "InvestigateAction should be created successfully");
            Assert(sayRandomAction != null, "SayRandomLineAction should be created successfully");

            Log("✓ All idle actions created successfully");
        }

        private void TestActionEffects()
        {
            Log("Test 2: Action Effects");

            var wanderAction = new WanderAction();
            var investigateAction = new InvestigateAction();
            var sayRandomAction = new SayRandomLineAction();

            var wanderEffects = wanderAction.GetEffects();
            var investigateEffects = investigateAction.GetEffects();
            var sayEffects = sayRandomAction.GetEffects();

            Assert(wanderEffects.ContainsKey("idle"), "WanderAction should have idle effect");
            Assert(investigateEffects.ContainsKey("idle"), "InvestigateAction should have idle effect");
            Assert(sayEffects.ContainsKey("idle"), "SayRandomLineAction should have idle effect");

            Log("✓ All actions have correct idle effects");
        }

        private void TestIdleGoalIntegration()
        {
            Log("Test 3: Idle Goal Integration");

            var idleGoal = new IdleGoal();
            var agentState = new BasicAgentState();
            
            // Create a mock world state for testing
            var worldStateGO = new GameObject("MockWorldState");
            var worldState = worldStateGO.AddComponent<BasicWorldState>();

            bool canSatisfy = idleGoal.CanSatisfy(agentState, worldState);
            float priority = idleGoal.CalculatePriority(agentState, worldState);
            var desiredState = idleGoal.GetDesiredState();

            Assert(canSatisfy, "IdleGoal should be satisfiable");
            Assert(priority > 0, "IdleGoal should have positive priority");
            Assert(desiredState.ContainsKey("idle"), "IdleGoal should desire idle state");

            DestroyImmediate(worldStateGO);
            Log("✓ IdleGoal integrates correctly");
        }

        private void TestActionGoalCompatibility()
        {
            Log("Test 4: Action-Goal Compatibility");

            var idleGoal = new IdleGoal();
            var desiredState = idleGoal.GetDesiredState();

            var wanderAction = new WanderAction();
            var investigateAction = new InvestigateAction();
            var sayRandomAction = new SayRandomLineAction();

            var wanderEffects = wanderAction.GetEffects();
            var investigateEffects = investigateAction.GetEffects();
            var sayEffects = sayRandomAction.GetEffects();

            // Check if actions can satisfy the idle goal
            bool wanderSatisfiesIdle = CheckEffectsSatisfyGoal(wanderEffects, desiredState);
            bool investigateSatisfiesIdle = CheckEffectsSatisfyGoal(investigateEffects, desiredState);
            bool saySatisfiesIdle = CheckEffectsSatisfyGoal(sayEffects, desiredState);

            Assert(wanderSatisfiesIdle, "WanderAction should satisfy IdleGoal");
            Assert(investigateSatisfiesIdle, "InvestigateAction should satisfy IdleGoal");
            Assert(saySatisfiesIdle, "SayRandomLineAction should satisfy IdleGoal");

            Log("✓ All idle actions can satisfy IdleGoal");
        }

        private bool CheckEffectsSatisfyGoal(Dictionary<string, object> effects, Dictionary<string, object> desiredState)
        {
            foreach (var desired in desiredState)
            {
                if (!effects.ContainsKey(desired.Key))
                    return false;
                
                if (!effects[desired.Key].Equals(desired.Value))
                    return false;
            }
            return true;
        }

        private void Log(string message)
        {
            if (verbose)
            {
                Debug.Log($"[IdleBehaviorTest] {message}");
            }
        }

        private void Assert(bool condition, string message)
        {
            if (!condition)
            {
                Debug.LogError($"[IdleBehaviorTest] ASSERTION FAILED: {message}");
                throw new System.Exception($"Test assertion failed: {message}");
            }
        }
    }
}