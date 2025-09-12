using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Basic implementation of agent state
    /// </summary>
    public class BasicAgentState : IAgentState
    {
        private Dictionary<string, float> needs = new Dictionary<string, float>();
        private Dictionary<string, int> inventory = new Dictionary<string, int>();
        private Dictionary<string, float> effects = new Dictionary<string, float>();
        
        public BasicAgentState()
        {
            // Initialize basic needs
            needs["hunger"] = 0.3f;
            needs["thirst"] = 0.2f;
            needs["sleep"] = 0.1f;
            needs["play"] = 0.4f;
        }
        
        public float GetNeed(string needType)
        {
            return needs.TryGetValue(needType, out float value) ? value : 0f;
        }
        
        public void SetNeed(string needType, float value)
        {
            needs[needType] = Mathf.Clamp01(value);
        }
        
        public Dictionary<string, float> GetAllNeeds()
        {
            return new Dictionary<string, float>(needs);
        }
        
        public bool HasItem(string itemType)
        {
            return GetItemCount(itemType) > 0;
        }
        
        public int GetItemCount(string itemType)
        {
            return inventory.TryGetValue(itemType, out int count) ? count : 0;
        }
        
        public void AddItem(string itemType, int quantity = 1)
        {
            if (inventory.ContainsKey(itemType))
                inventory[itemType] += quantity;
            else
                inventory[itemType] = quantity;
        }
        
        public bool RemoveItem(string itemType, int quantity = 1)
        {
            if (!inventory.ContainsKey(itemType) || inventory[itemType] < quantity)
                return false;
                
            inventory[itemType] -= quantity;
            if (inventory[itemType] <= 0)
                inventory.Remove(itemType);
                
            return true;
        }
        
        public bool HasEffect(string effectType)
        {
            return effects.ContainsKey(effectType) && effects[effectType] > 0f;
        }
        
        public void ApplyEffect(string effectType, float duration = -1f)
        {
            effects[effectType] = duration;
        }
        
        public void RemoveEffect(string effectType)
        {
            effects.Remove(effectType);
        }
        
        public void Update(float deltaTime)
        {
            // Slowly increase needs over time
            var needsToUpdate = new List<string>(needs.Keys);
            foreach (var needType in needsToUpdate)
            {
                float rate = GetNeedDecayRate(needType);
                SetNeed(needType, GetNeed(needType) + rate * deltaTime);
            }
            
            // Update effects
            var effectsToUpdate = new List<string>(effects.Keys);
            foreach (var effectType in effectsToUpdate)
            {
                if (effects[effectType] > 0f)
                {
                    effects[effectType] -= deltaTime;
                    if (effects[effectType] <= 0f)
                        effects.Remove(effectType);
                }
            }
        }
        
        private float GetNeedDecayRate(string needType)
        {
            return needType switch
            {
                "hunger" => 0.02f,  // 2% per second
                "thirst" => 0.03f,  // 3% per second
                "sleep" => 0.01f,   // 1% per second
                "play" => 0.015f,   // 1.5% per second
                _ => 0.01f
            };
        }
    }
}