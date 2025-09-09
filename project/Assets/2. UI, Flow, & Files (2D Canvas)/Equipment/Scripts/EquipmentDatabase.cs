using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sammoh.Two
{
    /// <summary>
    /// ScriptableObject that serves as a centralized database for all equipment assets.
    /// Provides fast lookup tables and manages equipment references.
    /// </summary>
    [CreateAssetMenu(fileName = "EquipmentDatabase", menuName = "Equipment/Equipment Database", order = 0)]
    public class EquipmentDatabase : ScriptableObject
    {
        [Header("Equipment Collections")]
        [SerializeField] private List<Weapon> weapons = new List<Weapon>();
        [SerializeField] private List<Accessory> accessories = new List<Accessory>();
        [SerializeField] private List<Armor> armors = new List<Armor>();
        
        [Header("Database Info")]
        [SerializeField] private int totalEquipmentCount;
        [SerializeField] private string lastUpdated;
        
        // Fast lookup dictionaries (initialized at runtime)
        private Dictionary<string, Equipment> _equipmentLookup;
        private Dictionary<EquipmentType, List<Equipment>> _equipmentByType;
        private Dictionary<EquipmentRarity, List<Equipment>> _equipmentByRarity;
        
        /// <summary>
        /// Gets all weapons in the database.
        /// </summary>
        public IReadOnlyList<Weapon> Weapons => weapons;
        
        /// <summary>
        /// Gets all accessories in the database.
        /// </summary>
        public IReadOnlyList<Accessory> Accessories => accessories;
        
        /// <summary>
        /// Gets all armor pieces in the database.
        /// </summary>
        public IReadOnlyList<Armor> Armors => armors;
        
        /// <summary>
        /// Gets the total number of equipment items in the database.
        /// </summary>
        public int TotalEquipmentCount => totalEquipmentCount;
        
        /// <summary>
        /// Gets the last updated timestamp.
        /// </summary>
        public string LastUpdated => lastUpdated;
        
        /// <summary>
        /// Initializes the fast lookup dictionaries.
        /// Call this before using lookup methods.
        /// </summary>
        public void InitializeLookupTables()
        {
            _equipmentLookup = new Dictionary<string, Equipment>();
            _equipmentByType = new Dictionary<EquipmentType, List<Equipment>>();
            _equipmentByRarity = new Dictionary<EquipmentRarity, List<Equipment>>();
            
            var allEquipment = GetAllEquipment();
            
            // Build ID lookup table
            foreach (var equipment in allEquipment)
            {
                if (equipment != null && !string.IsNullOrEmpty(equipment.Id))
                {
                    _equipmentLookup[equipment.Id] = equipment;
                }
            }
            
            // Build type lookup table
            foreach (EquipmentType type in System.Enum.GetValues(typeof(EquipmentType)))
            {
                _equipmentByType[type] = allEquipment.Where(e => e != null && e.Type == type).ToList();
            }
            
            // Build rarity lookup table
            foreach (EquipmentRarity rarity in System.Enum.GetValues(typeof(EquipmentRarity)))
            {
                _equipmentByRarity[rarity] = allEquipment.Where(e => e != null && e.Rarity == rarity).ToList();
            }
        }
        
        /// <summary>
        /// Finds equipment by its ID (file name).
        /// </summary>
        public Equipment FindEquipmentById(string id)
        {
            if (_equipmentLookup == null)
                InitializeLookupTables();
                
            return _equipmentLookup.TryGetValue(id, out var equipment) ? equipment : null;
        }
        
        /// <summary>
        /// Gets all equipment of a specific type.
        /// </summary>
        public List<Equipment> GetEquipmentByType(EquipmentType type)
        {
            if (_equipmentByType == null)
                InitializeLookupTables();
                
            return _equipmentByType.TryGetValue(type, out var equipment) ? equipment : new List<Equipment>();
        }
        
        /// <summary>
        /// Gets all equipment of a specific rarity.
        /// </summary>
        public List<Equipment> GetEquipmentByRarity(EquipmentRarity rarity)
        {
            if (_equipmentByRarity == null)
                InitializeLookupTables();
                
            return _equipmentByRarity.TryGetValue(rarity, out var equipment) ? equipment : new List<Equipment>();
        }
        
        /// <summary>
        /// Gets equipment that matches both type and rarity.
        /// </summary>
        public List<Equipment> GetEquipmentByTypeAndRarity(EquipmentType type, EquipmentRarity rarity)
        {
            return GetEquipmentByType(type).Where(e => e.Rarity == rarity).ToList();
        }
        
        /// <summary>
        /// Gets all equipment items as a single collection.
        /// </summary>
        public List<Equipment> GetAllEquipment()
        {
            var allEquipment = new List<Equipment>();
            allEquipment.AddRange(weapons.Cast<Equipment>());
            allEquipment.AddRange(accessories.Cast<Equipment>());
            allEquipment.AddRange(armors.Cast<Equipment>());
            return allEquipment;
        }
        
        /// <summary>
        /// Adds a weapon to the database if it's not already included.
        /// </summary>
        public bool AddWeapon(Weapon weapon)
        {
            if (weapon == null || weapons.Contains(weapon))
                return false;
                
            weapons.Add(weapon);
            UpdateDatabaseInfo();
            return true;
        }
        
        /// <summary>
        /// Adds an accessory to the database if it's not already included.
        /// </summary>
        public bool AddAccessory(Accessory accessory)
        {
            if (accessory == null || accessories.Contains(accessory))
                return false;
                
            accessories.Add(accessory);
            UpdateDatabaseInfo();
            return true;
        }
        
        /// <summary>
        /// Adds armor to the database if it's not already included.
        /// </summary>
        public bool AddArmor(Armor armor)
        {
            if (armor == null || armors.Contains(armor))
                return false;
                
            armors.Add(armor);
            UpdateDatabaseInfo();
            return true;
        }
        
        /// <summary>
        /// Removes a weapon from the database.
        /// </summary>
        public bool RemoveWeapon(Weapon weapon)
        {
            if (weapons.Remove(weapon))
            {
                UpdateDatabaseInfo();
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Removes an accessory from the database.
        /// </summary>
        public bool RemoveAccessory(Accessory accessory)
        {
            if (accessories.Remove(accessory))
            {
                UpdateDatabaseInfo();
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Removes armor from the database.
        /// </summary>
        public bool RemoveArmor(Armor armor)
        {
            if (armors.Remove(armor))
            {
                UpdateDatabaseInfo();
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Clears all equipment from the database.
        /// </summary>
        public void ClearAll()
        {
            weapons.Clear();
            accessories.Clear();
            armors.Clear();
            UpdateDatabaseInfo();
        }
        
        /// <summary>
        /// Refreshes the database by scanning for all equipment assets in Resources.
        /// </summary>
        public void RefreshDatabase()
        {
            weapons.Clear();
            accessories.Clear();
            armors.Clear();
            
            // Load all equipment from Resources
            var allWeapons = Resources.LoadAll<Weapon>("Equipment/Weapons");
            var allAccessories = Resources.LoadAll<Accessory>("Equipment/Accessories");
            var allArmors = Resources.LoadAll<Armor>("Equipment/Armor");
            
            weapons.AddRange(allWeapons);
            accessories.AddRange(allAccessories);
            armors.AddRange(allArmors);
            
            UpdateDatabaseInfo();
            InitializeLookupTables();
        }
        
        /// <summary>
        /// Gets statistics about the equipment in the database.
        /// </summary>
        public EquipmentStatistics GetStatistics()
        {
            var stats = new EquipmentStatistics();
            var allEquipment = GetAllEquipment();
            
            stats.TotalCount = allEquipment.Count;
            stats.WeaponCount = weapons.Count;
            stats.AccessoryCount = accessories.Count;
            stats.ArmorCount = armors.Count;
            
            // Count by rarity
            foreach (EquipmentRarity rarity in System.Enum.GetValues(typeof(EquipmentRarity)))
            {
                int count = allEquipment.Count(e => e.Rarity == rarity);
                stats.CountByRarity[rarity] = count;
            }
            
            return stats;
        }
        
        private void UpdateDatabaseInfo()
        {
            totalEquipmentCount = weapons.Count + accessories.Count + armors.Count;
            lastUpdated = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
            // Clear lookup tables to force refresh
            _equipmentLookup = null;
            _equipmentByType = null;
            _equipmentByRarity = null;
        }
        
        private void OnValidate()
        {
            UpdateDatabaseInfo();
        }
    }
    
    /// <summary>
    /// Contains statistics about equipment in the database.
    /// </summary>
    [System.Serializable]
    public class EquipmentStatistics
    {
        public int TotalCount;
        public int WeaponCount;
        public int AccessoryCount;
        public int ArmorCount;
        public Dictionary<EquipmentRarity, int> CountByRarity = new Dictionary<EquipmentRarity, int>();
    }
}