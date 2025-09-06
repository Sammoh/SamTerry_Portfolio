using NUnit.Framework;
using UnityEngine;
using Sammoh.TurnBasedStrategy;

public class SkillTreeTests
{
    private SkillTree skillTree;
    private CharacterAbility fireball;
    private CharacterAbility heal;

    [SetUp]
    public void Setup()
    {
        skillTree = new SkillTree();
        fireball = new CharacterAbility("Fireball", AbilityType.Attack, 20, 15, "Fire magic attack");
        heal = new CharacterAbility("Greater Heal", AbilityType.Heal, 30, 20, "Powerful healing spell");
    }

    [Test]
    public void SkillTree_Constructor_InitializesWithDefaultNodes()
    {
        Assert.IsNotNull(skillTree.Nodes);
        Assert.IsTrue(skillTree.Nodes.Count > 0);
        Assert.IsNotNull(skillTree.CurrentAbilities);
        Assert.AreEqual(4, skillTree.CurrentAbilities.Length);
    }

    [Test]
    public void SkillTree_GetValidAbilities_ReturnsOnlyNonNullAbilities()
    {
        CharacterAbility[] validAbilities = skillTree.GetValidAbilities();
        
        // Should have at least the default abilities (basic attack, heal, defend)
        Assert.IsTrue(validAbilities.Length >= 3);
        foreach (var ability in validAbilities)
        {
            Assert.IsNotNull(ability);
        }
    }

    [Test]
    public void SkillTreeNode_Constructor_SetsPropertiesCorrectly()
    {
        var node = new SkillTreeNode("test_fireball", fireball, AbilityCategory.Skill, new string[] { "basic_attack" });
        
        Assert.AreEqual("test_fireball", node.NodeId);
        Assert.AreEqual(fireball, node.Ability);
        Assert.AreEqual(AbilityCategory.Skill, node.Category);
        Assert.AreEqual(1, node.Prerequisites.Length);
        Assert.IsFalse(node.IsUnlocked); // Should be locked due to prerequisites
    }

    [Test]
    public void SkillTreeNode_CanUnlock_ReturnsTrueWhenPrerequisitesMet()
    {
        var node = new SkillTreeNode("test_fireball", fireball, AbilityCategory.Skill, new string[] { "basic_attack" });
        skillTree.AddNode(node);
        
        // basic_attack should be unlocked by default
        Assert.IsTrue(node.CanUnlock(skillTree));
    }

    [Test]
    public void SkillTreeNode_CanUnlock_ReturnsFalseWhenPrerequisitesNotMet()
    {
        var prereqNode = new SkillTreeNode("advanced_skill", heal, AbilityCategory.Skill, new string[] { "basic_attack" });
        var testNode = new SkillTreeNode("master_skill", fireball, AbilityCategory.Special, new string[] { "advanced_skill" });
        
        skillTree.AddNode(prereqNode);
        skillTree.AddNode(testNode);
        
        // advanced_skill is not unlocked yet, so master_skill can't be unlocked
        Assert.IsFalse(testNode.CanUnlock(skillTree));
    }

    [Test]
    public void SkillTree_UnlockNode_UnlocksWhenPossible()
    {
        var node = new SkillTreeNode("test_fireball", fireball, AbilityCategory.Skill, new string[] { "basic_attack" });
        skillTree.AddNode(node);
        
        skillTree.UnlockNode("test_fireball");
        
        Assert.IsTrue(skillTree.IsNodeUnlocked("test_fireball"));
    }

    [Test]
    public void SkillTree_GetAvailableNodesForCategory_ReturnsCorrectNodes()
    {
        var attackNode = new SkillTreeNode("power_attack", fireball, AbilityCategory.BasicAttack);
        var skillNode = new SkillTreeNode("heal_spell", heal, AbilityCategory.Skill);
        
        skillTree.AddNode(attackNode);
        skillTree.AddNode(skillNode);
        
        var attackNodes = skillTree.GetAvailableNodesForCategory(AbilityCategory.BasicAttack);
        var skillNodes = skillTree.GetAvailableNodesForCategory(AbilityCategory.Skill);
        
        Assert.IsTrue(attackNodes.Count >= 1); // At least our added node + default basic attack
        Assert.IsTrue(skillNodes.Count >= 1);  // At least our added node + default heal
    }

    [Test]
    public void SkillTree_ReplaceAbility_WorksCorrectly()
    {
        bool result = skillTree.ReplaceAbility(0, fireball);
        
        Assert.IsTrue(result);
        Assert.AreEqual(fireball, skillTree.CurrentAbilities[0]);
    }

    [Test]
    public void SkillTree_ReplaceAbility_FailsWithInvalidSlot()
    {
        bool result1 = skillTree.ReplaceAbility(-1, fireball);
        bool result2 = skillTree.ReplaceAbility(10, fireball);
        
        Assert.IsFalse(result1);
        Assert.IsFalse(result2);
    }

    [Test]
    public void SkillTree_GetAbilitySlot_ReturnsCorrectSlot()
    {
        skillTree.ReplaceAbility(2, fireball);
        
        int slot = skillTree.GetAbilitySlot(fireball);
        Assert.AreEqual(2, slot);
    }

    [Test]
    public void SkillTree_GetAbilitySlot_ReturnsNegativeOneWhenNotFound()
    {
        int slot = skillTree.GetAbilitySlot(fireball);
        Assert.AreEqual(-1, slot);
    }
}