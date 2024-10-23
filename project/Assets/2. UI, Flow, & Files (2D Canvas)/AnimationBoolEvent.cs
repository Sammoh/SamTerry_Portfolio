using UnityEngine.Events;

/// <summary>
/// Represents an event that takes a string and a boolean as parameters.
/// </summary>


namespace Sammoh.Two
{
    [System.Serializable]
    public class AnimationBoolEvent : UnityEvent<string, bool>
    {
    }
}