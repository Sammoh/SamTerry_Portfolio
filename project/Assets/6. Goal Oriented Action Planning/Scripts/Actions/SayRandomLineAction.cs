using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Makes agents speak random dialogue lines.
    /// Context-aware dialogue selection based on world state with ambient chatter.
    /// </summary>
    public class SayRandomLineAction : IAction
    {
        public string ActionType => "say_random";
        public float Cost { get; private set; }
        public bool IsExecuting { get; private set; }

        private readonly float duration;
        private readonly float barkDuration;
        
        private float timer;
        private bool hasBarked;
        
        // Context-aware dialogue categories
        private readonly Dictionary<string, List<string>> contextDialogue = new Dictionary<string, List<string>>
        {
            ["general"] = new List<string>
            {
                "What are we doing today?",
                "Anyone else around?",
                "Nice weather, isn't it?",
                "I wonder what's happening over there...",
                "Just taking a moment to think...",
                "Time to see what's around here.",
                "Everything seems quiet today."
            },
            ["morning"] = new List<string>
            {
                "Good morning!",
                "What a lovely morning!",
                "Time to start the day.",
                "Morning already? Time flies."
            },
            ["evening"] = new List<string>
            {
                "Getting late...",
                "Long day today.",
                "Almost time to rest.",
                "The day is winding down."
            },
            ["safe"] = new List<string>
            {
                "All seems peaceful.",
                "Nice and quiet here.",
                "Good to feel safe.",
                "No worries here."
            },
            ["crowded"] = new List<string>
            {
                "Lots of people around today.",
                "Busy place, this.",
                "So many faces to see.",
                "Quite the gathering here."
            },
            ["alone"] = new List<string>
            {
                "Enjoying the solitude.",
                "Nice to have some quiet time.",
                "Just me and my thoughts.",
                "Peaceful when it's quiet."
            },
            ["tired"] = new List<string>
            {
                "Feeling a bit tired...",
                "Could use a rest soon.",
                "Long day ahead.",
                "Need to pace myself."
            },
            ["energetic"] = new List<string>
            {
                "Feeling good today!",
                "Ready for anything!",
                "Full of energy!",
                "What shall we do next?"
            }
        };

        public SayRandomLineAction(float cost = 0.1f, float speakDuration = 3f, float displayDuration = 2f)
        {
            Cost = Mathf.Max(0.01f, cost);
            duration = Mathf.Max(1f, speakDuration);
            barkDuration = Mathf.Max(1f, displayDuration);
        }

        public bool CheckPreconditions(IAgentState agentState, IWorldState worldState)
        {
            // Can always say something
            return true;
        }

        public Dictionary<string, object> GetEffects()
        {
            return new Dictionary<string, object>
            {
                { "idle", true }
            };
        }

        public void StartExecution(IAgentState agentState, IWorldState worldState)
        {
            IsExecuting = true;
            timer = 0f;
            hasBarked = false;
            
            Debug.Log("SayRandomLineAction: Starting to speak");
        }

        public ActionResult UpdateExecution(IAgentState agentState, IWorldState worldState, float deltaTime)
        {
            if (!IsExecuting)
                return ActionResult.Failed;

            timer += deltaTime;

            // Bark the message early in the execution
            if (!hasBarked && timer >= 0.1f)
            {
                string message = SelectContextualDialogue(agentState, worldState);
                TriggerBark(message);
                hasBarked = true;
            }

            // Check if duration has elapsed
            if (timer >= duration)
            {
                IsExecuting = false;
                return ActionResult.Success;
            }

            return ActionResult.Running;
        }

        public void CancelExecution()
        {
            IsExecuting = false;
        }

        public void ApplyEffects(IAgentState agentState, IWorldState worldState)
        {
            worldState.SetFact("idle", true);
        }

        public string GetDescription()
        {
            return $"Say a random contextual line for {duration}s";
        }

        private string SelectContextualDialogue(IAgentState agentState, IWorldState worldState)
        {
            List<string> availableLines = new List<string>();
            
            // Always include general dialogue
            availableLines.AddRange(contextDialogue["general"]);
            
            // Add context-specific dialogue based on world state and agent state
            if (worldState.GetFact("dayTime"))
            {
                // Check time of day - this is simplified, in a real system you might check actual time
                if (Random.value < 0.3f) // 30% chance to use morning dialogue during day
                {
                    availableLines.AddRange(contextDialogue["morning"]);
                }
            }
            else
            {
                availableLines.AddRange(contextDialogue["evening"]);
            }
            
            if (worldState.GetFact("safe"))
            {
                availableLines.AddRange(contextDialogue["safe"]);
            }
            
            // Check agent state for energy/tiredness
            if (agentState != null)
            {
                float sleepNeed = agentState.GetNeed("sleep");
                if (sleepNeed > 0.7f) // High sleep need = tired
                {
                    availableLines.AddRange(contextDialogue["tired"]);
                }
                else if (sleepNeed < 0.3f) // Low sleep need = energetic
                {
                    availableLines.AddRange(contextDialogue["energetic"]);
                }
            }
            
            // Check for crowding by looking for other agents nearby
            // This is a simplified check - in a real system you might count actual nearby agents
            int nearbyAgentCount = CountNearbyAgents();
            if (nearbyAgentCount > 3)
            {
                availableLines.AddRange(contextDialogue["crowded"]);
            }
            else if (nearbyAgentCount == 0)
            {
                availableLines.AddRange(contextDialogue["alone"]);
            }
            
            // Select a random line from available options
            if (availableLines.Count > 0)
            {
                return availableLines[Random.Range(0, availableLines.Count)];
            }
            
            return "..."; // Fallback
        }

        private void TriggerBark(string message)
        {
            // Find the GOAP agent to trigger the bark
            var goapAgent = Object.FindObjectOfType<GOAPAgent>();
            if (goapAgent != null)
            {
                // Use reflection to access the private Bark method
                var barkMethod = typeof(GOAPAgent).GetMethod("Bark", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (barkMethod != null)
                {
                    barkMethod.Invoke(goapAgent, new object[] { message, barkDuration, "Agent" });
                }
                else
                {
                    Debug.Log($"SayRandomLineAction: '{message}'");
                }
            }
            else
            {
                Debug.Log($"SayRandomLineAction: '{message}'");
            }
        }

        private int CountNearbyAgents()
        {
            // Simple proximity check for other GOAP agents
            var allAgents = Object.FindObjectsOfType<GOAPAgent>();
            if (allAgents.Length <= 1) return 0; // Only this agent or no agents
            
            // This is a simplified implementation
            // In a real system, you'd check distance to the current agent
            return allAgents.Length - 1; // Count other agents
        }
    }
}