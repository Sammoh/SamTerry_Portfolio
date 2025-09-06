using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    [CreateAssetMenu(fileName = "New Skill Tree Database", menuName = "Turn-Based Strategy/Skill Tree Database")]
    public class SkillTreeDatabase : ScriptableObject
    {
        [SerializeField] private SkillTreeNode[] allNodes;

        public SkillTreeNode[] AllNodes => allNodes;

        /// <summary>
        /// Creates a complete skill tree with various abilities for demonstration
        /// </summary>
        public static SkillTreeNode[] CreateDemoSkillTreeNodes()
        {
            return new SkillTreeNode[]
            {
                // Basic Attack Category
                new SkillTreeNode("basic_attack", 
                    new CharacterAbility("Basic Attack", AbilityType.Attack, 10, 0, "Standard melee attack"), 
                    AbilityCategory.BasicAttack),

                new SkillTreeNode("power_attack", 
                    new CharacterAbility("Power Attack", AbilityType.Attack, 18, 5, "A powerful melee strike"), 
                    AbilityCategory.BasicAttack, new string[] { "basic_attack" }),

                new SkillTreeNode("fury_strike", 
                    new CharacterAbility("Fury Strike", AbilityType.Attack, 25, 10, "An aggressive attack that sacrifices defense"), 
                    AbilityCategory.BasicAttack, new string[] { "power_attack" }),

                // Skill Category
                new SkillTreeNode("basic_heal", 
                    new CharacterAbility("Basic Heal", AbilityType.Heal, 15, 10, "Basic healing spell"), 
                    AbilityCategory.Skill),

                new SkillTreeNode("greater_heal", 
                    new CharacterAbility("Greater Heal", AbilityType.Heal, 25, 18, "More powerful healing magic"), 
                    AbilityCategory.Skill, new string[] { "basic_heal" }),

                new SkillTreeNode("flame_bolt", 
                    new CharacterAbility("Flame Bolt", AbilityType.Attack, 20, 12, "Ranged fire magic attack"), 
                    AbilityCategory.Skill, new string[] { "basic_heal" }),

                new SkillTreeNode("ice_shard", 
                    new CharacterAbility("Ice Shard", AbilityType.Attack, 16, 8, "Ranged ice magic that may slow enemies"), 
                    AbilityCategory.Skill, new string[] { "basic_heal" }),

                // Secondary Category
                new SkillTreeNode("defend", 
                    new CharacterAbility("Defend", AbilityType.Defend, 5, 0, "Defensive stance that reduces damage"), 
                    AbilityCategory.Secondary),

                new SkillTreeNode("shield_wall", 
                    new CharacterAbility("Shield Wall", AbilityType.Defend, 12, 8, "Strong defensive barrier"), 
                    AbilityCategory.Secondary, new string[] { "defend" }),

                new SkillTreeNode("counter_attack", 
                    new CharacterAbility("Counter Attack", AbilityType.Defend, 8, 6, "Defensive stance that strikes back when hit"), 
                    AbilityCategory.Secondary, new string[] { "defend" }),

                new SkillTreeNode("meditation", 
                    new CharacterAbility("Meditation", AbilityType.Heal, 0, 0, "Restores mana over time"), 
                    AbilityCategory.Secondary, new string[] { "defend" }),

                // Special Category
                new SkillTreeNode("special_strike", 
                    new CharacterAbility("Special Strike", AbilityType.Special, 35, 20, "Powerful special attack"), 
                    AbilityCategory.Special, new string[] { "power_attack", "greater_heal" }),

                new SkillTreeNode("meteor", 
                    new CharacterAbility("Meteor", AbilityType.Special, 45, 30, "Devastating area magic attack"), 
                    AbilityCategory.Special, new string[] { "flame_bolt", "ice_shard" }),

                new SkillTreeNode("divine_intervention", 
                    new CharacterAbility("Divine Intervention", AbilityType.Heal, 50, 35, "Ultimate healing ability"), 
                    AbilityCategory.Special, new string[] { "greater_heal", "meditation" }),

                new SkillTreeNode("berserker_rage", 
                    new CharacterAbility("Berserker Rage", AbilityType.Special, 40, 25, "Increases all stats temporarily"), 
                    AbilityCategory.Special, new string[] { "fury_strike", "counter_attack" })
            };
        }

        public SkillTreeNode GetNodeById(string nodeId)
        {
            foreach (var node in allNodes)
            {
                if (node.NodeId == nodeId)
                    return node;
            }
            return null;
        }

        public SkillTreeNode[] GetNodesByCategory(AbilityCategory category)
        {
            return System.Array.FindAll(allNodes, node => node.Category == category);
        }
    }
}