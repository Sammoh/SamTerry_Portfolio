using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    /// <summary>
    /// ScriptableObject that serves as a centralized database for all equipment assets.
    /// Provides fast lookup tables and manages equipment references.
    /// </summary>
    [CreateAssetMenu(fileName = "EquipmentDatabase", menuName = "Equipment/Equipment Database", order = 0)]
    public class EquipmentDatabase : ScriptableObject
    {
        [Header("Equipment Collections")]
        [SerializeField] private List<WeaponSO> weapons = new List<WeaponSO>();
        [SerializeField] private List<EquipmentSO> accessories = new List<EquipmentSO>();
        [SerializeField] private List<EquipmentSO> armors = new List<EquipmentSO>();
        
        [Header("Database Info")]
        [SerializeField] private int totalEquipmentCount;
        [SerializeField] private string lastUpdated;
        
        // Fast lookup dictionaries (initialized at runtime)
        private Dictionary<string, EquipmentSO> _equipmentLookup;
        private Dictionary<EquipmentType, List<EquipmentSO>> _equipmentByType;
        private Dictionary<EquipmentRarity, List<EquipmentSO>> _equipmentByRarity;
        
        /// <summary>
        /// Gets all weapons in the database.
        /// </summary>
        public IReadOnlyList<WeaponSO> Weapons => weapons;
        
        /// <summary>
        /// Gets all accessories in the database.
        /// </summary>
        public IReadOnlyList<EquipmentSO> Accessories => accessories;
        
        /// <summary>
        /// Gets all armor pieces in the database.
        /// </summary>
        public IReadOnlyList<EquipmentSO> Armors => armors;
        
        /// <summary>
        /// Gets the total number of equipment items in the database.
        /// </summary>
        public int TotalEquipmentCount => totalEquipmentCount;
        
        /// <summary>
        /// Gets the last updated timestamp.
        /// </summary>
        public string LastUpdated => lastUpdated;

        /// <summary>
        /// Initializes lookup tables for fast equipment retrieval.
        /// Call this before using the database at runtime.
        /// </summary>
        public void InitializeLookupTables()
        {
            _equipmentLookup = new Dictionary<string, EquipmentSO>();
            _equipmentByType = new Dictionary<EquipmentType, List<EquipmentSO>>();
            _equipmentByRarity = new Dictionary<EquipmentRarity, List<EquipmentSO>>();
            
            var allEquipment = GetAllEquipment();
            
            foreach (var equipment in allEquipment)
            {
                // ID-based lookup
                _equipmentLookup[equipment.Id] = equipment;
                
                // Type-based lookup
                if (!_equipmentByType.ContainsKey(equipment.Type))
                    _equipmentByType[equipment.Type] = new List<EquipmentSO>();
                _equipmentByType[equipment.Type].Add(equipment);
                
                // Rarity-based lookup
                if (!_equipmentByRarity.ContainsKey(equipment.Rarity))
                    _equipmentByRarity[equipment.Rarity] = new List<EquipmentSO>();
                _equipmentByRarity[equipment.Rarity].Add(equipment);
            }
            
            totalEquipmentCount = allEquipment.Count;
            lastUpdated = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// Finds equipment by its unique ID (asset name).
        /// </summary>
        /// <param name="id">Equipment ID</param>
        /// <returns>Equipment if found, null otherwise</returns>
        public EquipmentSO FindEquipmentById(string id)
        {
            if (_equipmentLookup == null) InitializeLookupTables();
            return _equipmentLookup.TryGetValue(id, out var equipment) ? equipment : null;
        }

        /// <summary>
        /// Gets all equipment of a specific type.
        /// </summary>
        /// <param name="type">Equipment type</param>
        /// <returns>List of equipment of the specified type</returns>
        public List<EquipmentSO> GetEquipmentByType(EquipmentType type)
        {
            if (_equipmentByType == null) InitializeLookupTables();
            return _equipmentByType.TryGetValue(type, out var list) ? new List<EquipmentSO>(list) : new List<EquipmentSO>();
        }

        /// <summary>
        /// Gets all equipment of a specific rarity.
        /// </summary>
        /// <param name="rarity">Equipment rarity</param>
        /// <returns>List of equipment of the specified rarity</returns>
        public List<EquipmentSO> GetEquipmentByRarity(EquipmentRarity rarity)
        {
            if (_equipmentByRarity == null) InitializeLookupTables();
            return _equipmentByRarity.TryGetValue(rarity, out var list) ? new List<EquipmentSO>(list) : new List<EquipmentSO>();
        }

        /// <summary>
        /// Gets all equipment in the database.
        /// </summary>
        /// <returns>List of all equipment</returns>
        public List<EquipmentSO> GetAllEquipment()
        {
            var allEquipment = new List<EquipmentSO>();
            allEquipment.AddRange(weapons.Cast<EquipmentSO>());
            allEquipment.AddRange(accessories);
            allEquipment.AddRange(armors);
            return allEquipment;
        }

        /// <summary>
        /// Adds equipment to the database.
        /// </summary>
        /// <param name="equipment">Equipment to add</param>
        public void AddEquipment(EquipmentSO equipment)
        {
            if (equipment == null) return;

            switch (equipment.Type)
            {
                case EquipmentType.Weapon:
                    if (equipment is WeaponSO weapon && !weapons.Contains(weapon))
                        weapons.Add(weapon);
                    break;
                case EquipmentType.Accessory:
                    if (!accessories.Contains(equipment))
                        accessories.Add(equipment);
                    break;
                case EquipmentType.Armor:
                    if (!armors.Contains(equipment))
                        armors.Add(equipment);
                    break;
            }

            // Clear cached lookups to force regeneration
            _equipmentLookup = null;
            _equipmentByType = null;
            _equipmentByRarity = null;
        }

        /// <summary>
        /// Removes equipment from the database.
        /// </summary>
        /// <param name="equipment">Equipment to remove</param>
        /// <returns>True if removed, false otherwise</returns>
        public bool RemoveEquipment(EquipmentSO equipment)
        {
            if (equipment == null) return false;

            bool removed = false;

            switch (equipment.Type)
            {
                case EquipmentType.Weapon:
                    if (equipment is WeaponSO weapon)
                        removed = weapons.Remove(weapon);
                    break;
                case EquipmentType.Accessory:
                    removed = accessories.Remove(equipment);
                    break;
                case EquipmentType.Armor:
                    removed = armors.Remove(equipment);
                    break;
            }

            if (removed)
            {
                // Clear cached lookups to force regeneration
                _equipmentLookup = null;
                _equipmentByType = null;
                _equipmentByRarity = null;
            }

            return removed;
        }

        /// <summary>
        /// Gets statistics about the equipment in the database.
        /// </summary>
        /// <returns>Equipment statistics</returns>
        public EquipmentStatistics GetStatistics()
        {
            var stats = new EquipmentStatistics();
            var allEquipment = GetAllEquipment();

            stats.TotalCount = allEquipment.Count;
            stats.WeaponCount = weapons.Count;
            stats.AccessoryCount = accessories.Count;
            stats.ArmorCount = armors.Count;

            // Count by rarity
            foreach (var equipment in allEquipment)
            {
                switch (equipment.Rarity)
                {
                    case EquipmentRarity.Common:
                        stats.CommonCount++;
                        break;
                    case EquipmentRarity.Uncommon:
                        stats.UncommonCount++;
                        break;
                    case EquipmentRarity.Rare:
                        stats.RareCount++;
                        break;
                    case EquipmentRarity.Epic:
                        stats.EpicCount++;
                        break;
                    case EquipmentRarity.Legendary:
                        stats.LegendaryCount++;
                        break;
                }
            }

            return stats;
        }

        /// <summary>
        /// Scans the Resources folder for equipment assets and updates references.
        /// </summary>
        [ContextMenu("Refresh Database")]
        public void RefreshDatabase()
        {
#if UNITY_EDITOR
            weapons.Clear();
            accessories.Clear();
            armors.Clear();

            // Load all equipment from Resources folder
            var allWeapons = Resources.LoadAll<WeaponSO>("Equipment/Weapons");
            var allAccessories = Resources.LoadAll<EquipmentSO>("Equipment/Accessories");
            var allArmors = Resources.LoadAll<EquipmentSO>("Equipment/Armor");

            weapons.AddRange(allWeapons);
            accessories.AddRange(allAccessories.Where(eq => eq.Type == EquipmentType.Accessory));
            armors.AddRange(allArmors.Where(eq => eq.Type == EquipmentType.Armor));

            InitializeLookupTables();

            UnityEditor.EditorUtility.SetDirty(this);
            Debug.Log($"Database refreshed. Found {totalEquipmentCount} equipment items.");
#endif
        }
    }

    /// <summary>
    /// Statistics about equipment in the database.
    /// </summary>
    [System.Serializable]
    public class EquipmentStatistics
    {
        public int TotalCount;
        public int WeaponCount;
        public int AccessoryCount;
        public int ArmorCount;
        
        public int CommonCount;
        public int UncommonCount;
        public int RareCount;
        public int EpicCount;
        public int LegendaryCount;
    }
}