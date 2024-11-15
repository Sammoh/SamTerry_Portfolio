using UnityEngine;

namespace Sammoh.CrowdSystem
{
    [CreateAssetMenu(fileName = "CrowdingBehaviour", menuName = "ScriptableObjects/CrowdingBehaviour", order = 3)]
    public class CrowdingBehaviour : BehaviourBase
    {
        public override void InitBehaviour(CrowdAgentAi agentAi, Vector3[] newWaypoints)
        {
            Debug.LogError("Searching for the next crowd");
        }

        public override Vector3 GetBehaviourDestination(int step)
        {
            Debug.LogError("No Destination");
            return Vector3.zero;
        }
    }
}