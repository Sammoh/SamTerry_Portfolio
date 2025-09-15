using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Makes the agent say random lines when idle.
    /// Integrates with the existing bark system to display dialogue.
    /// </summary>
    public class SayRandomLineAction : IAction
    {
        public string ActionType => "say_random";
        public float Cost => 0.1f; // Very low cost for idle chatter
        public bool IsExecuting { get; private set; }

        private readonly float duration;
        private float timer;
        
        // Random lines for different contexts
        private static readonly string[] IdleLines = new string[]
        {
            "What are we doing today?",
            "I wonder what's over there...",
            "Nice weather today, isn't it?",
            "Hmm, what should I do next?",
            "This place is quite interesting.",
            "I feel like exploring a bit.",
            "Anyone else around here?",
            "Time seems to pass slowly when idle.",
            "I should probably find something to do.",
            "The world is full of mysteries.",
            "What's that noise?",
            "I love observing my surroundings.",
            "Life is good when you have time to think.",
            "I wonder what other agents are up to.",
            "Maybe I should go for a walk."
        };

        private static readonly string[] CuriousLines = new string[]
        {
            "What's that over there?",
            "That looks interesting!",
            "I've never seen anything like that before.",
            "Curious... very curious indeed.",
            "This deserves a closer look.",
            "How fascinating!",
            "What could that be used for?",
            "I wonder how this works.",
            "That's quite remarkable.",
            "Intriguing design."
        };

        // Reference to the GOAPAgent for bark functionality
        private GOAPAgent agent;

        public SayRandomLineAction(float duration = 2f)
        {
            this.duration = Mathf.Max(0.5f, duration);
        }

        // Injection method for agent reference
        public void InjectAgent(Transform agentTransform, UnityEngine.AI.NavMeshAgent navMeshAgent = null)
        {
            agent = agentTransform?.GetComponent<GOAPAgent>();
        }

        public bool CheckPreconditions(IAgentState agentState, IWorldState worldState)
        {
            // Can always say something when idle
            return true;
        }

        public Dictionary<string, object> GetEffects()
        {
            return new Dictionary<string, object>
            {
                { "talking", true },
                { "idle", true }
            };
        }

        public void StartExecution(IAgentState agentState, IWorldState worldState)
        {
            IsExecuting = true;
            timer = 0f;
            
            // Choose and say a random line
            SayRandomLine(worldState);
        }

        public ActionResult UpdateExecution(IAgentState agentState, IWorldState worldState, float deltaTime)
        {
            if (!IsExecuting) return ActionResult.Failed;

            timer += deltaTime;
            return timer >= duration ? ActionResult.Success : ActionResult.Running;
        }

        public void CancelExecution()
        {
            IsExecuting = false;
        }

        public void ApplyEffects(IAgentState agentState, IWorldState worldState)
        {
            IsExecuting = false;
            worldState?.SetFact("talking", false);
            worldState?.SetFact("idle", true);
        }

        public string GetDescription()
        {
            return $"Say a random line for {duration}s";
        }

        private void SayRandomLine(IWorldState worldState)
        {
            string lineToSay;
            
            // Choose line based on context
            if (worldState != null && worldState.GetFact("investigating"))
            {
                // If we were just investigating, use curious lines
                lineToSay = CuriousLines[Random.Range(0, CuriousLines.Length)];
            }
            else
            {
                // Default to idle conversation
                lineToSay = IdleLines[Random.Range(0, IdleLines.Length)];
            }

            // Log the message - the GOAPAgent will pick this up through its action bark system
            Debug.Log($"Agent says: {lineToSay}");
        }
    }
}