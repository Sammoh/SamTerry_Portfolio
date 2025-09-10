using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    public class Character : MonoBehaviour, ICharacter
    {
        [SerializeField] private string characterName;
        [SerializeField] private CharacterStats stats;
        [SerializeField] private CharacterAbility[] abilities;
        [SerializeField] private bool isPlayerControlled;
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private EquipmentManager equipmentManager;

        public string CharacterName => characterName;
        public CharacterStats Stats => stats;
        public CharacterAbility[] Abilities => abilities;
        public bool IsPlayerControlled => isPlayerControlled;
        public CharacterClass CharacterClass => characterClass;
        public EquipmentManager Equipment => equipmentManager;

        public void Initialize(string name, CharacterStats characterStats, CharacterAbility[] characterAbilities, bool isPlayer = false, CharacterClass charClass = CharacterClass.Warrior)
        {
            characterName = name;
            stats = characterStats;
            abilities = characterAbilities;
            isPlayerControlled = isPlayer;
            characterClass = charClass;
            
            // Initialize equipment manager
            equipmentManager = new EquipmentManager();
            equipmentManager.OnEquipmentChanged += OnEquipmentChanged;
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

        public bool EquipItem(Equipment equipment)
        {
            if (equipmentManager == null)
            {
                equipmentManager = new EquipmentManager();
                equipmentManager.OnEquipmentChanged += OnEquipmentChanged;
            }
            
            return equipmentManager.EquipItem(equipment, characterClass);
        }

        public Equipment UnequipItem(EquipmentSlot slot)
        {
            if (equipmentManager == null)
                return null;
                
            return equipmentManager.UnequipItem(slot);
        }

        public Equipment GetEquippedItem(EquipmentSlot slot)
        {
            if (equipmentManager == null)
                return null;
                
            return equipmentManager.GetEquippedItem(slot);
        }

        private void OnEquipmentChanged(Equipment previousEquipment, Equipment newEquipment)
        {
            // Update character stats based on new equipment
            if (equipmentManager != null)
            {
                EquipmentStats totalBonus = equipmentManager.GetTotalEquipmentStats();
                stats.UpdateEquipmentBonus(totalBonus);
            }
        }

        private void Start()
        {
            // Ensure we have a valid name if not set
            if (string.IsNullOrEmpty(characterName))
            {
                characterName = gameObject.name;
            }
            
            // Initialize equipment manager if null
            if (equipmentManager == null)
            {
                equipmentManager = new EquipmentManager();
                equipmentManager.OnEquipmentChanged += OnEquipmentChanged;
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from equipment events to prevent memory leaks
            if (equipmentManager != null)
            {
                equipmentManager.OnEquipmentChanged -= OnEquipmentChanged;
            }
        }
    }
}