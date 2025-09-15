using UnityEngine;
using Sammoh.GOAP;
using System.Collections.Generic;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Example script demonstrating the new idle actions in action.
    /// This would be attached to a GameObject in a Unity scene for testing.
    /// </summary>
    public class IdleActionsDemonstration : MonoBehaviour
    {
        [Header("Demo Configuration")]
        [SerializeField] private bool enableDemo = true;
        [SerializeField] private float demoInterval = 10f;
        
        private BasicAgentState testAgentState;
        private BasicWorldState testWorldState;
        private List<IAction> idleActions;
        private float lastDemoTime;
        
        private void Start()
        {
            if (!enableDemo) return;
            
            SetupDemo();
        }
        
        private void Update()
        {
            if (!enableDemo) return;
            
            if (Time.time - lastDemoTime >= demoInterval)
            {
                RunIdleActionDemo();
                lastDemoTime = Time.time;
            }
        }
        
        private void SetupDemo()
        {
            Debug.Log("=== Setting up Idle Actions Demo ===");
            
            // Create test agent state with varying needs
            testAgentState = new BasicAgentState();
            testAgentState.SetNeed("hunger", Random.Range(0.1f, 0.9f));
            testAgentState.SetNeed("thirst", Random.Range(0.1f, 0.9f));
            testAgentState.SetNeed("sleep", Random.Range(0.1f, 0.9f));
            testAgentState.SetNeed("play", Random.Range(0.1f, 0.9f));
            
            // Create test world state
            var worldGO = new GameObject("DemoWorldState");
            testWorldState = worldGO.AddComponent<BasicWorldState>();
            testWorldState.SetFact("dayTime", Random.value > 0.5f);
            testWorldState.SetFact("safe", true);
            
            // Setup idle actions
            idleActions = new List<IAction>();
            
            // WanderAction
            var wanderAction = new WanderAction(cost: 0.5f, radius: 8f, speed: 2.5f, wanderDuration: 4f);
            wanderAction.InjectAgent(transform);
            idleActions.Add(wanderAction);
            
            // InvestigateAction
            var investigateAction = new InvestigateAction(cost: 0.3f, radius: 6f, investigateDuration: 3f);
            investigateAction.InjectAgent(transform);
            idleActions.Add(investigateAction);
            
            // SayRandomLineAction
            var sayAction = new SayRandomLineAction(cost: 0.1f, speakDuration: 2f);
            idleActions.Add(sayAction);
            
            Debug.Log($"Demo setup complete with {idleActions.Count} idle actions");
            LogAgentState();
        }
        
        private void RunIdleActionDemo()
        {
            Debug.Log("=== Running Idle Actions Demo ===");
            
            // Test enhanced IdleGoal priority calculation
            var idleGoal = new IdleGoal();
            float priority = idleGoal.CalculatePriority(testAgentState, testWorldState);
            Debug.Log($"IdleGoal priority: {priority:F2} (base 0.3, max 0.8)");
            
            // Demonstrate each action
            foreach (var action in idleActions)
            {
                DemonstrateAction(action);
            }
            
            // Randomize agent state for next demo
            RandomizeAgentState();
            LogAgentState();
        }
        
        private void DemonstrateAction(IAction action)
        {
            Debug.Log($"\n--- Demonstrating {action.ActionType.ToUpper()} ---");
            Debug.Log($"Cost: {action.Cost}, Description: {action.GetDescription()}");
            
            // Check preconditions
            bool canExecute = action.CheckPreconditions(testAgentState, testWorldState);
            Debug.Log($"Can execute: {canExecute}");
            
            if (canExecute)
            {
                // Show effects
                var effects = action.GetEffects();
                Debug.Log($"Effects: {string.Join(", ", effects.Keys)}");
                
                // Simulate execution start
                action.StartExecution(testAgentState, testWorldState);
                Debug.Log($"Execution started: {action.IsExecuting}");
                
                // Simulate a short execution update
                var result = action.UpdateExecution(testAgentState, testWorldState, 0.1f);
                Debug.Log($"Update result: {result}");
                
                // Apply effects if successful
                if (result == ActionResult.Success)
                {
                    action.ApplyEffects(testAgentState, testWorldState);
                    Debug.Log("Effects applied successfully");
                }
                
                // Cancel to clean up
                action.CancelExecution();
            }
        }
        
        private void RandomizeAgentState()
        {
            testAgentState.SetNeed("hunger", Random.Range(0.0f, 1.0f));
            testAgentState.SetNeed("thirst", Random.Range(0.0f, 1.0f));
            testAgentState.SetNeed("sleep", Random.Range(0.0f, 1.0f));
            testAgentState.SetNeed("play", Random.Range(0.0f, 1.0f));
            
            testWorldState.SetFact("dayTime", Random.value > 0.5f);
        }
        
        private void LogAgentState()
        {
            var needs = testAgentState.GetAllNeeds();
            Debug.Log($"Agent State - Hunger: {needs["hunger"]:F2}, Thirst: {needs["thirst"]:F2}, " +
                     $"Sleep: {needs["sleep"]:F2}, Play: {needs["play"]:F2}");
            Debug.Log($"World State - DayTime: {testWorldState.GetFact("dayTime")}, Safe: {testWorldState.GetFact("safe")}");
        }
    }
}