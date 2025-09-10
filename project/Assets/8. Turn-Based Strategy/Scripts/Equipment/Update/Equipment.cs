using System;
using UnityEngine;
// Bring the TBS types into scope so we can share the same data model and logic.
using Sammoh.TurnBasedStrategy;

namespace Sammoh.Two
{
    /// <summary>
    /// ScriptableObject base for all equipment assets in the "Two" module,
    /// aligned with Sammoh.TurnBasedStrategy's Equipment stat modifier model.
    ///
    /// - Keeps authoring-centric fields (icon, rarity, value, weight, etc.)
    /// - Adds Slot + StatModifiers (shared with TBS)
    /// - Mirrors TBS evaluation order: additive, then multiplicative (percent)
    /// </summary>
    public abstract class Equipment : ScriptableObject
    {
        [Header("Basic Information")]
        [SerializeField] protected string equipmentName;
        [TextArea]
        [SerializeField] protected string description;
        [SerializeField] protected Sprite icon;

        [Header("Authoring Properties")]
        [Min(1)]
        [SerializeField] protected int level = 1;
        [Min(0f)]
        [SerializeField] protected float weight = 1.0f;
        [Min(0)]
        [SerializeField] protected int value = 100;
        [SerializeField] protected EquipmentRarity rarity = EquipmentRarity.Common;

        [Header("TBS Alignment")]
        [Tooltip("Which slot this equipment occupies (shared with Sammoh.TurnBasedStrategy)")]
        [SerializeField] protected EquipmentSlot slot;

        [Tooltip("Stat modifiers evaluated in additive -> multiplicative order (shared with Sammoh.TurnBasedStrategy)")]
        [SerializeField] protected StatModifier[] statModifiers = Array.Empty<StatModifier>();

        /// <summary>
        /// Equipment ID derived from the asset file name for fast lookup tables.
        /// </summary>
        public string Id => name;

        /// <summary>Display name of the equipment (falls back to asset name).</summary>
        public string EquipmentName
        {
            get => string.IsNullOrEmpty(equipmentName) ? name : equipmentName;
            set => equipmentName = value;
        }

        /// <summary>Long-form description for tooltips, UI, etc.</summary>
        public string Description
        {
            get => description;
            set => description = value;
        }

        /// <summary>UI icon.</summary>
        public Sprite Icon
        {
            get => icon;
            set => icon = value;
        }

        /// <summary>Level requirement or authoring level.</summary>
        public int Level
        {
            get => level;
            set => level = Mathf.Max(1, value);
        }

        /// <summary>Weight for encumbrance systems, etc.</summary>
        public float Weight
        {
            get => weight;
            set => weight = Mathf.Max(0f, value);
        }

        /// <summary>Base value / sell price anchor.</summary>
        public int Value
        {
            get => value;
            set => this.value = Mathf.Max(0, value);
        }

        /// <summary>Rarity bucket (influences value/weight by default).</summary>
        public EquipmentRarity Rarity
        {
            get => rarity;
            set => rarity = value;
        }

        /// <summary>Category for authoring & filtering inside the Two module.</summary>
        public abstract EquipmentType Type { get; }

        /// <summary>Slot compatibility (shared with TBS).</summary>
        public EquipmentSlot Slot
        {
            get => slot;
            set => slot = value;
        }

        /// <summary>Full set of stat modifiers (shared with TBS).</summary>
        public StatModifier[] StatModifiers
        {
            get => statModifiers ?? Array.Empty<StatModifier>();
            set => statModifiers = value ?? Array.Empty<StatModifier>();
        }

        #region TBS-Compatible Logic

        /// <summary>
        /// TBS-aligned evaluation: apply additive first, then multiplicative (percent).
        /// Mirrors Sammoh.TurnBasedStrategy.Equipment.GetModifiedValue.
        /// </summary>
        /// <param name="statType">Target stat type.</param>
        /// <param name="baseValue">Unmodified base value.</param>
        /// <returns>Final value after applying all matching modifiers.</returns>
        public float GetModifiedValue(StatType statType, float baseValue)
        {
            float result = baseValue;

            // 1) Additive
            var mods = StatModifiers;
            for (int i = 0; i < mods.Length; i++)
            {
                ref readonly var m = ref mods[i];
                if (m.StatType == statType && m.ModifierType == ModifierType.Flat)
                    result += m.Value;
            }

            // 2) Multiplicative (percent as 100 = +100%)
            for (int i = 0; i < mods.Length; i++)
            {
                ref readonly var m = ref mods[i];
                if (m.StatType == statType && m.ModifierType == ModifierType.Percentage)
                    result *= (1f + m.Value / 100f);
            }

            return result;
        }

        /// <summary>
        /// Returns a copy of all modifiers that target the given stat type.
        /// </summary>
        public StatModifier[] GetModifiersForStat(StatType statType)
        {
            var mods = StatModifiers;
            int count = 0;
            // First pass: count
            for (int i = 0; i < mods.Length; i++)
                if (mods[i].StatType == statType) count++;

            if (count == 0) return Array.Empty<StatModifier>();

            // Second pass: copy
            var result = new StatModifier[count];
            int idx = 0;
            for (int i = 0; i < mods.Length; i++)
                if (mods[i].StatType == statType) result[idx++] = mods[i];

            return result;
        }

        #endregion

        #region Authoring Helpers

        /// <summary>
        /// Generates reasonable defaults and rarity-scaled economics.
        /// </summary>
        public virtual void GenerateDefaultValues()
        {
            if (string.IsNullOrEmpty(equipmentName))
                equipmentName = name.Replace("_", " ");

            // Simple rarity scaling (tunable)
            float rarityMultiplier = GetRarityMultiplier(rarity);
            value = Mathf.RoundToInt(Mathf.Max(0, value) * rarityMultiplier);
            weight = Mathf.Max(0f, weight) * (1f + (int)rarity * 0.1f);
        }

        protected static float GetRarityMultiplier(EquipmentRarity r)
        {
            return r switch
            {
                EquipmentRarity.Common     => 1f,
                EquipmentRarity.Uncommon   => 1.5f,
                EquipmentRarity.Rare       => 2.5f,
                EquipmentRarity.Epic       => 4f,
                EquipmentRarity.Legendary  => 6f,
                _                          => 1f
            };
        }

        private void OnValidate()
        {
            level = Mathf.Max(1, level);
            weight = Mathf.Max(0f, weight);
            value = Mathf.Max(0, value);
            if (statModifiers == null) statModifiers = Array.Empty<StatModifier>();
        }

        public override string ToString()
        {
            var display = string.IsNullOrEmpty(EquipmentName) ? name : EquipmentName;
            return $"{display} [{slot}]";
        }

        #endregion
    }

    /// <summary>Local authoring category for Two's filtering/UX.</summary>
    public enum EquipmentType
    {
        Weapon,
        Accessory,
        Armor
    }

    /// <summary>Rarity buckets for economic/scaling helpers.</summary>
    public enum EquipmentRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
}
