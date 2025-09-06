using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    /// <summary>
    /// Demonstration component that shows how to use the equipment and skill tree systems
    /// </summary>
    public class EquipmentSkillTreeDemo : MonoBehaviour
    {
        [Header("Demo Characters")]
        [SerializeField] private Character demoCharacter;
        
        [Header("Demo Settings")]
        [SerializeField] private bool enableDemoOnStart = true;
        [SerializeField] private bool logDemoResults = true;

        private void Start()
        {
            if (enableDemoOnStart)
            {
                RunDemo();
            }
        }

        [ContextMenu("Run Equipment and Skill Tree Demo")]
        public void RunDemo()
        {
            if (demoCharacter == null)
            {
                Debug.LogError("Demo character is not assigned!");
                return;
            }

            Log("=== Equipment and Skill Tree Demo Starting ===");
            
            DemonstrateBaseStats();
            DemonstrateEquipmentSystem();
            DemonstrateSkillTreeSystem();
            
            Log("=== Demo Complete ===");
        }

        private void DemonstrateBaseStats()
        {
            Log("\n--- Base Character Stats ---");
            var stats = demoCharacter.Stats;
            Log($"Health: {stats.CurrentHealth}/{stats.MaxHealth}");
            Log($"Attack: {stats.Attack} (Base: {stats.BaseAttack})");
            Log($"Defense: {stats.Defense} (Base: {stats.BaseDefense})");
            Log($"Speed: {stats.Speed} (Base: {stats.BaseSpeed})");
            Log($"Mana: {stats.CurrentMana}/{stats.Mana} (Base: {stats.BaseMana})");
        }

        private void DemonstrateEquipmentSystem()
        {
            Log("\n--- Equipment System Demo ---");
            
            // Create some demo equipment
            Equipment[] demoEquipment = EquipmentDatabase.CreateDefaultEquipment();
            
            // Equip a weapon
            Equipment weapon = System.Array.Find(demoEquipment, e => e.Slot == EquipmentSlot.Weapon);
            if (weapon != null)
            {
                Log($"Equipping: {weapon.EquipmentName} - {weapon.Description}");
                demoCharacter.EquipItem(weapon);
                
                Log("Stats after equipping weapon:");
                LogCurrentStats();
            }

            // Equip armor
            Equipment armor = System.Array.Find(demoEquipment, e => e.Slot == EquipmentSlot.Armor);
            if (armor != null)
            {
                Log($"Equipping: {armor.EquipmentName} - {armor.Description}");
                demoCharacter.EquipItem(armor);
                
                Log("Stats after equipping armor:");
                LogCurrentStats();
            }

            // Equip accessory
            Equipment accessory = System.Array.Find(demoEquipment, e => e.Slot == EquipmentSlot.Accessory);
            if (accessory != null)
            {
                Log($"Equipping: {accessory.EquipmentName} - {accessory.Description}");
                demoCharacter.EquipItem(accessory);
                
                Log("Final stats with all equipment:");
                LogCurrentStats();
            }
        }

        private void DemonstrateSkillTreeSystem()
        {
            Log("\n--- Skill Tree System Demo ---");
            
            // Show current abilities
            Log("Current abilities:");
            var currentAbilities = demoCharacter.Abilities;
            for (int i = 0; i < currentAbilities.Length; i++)
            {
                if (currentAbilities[i] != null)
                {
                    var ability = currentAbilities[i];
                    Log($"  Slot {i}: {ability.AbilityName} - Power: {ability.Power}, Mana: {ability.ManaCost}");
                }
            }

            // Add some advanced nodes to the skill tree
            var skillTree = demoCharacter.SkillTree;
            var advancedNodes = SkillTreeDatabase.CreateDemoSkillTreeNodes();
            
            foreach (var node in advancedNodes)
            {
                skillTree.AddNode(node);
            }

            // Unlock some nodes for demonstration
            skillTree.UnlockNode("power_attack");
            skillTree.UnlockNode("greater_heal");
            skillTree.UnlockNode("shield_wall");

            Log("\nUnlocked new abilities. Available upgrades by category:");
            
            // Show available abilities for each category
            for (int categoryIndex = 0; categoryIndex < 4; categoryIndex++)
            {
                AbilityCategory category = (AbilityCategory)categoryIndex;
                var availableNodes = skillTree.GetAvailableNodesForCategory(category);
                
                Log($"\n  {category} abilities:");
                foreach (var node in availableNodes)
                {
                    var ability = node.Ability;
                    Log($"    - {ability.AbilityName}: Power {ability.Power}, Mana {ability.ManaCost}");
                }
            }

            // Demonstrate ability replacement
            Log("\n--- Demonstrating Ability Replacement ---");
            
            // Replace basic attack with power attack
            var powerAttackNode = System.Array.Find(advancedNodes, n => n.NodeId == "power_attack");
            if (powerAttackNode != null)
            {
                var oldAbility = skillTree.CurrentAbilities[0];
                skillTree.ReplaceAbility(0, powerAttackNode.Ability);
                
                Log($"Replaced '{oldAbility?.AbilityName}' with '{powerAttackNode.Ability.AbilityName}'");
                Log($"Power increased from {oldAbility?.Power} to {powerAttackNode.Ability.Power}");
                Log($"Mana cost changed from {oldAbility?.ManaCost} to {powerAttackNode.Ability.ManaCost}");
            }

            // Show final abilities
            Log("\nFinal character abilities:");
            var finalAbilities = demoCharacter.Abilities;
            for (int i = 0; i < finalAbilities.Length; i++)
            {
                if (finalAbilities[i] != null)
                {
                    var ability = finalAbilities[i];
                    Log($"  Slot {i}: {ability.AbilityName} - Power: {ability.Power}, Mana: {ability.ManaCost}");
                }
            }
        }

        private void LogCurrentStats()
        {
            var stats = demoCharacter.Stats;
            Log($"  Health: {stats.CurrentHealth}/{stats.MaxHealth}");
            Log($"  Attack: {stats.Attack} (Base: {stats.BaseAttack})");
            Log($"  Defense: {stats.Defense} (Base: {stats.BaseDefense})");
            Log($"  Speed: {stats.Speed} (Base: {stats.BaseSpeed})");
            Log($"  Mana: {stats.CurrentMana}/{stats.Mana} (Base: {stats.BaseMana})");
        }

        private void Log(string message)
        {
            if (logDemoResults)
            {
                Debug.Log($"[Equipment/SkillTree Demo] {message}");
            }
        }

        [ContextMenu("Reset Character")]
        public void ResetCharacter()
        {
            if (demoCharacter == null) return;

            // Reset equipment
            demoCharacter.UnequipItem(EquipmentSlot.Weapon);
            demoCharacter.UnequipItem(EquipmentSlot.Armor);
            demoCharacter.UnequipItem(EquipmentSlot.Accessory);

            // Reset skill tree to default
            var newSkillTree = new SkillTree();
            demoCharacter.SkillTree.ReplaceAbility(0, newSkillTree.CurrentAbilities[0]);
            demoCharacter.SkillTree.ReplaceAbility(1, newSkillTree.CurrentAbilities[1]);
            demoCharacter.SkillTree.ReplaceAbility(2, newSkillTree.CurrentAbilities[2]);
            demoCharacter.SkillTree.ReplaceAbility(3, newSkillTree.CurrentAbilities[3]);

            // Restore health and mana
            demoCharacter.RestoreToFull();

            Log("Character reset to default state");
        }
    }
}