namespace Sammoh.TurnBasedStrategy
{
    /// <summary>
    /// Defines how stat modifications are applied
    /// </summary>
    public enum ModifierType
    {
        /// <summary>
        /// Adds the value directly to the base stat (e.g., +5 Attack)
        /// </summary>
        Additive,
        
        /// <summary>
        /// Multiplies the base stat by (1 + value/100) (e.g., +20% Attack)
        /// </summary>
        Multiplicative
    }
}