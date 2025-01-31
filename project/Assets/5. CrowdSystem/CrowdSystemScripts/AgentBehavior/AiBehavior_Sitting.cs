using System;
using Sammoh.CrowdSystem;
using UnityEngine;

// Concrete Behavior Scripts (Attach them to the AI GameObject)
[Serializable]
[CreateAssetMenu(fileName = "New Sitting Behavior", menuName = "AI/AIBehavior_Sitting")]
public class AiBehavior_Sitting : AIBehavior
{
    public float duration = 5f; // Editable in Inspector
    
    public override void Control(CrowdAgentAi crowdAgentAi)
    {
        // throw new NotImplementedException();
    }
}
