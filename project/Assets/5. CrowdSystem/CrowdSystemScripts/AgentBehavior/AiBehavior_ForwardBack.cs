using System;
using UnityEngine;
using UnityEngine.Serialization;

// Concrete Behavior Scripts (Attach them to the AI GameObject)
[Serializable]
[CreateAssetMenu(fileName = "New Sitting Behavior", menuName = "AI/AIBehavior_Sitting")]
public class AiBehavior_ForwardBack : AIBehavior
{
    public float travelDistance = 5f; // Editable in Inspector

    public override void Execute()
    {
        Debug.Log($"Executing Sitting Behavior for {travelDistance} seconds");
    }
}
