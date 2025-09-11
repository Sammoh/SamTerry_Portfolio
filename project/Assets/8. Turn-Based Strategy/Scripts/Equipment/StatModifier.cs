using System;
using UnityEngine;

/// <summary>
/// Represents a stat modification provided by equipment
/// </summary>
[Serializable]
public class StatModifier
{
    [SerializeField] private StatType statType;
    [SerializeField] private float value;
    [SerializeField] private ModifierType modifierType;

    public StatType StatType => statType;
    public float Value => value;
    public ModifierType ModifierType => modifierType;

    public StatModifier(StatType statType, float value, ModifierType modifierType)
    {
        this.statType = statType;
        this.value = value;
        this.modifierType = modifierType;
    }
    
    public StatModifier(StatModifier other)
    {
        this.statType = other.statType;
        this.value = other.value;
        this.modifierType = other.modifierType;
    }

    /// <summary>
    /// Applies this modifier to a base stat value
    /// </summary>
    /// <param name="baseValue">The base stat value</param>
    /// <returns>The modified stat value</returns>
    public float ApplyModifier(float baseValue)
    {
        switch (modifierType)
        {
            case ModifierType.Flat:
                return baseValue + value;
            case ModifierType.Percentage:
                return baseValue * (1 + value / 100f);
            default:
                return baseValue;
        }
    }
    
    public void Validate()
    {
        value = Mathf.Max(0f, value);
    }
    
    public override string ToString()
    {
        string sign = value >= 0 ? "+" : "";
        switch (modifierType)
        {
            case ModifierType.Flat:
                return $"{sign}{value} {statType}";
            case ModifierType.Percentage:
                return $"{sign}{value}% {statType}";
            default:
                return $"{value} {statType}";
        }
    }
}
