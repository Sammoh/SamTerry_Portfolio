using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    public class Character : MonoBehaviour, ICharacter
    {
        [SerializeField] private string characterName;
        [SerializeField] private CharacterStats stats;
        [SerializeField] private CharacterAbility[] abilities;
        [SerializeField] private bool isPlayerControlled;
        [SerializeField] private EquipmentManager equipmentManager = new EquipmentManager();

        public string CharacterName => characterName;
        public CharacterStats Stats => stats;
        public CharacterAbility[] Abilities => abilities;
        public bool IsPlayerControlled => isPlayerControlled;
        public EquipmentManager EquipmentManager => equipmentManager;

        public void Initialize(string name, CharacterStats characterStats, CharacterAbility[] characterAbilities, bool isPlayer = false)
        {
            characterName = name;
            stats = characterStats;
            abilities = characterAbilities;
            isPlayerControlled = isPlayer;
            
            // Connect equipment manager to stats
            if (stats != null && equipmentManager != null)
            {
                stats.SetEquipmentManager(equipmentManager);
            }
        }

        /// <summary>
        /// Equips an item to this character
        /// </summary>
        /// <param name="equipment">The equipment to equip</param>
        /// <returns>True if equipment was successfully equipped, false otherwise</returns>
        public bool EquipItem(Equipment equipment)
        {
            if (equipment == null || equipmentManager == null)
                return false;

            equipmentManager.EquipItem(equipment);
            return true;
        }

        /// <summary>
        /// Unequips an item from the specified slot
        /// </summary>
        /// <param name="slot">The slot to unequip</param>
        /// <returns>The unequipped item, or null if no item was equipped</returns>
        public Equipment UnequipItem(EquipmentSlot slot)
        {
            if (equipmentManager == null)
                return null;

            return equipmentManager.UnequipItem(slot);
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
            
            // Initialize equipment manager connection to stats
            EnsureEquipmentManagerInitialized();
        }

        private void Awake()
        {
            // Ensure equipment manager is initialized
            EnsureEquipmentManagerInitialized();
        }

        /// <summary>
        /// Ensures the equipment manager is properly initialized and connected to stats
        /// </summary>
        private void EnsureEquipmentManagerInitialized()
        {
            if (equipmentManager == null)
            {
                equipmentManager = new EquipmentManager();
            }

            // Connect equipment manager to stats if we have stats
            if (stats != null)
            {
                stats.SetEquipmentManager(equipmentManager);
            }
        }
    }
}