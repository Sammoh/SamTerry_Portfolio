using System.Collections.Generic;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Represents the internal state of an agent including needs, stats, inventory, and effects
    /// </summary>
    public interface IAgentState
    {
        /// <summary>
        /// Get the value of a specific need (hunger, thirst, sleep, etc.)
        /// Values typically range from 0.0 (satisfied) to 1.0 (critical need)
        /// </summary>
        float GetNeed(string needType);
        
        /// <summary>
        /// Set the value of a specific need
        /// </summary>
        void SetNeed(string needType, float value);
        
        /// <summary>
        /// Get all current needs as a dictionary
        /// </summary>
        Dictionary<string, float> GetAllNeeds();
        
        /// <summary>
        /// Check if the agent has a specific item in inventory
        /// </summary>
        bool HasItem(string itemType);
        
        /// <summary>
        /// Get the quantity of a specific item in inventory
        /// </summary>
        int GetItemCount(string itemType);
        
        /// <summary>
        /// Add an item to the agent's inventory
        /// </summary>
        void AddItem(string itemType, int quantity = 1);
        
        /// <summary>
        /// Remove an item from the agent's inventory
        /// </summary>
        bool RemoveItem(string itemType, int quantity = 1);
        
        /// <summary>
        /// Check if the agent has a specific active effect
        /// </summary>
        bool HasEffect(string effectType);
        
        /// <summary>
        /// Apply an effect to the agent
        /// </summary>
        void ApplyEffect(string effectType, float duration = -1f);
        
        /// <summary>
        /// Remove an effect from the agent
        /// </summary>
        void RemoveEffect(string effectType);
        
        /// <summary>
        /// Update the agent state (typically called each frame to decay needs, update effects, etc.)
        /// </summary>
        void Update(float deltaTime);
    }
}