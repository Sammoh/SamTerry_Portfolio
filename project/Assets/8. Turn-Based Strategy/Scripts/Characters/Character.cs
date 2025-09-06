using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    public class Character : MonoBehaviour, ICharacter
    {
        [SerializeField] private string characterName;
        [SerializeField] private CharacterStats stats;
        [SerializeField] private CharacterAbility[] abilities;
        [SerializeField] private bool isPlayerControlled;

        // New systems
        [SerializeField] private EquipmentManager equipmentManager = new EquipmentManager();
        [SerializeField] private SkillTree skillTree = new SkillTree();

        public string CharacterName => characterName;
        public CharacterStats Stats => stats;
        public CharacterAbility[] Abilities => skillTree?.GetValidAbilities() ?? abilities; // Use skill tree abilities if available
        public bool IsPlayerControlled => isPlayerControlled;
        
        // New properties for equipment and skill tree
        public EquipmentManager EquipmentManager => equipmentManager;
        public SkillTree SkillTree => skillTree;

        public void Initialize(string name, CharacterStats characterStats, CharacterAbility[] characterAbilities, bool isPlayer = false)
        {
            characterName = name;
            stats = characterStats;
            abilities = characterAbilities;
            isPlayerControlled = isPlayer;

            // Initialize equipment system
            if (equipmentManager == null)
                equipmentManager = new EquipmentManager();
            stats.SetEquipmentManager(equipmentManager);

            // Initialize skill tree
            if (skillTree == null)
                skillTree = new SkillTree();
        }

        public bool EquipItem(Equipment equipment)
        {
            if (equipmentManager.EquipItem(equipment))
            {
                stats.SetEquipmentManager(equipmentManager); // Refresh stats
                return true;
            }
            return false;
        }

        public Equipment UnequipItem(EquipmentSlot slot)
        {
            Equipment unequipped = equipmentManager.UnequipItem(slot);
            if (unequipped != null)
            {
                stats.SetEquipmentManager(equipmentManager); // Refresh stats
            }
            return unequipped;
        }

        public bool CanAct()
        {
            return stats.IsAlive;
        }

        public void ResetForNewTurn()
        {
            // Restore a small amount of mana each turn
            stats.RestoreMana(5);
        }

        public void RestoreToFull()
        {
            stats.FullRestore();
        }

        private void Start()
        {
            // Ensure we have a valid name if not set
            if (string.IsNullOrEmpty(characterName))
            {
                characterName = gameObject.name;
            }

            // Initialize systems if not already done
            if (equipmentManager == null)
                equipmentManager = new EquipmentManager();
            
            if (skillTree == null)
                skillTree = new SkillTree();

            if (stats != null)
                stats.SetEquipmentManager(equipmentManager);
        }
    }
}