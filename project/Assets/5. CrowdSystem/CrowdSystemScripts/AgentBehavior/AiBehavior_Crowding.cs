using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Concrete Behavior Scripts (Attach them to the AI GameObject)
[Serializable]
[CreateAssetMenu(fileName = "New Sitting Behavior", menuName = "AI/AIBehavior_Sitting")]
public class AiBehavior_Crowding : AIBehavior
{
    public float duration = 5f; // Editable in Inspector

    public override void Execute()
    {
        Debug.Log($"Executing Sitting Behavior for {duration} seconds");
    }
}
