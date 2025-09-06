using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    public enum AbilityCategory
    {
        BasicAttack,
        Skill,
        Secondary,
        Special
    }

    [Serializable]
    public class SkillTreeNode
    {
        [SerializeField] private string nodeId;
        [SerializeField] private CharacterAbility ability;
        [SerializeField] private AbilityCategory category;
        [SerializeField] private string[] prerequisites; // Node IDs that must be unlocked first
        [SerializeField] private bool isUnlocked;

        public string NodeId => nodeId;
        public CharacterAbility Ability => ability;
        public AbilityCategory Category => category;
        public string[] Prerequisites => prerequisites;
        public bool IsUnlocked => isUnlocked;

        public SkillTreeNode(string id, CharacterAbility ability, AbilityCategory category, string[] prerequisites = null)
        {
            this.nodeId = id;
            this.ability = ability;
            this.category = category;
            this.prerequisites = prerequisites ?? new string[0];
            this.isUnlocked = prerequisites == null || prerequisites.Length == 0; // Auto-unlock if no prerequisites
        }

        public void Unlock()
        {
            isUnlocked = true;
        }

        public bool CanUnlock(SkillTree skillTree)
        {
            if (isUnlocked) return false;

            foreach (string prereqId in prerequisites)
            {
                if (!skillTree.IsNodeUnlocked(prereqId))
                    return false;
            }
            return true;
        }
    }

    [Serializable]
    public class SkillTree
    {
        [SerializeField] private List<SkillTreeNode> nodes = new List<SkillTreeNode>();
        [SerializeField] private CharacterAbility[] currentAbilities = new CharacterAbility[4]; // 4 slots max

        public List<SkillTreeNode> Nodes => nodes;
        public CharacterAbility[] CurrentAbilities => currentAbilities;

        public SkillTree()
        {
            InitializeDefaultNodes();
        }

        private void InitializeDefaultNodes()
        {
            // Add default basic abilities
            nodes.Add(new SkillTreeNode("basic_attack", 
                new CharacterAbility("Basic Attack", AbilityType.Attack, 10, 0, "Standard melee attack"), 
                AbilityCategory.BasicAttack));
            
            nodes.Add(new SkillTreeNode("basic_heal", 
                new CharacterAbility("Basic Heal", AbilityType.Heal, 15, 10, "Basic healing spell"), 
                AbilityCategory.Skill));
            
            nodes.Add(new SkillTreeNode("defend", 
                new CharacterAbility("Defend", AbilityType.Defend, 5, 0, "Defensive stance"), 
                AbilityCategory.Secondary));
            
            nodes.Add(new SkillTreeNode("special_strike", 
                new CharacterAbility("Special Strike", AbilityType.Special, 25, 20, "Powerful special attack"), 
                AbilityCategory.Special, new string[] { "basic_attack" }));

            // Set initial abilities
            currentAbilities[0] = GetNode("basic_attack")?.Ability;
            currentAbilities[1] = GetNode("basic_heal")?.Ability;
            currentAbilities[2] = GetNode("defend")?.Ability;
            currentAbilities[3] = null; // Special slot starts empty
        }

        public void AddNode(SkillTreeNode node)
        {
            if (GetNode(node.NodeId) == null)
            {
                nodes.Add(node);
            }
        }

        public SkillTreeNode GetNode(string nodeId)
        {
            return nodes.Find(node => node.NodeId == nodeId);
        }

        public bool IsNodeUnlocked(string nodeId)
        {
            SkillTreeNode node = GetNode(nodeId);
            return node != null && node.IsUnlocked;
        }

        public void UnlockNode(string nodeId)
        {
            SkillTreeNode node = GetNode(nodeId);
            if (node != null && node.CanUnlock(this))
            {
                node.Unlock();
            }
        }

        public List<SkillTreeNode> GetAvailableNodesForCategory(AbilityCategory category)
        {
            return nodes.FindAll(node => node.Category == category && node.IsUnlocked);
        }

        public bool ReplaceAbility(int slotIndex, CharacterAbility newAbility)
        {
            if (slotIndex >= 0 && slotIndex < currentAbilities.Length)
            {
                currentAbilities[slotIndex] = newAbility;
                return true;
            }
            return false;
        }

        public int GetAbilitySlot(CharacterAbility ability)
        {
            for (int i = 0; i < currentAbilities.Length; i++)
            {
                if (currentAbilities[i] == ability)
                    return i;
            }
            return -1;
        }

        public CharacterAbility[] GetValidAbilities()
        {
            List<CharacterAbility> validAbilities = new List<CharacterAbility>();
            foreach (var ability in currentAbilities)
            {
                if (ability != null)
                    validAbilities.Add(ability);
            }
            return validAbilities.ToArray();
        }
    }
}